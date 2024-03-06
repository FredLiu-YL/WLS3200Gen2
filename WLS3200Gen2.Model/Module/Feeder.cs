using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Data;
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
        public Feeder(IEFEMRobot robot, ILoadPort loadPortL, ILoadPort loadPortR, IMacro macro, IAligner aligner, Axis axis, MachineSetting machineSetting)
        {
            this.Robot = robot;
            this.Macro = macro;
            this.AlignerL = aligner;
            this.RobotAxis = axis;
            this.LoadPortL = loadPortL;
            this.LoadPortR = loadPortR;

            this.machineSetting = machineSetting;
        }


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
        public Cassette Cassette { get => cassette; }
        public bool IsCassetteDone { get => isCassetteDone; }
        public string WaferID { get; }


        public Task WaitEFEMonSafe = Task.CompletedTask;

        public event Action<string> WriteLog;
        public Action MicroFixed;

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

                Task robotAxisHome = RobotAxis.HomeAsync();

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

                await Task.WhenAll(aligner8Home, aligner12Home);
                await Task.WhenAll(loadPort8Home, loadPort12Home);
                await Task.WhenAll(robotAxisHome);
                await Task.WhenAll(macroHome);

                WriteLog?.Invoke("EFEM Homing End");
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async void ProcessInitial(InchType inchType, MachineSetting machineSetting, PauseTokenSource pauseToken, CancellationTokenSource cancellationToken)
        {

            this.pauseToken = pauseToken;
            this.cancelToken = cancellationToken;
            this.machineSetting = machineSetting;//重新將設定檔傳入
            Robot.cancelToken = cancellationToken;

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
                    tempLoadPort = LoadPortR;
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
        public async Task LoadToReadyAsync(int cassetteIndex, bool isLoadport1)
        {
            try
            {
                Wafer TempPre_Wafer = null;
                await Task.Run(async () =>
                 {

                     // if (processWafers.Count() > 0)
                     {
                         //      TempPre_Wafer = processWafers.Dequeue();
                         WriteLog?.Invoke($"Wafer Preload Start: CassetteIndex  {cassetteIndex}");

                         await LoadWaferFromCassette(cassetteIndex, isLoadport1);
                         await RobotAxis.MoveToAsync(machineSetting.RobotAxisMacroTakePosition);
                         await WaferStandByToMacro();
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
            await Macro.GoInnerRingCheckPos();

            Stopwatch stopwatchPitchX = new Stopwatch();
            stopwatchPitchX.Start();
            Task startPitchX = Task.Run(() =>
            {
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
                    stopwatchPitchX.Restart();
                    while (true)
                    {
                        if (stopwatchPitchX.ElapsedMilliseconds > eFEMtionRecipe.MacroTopStartPitchX)
                        {
                            Macro.InnerRingPitchX_Stop();
                            break;
                        }
                    }
                    stopwatchPitchX.Stop();
                }
            });

            Stopwatch stopwatchRollY = new Stopwatch();
            stopwatchRollY.Start();
            Task startRollY = Task.Run(() =>
            {
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
                    stopwatchRollY.Restart();
                    while (true)
                    {
                        if (stopwatchRollY.ElapsedMilliseconds > eFEMtionRecipe.MacroTopStartRollY)
                        {
                            Macro.InnerRingRollY_Stop();
                            break;
                        }
                    }
                    stopwatchRollY.Stop();
                }
            });

            Stopwatch stopwatchYawT = new Stopwatch();
            stopwatchYawT.Start();
            Task startYawT = Task.Run(async () =>
            {
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
                        if (stopwatchYawT.ElapsedMilliseconds > eFEMtionRecipe.MacroTopStartYawT)
                        {
                            Macro.InnerRingYawT_Stop();
                            break;
                        }
                    }
                    stopwatchYawT.Stop();
                }
            });

            await Task.WhenAll(startPitchX, startRollY, startYawT);
        }
        public async Task TurnBackWafer(int MacroBackStartPos)
        {
            await Macro.GoOuterRingCheckPos();
            Stopwatch stopwatchRollY = new Stopwatch();
            stopwatchRollY.Start();
            await Task.Run(() =>
           {
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
                   stopwatchRollY.Restart();
                   while (true)
                   {
                       if (stopwatchRollY.ElapsedMilliseconds > MacroBackStartPos)
                       {
                           Macro.OuterRingRollY_Stop();
                           break;
                       }
                   }
                   stopwatchRollY.Stop();
               }
           });
        }


        /// <summary>
        /// 從Macro->Aligner位置
        /// </summary>
        /// <returns></returns>
        public async Task LoadToAlignerAsync(WaferProcessStatus station, EFEMtionRecipe eFEMtionRecipe)
        {
            await Task.Run(async () =>
            {
                WriteLog?.Invoke("LoadToAligner Start");
                Task alignerHome = tempAligner.Home();
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisMacroTakePosition);
                await WaferMacroToStandBy();
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
                await alignerHome;
                await tempAligner.Vaccum(true);
                await WaferStandByToAligner();
                station = WaferProcessStatus.Complate;
                WriteLog?.Invoke("LoadToAligner  End");
                if (station == WaferProcessStatus.Select)
                {
                    await tempAligner.Run(eFEMtionRecipe.AlignerWaferIDAngle);
                    //WaferID讀取   
                }
                await tempAligner.Run(eFEMtionRecipe.AlignerMicroAngle);
            });

        }

        /// <summary>
        /// Aligner-> StandBy位置
        /// </summary>
        /// <returns></returns>
        public async Task AlignerToStandByAsync()
        {
            await Task.Run(async () =>
            {
                WriteLog?.Invoke("LoadToMicroReadyPos Start");

                await RobotAxis.MoveToAsync(machineSetting.RobotAxisAlignTakePosition);
                await WaferAlignerToStandBy();
                WriteLog?.Invoke("LoadToMicroReadyPos  End");
            });

        }
        /// <summary>
        /// wafer放進主設備
        /// </summary>
        /// <returns></returns>
        public async Task<Wafer> LoadToMicroAsync(Wafer wafer)
        {
            //await WaferAlignerToStandBy();
            await Task.Run(async () =>
            {
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
                await Robot.PutWafer_Standby(ArmStation.Micro);
                await Robot.PutWafer_GoIn(ArmStation.Micro);
                await Robot.ReleaseWafer();
                MicroFixed?.Invoke(); //顯微鏡平台固定WAFER ( 真空開)
                await Robot.PutWafer_PutDown(ArmStation.Micro);
                await Robot.PutWafer_Retract(ArmStation.Micro);
                await Robot.PutWafer_Safety(ArmStation.Micro);
            });


            return wafer;
        }


        public async Task MicroUnLoadToStandByAsync()
        {
            WriteLog?.Invoke("UnLoad Wafer Start ");
            await RobotAxis.MoveAsync(machineSetting.RobotAxisMicroTakePosition);
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
            if (isLoadPort1)
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition);
            else
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition);

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
            if (isLoadPort1)
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition);
            else
                await RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition);

            await WaferStandByToLoadPort(wafer.CassetteIndex, ArmStation.Cassette1);
            wafer.ProcessStatus.Totally = WaferProcessStatus.Complate;
            //設定 Cassette內WAFER的狀態
            //    Cassette.Wafers[wafer.CassetteIndex] = wafer;
            return wafer;
        }

        public Task WaferStandByToLoadPort(int cassetteIndex, ArmStation armStation)
        {
            return Task.Run(async () =>
           {
               await Robot.PutWafer_Standby(armStation, cassetteIndex);
               await Robot.PutWafer_GoIn(armStation, cassetteIndex);
               await Robot.ReleaseWafer();
               await Robot.PutWafer_PutDown(armStation, cassetteIndex);
               await Robot.PutWafer_Retract(armStation, cassetteIndex);
               await Robot.PutWafer_Safety(armStation);
           });


        }
        public Task WaferLoadPortToStandBy(int cassetteIndex, ArmStation armStation)
        {
            return Task.Run(async () =>
            {
                await Robot.PickWafer_Standby(armStation, cassetteIndex);
                await Robot.PickWafer_GoIn(armStation, cassetteIndex);
                await Robot.PickWafer_LiftUp(armStation, cassetteIndex);
                await Robot.FixWafer();
                await Robot.PickWafer_Retract(armStation, cassetteIndex);
                await Robot.PickWafer_Safety(armStation);
            });

        }
        /// <summary>
        /// Robot放片置Macro
        /// </summary>
        /// <returns></returns>
        public async Task WaferStandByToMacro()
        {
            try
            {
                await Task.Run(async () =>
               {
                   await Robot.PutWafer_Standby(ArmStation.Macro);
                   await Robot.PutWafer_GoIn(ArmStation.Macro);
                   await Robot.ReleaseWafer();
                   await Robot.PutWafer_PutDown(ArmStation.Macro);
                   await Robot.PutWafer_Retract(ArmStation.Macro);
                   Macro.FixWafer();
                   await Robot.PutWafer_Safety(ArmStation.Macro);
               });

            }
            catch (Exception)
            {

                throw;
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
                await Task.Run(async () =>
               {
                   Macro.ReleaseWafer();
                   await Robot.PickWafer_Standby(ArmStation.Macro);
                   await Robot.PickWafer_GoIn(ArmStation.Macro);
                   await Robot.FixWafer();
                   await Robot.PickWafer_LiftUp(ArmStation.Macro);
                   await Robot.PickWafer_Retract(ArmStation.Macro);
                   await Robot.PickWafer_Safety(ArmStation.Macro);
               });
            }
            catch (Exception)
            {

                throw;
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
                await Task.Run(async () =>
                {
                    await Robot.PutWafer_Standby(ArmStation.Align);
                    await Robot.PutWafer_GoIn(ArmStation.Align);
                    await Robot.ReleaseWafer();
                    await Robot.PutWafer_PutDown(ArmStation.Align);
                    await Robot.PutWafer_Retract(ArmStation.Align);
                    await Robot.PutWafer_Safety(ArmStation.Align);
                });

            }
            catch (Exception)
            {

                throw;
            }


        }
        public async Task WaferAlignerToStandBy()
        {
            try
            {
                await Task.Run(async () =>
               {
                   await Robot.PickWafer_Standby(ArmStation.Align);
                   await Robot.PickWafer_GoIn(ArmStation.Align);
                   await Robot.FixWafer();
                   await Robot.PickWafer_LiftUp(ArmStation.Align);
                   await Robot.PickWafer_Retract(ArmStation.Align);
                   await Robot.PickWafer_Safety(ArmStation.Align);
               });

            }
            catch (Exception)
            {

                throw;
            }


        }
        public async Task WaferStandByToMicro()
        {
            await Task.Run(async () =>
            {
                //await RobotAxis.MoveAsync(Setting.MicroPos);
                await Robot.PutWafer_Standby(ArmStation.Micro);
                await Robot.PutWafer_GoIn(ArmStation.Micro);
                await Robot.PutWafer_PutDown(ArmStation.Micro);
                await Robot.ReleaseWafer();
                await Robot.PutWafer_Retract(ArmStation.Micro);
                await Robot.PutWafer_Safety(ArmStation.Micro);
            });
        }
        public async Task WaferMicroToStandBy()
        {
            await Task.Run(async () =>
            {
                await Robot.PickWafer_Standby(ArmStation.Micro);
                await Robot.PickWafer_GoIn(ArmStation.Micro);
                await Robot.FixWafer();
                await Robot.PickWafer_LiftUp(ArmStation.Micro);
                await Robot.PickWafer_Retract(ArmStation.Micro);
                await Robot.PickWafer_Safety(ArmStation.Micro);
            });
        }




        public async Task<Cassette> SlotMapping(ILoadPort loadPort)
        {
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



        /// <summary>
        /// 從Loadport 到 手臂上
        /// </summary>
        private void Load() { }
        /// <summary>
        ///  手臂上到Loadport 
        /// </summary>
        private void UnLoad() { }
        /// <summary>
        /// 移動到Ali
        /// </summary>
        private void MoveToAliner() { }

        private void MoveToMacro() { }
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
