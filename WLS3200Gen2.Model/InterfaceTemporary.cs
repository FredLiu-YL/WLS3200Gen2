using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// 從Loadport 到 手臂上
        /// </summary>
        void Load();
        /// <summary>
        ///  手臂上到Loadport 
        /// </summary>
        void UnLoad();

        void MoveToAliner();

        void MoveToMacro();
        void VacuumOn();
        void VacuumOff();

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
