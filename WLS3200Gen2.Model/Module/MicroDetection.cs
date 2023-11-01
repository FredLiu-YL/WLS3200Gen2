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
        private IMicroscope microscope;
        private Axis axisX;
        private Axis axisY;
        private SignalDO tableVacuum;
        private SignalDO liftPin;
        public MicroDetection(ICamera camera, IMicroscope microscope, Axis[] axes , SignalDO[] outputs, SignalDI[] inputs )
        {
            this.camera = camera;
            this.microscope = microscope;
            axisX = axes[0];
            axisY = axes[1];
            tableVacuum = outputs[1];
            liftPin = outputs[2];

        }


        public async Task Home()
        {



        }
    }
}
