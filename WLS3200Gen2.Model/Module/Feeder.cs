using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.CameraLib;
using YuanliCore.Data;
using YuanliCore.Model.Interface.Component;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    /// <summary>
    /// EFEM 負責整個傳送wafer的任務
    /// </summary>
    public class Feeder
    {
        private bool isSotMapping;
        private bool isCassetteDone;
        private Cassette cassette; //由 ProcessInitial 決定是哪一個Loadport
        private ILoadPort tempLoadPort;
        private IAligner tempAligner;
        private int vaccumDelay = 300;
        private PauseTokenSource pauseToken = new PauseTokenSource();
        private CancellationTokenSource cancelToken = new CancellationTokenSource();
        private MachineSetting machineSetting;
        //     private Queue<Wafer> processWafers;
        //預載暫存WAFER紀錄
        //  private Wafer processTempPre_Wafer;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="robot"> 機械手臂</param>
        /// <param name="loadPortL">只有一支時  預設使用L </param>
        /// <param name="loadPortR"></param>
        /// <param name="macro">巨觀檢查機構 </param>
        /// <param name="aligner">晶圓定位機</param>
        /// <param name="axis">乘載機械手臂的移動軸</param>
        public Feeder(IEFEMRobot robot, ILoadPort loadPortL, ILoadPort loadPortR, IMacro macro, IAligner aligner, ILampControl lampControl1, ILampControl lampControl2, IReader reader, Axis axis, MachineSetting machineSetting)
        {
            this.Robot = robot;
            this.Macro = macro;
            this.AlignerL = aligner;
            this.RobotAxis = axis;
            this.LoadPortL = loadPortL;
            this.LoadPortR = loadPortR;
            this.LampControl1 = lampControl1;
            this.LampControl2 = lampControl2;
            this.Reader = reader;
            this.machineSetting = machineSetting;
        }
        public bool IsInitial { get; set; } = false;

        public IEFEMRobot Robot { get; }
        public IMacro Macro { get; }
        public Axis RobotAxis { get; }

        /// <summary>
        /// 當只有一個 Aligner  只啟用AlignerL
        /// </summary>
        public IAligner AlignerL { get; }
        public IAligner AlignerR { get; }
        /// <summary>
        /// 當只有一個 LoadPort  只啟用LoadPortL
        /// </summary>
        public ILoadPort LoadPortL { get; }
        public ILoadPort LoadPortR { get; }

        public ILampControl LampControl1 { get; }
        public ILampControl LampControl2 { get; }
        public IReader Reader { get; }
        public Cassette Cassette { get => cassette; }
        public bool IsCassetteDone { get => isCassetteDone; }
        public string WaferID { get; }


        public Task WaitEFEMonSafe = Task.CompletedTask;

        public event Action<YuanliCore.Logger.LogType, string> WriteLog;

        public Action MicroFixed;

        public event Func<PauseTokenSource, CancellationTokenSource, Task<String>> WaferIDReady;

        public event Action<BitmapSource> WaferIDRecord;
        public async Task Home()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "EFEM Homing Start");

                WaitEFEMonSafe = Robot.Home();

                await WaitEFEMonSafe;

                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Robot Homing End");

                Task aligner8Home = Task.CompletedTask;
                if (AlignerL != null)
                {
                    aligner8Home = AlignerL.Home();
                }

                Task robotAxisHome = Task.CompletedTask;
                if (RobotAxis != null)
                {
                    robotAxisHome = RobotAxis.HomeAsync();
                }

                Task aligner12Home = Task.CompletedTask;
                if (AlignerR != null)
                {
                    aligner12Home = AlignerR.Home();
                }

                Task loadPort8Home = Task.CompletedTask;
                if (LoadPortL != null)
                {
                    loadPort8Home = LoadPortL.Home();
                }

                Task loadPort12Home = Task.CompletedTask;
                if (LoadPortR != null)
                {
                    loadPort12Home = LoadPortR.Home();
                }

                Task macroHome = Macro.Home();

                Task lamp1Home = LampControl1.ChangeLightAsync(0);

                Task lamp2Home = Task.CompletedTask;
                if (LampControl2 != null)
                {
                    lamp2Home = LampControl2.ChangeLightAsync(0);
                }
                await Task.WhenAll(aligner8Home, aligner12Home);
                await Task.WhenAll(loadPort8Home, loadPort12Home);
                await Task.WhenAll(robotAxisHome);
                await Task.WhenAll(macroHome, lamp1Home, lamp2Home);
                //測試WaferID有沒有正常
                string testWaferID = await Reader.ReadAsync();
                IsInitial = true;
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "EFEM Homing End");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ProcessInitial(InchType inchType, MachineSetting machineSetting, PauseTokenSource pauseToken, CancellationTokenSource cancellationToken)
        {
            if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
            this.pauseToken = pauseToken;
            this.cancelToken = cancellationToken;
            this.machineSetting = machineSetting;//重新將設定檔傳入
            //Robot.cancelToken = cancellationToken;

            //判斷 8吋 或 12吋 啟用不同的硬體裝置
            switch (inchType)
            {
                case InchType.None:
                    //    tempAligner = AlignerL;
                    tempLoadPort = LoadPortL;
                    break;
                case InchType.Inch8:
                    //   tempAligner = AlignerL;
                    tempLoadPort = LoadPortL;
                    break;
                case InchType.Inch12:
                    //    tempAligner = AlignerR;
                    tempLoadPort = LoadPortL;
                    break;

                default:
                    break;
            }

            tempAligner = AlignerL; //現階段只有一台Aligner 可以共用8、12吋 ， 所以不需要區分

            //如果沒有做過Mapping 
            /*   if (!isSotMapping)
               {
                   cassette = await SlotMapping(tempLoadPort);
                   isSotMapping = true;
               }

               //判斷有WAFER的格子
               var waferuse = cassette.Wafers.Where(w => w != null); */

        }

        public void ProcessEnd()
        {



        }

        /// <summary>
        /// 預載Wafer 到  Macro
        /// </summary>
        /// <param name="inchType"></param>
        /// <returns></returns>
        public async Task LoadCassetteToMacroAsync(WaferProcessStatus station, int cassetteIndex, bool isLoadport1)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
                 {

                     // if (processWafers.Count() > 0)
                     {
                         //      TempPre_Wafer = processWafers.Dequeue();
                         WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, $"Wafer Preload Start: CassetteIndex  {cassetteIndex}");
                         await LoadWaferFromCassette(cassetteIndex, isLoadport1);
                         await WaferStandByToMacroAsync();
                         WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Wafer Preload End");
                     }
                     //   if (processWafers.Count == 0)
                     isCassetteDone = true;
                 });
            }
            catch (Exception ex)
            {
                station = WaferProcessStatus.Reject;
                throw ex;
            }
        }

        public async Task TurnWafer(EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");

                await Macro.InnerRingPitchXMoveToAsync(eFEMtionRecipe.MacroTopStartPitchX);
                await Macro.InnerRingRollYMoveToAsync(eFEMtionRecipe.MacroTopStartRollY);
                await Macro.InnerRingYawTMoveToAsync(eFEMtionRecipe.MacroTopStartYawT);

                //Task pitchX = Macro.InnerRingPitchXMoveToAsync(eFEMtionRecipe.MacroTopStartPitchX);
                //Task rollY = Macro.InnerRingRollYMoveToAsync(eFEMtionRecipe.MacroTopStartRollY);
                //Task yawT =  Macro.InnerRingYawTMoveToAsync(eFEMtionRecipe.MacroTopStartYawT);

                //Task pitchX = Task.CompletedTask;
                //Task rollY = Task.CompletedTask;
                //Task yawT = Task.CompletedTask;

                //await Task.WhenAll(pitchX, rollY, yawT);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task TurnBackWafer(double MacroBackStartPos)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");

                Task rollY = Macro.OuterRingRollYMoveToAsync(MacroBackStartPos);
                await Task.WhenAll(rollY);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 從Cassette->Aligner位置
        /// </summary>
        /// <returns></returns>
        public async Task LoadCassetteToAlignerAsync(WaferProcessStatus station, int cassetteIndex, bool isLoadport1)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                Wafer TempPre_Wafer = null;
                await Task.Run(async () =>
                {

                    // if (processWafers.Count() > 0)
                    {
                        //      TempPre_Wafer = processWafers.Dequeue();
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, $"Wafer Preload Start: CassetteIndex  {cassetteIndex}");
                        Task alignerHome = tempAligner.Home();
                        await LoadWaferFromCassette(cassetteIndex, isLoadport1);
                        await alignerHome;
                        await WaferStandByToAligner();
                        await tempAligner.FixWafer();
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Wafer Preload End");
                    }
                    //   if (processWafers.Count == 0)
                    isCassetteDone = true;
                });
            }
            catch (Exception ex)
            {
                station = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        public async Task AlignerAsync(Wafer currentWafer, EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Aligner  Start");
                    if (tempAligner.IsLockOK == false)
                    {
                        throw new FlowException("LoadToAlignerAsync:AlignerFixWafer Error!!");
                    }
                    if (currentWafer.ProcessStatus.WaferID == WaferProcessStatus.Select)
                    {
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "WaferID  Start");

                        await tempAligner.WaferIDRun1(eFEMtionRecipe.AlignerWaferIDAngle, eFEMtionRecipe.AlignerMicroAngle);
                        //WaferID讀取   
                        string result = await Reader.ReadAsync();
                        //如果讀取失敗自己KeyIn
                        if (result == "")
                        {
                            Task<String> waferID = WaferIDReady?.Invoke(pauseToken, cancelToken);
                            var cc = await waferID;
                        }
                        await tempAligner.WaferIDRun2();
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "WaferID  End");
                        currentWafer.ProcessStatus.WaferID = WaferProcessStatus.Pass;
                    }
                    else
                    {
                        await tempAligner.Run(eFEMtionRecipe.AlignerMicroAngle);
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Aligner  End");
                    }
                });
            }
            catch (Exception ex)
            {
                currentWafer.ProcessStatus.WaferID = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        /// <summary>
        /// 從Macro->Aligner位置
        /// </summary>
        /// <returns></returns>
        public async Task LoadMacroToAlignerAsync(Wafer currentWafer, EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
                {
                    if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                    {
                        await LoadMacroToAlignerOnPortAsync(currentWafer.ProcessStatus.WaferID, eFEMtionRecipe);
                    }
                    else
                    {
                        await LoadMacroToAlignerTwoPortAsync(currentWafer.ProcessStatus.WaferID, eFEMtionRecipe);
                    }
                });
            }
            catch (Exception ex)
            {
                currentWafer.ProcessStatus.WaferID = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        private async Task LoadMacroToAlignerOnPortAsync(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Aligner Start");
                Task alignerHome = tempAligner.Home();
                await WaferMacroToStandBy();
                await alignerHome;
                await WaferStandByToAligner();
                await tempAligner.FixWafer();
                if (tempAligner.IsLockOK == false)
                {
                    throw new FlowException("LoadToAlignerAsync:AlignerFixWafer Error!!");
                }
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Aligner  End");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task LoadMacroToAlignerTwoPortAsync(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Aligner Start");
                Task alignerHome = tempAligner.Home();
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisMacroTakePosition);
                await WaferMacroToStandBy();
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
                await alignerHome;
                await WaferStandByToAligner();
                await tempAligner.FixWafer();
                if (tempAligner.IsLockOK == false)
                {
                    throw new FlowException("LoadToAlignerAsync:AlignerFixWafer Error!!");
                }
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Aligner  End");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Aligner-> StandBy位置
        /// </summary>
        /// <returns></returns>
        public async Task AlignerToStandByAsync(Wafer currentWafer)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
                {
                    if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                    {
                        await AlignerToStandByOnePortAsync();
                    }
                    else
                    {
                        await AlignerToStandByTwoPortAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                currentWafer.ProcessStatus.WaferID = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        private async Task AlignerToStandByOnePortAsync()
        {
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Un Load From Aligner Start");
            await tempAligner.ReleaseWafer();
            await WaferAlignerToStandBy();
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Un Load From Aligner End");
        }
        private async Task AlignerToStandByTwoPortAsync()
        {
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Un Load From Aligner Start");
            await RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
            await tempAligner.ReleaseWafer();
            await WaferAlignerToStandBy();
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Un Load From Aligner End");
        }
        /// <summary>
        /// wafer放進主設備
        /// </summary>
        /// <returns></returns>
        public async Task LoadToMicroAsync(Wafer currentWafer)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                //await WaferAlignerToStandBy();
                await Task.Run(async () =>
                {
                    if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                    {
                        await LoadToMicroOnePortAsync();
                    }
                    else
                    {
                        await LoadToMicroTwoPortAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                currentWafer.ProcessStatus.Micro = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        private async Task LoadToMicroOnePortAsync()
        {
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Micro Start");
            await Robot.PutWafer_Standby(ArmStation.Micro);
            await Robot.PutWafer_GoIn(ArmStation.Micro);
            await Robot.ReleaseWafer();
            MicroFixed?.Invoke(); //顯微鏡平台固定WAFER ( 真空開)
            await Robot.PutWafer_PutDown(ArmStation.Micro);
            await Robot.PutWafer_Retract(ArmStation.Micro);
            await Robot.PutWafer_Safety(ArmStation.Micro);
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Micro End");
        }
        private async Task LoadToMicroTwoPortAsync()
        {
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Micro Start");
            await RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
            await Robot.PutWafer_Standby(ArmStation.Micro);
            await Robot.PutWafer_GoIn(ArmStation.Micro);
            await Robot.ReleaseWafer();
            MicroFixed?.Invoke(); //顯微鏡平台固定WAFER ( 真空開)
            await Robot.PutWafer_PutDown(ArmStation.Micro);
            await Robot.PutWafer_Retract(ArmStation.Micro);
            await Robot.PutWafer_Safety(ArmStation.Micro);
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Micro End");
        }
        /// <summary>
        /// Micro-> StandBy位置
        /// </summary>
        /// <returns></returns>
        public async Task MicroUnLoadToStandByAsync()
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    await MicroUnLoadToStandByOnePortAsync();
                }
                else
                {
                    await MicroUnLoadToStandByTwoPortAsync();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task MicroUnLoadToStandByOnePortAsync()
        {
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Micro Start ");
            await WaferMicroToStandBy();
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Micro End ");
        }
        private async Task MicroUnLoadToStandByTwoPortAsync()
        {
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Micro Start ");
            await RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
            await WaferMicroToStandBy();
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Micro End ");
        }

        /// <summary>
        /// 從Loadport 拿一片wafer
        /// </summary>
        /// <param name="cassetteIndex">第幾格</param>
        /// <returns></returns>
        private async Task LoadWaferFromCassette(int cassetteIndex, bool isLoadPort1)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    await LoadWaferFromPortCassetteOneLoad(cassetteIndex, isLoadPort1);
                }
                else
                {
                    await LoadWaferFromPortCassetteTwoLoad(cassetteIndex, isLoadPort1);
                }
                //設定 Cassette內WAFER的狀態  
                //  Cassette.Wafers[cassetteIndex].ProcessStatus.Totally = WaferProcessStatus.InProgress;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task LoadWaferFromPortCassetteOneLoad(int cassetteIndex, bool isLoadPort1)
        {
            if (isLoadPort1)
            {
                if (LoadPortL.IsDoorOpen == false)
                {
                    throw new Exception("UnLoadWaferToCassette:LoadPortNotOpen Error!!");
                }
            }
            else
            {
                if (LoadPortR.IsDoorOpen == false)
                {
                    throw new Exception("UnLoadWaferToCassette:LoadPortNotOpen Error!!");
                }
            }
            await WaferLoadPortToStandBy(cassetteIndex, ArmStation.Cassette1);

            //設定 Cassette內WAFER的狀態  
            //  Cassette.Wafers[cassetteIndex].ProcessStatus.Totally = WaferProcessStatus.InProgress;
        }
        private async Task LoadWaferFromPortCassetteTwoLoad(int cassetteIndex, bool isLoadPort1)
        {

            if (isLoadPort1)
            {
                if (LoadPortL.IsDoorOpen == false)
                {
                    throw new Exception("UnLoadWaferToCassette:LoadPortNotOpen Error!!");
                }
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition);
            }
            else
            {
                if (LoadPortR.IsDoorOpen == false)
                {
                    throw new Exception("UnLoadWaferToCassette:LoadPortNotOpen Error!!");
                }
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition);
            }
            await WaferLoadPortToStandBy(cassetteIndex, ArmStation.Cassette1);

            //設定 Cassette內WAFER的狀態  
            //  Cassette.Wafers[cassetteIndex].ProcessStatus.Totally = WaferProcessStatus.InProgress;
        }

        /// <summary>
        /// wafer放回loadport
        /// </summary>
        /// <param name="cassetteIndex">第幾格</param>
        /// <returns></returns>
        public async Task<Wafer> UnLoadWaferToCassette(Wafer wafer, bool isLoadPort1)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    return await UnLoadWaferToCassetteOnePort(wafer, isLoadPort1);
                }
                else
                {
                    return await UnLoadWaferToCassetteTwoPort(wafer, isLoadPort1);
                }
            }
            catch (Exception ex)
            {
                wafer.ProcessStatus.Micro = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        private async Task<Wafer> UnLoadWaferToCassetteOnePort(Wafer wafer, bool isLoadPort1)
        {
            if (isLoadPort1)
            {
                if (LoadPortL.IsDoorOpen == false)
                {
                    throw new Exception("UnLoadWaferToCassette:LoadPortNotOpen Error!!");
                }
            }
            await WaferStandByToLoadPort(wafer.CassetteIndex, ArmStation.Cassette1);
            wafer.ProcessStatus.Totally = WaferProcessStatus.Complate;
            //設定 Cassette內WAFER的狀態
            //    Cassette.Wafers[wafer.CassetteIndex] = wafer;
            return wafer;
        }
        private async Task<Wafer> UnLoadWaferToCassetteTwoPort(Wafer wafer, bool isLoadPort1)
        {
            if (isLoadPort1)
            {
                if (LoadPortL.IsDoorOpen == false)
                {
                    throw new Exception("UnLoadWaferToCassette:LoadPortNotOpen Error!!");
                }
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition);
            }
            else
            {
                if (LoadPortR.IsDoorOpen == false)
                {
                    throw new Exception("UnLoadWaferToCassette:LoadPortNotOpen Error!!");
                }
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition);
            }
            await WaferStandByToLoadPort(wafer.CassetteIndex, ArmStation.Cassette1);
            wafer.ProcessStatus.Totally = WaferProcessStatus.Complate;
            //設定 Cassette內WAFER的狀態
            //    Cassette.Wafers[wafer.CassetteIndex] = wafer;
            return wafer;
        }
        /// <summary>
        /// UnLoadWafer後，到Macro準備位子
        /// </summary>
        /// <param name="wafer"></param>
        /// <param name="isLoadPort1"></param>
        /// <returns></returns>
        public async Task<Wafer> UnLoadWaferMacroPrePare(Wafer wafer)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                Task robot = Robot.PickWafer_Standby(ArmStation.Macro);
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    await robot;
                    return wafer;
                }
                else
                {
                    Task robotAxis = RobotAxis.MoveToAsync(machineSetting.RobotAxisMacroTakePosition);
                    await Task.WhenAll(robot, robotAxis);
                    return wafer;
                }
            }
            catch (Exception ex)
            {
                wafer.ProcessStatus.Micro = WaferProcessStatus.Reject;
                throw ex;
            }
        }
        /// <summary>
        /// wafer放回loadport(Robot動作)
        /// </summary>
        /// <param name="cassetteIndex"></param>
        /// <param name="armStation"></param>
        /// <returns></returns>
        public Task WaferStandByToLoadPort(int cassetteIndex, ArmStation armStation)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                return Task.Run(async () =>
                {
                    if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                    {
                        await WaferStandByToLoadPortOnePort(cassetteIndex, armStation);
                    }
                    else
                    {
                        await WaferStandByToLoadPortTwoPort(cassetteIndex, armStation);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async Task WaferStandByToLoadPortOnePort(int cassetteIndex, ArmStation armStation)
        {
            if (armStation == ArmStation.Cassette1)
            {
                if (LoadPortL.IsDoorOpen == false)
                {
                    throw new Exception("WaferStandByToLoadPort:LoadPortLNotOpen Error!!");
                }
                await Robot.PutWafer_Standby(armStation, cassetteIndex);
                await Robot.PutWafer_GoIn(armStation, cassetteIndex);
                await Robot.ReleaseWafer();
                await Robot.PutWafer_PutDown(armStation, cassetteIndex);
                await Robot.PutWafer_Retract(armStation, cassetteIndex);
                await Robot.PutWafer_Safety(armStation);
            }
            else
            {
                throw new Exception("WaferStandByToLoadPort:Param Error!!");
            }
        }
        private async Task WaferStandByToLoadPortTwoPort(int cassetteIndex, ArmStation armStation)
        {
            if (armStation == ArmStation.Cassette1 || armStation == ArmStation.Cassette2)
            {
                if (armStation == ArmStation.Cassette1)
                {
                    if (LoadPortL.IsDoorOpen == false)
                    {
                        throw new Exception("WaferStandByToLoadPort:LoadPortLNotOpen Error!!");
                    }
                    await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    if (LoadPortR.IsDoorOpen == false)
                    {
                        throw new Exception("WaferStandByToLoadPort:LoadPortRNotOpen Error!!");
                    }
                    await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition);
                }
                await Robot.PutWafer_Standby(armStation, cassetteIndex);
                await Robot.PutWafer_GoIn(armStation, cassetteIndex);
                await Robot.ReleaseWafer();
                await Robot.PutWafer_PutDown(armStation, cassetteIndex);
                await Robot.PutWafer_Retract(armStation, cassetteIndex);
                await Robot.PutWafer_Safety(armStation);
            }
            else
            {
                throw new Exception("WaferStandByToLoadPort:Param Error!!");
            }
        }
        public Task WaferLoadPortToStandBy(int cassetteIndex, ArmStation armStation)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                return Task.Run(async () =>
                           {
                               if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                               {
                                   await WaferLoadPortToStandByOnePort(cassetteIndex, armStation);
                               }
                               else
                               {
                                   await WaferLoadPortToStandByTwoPort(cassetteIndex, armStation);
                               }
                           });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task WaferLoadPortToStandByOnePort(int cassetteIndex, ArmStation armStation)
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From LoadPort Start ");
                if (armStation == ArmStation.Cassette1)
                {
                    if (LoadPortL.IsDoorOpen == false)
                    {
                        throw new FlowException("WaferLoadPortToStandBy:LoadPortLNotOpen Error!!");
                    }
                    await Robot.PickWafer_Standby(armStation, cassetteIndex);
                    await Robot.PickWafer_GoIn(armStation, cassetteIndex);
                    await Robot.FixWafer();
                    await Robot.PickWafer_LiftUp(armStation, cassetteIndex);
                    Task taskDelay = Task.Delay(vaccumDelay);
                    for (int i = 0; i <= 2; i++)//等一下真空建立
                    {
                        await taskDelay;
                        if (Robot.IsLockOK)
                        {
                            await Robot.PickWafer_Retract(armStation, cassetteIndex);
                            await Robot.PickWafer_Safety(armStation);
                            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From LoadPort End ");
                            return;
                        }
                        else
                        {
                            taskDelay = Task.Delay(vaccumDelay);
                        }
                    }
                    await Robot.ReleaseWafer();
                    await Robot.PickWafer_GoIn(armStation, cassetteIndex);
                    await Robot.PickWafer_Standby(armStation, cassetteIndex);
                    await Robot.PickWafer_Safety(armStation);
                    throw new FlowException("WaferLoadPortToStandBy:FixWafer Error!!");
                }
                else
                {
                    throw new FlowException("WaferLoadPortToStandBy:Param Error!!");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task WaferLoadPortToStandByTwoPort(int cassetteIndex, ArmStation armStation)
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From LoadPort Start ");
                if (armStation == ArmStation.Cassette1 || armStation == ArmStation.Cassette2)
                {
                    if (armStation == ArmStation.Cassette1)
                    {
                        if (LoadPortL.IsDoorOpen == false)
                        {
                            throw new FlowException("WaferLoadPortToStandBy:LoadPortLNotOpen Error!!");
                        }
                        await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition);
                    }
                    else if (armStation == ArmStation.Cassette2)
                    {
                        if (LoadPortR.IsDoorOpen == false)
                        {
                            throw new FlowException("WaferLoadPortToStandBy:LoadPortRNotOpen Error!!");
                        }
                        await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition);
                    }
                    await Robot.PickWafer_Standby(armStation, cassetteIndex);
                    await Robot.PickWafer_GoIn(armStation, cassetteIndex);
                    await Robot.FixWafer();
                    await Robot.PickWafer_LiftUp(armStation, cassetteIndex);
                    Task taskDelay = Task.Delay(vaccumDelay);
                    for (int i = 0; i <= 2; i++)//等一下真空建立
                    {
                        await taskDelay;
                        if (Robot.IsLockOK)
                        {
                            await Robot.PickWafer_Retract(armStation, cassetteIndex);
                            await Robot.PickWafer_Safety(armStation);
                            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From LoadPort End ");
                            return;
                        }
                        else
                        {
                            taskDelay = Task.Delay(vaccumDelay);
                        }
                    }
                    await Robot.ReleaseWafer();
                    await Robot.PickWafer_GoIn(armStation, cassetteIndex);
                    await Robot.PickWafer_Standby(armStation, cassetteIndex);
                    await Robot.PickWafer_Safety(armStation);
                    throw new FlowException("WaferLoadPortToStandBy:FixWafer Error!!");
                }
                else
                {
                    throw new FlowException("WaferLoadPortToStandBy:Param Error!!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Robot放片置Macro
        /// </summary>
        /// <returns></returns>
        public async Task WaferStandByToMacroAsync()
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
               {
                   if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                   {
                       await WaferStandByToMacroOnePortAsync();
                   }
                   else
                   {
                       await WaferStandByToMacroTwoPortAsync();
                   }
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task WaferStandByToMacroOnePortAsync()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Macro Start ");
                await Robot.PutWafer_Standby(ArmStation.Macro);
                await Robot.PutWafer_GoIn(ArmStation.Macro);
                await Robot.ReleaseWafer();
                Macro.FixWafer();
                await Robot.PutWafer_PutDown(ArmStation.Macro);
                Task taskDelay = Task.Delay(vaccumDelay);
                await Robot.PutWafer_Retract(ArmStation.Macro);
                await Robot.PutWafer_Safety(ArmStation.Macro);
                //等一下真空建立
                for (int i = 0; i <= 2; i++)
                {
                    await taskDelay;
                    if (Macro.IsLockOK)
                    {
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Macro End ");
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                throw new FlowException("WaferStandByToMacro:FixWafer Error!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task WaferStandByToMacroTwoPortAsync()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Macro Start ");
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisMacroTakePosition);
                await Robot.PutWafer_Standby(ArmStation.Macro);
                await Robot.PutWafer_GoIn(ArmStation.Macro);
                await Robot.ReleaseWafer();
                Macro.FixWafer();
                await Robot.PutWafer_PutDown(ArmStation.Macro);
                Task taskDelay = Task.Delay(vaccumDelay);
                await Robot.PutWafer_Retract(ArmStation.Macro);
                await Robot.PutWafer_Safety(ArmStation.Macro);
                //等一下真空建立
                for (int i = 0; i <= 2; i++)
                {
                    await taskDelay;
                    if (Macro.IsLockOK)
                    {
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Macro End ");
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                throw new FlowException("WaferStandByToMacro:FixWafer Error!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Robor從Macro取片
        /// </summary>
        /// <returns></returns>
        public async Task WaferMacroToStandBy()
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
               {
                   if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                   {
                       await WaferMacroToStandByOnePort();
                   }
                   else
                   {
                       await WaferMacroToStandByTwoPort();
                   }
               });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task WaferMacroToStandByOnePort()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Macro Start ");
                Macro.ReleaseWafer();
                await Robot.PickWafer_Standby(ArmStation.Macro);
                await Robot.PickWafer_GoIn(ArmStation.Macro);
                await Robot.FixWafer();
                await Robot.PickWafer_LiftUp(ArmStation.Macro);
                Task taskDelay = Task.Delay(vaccumDelay);
                for (int i = 0; i <= 2; i++)//等一下真空建立
                {
                    await taskDelay;
                    if (Robot.IsLockOK)
                    {
                        await Robot.PickWafer_Retract(ArmStation.Macro);
                        await Robot.PickWafer_Safety(ArmStation.Macro);
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Macro End ");
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                await Robot.ReleaseWafer();
                await Robot.PickWafer_GoIn(ArmStation.Macro);
                Macro.FixWafer();
                await Robot.PickWafer_Standby(ArmStation.Macro);
                await Robot.PickWafer_Safety(ArmStation.Macro);
                throw new FlowException("WaferMacroToStandBy:FixWafer Error!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task WaferMacroToStandByTwoPort()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Macro Start ");
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisMacroTakePosition);
                Macro.ReleaseWafer();
                await Robot.PickWafer_Standby(ArmStation.Macro);
                await Robot.PickWafer_GoIn(ArmStation.Macro);
                await Robot.FixWafer();
                await Robot.PickWafer_LiftUp(ArmStation.Macro);
                Task taskDelay = Task.Delay(vaccumDelay);
                for (int i = 0; i <= 2; i++)//等一下真空建立
                {
                    await taskDelay;
                    if (Robot.IsLockOK)
                    {
                        await Robot.PickWafer_Retract(ArmStation.Macro);
                        await Robot.PickWafer_Safety(ArmStation.Macro);
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Macro End ");
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                await Robot.ReleaseWafer();
                await Robot.PickWafer_GoIn(ArmStation.Macro);
                Macro.FixWafer();
                await Robot.PickWafer_Standby(ArmStation.Macro);
                await Robot.PickWafer_Safety(ArmStation.Macro);
                throw new FlowException("WaferMacroToStandBy:FixWafer Error!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Robor放片置Aligner
        /// </summary>
        /// <returns></returns>
        public async Task WaferStandByToAligner()
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
                {
                    if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                    {
                        await WaferStandByToAlignerOnePort();
                    }
                    else
                    {
                        await WaferStandByToAlignerTwoPort();
                    }
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task WaferStandByToAlignerOnePort()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Aligner Start ");
                await Robot.PutWafer_Standby(ArmStation.Align);
                await Robot.PutWafer_GoIn(ArmStation.Align);
                await Robot.ReleaseWafer();
                await Robot.PutWafer_PutDown(ArmStation.Align);
                await Robot.PutWafer_Retract(ArmStation.Align);
                await Robot.PutWafer_Safety(ArmStation.Align);
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Aligner End ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task WaferStandByToAlignerTwoPort()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Aligner Start ");
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
                await Robot.PutWafer_Standby(ArmStation.Align);
                await Robot.PutWafer_GoIn(ArmStation.Align);
                await Robot.ReleaseWafer();
                await Robot.PutWafer_PutDown(ArmStation.Align);
                await Robot.PutWafer_Retract(ArmStation.Align);
                await Robot.PutWafer_Safety(ArmStation.Align);
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Aligner End ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Aligner-> StandBy位置
        /// </summary>
        /// <returns></returns>
        public async Task WaferAlignerToStandBy()
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
               {
                   if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                   {
                       await WaferAlignerToStandByOnePort();
                   }
                   else
                   {
                       await WaferAlignerToStandByTwoPort();
                   }
               });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task WaferAlignerToStandByOnePort()
        {
            try
            {
                await Robot.PickWafer_Standby(ArmStation.Align);
                await Robot.PickWafer_GoIn(ArmStation.Align);
                await Robot.FixWafer();
                await Robot.PickWafer_LiftUp(ArmStation.Align);
                Task taskDelay = Task.Delay(vaccumDelay);
                for (int i = 0; i <= 2; i++)//等一下真空建立
                {
                    await taskDelay;
                    if (Robot.IsLockOK)
                    {
                        await Robot.PickWafer_Retract(ArmStation.Align);
                        await Robot.PickWafer_Safety(ArmStation.Align);
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                await Robot.PickWafer_GoIn(ArmStation.Align);
                await Robot.PickWafer_Standby(ArmStation.Align);
                await Robot.ReleaseWafer();
                await Robot.PickWafer_Safety(ArmStation.Align);
                throw new FlowException("WaferAlignerToStandBy:FixWafer Error!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task WaferAlignerToStandByTwoPort()
        {
            try
            {
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
                await Robot.PickWafer_Standby(ArmStation.Align);
                await Robot.PickWafer_GoIn(ArmStation.Align);
                await Robot.FixWafer();
                await Robot.PickWafer_LiftUp(ArmStation.Align);
                Task taskDelay = Task.Delay(vaccumDelay);
                for (int i = 0; i <= 2; i++)//等一下真空建立
                {
                    await taskDelay;
                    if (Robot.IsLockOK)
                    {
                        await Robot.PickWafer_Retract(ArmStation.Align);
                        await Robot.PickWafer_Safety(ArmStation.Align);
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                await Robot.PickWafer_GoIn(ArmStation.Align);
                await Robot.PickWafer_Standby(ArmStation.Align);
                await Robot.ReleaseWafer();
                await Robot.PickWafer_Safety(ArmStation.Align);
                throw new FlowException("WaferAlignerToStandBy:FixWafer Error!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task WaferStandByToMicro()
        {
            await Task.Run(async () =>
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    await WaferStandByToMicroOnePort();
                }
                else
                {
                    await WaferStandByToMicroTwoPort();
                }
            });
        }
        private async Task WaferStandByToMicroOnePort()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Micro Start ");
                await Robot.PutWafer_Standby(ArmStation.Micro);
                await Robot.PutWafer_GoIn(ArmStation.Micro);
                await Robot.ReleaseWafer();
                MicroFixed?.Invoke(); //顯微鏡平台固定WAFER ( 真空開)
                Task taskDelay = Task.Delay(vaccumDelay);
                await Robot.PutWafer_PutDown(ArmStation.Micro);
                await Robot.PutWafer_Retract(ArmStation.Micro);
                await Robot.PutWafer_Safety(ArmStation.Micro);
                //等一下真空建立
                await taskDelay;
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Micro End ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task WaferStandByToMicroTwoPort()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Micro Start ");
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
                await Robot.PutWafer_Standby(ArmStation.Micro);
                await Robot.PutWafer_GoIn(ArmStation.Micro);
                await Robot.ReleaseWafer();
                MicroFixed?.Invoke(); //顯微鏡平台固定WAFER ( 真空開)
                Task taskDelay = Task.Delay(vaccumDelay);
                await Robot.PutWafer_PutDown(ArmStation.Micro);
                await Robot.PutWafer_Retract(ArmStation.Micro);
                await Robot.PutWafer_Safety(ArmStation.Micro);
                //等一下真空建立
                await taskDelay;
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Load To Micro End ");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task WaferMicroToStandBy()
        {
            await Task.Run(async () =>
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    await WaferMicroToStandByOnePort();
                }
                else
                {
                    await WaferMicroToStandByTwoPort();
                }
            });
        }
        private async Task WaferMicroToStandByOnePort()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Micro Start ");
                await Robot.PickWafer_Standby(ArmStation.Micro);
                await Robot.PickWafer_GoIn(ArmStation.Micro);
                await Robot.FixWafer();
                await Robot.PickWafer_LiftUp(ArmStation.Micro);
                Task taskDelay = Task.Delay(vaccumDelay);
                for (int i = 0; i <= 2; i++)//等一下真空建立
                {
                    await taskDelay;
                    if (Robot.IsLockOK)
                    {
                        await Robot.PickWafer_Retract(ArmStation.Micro);
                        await Robot.PickWafer_Safety(ArmStation.Micro);
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Micro End ");
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                await Robot.ReleaseWafer();
                await Robot.PickWafer_GoIn(ArmStation.Micro);
                await Robot.PickWafer_Standby(ArmStation.Micro);
                await Robot.PickWafer_Safety(ArmStation.Micro);
                throw new FlowException("WaferMicroToStandBy:FixWafer Error!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task WaferMicroToStandByTwoPort()
        {
            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Micro Start ");
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
                await Robot.PickWafer_Standby(ArmStation.Micro);
                await Robot.PickWafer_GoIn(ArmStation.Micro);
                await Robot.FixWafer();
                await Robot.PickWafer_LiftUp(ArmStation.Micro);
                Task taskDelay = Task.Delay(vaccumDelay);
                for (int i = 0; i <= 2; i++)//等一下真空建立
                {
                    await taskDelay;
                    if (Robot.IsLockOK)
                    {
                        await Robot.PickWafer_Retract(ArmStation.Micro);
                        await Robot.PickWafer_Safety(ArmStation.Micro);
                        WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "UnLoad From Micro End ");
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                await Robot.ReleaseWafer();
                await Robot.PickWafer_GoIn(ArmStation.Micro);
                await Robot.PickWafer_Standby(ArmStation.Micro);
                await Robot.PickWafer_Safety(ArmStation.Micro);
                throw new FlowException("WaferMicroToStandBy:FixWafer Error!!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<Cassette> SlotMapping(ILoadPort loadPort)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                var cst = new Cassette();
                List<Wafer> wafers = new List<Wafer>();
                await loadPort.Load();
                var slots = loadPort.Slot;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].HasValue)
                    {
                        var wafer = new Wafer(i + 1);
                        wafers.Add(wafer);
                    }
                    else
                    {
                        wafers.Add(null);
                    }
                }
                cst.Wafers = wafers.ToArray();
                return cst;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
    /// <summary>
    /// 只在ProcessInitial 選擇啟用的 Loadport 與 Aligner  Inch8:L  Inch12:R
    /// 如果只有一個或不需要區分 那就選none ，統一啟用左邊的
    /// </summary>
    public enum InchType
    {
        None,
        Inch8,
        Inch12
    }


}
