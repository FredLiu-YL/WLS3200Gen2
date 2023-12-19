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

        bool IsMapping { get; }

        bool?[] Slot { get; }

        // 初始化
        void Initial();
        /// <summary>
        /// 做在籍檢知
        /// </summary>
        void SlotMapping();
        void Home();


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

        void Run();

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
