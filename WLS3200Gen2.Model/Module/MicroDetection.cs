using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Module
{
    public class MicroDetection
    {
        private ICamera camera;
        private IMicroscope microscope;
        private Axis axisX;
        private Axis axisY;
        private DigitalOutput tableVacuum;
        private DigitalOutput liftPin;
        public MicroDetection(ICamera camera, IMicroscope microscope, Axis[] axes , DigitalOutput[] outputs, DigitalInput[] inputs )
        {
            this.camera = camera;
            this.microscope = microscope;
            axisX = axes[0];
            axisY = axes[1];
            tableVacuum = outputs[9];
            liftPin = outputs[10];

        }


        public async Task Home()
        {



        }
    }
}
