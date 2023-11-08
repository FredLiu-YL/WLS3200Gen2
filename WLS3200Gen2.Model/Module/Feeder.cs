using Nito.AsyncEx;
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


        public Feeder(IRobot robot, ILoadPort loadPort, IMacro macro, IAligner aligner, Axis axis)
        {
            this.Robot = robot;
            this.Macro = macro;
            this.Aligner12 = aligner;
            this.RobotAxis = axis;
            this.LoadPort12 = loadPort;

        }

        public IRobot Robot { get; }
        public IMacro Macro { get; }
        public Axis RobotAxis { get; }
        public IAligner Aligner8 { get; }
        public IAligner Aligner12 { get; }
        public ILoadPort LoadPort8 { get; }
        public ILoadPort LoadPort12 { get; }
        public Cassette Cassette { get => cassette; }


        public async Task Home()
        {

            await Task.Run(() =>
            {


            });

        }

        public async Task LoadAsync(InchType inchType)
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

                         await LoadWafer(index);
                     }




                 });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public async Task LoadWafer(int waferIndex)
        {

            Robot.Load();

        }
        public async Task LoadWaferToMacro()
        {
            try
            {
                await Task.Run(() =>
                {
                    Robot.MoveToMacro();

                    Macro.FixWafer();
                
                });
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
    }


    public enum InchType
    {
        Inch8,
        Inch12
    }
}
