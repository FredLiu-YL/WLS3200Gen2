using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model
{
    public class MachineSetting : AbstractRecipe
    {
        public bool IsSimulate { get; set; }
        public string MotionSettingFileName { get; set; }
        public AxisConfig TableXConfig { get; set; } = new AxisConfig();
        public AxisConfig TableYConfig { get; set; } = new AxisConfig();
        public AxisConfig TableZConfig { get; set; } = new AxisConfig();
        public AxisConfig TableRConfig { get; set; } = new AxisConfig();
        public AxisConfig RobotAxisConfig { get; set; } = new AxisConfig();
        public double InnerRingPitchXPositionPEL { get; set; }
        public double InnerRingPitchXPositionNEL { get; set; }
        public double InnerRingYawTPositionPEL { get; set; }
        public double InnerRingYawTPositionNEL { get; set; }
        public double InnerRingRollYPositionPEL { get; set; }
        public double InnerRingRollYPositionNEL { get; set; }
        public double OuterRingRollYPositionPEL { get; set; }
        public double OuterRingRollYPositionNEL { get; set; }
        /// <summary>
        /// 給人設定Bincode 的參數 ，MachineSetting是放預設值  ，如果會跟著Recipe 再自行+入
        /// </summary>
        public IEnumerable<BincodeInfo> BincodeListDefault { get; set; }
        /// <summary>
        /// 平台Robot取放料位置
        /// </summary>
        public Point TableWaferCatchPosition { get; set; }
        /// <summary>
        /// 平台Robot取放料Z軸位置
        /// </summary>
        public double TableWaferCatchPositionZ { get; set; }
        /// <summary>
        /// 平台Robot取放料T軸位置
        /// </summary>
        public double TableWaferCatchPositionR { get; set; }
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
        public double RobotAxisLoadPort1TakePosition { get; set; }
        /// <summary>
        /// Robot 橫移軸 LoadPort位置的取放料座標
        /// </summary>
        public double RobotAxisLoadPort2TakePosition { get; set; }
        /// <summary>
        /// Robot 橫移軸Micro位置的取放料座標
        /// </summary>
        public double RobotAxisMicroTakePosition { get; set; }
        public RobotType RobotsType { get; set; }
        /// <summary>
        /// 手臂RS232的COM
        /// </summary>
        public string RobotsCOM { get; set; }
        /// <summary>
        /// 相機設定檔案路徑
        /// </summary>
        public string CamerasSettingFileName { get; set; }
        public CameraType CamerasType { get; set; }
        /// <summary>
        /// 1個Pixel平台走多少
        /// </summary>
        public Point CamerasPixelTable { get; set; }
        public LoadPortType LoadPortType { get; set; }
        public LoadPortQuantity LoadPortCount { get; set; }
        /// <summary>
        /// 有無DIC
        /// </summary>
        public bool IsHaveDIC { get; set; }
        /// <summary>
        /// LoadPort1RS232的COM
        /// </summary>
        public string LoadPort1COM { get; set; }
        /// <summary>
        /// LoadPort2RS232的COM
        /// </summary>
        public string LoadPort2COM { get; set; }
        /// <summary>
        /// AlignerRS232的COM
        /// </summary>
        public string AlignerCOM { get; set; }
        /// <summary>
        /// MicroscopeRS232的COM
        /// </summary>
        public string MicroscopeCOM { get; set; }
        /// <summary>
        /// DicRS232的COM
        /// </summary>
        public string DicCOM { get; set; }
        /// <summary>
        /// StrongLamp1的COM
        /// </summary>
        public string StrongLamp1COM { get; set; }
        /// <summary>
        /// StrongLamp2的COM
        /// </summary>
        public string StrongLamp2COM { get; set; }
        /// <summary>
        /// 鏡頭參數
        /// </summary>
        public IEnumerable<MicroscopeLens> MicroscopeLensDefault { get; set; }
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
    public class RobotAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

}
