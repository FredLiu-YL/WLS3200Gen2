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
    public class LoadPortStatus
    {
        /// <summary>
        /// Error的狀態 正常、可復原的錯誤、不可復原的錯誤
        /// </summary>
        public string ErrorStatus { get; set; }
        /// <summary>
        /// LoadPort目前運作的位置
        /// </summary>
        public string DeviceStatus { get; set; }
        /// <summary>
        /// 異常狀態Str:00-FF
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Cassette放置是否正確
        /// </summary>
        public bool IsCassettePutOK { get; set; }
        /// <summary>
        /// Cassette是否夾住
        /// </summary>
        public bool IsClamp { get; set; }
        /// <summary>
        /// 轉開門機構，是否有轉開
        /// </summary>
        public bool IsSwitchDoor { get; set; }
        /// <summary>
        /// 吸附門真空，是否開啟
        /// </summary>
        public bool IsVaccum { get; set; }
        /// <summary>
        /// 門是否打開了
        /// </summary>
        public bool IsDoorOpen { get; set; }
        /// <summary>
        /// Sensor確認門是否打開
        /// </summary>
        public bool IsSensorCheckDoorOpen { get; set; }
        /// <summary>
        ///  移動Cassette前進後退的平台，是否前進到可以運作的位置
        /// </summary>
        public bool IsDock { get; set; }
    }
    public class LoadPortParam
    {
        /// <summary>
        /// Wafer厚薄度(um)
        /// </summary>
        public int WaferThickness { get; set; }
        /// <summary>
        /// Cassette間距(um)
        /// </summary>
        public int CassettePitch { get; set; }
        /// <summary>
        /// Cassette間距(um)
        /// </summary>
        public int StarOffset { get; set; }
        /// <summary>
        /// Wafer間距容忍值
        /// </summary>
        public int WaferPitchTolerance { get; set; }
        /// <summary>
        /// Wafer位置容忍值
        /// </summary>
        public int WaferPositionTolerance { get; set; }
    }
    public interface ILoadPort
    {
        /// <summary>
        /// null:沒片子 true:有片子 false:片子異常
        /// </summary>
        bool?[] Slot { get; }
        /// <summary>
        /// 取得狀態
        /// </summary>
        /// <returns></returns>
        Task<LoadPortStatus> GetStatus();
        /// <summary>
        /// 取得設定參數
        /// </summary>
        /// <returns></returns>
        Task<LoadPortParam> GetParam();
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 關閉
        /// </summary>
        void Close();
        /// <summary>
        /// 打開門，會自動SlotMapping
        /// </summary>
        Task Load();
        /// <summary>
        /// 回歸到關門狀態，也可以應用在UnLoad
        /// </summary>
        Task Home();

        /// <summary>
        /// 異常復原
        /// </summary>
        Task AlarmReset();

        /// <summary>
        /// 參數設定
        /// </summary>
        Task SetParam(LoadPortParam loadPortParam);
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
    public class AlignerStatus
    {
        /// <summary>
        /// Aligner目前運作的狀態
        /// </summary>
        public string DeviceStatus { get; set; }
        /// <summary>
        /// 異常狀態Str:00-FF
        /// </summary>
        public string ErrorCode { get; set; }
        /// <summary>
        /// AlignerNotch偵測狀態
        /// </summary>
        public string NotchStatus { get; set; }
        /// <summary>
        /// 是否有Wafer
        /// </summary>
        public bool IsWafer { get; set; }
        /// <summary>
        /// 是否在原點
        /// </summary>
        public bool IsOrg { get; set; }
        /// <summary>
        /// 是否真空建立
        /// </summary>
        public bool IsVaccum { get; set; }
    }
    public interface IAligner
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Initial();
        /// <summary>
        /// 關閉
        /// </summary>
        void Close();
        /// <summary>
        /// 回到原點
        /// </summary>
        /// <returns></returns>
        Task Home();
        /// <summary>
        /// 找到Notch後，偏移多少角度(degree)
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        Task Run(double degree);
        /// <summary>
        /// 真空 IsOn=true 開啟 IsOn=fals 關閉
        /// </summary>
        /// <param name="IsOn"></param>
        /// <returns></returns>
        Task Vaccum(bool IsOn);
        /// <summary>
        /// 異常重置
        /// </summary>
        /// <returns></returns>
        Task AlarmReset();
        /// <summary>
        /// 取得目前狀態
        /// </summary>
        /// <returns></returns>
        Task<AlignerStatus> GetStatus();
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
