using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Data;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    public class Feeder
    {
        private bool isSotMapping;
        private Cassette cassette;

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
        public Cassette Cassette { get=> cassette; }


        public async Task Home()
        {

            await Task.Run(() =>
            {


            });

        }

        public async Task LoadAsync()
        {
            try
            {
                if (!isSotMapping)
                {
                    cassette = SlotMapping();

                    isSotMapping = true;

                }
                await Task.Run(() =>
                {
                    Robot.Load();



                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }



        public async Task UnLoadAsync()
        {



        }

        public Cassette SlotMapping()
        {
            var cst = new Cassette();
            List<Wafer> wafers = new List<Wafer>();
            var slots = LoadPort12.Slot;

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
}
