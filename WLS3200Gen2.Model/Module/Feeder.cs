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
        public bool IsInitial { get; private set; } = false;

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

        public event Action<string> WriteLog;

        public Action MicroFixed;

        public event Func<PauseTokenSource, CancellationTokenSource, Task<String>> WaferIDReady;

        public event Action<BitmapSource> WaferIDRecord;
        public async Task Home()
        {
            try
            {
                WriteLog?.Invoke("EFEM Homing Start");

                WaitEFEMonSafe = Robot.Home();

                await WaitEFEMonSafe;

                WriteLog?.Invoke("Robot Homing End");

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
                IsInitial = true;
                WriteLog?.Invoke("EFEM Homing End");
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
        public async Task LoadCassetteToMacroAsync(int cassetteIndex, bool isLoadport1)
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
                         WriteLog?.Invoke($"Wafer Preload Start: CassetteIndex  {cassetteIndex}");
                         await LoadWaferFromCassette(cassetteIndex, isLoadport1);
                         await WaferStandByToMacroAsync();
                         WriteLog?.Invoke("Wafer Preload End");
                     }
                     //   if (processWafers.Count == 0)
                     isCassetteDone = true;
                 });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task TurnWafer(EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
               
                Task startPitchX = Task.Run(async () =>
                {
                    Stopwatch stopwatchPitchX = new Stopwatch();
                    stopwatchPitchX.Start();
                    if (Math.Abs(eFEMtionRecipe.MacroTopStartPitchX) > 0)
                    {
                        if (eFEMtionRecipe.MacroTopStartPitchX > 0)
                        {
                            Macro.InnerRingPitchX_Move(true);
                        }
                        else
                        {
                            Macro.InnerRingPitchX_Move(false);
                        }
                        int countPitchX = 0;
                        stopwatchPitchX.Restart();
                        while (true)
                        {
                            if (stopwatchPitchX.ElapsedMilliseconds >= Math.Abs(eFEMtionRecipe.MacroTopStartPitchX))//if (stopwatchPitchX.ElapsedMilliseconds >= eFEMtionRecipe.MacroTopStartPitchX) countPitchX
                            {
                                Macro.InnerRingPitchX_Stop();
                                break;
                            }
                            countPitchX++;
                            await Task.Delay(1);
                        }
                        stopwatchPitchX.Stop();
                    }
                });
                Task startRollY = Task.Run(async () =>
                {
                    Stopwatch stopwatchRollY = new Stopwatch();
                    stopwatchRollY.Start();
                    if (Math.Abs(eFEMtionRecipe.MacroTopStartRollY) > 0)
                    {
                        if (eFEMtionRecipe.MacroTopStartRollY > 0)
                        {
                            Macro.InnerRingRollY_Move(true);
                        }
                        else
                        {
                            Macro.InnerRingRollY_Move(false);
                        }
                        int countRollY = 0;
                        stopwatchRollY.Restart();
                        while (true)
                        {
                            if (stopwatchRollY.ElapsedMilliseconds >= Math.Abs(eFEMtionRecipe.MacroTopStartRollY))
                            {
                                Macro.InnerRingRollY_Stop();
                                break;
                            }
                            countRollY++;
                            await Task.Delay(1);
                        }
                        //Thread.Sleep(800);
                        //Macro.InnerRingRollY_Stop();
                        stopwatchRollY.Stop();
                    }
                });
                Task startYawT = Task.Run(async () =>
                {
                    Stopwatch stopwatchYawT = new Stopwatch();
                    stopwatchYawT.Start();
                    if (Math.Abs(eFEMtionRecipe.MacroTopStartYawT) > 0)
                    {
                        if (eFEMtionRecipe.MacroTopStartYawT > 0)
                        {
                            Macro.InnerRingYawT_Move(true);
                        }
                        else
                        {
                            Macro.InnerRingYawT_Move(false);
                        }
                        stopwatchYawT.Restart();
                        while (true)
                        {
                            if (stopwatchYawT.ElapsedMilliseconds > Math.Abs(eFEMtionRecipe.MacroTopStartYawT))
                            {
                                Macro.InnerRingYawT_Stop();
                                break;
                            }
                            await Task.Delay(1);
                        }
                        stopwatchYawT.Stop();
                    }
                });

                await Task.WhenAll(startPitchX, startRollY, startYawT);
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

              
                await Task.Run(() =>
               {
                   Stopwatch stopwatchRollY = new Stopwatch();
                   stopwatchRollY.Start();
                   if (Math.Abs(MacroBackStartPos) > 0)
                   {
                       if (MacroBackStartPos > 0)
                       {
                           Macro.OuterRingRollY_Move(true);
                       }
                       else
                       {
                           Macro.OuterRingRollY_Move(false);
                       }
                       int countRollY = 0;
                       stopwatchRollY.Restart();
                       while (true)
                       {
                           if (stopwatchRollY.ElapsedMilliseconds >= Math.Abs(MacroBackStartPos))
                           {
                               Macro.OuterRingRollY_Stop();
                               break;
                           }
                           countRollY++;
                       }
                       stopwatchRollY.Stop();
                   }
               });
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
        public async Task LoadCassetteToAlignerAsync(int cassetteIndex, bool isLoadport1)
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
                        WriteLog?.Invoke($"Wafer Preload Start: CassetteIndex  {cassetteIndex}");
                        Task alignerHome = tempAligner.Home();
                        await LoadWaferFromCassette(cassetteIndex, isLoadport1);
                        await alignerHome;
                        await WaferStandByToAligner();
                        await tempAligner.FixWafer();
                        WriteLog?.Invoke("Wafer Preload End");
                    }
                    //   if (processWafers.Count == 0)
                    isCassetteDone = true;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task AlignerAsync(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
                {
                    WriteLog?.Invoke("Aligner  Start");
                    if (tempAligner.IsLockOK == false)
                    {
                        throw new FlowException("LoadToAlignerAsync:AlignerFixWafer Error!!");
                    }
                    if (station == WaferProcessStatus.Select)
                    {
                        await tempAligner.Run(eFEMtionRecipe.AlignerWaferIDAngle);
                        //WaferID讀取   
                        string result = await Reader.ReadAsync();

                        //如果讀取失敗自己KeyIn
                        if (false)
                        {
                            Task<String> waferID = WaferIDReady?.Invoke(pauseToken, cancelToken);
                            var cc = await waferID;
                        }
                    }
                    await tempAligner.Run(eFEMtionRecipe.AlignerMicroAngle);
                    WriteLog?.Invoke("Aligner  End");
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 從Macro->Aligner位置
        /// </summary>
        /// <returns></returns>
        public async Task LoadMacroToAlignerAsync(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                if (IsInitial == false) throw new FlowException("Feeder:Is Not Initial!!");
                await Task.Run(async () =>
                {
                    if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                    {
                        await LoadMacroToAlignerOnPortAsync(station, eFEMtionRecipe);
                    }
                    else
                    {
                        await LoadMacroToAlignerTwoPortAsync(station, eFEMtionRecipe);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task LoadMacroToAlignerOnPortAsync(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe)
        {
            try
            {
                WriteLog?.Invoke("LoadMacroToAligner Start");
                Task alignerHome = tempAligner.Home();
                await WaferMacroToStandBy();
                await alignerHome;
                await WaferStandByToAligner();
                await tempAligner.FixWafer();
                if (tempAligner.IsLockOK == false)
                {
                    throw new FlowException("LoadToAlignerAsync:AlignerFixWafer Error!!");
                }
                if (station == WaferProcessStatus.Select)
                {
                    await tempAligner.Run(eFEMtionRecipe.AlignerWaferIDAngle);
                    //WaferID讀取   
                    var ss = await Reader.ReadAsync();
                    WaferIDRecord?.Invoke(Reader.Image.ToBitmapSource());
                    //如果讀取失敗自己KeyIn
                    if (ss == "")
                    {
                        Task<String> waferID = WaferIDReady?.Invoke(pauseToken, cancelToken);
                        var cc = await waferID;
                    }
                }
                await tempAligner.Run(eFEMtionRecipe.AlignerMicroAngle);
                WriteLog?.Invoke("LoadMacroToAligner  End");
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
                WriteLog?.Invoke("LoadToAligner Start");
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
                if (station == WaferProcessStatus.Select)
                {
                    await tempAligner.Run(eFEMtionRecipe.AlignerWaferIDAngle);
                    //WaferID讀取   
                    var ss = await Reader.ReadAsync();
                    WaferIDRecord?.Invoke(Reader.Image.ToBitmapSource());
                    //如果讀取失敗自己KeyIn
                    if (ss == "")
                    {
                        Task<String> waferID = WaferIDReady?.Invoke(pauseToken, cancelToken);
                        var cc = await waferID;
                    }
                }
                await tempAligner.Run(eFEMtionRecipe.AlignerMicroAngle);
                WriteLog?.Invoke("LoadToAligner  End");
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
        public async Task AlignerToStandByAsync()
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
        private async Task AlignerToStandByOnePortAsync()
        {
            WriteLog?.Invoke("LoadToMicroReadyPos Start");
            await tempAligner.ReleaseWafer();
            await WaferAlignerToStandBy();
            WriteLog?.Invoke("LoadToMicroReadyPos  End");
        }
        private async Task AlignerToStandByTwoPortAsync()
        {
            WriteLog?.Invoke("LoadToMicroReadyPos Start");
            await RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
            await tempAligner.ReleaseWafer();
            await WaferAlignerToStandBy();
            WriteLog?.Invoke("LoadToMicroReadyPos  End");
        }
        /// <summary>
        /// wafer放進主設備
        /// </summary>
        /// <returns></returns>
        public async Task LoadToMicroAsync()
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
        private async Task LoadToMicroOnePortAsync()
        {
            await Robot.PutWafer_Standby(ArmStation.Micro);
            await Robot.PutWafer_GoIn(ArmStation.Micro);
            await Robot.ReleaseWafer();
            MicroFixed?.Invoke(); //顯微鏡平台固定WAFER ( 真空開)
            await Robot.PutWafer_PutDown(ArmStation.Micro);
            await Robot.PutWafer_Retract(ArmStation.Micro);
            await Robot.PutWafer_Safety(ArmStation.Micro);
        }
        private async Task LoadToMicroTwoPortAsync()
        {
            await RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
            await Robot.PutWafer_Standby(ArmStation.Micro);
            await Robot.PutWafer_GoIn(ArmStation.Micro);
            await Robot.ReleaseWafer();
            MicroFixed?.Invoke(); //顯微鏡平台固定WAFER ( 真空開)
            await Robot.PutWafer_PutDown(ArmStation.Micro);
            await Robot.PutWafer_Retract(ArmStation.Micro);
            await Robot.PutWafer_Safety(ArmStation.Micro);
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
            WriteLog?.Invoke("UnLoad Wafer Start ");
            await WaferMicroToStandBy();
            WriteLog?.Invoke("UnLoad Wafer End ");
        }
        private async Task MicroUnLoadToStandByTwoPortAsync()
        {
            WriteLog?.Invoke("UnLoad Wafer Start ");
            await RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
            await WaferMicroToStandBy();
            WriteLog?.Invoke("UnLoad Wafer End ");
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
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                await Robot.ReleaseWafer();
                await Robot.PickWafer_GoIn(ArmStation.Macro);
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
                        return;
                    }
                    else
                    {
                        taskDelay = Task.Delay(vaccumDelay);
                    }
                }
                await Robot.ReleaseWafer();
                await Robot.PickWafer_GoIn(ArmStation.Macro);
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
                await Robot.PutWafer_Standby(ArmStation.Align);
                await Robot.PutWafer_GoIn(ArmStation.Align);
                await Robot.ReleaseWafer();
                await Robot.PutWafer_PutDown(ArmStation.Align);
                await Robot.PutWafer_Retract(ArmStation.Align);
                await Robot.PutWafer_Safety(ArmStation.Align);
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
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
                await Robot.PutWafer_Standby(ArmStation.Align);
                await Robot.PutWafer_GoIn(ArmStation.Align);
                await Robot.ReleaseWafer();
                await Robot.PutWafer_PutDown(ArmStation.Align);
                await Robot.PutWafer_Retract(ArmStation.Align);
                await Robot.PutWafer_Safety(ArmStation.Align);
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
