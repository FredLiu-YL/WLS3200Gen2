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
    public class Feeder
    {
        private bool isSotMapping;
        private Cassette cassette;
        private ILoadPort tempLoadPort;
        private IAligner tempAligner;


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
        public FeederSetting Setting;

        public async Task Home()
        {


            await Task.Run(() =>
            {


            });


        }
        /// <summary>
        /// 預載Wafer 到  Macro
        /// </summary>
        /// <param name="inchType"></param>
        /// <returns></returns>
        public async Task LoadToReadyAsync(InchType inchType)
        {
            try
            {
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

                List<Wafer> waferList = cassette.Wafers.ToList();
                await Task.Run(async () =>
                 {


                     foreach (Wafer wafer in waferList)
                     {
                         int index = waferList.IndexOf(wafer);
                         await LoadWafer(index);

                         await WaferStandByToMacro();


                     }




                 });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        /// <summary>
        /// 從Macro->Aligner->主設備
        /// </summary>
        /// <returns></returns>
        public async Task LoadAsync()
        {
            await WaferMacroToStandBy();
            await WaferStandByToAligner();
            tempAligner.Run();
            await WaferAlignerToStandBy();

        }

        public async Task LoadWafer(int waferIndex)
        {
            await RobotAxis.MoveToAsync(Setting.LoadPortPos);

            Robot.TakeWaferCassette(waferIndex);
            Robot.ArmLiftup();
            Robot.VacuumOn();
            Robot.ArmToRetract(ArmStation.Cassette);
            Robot.ArmToStandby();



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
       


        public async Task UnLoadAsync()
        {



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
                    var wafer = new Wafer();

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
