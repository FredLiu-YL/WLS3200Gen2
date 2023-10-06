﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Module;
using YuanliCore.Interface;

namespace WLS3200Gen2.Model
{
    public partial class Machine
    {
        private ILoadPort loadPort;
        private IRobot robot;
        private IAligner aligner;
        private IMotionController motionController;
        private ICamera camera;
        private IMicroscope microscope;
        private IMacro macro;
        private MachineSetting machineSetting;
        private Feeder feeder;
        private MicroDetection microDetection;
        private bool isSimulate ;
        public Machine(bool isSimulate, MachineSetting machineSetting)
        {

            this.machineSetting = machineSetting;
            this.isSimulate = isSimulate;
        }




    }
}
