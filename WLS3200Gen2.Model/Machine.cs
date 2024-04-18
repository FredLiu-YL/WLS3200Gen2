using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Module;
using WLS3200Gen2.Module;
using YuanliCore.Interface;
using YuanliCore.Model.Communication;
using YuanliCore.Model.Interface;

namespace WLS3200Gen2.Model
{
    public partial class Machine
    {
        private ILoadPort loadPort;
        private IEFEMRobot robot;
        private IAligner aligner;
        private IMotionController motionController;
        private ICamera camera;
        private IMicroscope microscope;
        private IMacro macro;
        private MachineSetting machineSetting;
        private ICIM cim;
        private bool isSimulate;


        public Machine(bool isSimulate, MachineSetting machineSetting)
        {

            this.machineSetting = machineSetting;
            this.isSimulate = isSimulate;

        }
        public Feeder Feeder { get; private set; }
        public MicroDetection MicroDetection { get; private set; }

        public StackLight StackLight { get; private set; }
    }
}
