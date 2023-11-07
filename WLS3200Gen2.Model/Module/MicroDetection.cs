using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;
using YuanliCore.Model.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    public class MicroDetection
    {
        private ICamera camera;
      
      
      
        public MicroDetection(ICamera camera, IMicroscope microscope, Axis[] axes , DigitalOutput[] outputs, DigitalInput[] inputs )
        {
            this.camera = camera;
            this.Microscope = microscope;
            AxisX = axes[0];
            AxisY = axes[1];
            TableVacuum = outputs[1];
            LiftPin = outputs[2];

        }

        public Axis AxisX { get; }
        public Axis AxisY { get; }
        public DigitalOutput TableVacuum { get; }
        public DigitalOutput LiftPin { get; }
        public IMicroscope Microscope { get; }


        public async Task Home()
        {

            await Task.Run(() => { });

        }
    }
}
