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
        public string LogPath { get; set; }
        public string ResultPath { get; set; }
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
        /// Notch放置平台正面角度差
        /// </summary>
        public double AlignerMicroOffset { get; set; }
        /// <summary>
        /// Notch放置LoadPort正面角度差
        /// </summary>
        public double AlignerUnLoadOffset { get; set; }
        /// <summary>
        /// 給人設定Bincode 的參數 ，MachineSetting是放預設值  ，如果會跟著Recipe 再自行+入
        /// </summary>
        public IEnumerable<BincodeInfo> BincodeListDefault { get; set; }
        /// <summary>
        /// 平台Robot取放料位置
        /// </summary>
        public Point TableWaferCatchPosition { get; set; }
        /// <summary>
        /// 平台中心位置
        /// </summary>
        public Point TableCenterPosition { get; set; }
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
        /// <summary>
        /// SubDie參數
        /// </summary>
        public IEnumerable<SubDiePoint> SubDiePoint { get; set; }
        /// <summary>
        /// 原本的detectionPoint轉換Sub成新的Die
        /// </summary>
        /// <param name="detectionPoint"></param>
        /// <returns></returns>
        public IEnumerable<DetectionPoint> TransSubDetectionPoints(IEnumerable<DetectionPoint> detectionPoint, Point dieSize, Point cameraPixal)
        {
            List<DetectionPoint> DetectionPointList = new List<DetectionPoint>();
            int indexHeader = 1;
            foreach (DetectionPoint orgDetectionPoint in detectionPoint)
            {
                bool isFindSubProgram = false;
                if (orgDetectionPoint.SubProgramName == "DieAll")
                {
                    if (MicroscopeLensDefault.Count() - 1 < orgDetectionPoint.LensIndex)
                    {
                        isFindSubProgram = true;
                        List<Point> offsetPoint = TransLensDieAllPoint(orgDetectionPoint.LensIndex, dieSize, cameraPixal);
                        foreach (Point item in offsetPoint)
                        {
                            DetectionPoint addDetectionPoint = orgDetectionPoint.Copy();
                            addDetectionPoint.IndexHeader = indexHeader;
                            addDetectionPoint.IndexX = orgDetectionPoint.IndexX;
                            addDetectionPoint.IndexY = orgDetectionPoint.IndexY;
                            addDetectionPoint.Position = new Point(orgDetectionPoint.Position.X + item.X,
                                                                   orgDetectionPoint.Position.Y + item.Y);
                            indexHeader += 1;
                        }
                    }
                    break;
                }
                else
                {
                    if (SubDiePoint != null)
                    {
                        foreach (SubDiePoint item2 in SubDiePoint)
                        {
                            if (orgDetectionPoint.SubProgramName == item2.SubProgramName)
                            {
                                isFindSubProgram = true;
                                foreach (DetectionPoint subDetectionPoint in item2.DetectionPoints)
                                {
                                    DetectionPoint addDetectionPoint = subDetectionPoint.Copy();
                                    addDetectionPoint.IndexHeader = indexHeader;
                                    addDetectionPoint.IndexX = orgDetectionPoint.IndexX;
                                    addDetectionPoint.IndexY = orgDetectionPoint.IndexY;
                                    addDetectionPoint.Position = new Point(orgDetectionPoint.Position.X + subDetectionPoint.Position.X,
                                                                           orgDetectionPoint.Position.Y + subDetectionPoint.Position.Y);
                                    indexHeader += 1;
                                }
                                break;
                            }
                        }

                    }
                }
                if (isFindSubProgram == false)
                {
                    DetectionPoint addDetectionPoint = orgDetectionPoint.Copy();
                    addDetectionPoint.IndexHeader = indexHeader;
                    DetectionPointList.Add(addDetectionPoint);
                    indexHeader += 1;
                }

            }
            //return this.DetectionPoints.Select(dp => dp.DeepCopy()).ToList();
            return DetectionPointList;
        }
        /// <summary>
        /// 轉換Lens參數DieAll點位
        /// </summary>
        /// <returns></returns>
        public List<Point> TransLensDieAllPoint(int lensIdx, Point dieSize, Point cameraPixal)
        {
            try
            {
                List<Point> offsetPoint = new List<Point>();
                var lensParam = MicroscopeLensDefault.ElementAt(lensIdx);
                //視野範圍減掉10讓照片可以重疊
                Point fov = new Point(lensParam.RatioX * cameraPixal.X - 10, lensParam.RatioY * cameraPixal.Y - 10);
                //全部範圍增加5是怕邊緣沒拍到
                Point newDieSize = new Point(dieSize.X + lensParam.RatioX + 5, dieSize.Y + +lensParam.RatioY + 5);
                Point newDieCenter = new Point(newDieSize.X / 2, newDieSize.Y / 2);
                //中心到邊緣要拍幾張照片
                int countX = Convert.ToInt32((newDieCenter.X - fov.X / 2) / fov.X);
                if ((newDieCenter.X - fov.X / 2) % fov.X != 0)//要多拍一張
                {
                    countX++;
                }
                int countY = Convert.ToInt32((newDieCenter.Y - fov.Y / 2) / fov.Y);
                if ((newDieCenter.Y - fov.Y / 2) % fov.Y != 0)//要多拍一張
                {
                    countY++;
                }
                //左上角第一張照片位置
                Point firstPicturePos = new Point(newDieCenter.X - fov.X * countX, newDieCenter.Y - fov.Y * countY);


                for (int j = 0; j < countY * 2; j++)
                {
                    if (j % 2 == 0)
                    {
                        for (int i = 0; i < countX * 2; i++)
                        {
                            offsetPoint.Add(new Point(firstPicturePos.X + i * fov.X, firstPicturePos.Y + j * fov.Y));
                        }
                    }
                    else
                    {
                        for (int i = countX * 2; i > 0; i--)
                        {
                            offsetPoint.Add(new Point(firstPicturePos.X + i * fov.X, firstPicturePos.Y + j * fov.Y));
                        }
                    }

                }
                return offsetPoint;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

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
    public class SubDiePoint
    {
        /// <summary>
        /// Sub的名稱
        /// </summary>
        public string SubProgramName { get; set; }
        /// <summary>
        /// 檢查點
        /// </summary>
        public IEnumerable<DetectionPoint> DetectionPoints { get; set; }
    }
}
