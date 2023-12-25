using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model
{
    public class MachineSetting
    {

        public AxisConfig TableXConfig { get; set; } = new AxisConfig();
        public AxisConfig TableYConfig { get; set; } = new AxisConfig();
        public AxisConfig TableRConfig { get; set; } = new AxisConfig();
        public AxisConfig RobotAxisConfig { get; set; } = new AxisConfig();

        public Point TableWaferCatchPosition { get; set; }
        public double RobotAxisStandbyPosition { get; set; }
        public double RobotAxisAlignTakePosition { get; set; }
        public double RobotAxisMacroTakePosition { get; set; }
        public double RobotAxisLoadPortTakePosition { get; set; }


        public RobotType RobotsType { get; set; }
        public CameraType CamerasType { get; set; }





    }



    public enum RobotType
    {


    }

    public enum CameraType
    {


    }
    public enum MotionControlorType
    {


    }
}
