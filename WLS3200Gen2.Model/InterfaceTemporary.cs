using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WLS3200Gen2.Model
{

    public interface ILoadPort
    {
        /// <summary>
        /// 是否有Mapping資訊
        /// </summary>
        bool IsMapping { get; }
        /// <summary>
        /// null:沒片子 true:有片子 false:片子異常
        /// </summary>
        bool?[] Slot { get; }


        /// <summary>
        /// Error的狀態 正常、可復原的錯誤、不可復原的錯誤
        /// </summary>
        string ErrorStatus { get; }
        /// <summary>
        /// LoadPort目前運作的位置
        /// </summary>
        string DeviceStatus { get; }
        /// <summary>
        /// 異常狀態Str:00-FF
        /// </summary>
        string ErrorCode { get; }
        /// <summary>
        /// Cassette放置是否正確
        /// </summary>
        bool IsCassettePutOK { get; }
        /// <summary>
        /// Cassette是否夾住
        /// </summary>
        bool IsClamp { get; }
        /// <summary>
        /// 轉開門機構，是否有轉開
        /// </summary>
        bool IsSwitchDoor { get; }
        /// <summary>
        /// 吸附門真空，是否開啟
        /// </summary>
        bool IsVaccum { get; }
        /// <summary>
        /// 門是否打開了
        /// </summary>
        bool IsDoorOpen { get; }
        /// <summary>
        /// Sensor確認門是否打開
        /// </summary>
        bool IsSensorCheckDoorOpen { get; }
        /// <summary>
        ///  移動Cassette前進後退的平台，是否前進到可以運作的位置
        /// </summary>
        bool IsDock { get; }

        /// <summary>
        /// Wafer厚薄度(um)
        /// </summary>
        int WaferThickness { get; set; }
        /// <summary>
        /// Cassette間距(um)
        /// </summary>
        int CassettePitch { get; set; }
        /// <summary>
        /// Cassette間距(um)
        /// </summary>
        int StarOffset { get; set; }
        /// <summary>
        /// Wafer間距容忍值
        /// </summary>
        int WaferPitchTolerance { get; set; }
        /// <summary>
        /// Wafer位置容忍值
        /// </summary>
        int WaferPositionTolerance { get; set; }



        // 初始化
        void Initial();
        /// <summary>
        /// 打開門，且自動SlotMapping
        /// </summary>
        void Load();
        /// <summary>
        /// 回歸到關門狀態，也可以應用在UnLoad
        /// </summary>
        void Home();
        /// <summary>
        /// 取得目前狀態
        /// </summary>
        void GetStatus();
        /// <summary>
        /// 取得參數設定
        /// </summary>
        void GetParam();
        /// <summary>
        /// 設定參數
        /// </summary>
        void SetParam();
        /// <summary>
        /// 異常復原
        /// </summary>
        void AlarmReset();
    }
    public interface IRobot
    {
        void Initial();

        void Home();
        /// <summary>
        /// 手臂真空開
        /// </summary>
        void VacuumOn();

        /// <summary>
        /// 手臂真空關
        /// </summary>
        void VacuumOff();
    }
    public interface IEFEMRobot : IRobot
    {

        PauseTokenSource pauseToken { get; set; }
        CancellationTokenSource cancelToken { get; set; }



        /// <summary>
        /// 取片 伸出手臂進卡匣 (尚未抬起或下降)
        /// </summary>
        /// <param name="Layer"></param>
        void TakeWaferCassette(int layer);
        /// <summary>
        /// 放片 伸出手臂進卡匣 (尚未抬起或下降)
        /// </summary>
        /// <param name="Layer"></param>
        void PutBackWaferCassette(int layer);


        /// <summary>
        /// 取片 伸出手臂 (尚未抬起或下降)
        /// </summary>
        /// <param name="Layer"></param>
        void TakeWafer(ArmStation armPosition);

        /// <summary>
        /// 放片 伸出手臂 (尚未抬起或下降)
        /// </summary>
        /// <param name="Layer"></param>
        void PutBackWafer(ArmStation armPosition);


        /// <summary>
        /// 手臂抬起
        /// </summary>
        void ArmLiftup();
        /// <summary>
        /// 手臂放下
        /// </summary>
        void ArmPutdown();
        /// <summary>
        /// 手臂轉成待機姿態
        /// </summary>
        void ArmToStandby();
        /// <summary>
        /// 手臂收回
        /// </summary>
        void ArmToRetract(ArmStation armPosition);
        /// <summary>
        /// 接料位置
        /// </summary>
        /// <param name="armPosition"></param>
        void ArmcatchPos(ArmStation armPosition);


    }
    public enum ArmStation
    {
        Cassette,
        Align,
        Macro,
        Micro
    }


    public interface IAligner
    {
        void Initial();
        void Home();
        void Run(double degree);
        void VaccumOn();
        void VaccumOff();
        void AlarmReset();

    }
    public interface IMacro
    {
        void Initial();
        void FixWafer();
        void ReleaseWafer();


        void Vertical(double pos);
        void Flip(double pos);

        void Rotate(double pos);

        void Home();



    }


    public class DieInfo
    {
        /// <summary>
        /// Wafer上的座標
        /// </summary>
        public System.Drawing.Point Index;
        /// <summary>
        /// Die 直徑 um
        /// </summary>
        public Size DieSize { get; set; }

    }
    public class WaferInfo
    {
        /// <summary>
        /// Wafer名稱
        /// </summary>
        public string WaferID { get; set; }
        /// <summary>
        /// Wafer 直徑 um
        /// </summary>
        public Size WaferSize { get; set; }
        /// <summary>
        /// Wafer Col  Row數量
        /// </summary>
        public Size WaferLayoutSize { get; set; }
        /// <summary>
        /// 切割道寬度  um
        /// </summary>
        public double ScribeLine { get; set; }

        public DieInfo[] Dies { get; set; }
    }
}
