using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YuanliCore.Data;

namespace WLS3200Gen2.Model.Recipe
{
    public abstract class WaferMapping
    {

        public WaferMapping(string path)
        {

            var a = ReadWaferFile(path);

            Dies = a.dies;
            WaferSize = a.waferSize;

        }

        /// <summary>
        /// Wafer 計算起點
        /// </summary>
        public System.Drawing.Point OriginPoint { get; set; }
        /// <summary>
        /// Notch角度 0  90  180 270
        /// </summary>
        public double NotchDirection { get; set; }
        /// <summary>
        /// Map 中心位置
        /// </summary>
        public Point MapCenterPoint { get; set; }
        /// <summary>
        /// 每個Die的資訊
        /// </summary>
        public Die[] Dies { get; set; }
        /// <summary>
        /// Wafer尺寸
        /// </summary>
        public Size WaferSize { get; set; }
        /// <summary>
        /// 讀取WaferMapping
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public abstract (Die[] dies, Size waferSize) ReadWaferFile(string path);
    }

    public class SinfWaferMapping : WaferMapping
    {
        public SinfWaferMapping(string path) : base(path)
        {
        }
        public override (Die[] dies, Size waferSize) ReadWaferFile(string path)
        {
            try
            {
                Die[] die = new Die[10];
                Size waferSize = new Size();


                return (die, waferSize);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
    public class KLAWaferMapping : WaferMapping
    {
        public KLAWaferMapping(string path) : base(path)
        {
        }
        public override (Die[] dies, Size waferSize) ReadWaferFile(string path)
        {
            throw new NotImplementedException();
        }
    }
}
