﻿using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private Cassette cassette;
        private ILoadPort tempLoadPort;
        private IAligner tempAligner;

        private PauseTokenSource pauseToken = new PauseTokenSource();
        private CancellationTokenSource cancelToken = new CancellationTokenSource();

        private Queue<Wafer> processWafers;
        //預載暫存WAFER紀錄
        private Wafer processTempPre_Wafer;



        public Feeder(IEFEMRobot robot, ILoadPort loadPort, IMacro macro, IAligner aligner, Axis axis)
        {
            this.Robot = robot;
            this.Macro = macro;
            this.Aligner12 = aligner;
            this.RobotAxis = axis;
            this.LoadPort12 = loadPort;

        }

        public IEFEMRobot Robot { get; }
        public IMacro Macro { get; }
        public Axis RobotAxis { get; }
        public IAligner Aligner8 { get; }
        public IAligner Aligner12 { get; }
        public ILoadPort LoadPort8 { get; }
        public ILoadPort LoadPort12 { get; }
        public Cassette Cassette { get => cassette; }
        public bool IsCassetteDone { get => isCassetteDone; }

        public FeederSetting Setting;

        public Task WaitEFEMonSafe = Task.CompletedTask;


        public Action MicroFixed;

        public async Task Home()
        {
            try
            {
                WaitEFEMonSafe = Task.Run(() =>
              {
                  Robot.Home();

              });

                await WaitEFEMonSafe;

                Task aligner8Home = Task.Run(() =>
                {
                    if (Aligner8 != null)
                        Aligner8.Home();

                });
                Task robotAxisHome = RobotAxis.HomeAsync();


                Task aligner12Home = Task.Run(() =>
                {
                    Aligner12.Home();

                });
                Task loadPort8Home = Task.Run(() =>
                {
                    if (LoadPort8 != null)
                        LoadPort8.Home();

                });
                Task loadPort12Home = Task.Run(() =>
                {
                    LoadPort12.Home();

                });
                await Task.WhenAll(aligner8Home, aligner12Home);
                await Task.WhenAll(loadPort8Home, loadPort12Home, robotAxisHome);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void ProcessInitial(InchType inchType, PauseTokenSource pauseToken, CancellationTokenSource cancellationToken)
        {

            this.pauseToken = pauseToken;
            this.cancelToken = cancellationToken;

            Robot.cancelToken = cancellationToken;

            //判斷 8吋 或 12吋 啟用不同的硬體裝置
            switch (inchType)
            {
                case InchType.Inch8:
                    tempAligner = Aligner8;
                    tempLoadPort = LoadPort8;
                    break;
                case InchType.Inch12:
                    tempAligner = Aligner12;
                    tempLoadPort = LoadPort12;
                    break;

                default:
                    break;
            }

            //如果沒有做過Mapping 
            if (!isSotMapping)
            {
                cassette = SlotMapping(tempLoadPort);

                isSotMapping = true;

            }
            //判斷有WAFER的格子
            var waferuse = cassette.Wafers.Where(w => w != null);
            processWafers = new Queue<Wafer>(waferuse);


        }
        public void ProcessEnd()
        {



        }

        /// <summary>
        /// 預載Wafer 到  Macro
        /// </summary>
        /// <param name="inchType"></param>
        /// <returns></returns>
        public Task LoadToReadyAsync()
        {
            try
            {

                return Task.Run(async () =>
                 {

                     if (processWafers.Count() > 0)
                     {
                         processTempPre_Wafer = processWafers.Dequeue();
                       

                          await LoadWaferFromCassette(processTempPre_Wafer.CassetteIndex);
                         await WaferStandByToMacro();

                     }




                     if (processWafers.Count == 0)
                         isCassetteDone = true;



                 });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// 從Macro->Aligner-> StandBy位置
        /// </summary>
        /// <returns></returns>
        public async Task LoadToMicroReadyAsync()
        {
            await WaferMacroToStandBy();
            await WaferStandByToAligner();
            tempAligner.Run(0);
            await WaferAlignerToStandBy();


        }
        /// <summary>
        /// wafer放進主設備
        /// </summary>
        /// <returns></returns>
        public async Task<Wafer> LoadToMicroAsync()
        {
   
            
            //await WaferAlignerToStandBy();
  
            await RobotAxis.MoveToAsync(Setting.MicroPos);
            Robot.PutBackWafer(ArmStation.Micro);
            Robot.ArmcatchPos(ArmStation.Micro);
            Robot.VacuumOff();
            MicroFixed?.Invoke(); //顯微鏡平台固定WAFER ( 真空開)
            Robot.ArmPutdown();
            Robot.ArmToRetract(ArmStation.Micro);
            Robot.ArmToStandby();

            return processTempPre_Wafer;
        }


        public async Task UnLoadAsync(Wafer wafer)
        {
            await WaferMicroToStandBy();
            await UnLoadWaferToCassette(wafer);

        }
        /// <summary>
        /// 從Loadport 拿一片wafer
        /// </summary>
        /// <param name="cassetteIndex">第幾格</param>
        /// <returns></returns>
        public async Task LoadWaferFromCassette(int cassetteIndex)
        {


            await RobotAxis.MoveToAsync(Setting.LoadPortPos);

            Robot.TakeWaferCassette(cassetteIndex);
            Robot.ArmLiftup();
            Robot.VacuumOn();
            Robot.ArmToRetract(ArmStation.Cassette);
            Robot.ArmToStandby();

            //設定 Cassette內WAFER的狀態
            Cassette.Wafers[cassetteIndex].ProcessStatus = WaferProcessStatus.InProgress;

        }
        /// <summary>
        /// wafer放回loadport
        /// </summary>
        /// <param name="cassetteIndex">第幾格</param>
        /// <returns></returns>
        public async Task UnLoadWaferToCassette(Wafer wafer)
        {
            await RobotAxis.MoveToAsync(Setting.LoadPortPos);
            Robot.PutBackWafer(ArmStation.Cassette);
            Robot.ArmcatchPos(ArmStation.Cassette);
            Robot.VacuumOff();

            Robot.ArmPutdown();
            Robot.ArmToRetract(ArmStation.Cassette);
            Robot.ArmToStandby();

            wafer.ProcessStatus = WaferProcessStatus.Complate;
            //設定 Cassette內WAFER的狀態
            Cassette.Wafers[wafer.CassetteIndex] = wafer;

        }

        public async Task WaferStandByToMacro()
        {
            try
            {


                await RobotAxis.MoveToAsync(Setting.MacroPos);
                Robot.PutBackWafer(ArmStation.Macro);
                Robot.ArmcatchPos(ArmStation.Macro);
                Robot.VacuumOff();
                Robot.ArmPutdown();

                Robot.ArmToRetract(ArmStation.Macro);
                Macro.FixWafer();
                Robot.ArmToStandby();


            }
            catch (Exception)
            {

                throw;
            }


        }
        public async Task WaferMacroToStandBy()
        {
            try
            {


                await RobotAxis.MoveToAsync(Setting.MacroPos);
                Macro.ReleaseWafer();
                Robot.TakeWafer(ArmStation.Macro);
                Robot.ArmcatchPos(ArmStation.Macro);
                Robot.VacuumOn();
                Robot.ArmLiftup();
                Robot.ArmToRetract(ArmStation.Macro);
                Robot.ArmToStandby();




            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task WaferStandByToAligner()
        {
            try
            {


                await RobotAxis.MoveToAsync(Setting.AlignPos);
                Robot.PutBackWafer(ArmStation.Align);
                Robot.ArmcatchPos(ArmStation.Align);
                Robot.VacuumOff();
                Robot.ArmPutdown();
                Robot.ArmToRetract(ArmStation.Align);
                Robot.ArmToStandby();


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

                await RobotAxis.MoveToAsync(Setting.AlignPos);

                Robot.TakeWafer(ArmStation.Align);
                Robot.ArmcatchPos(ArmStation.Align);
                Robot.VacuumOn();
                Robot.ArmLiftup();
                Robot.ArmToRetract(ArmStation.Align);
                Robot.ArmToStandby();

            }
            catch (Exception)
            {

                throw;
            }


        }

        public async Task WaferMicroToStandBy()
        {
            await RobotAxis.MoveAsync(Setting.MicroPos);
            Robot.TakeWafer(ArmStation.Micro);
            Robot.ArmcatchPos(ArmStation.Micro);
            Robot.VacuumOn();
            Robot.ArmLiftup();
            Robot.ArmToRetract(ArmStation.Micro);
            Robot.ArmToStandby();

        }
     

        public Cassette SlotMapping(ILoadPort loadPort)
        {
            var cst = new Cassette();
            List<Wafer> wafers = new List<Wafer>();
            var slots = loadPort.Slot;

            foreach (var slot in slots)
            {

                if (slot.HasValue)
                {
                    var wafer = new Wafer(0);

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


    public enum InchType
    {
        Inch8,
        Inch12
    }

    public class FeederSetting
    {
        public double LoadPortPos { get; set; }
        public double AlignPos { get; set; }
        public double MacroPos { get; set; }
        public double MicroPos { get; set; }

    }
}
