using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YuanliCore.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model
{
    public class MachineSetting : AbstractRecipe
    {

        public AxisConfig TableXConfig { get; set; } = new AxisConfig();
        public AxisConfig TableYConfig { get; set; } = new AxisConfig();
        public AxisConfig TableRConfig { get; set; } = new AxisConfig();
        public AxisConfig RobotAxisConfig { get; set; } = new AxisConfig();

        public Point TableWaferCatchPosition { get; set; }
        /// <summary>
        /// Robot 橫移軸 待機位置的座標
        /// </summary>
        public double RobotAxisStandbyPosition { get; set; }
        /// <summary>
        /// Robot 橫移軸Aligner位置的取放料座標
        /// </summary>
        public double RobotAxisAlignTakePosition { get; set; }
        /// <summary>
        /// Robot 橫移軸Macro位置的取放料座標
        /// </summary>
        public double RobotAxisMacroTakePosition { get; set; }
        /// <summary>
        /// Robot 橫移軸 LoadPort位置的取放料座標
        /// </summary>
        public double RobotAxisLoadPortTakePosition { get; set; }
        /// <summary>
        /// Robot 橫移軸Micro位置的取放料座標
        /// </summary>
        public double RobotAxisMicroTakePosition { get; set; }


        /// <summary>
        /// Die 判定需給定BinCode
        /// </summary>
        public BinCode[] BinCodes { get; set; }

        public RobotType RobotsType { get; set; }
        public CameraType CamerasType { get; set; }

        public LoadPortType LoadPortType { get; set; }
        public LoadPortQuantity LoadPortCount { get; set; }

     
    }



    public enum RobotType
    {
        Hirata,
        Tazimo

    }

    public enum CameraType
    {
        ImageSource,
        IDS

    }
    public enum MotionControlorType
    {
        ADLink,
        ADTech,

    }
    public enum LoadPortType
    {
        Hirata,
        Tazimo

    }
    public enum LoadPortQuantity
    {
        Single,
        Pair

    }


    public class BinCode
    {
        public string Code { get; set; }
        public string Describe { get; set; }

        public Brush CodeColor { get; set; } = Brushes.White;


    }
}
