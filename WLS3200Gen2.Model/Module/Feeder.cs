using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Module
{
    public class Feeder
    {
        private IRobot robot;
        private IMacro macro;
        private Axis axis;
        private IAligner aligner;
        private ILoadPort loadPort;


        public Feeder(IRobot robot, ILoadPort loadPort , IMacro macro,IAligner aligner , Axis axis)
        {
            this.robot = robot;
            this.macro = macro;
            this.aligner = aligner;
            this.axis = axis;
            this.loadPort = loadPort;

        }


        public async Task Home()
        {



        }

        public async Task LoadAsync()
        {

           
            robot.Load();


        }
        public async Task UnLoadAsync()
        {



        }


    }
}
