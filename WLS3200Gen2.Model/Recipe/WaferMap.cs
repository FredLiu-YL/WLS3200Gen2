using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YuanliCore.Data;

namespace WLS3200Gen2.Model.Recipe
{
    /// <summary>
    /// 紀錄Map圖 的全部座標
    /// </summary>
    public class WaferMapping
    {
        /// <summary>
        /// Wafer 計算起點
        /// </summary>
        public System.Drawing.Point OriginPoint { get; set; }

        /// <summary>
        /// Notch角度 0  90  180 270
        /// </summary>
        public double NotchDirection { get; set; }


        public Die[] Dies { get; set; }

        public Size WaferSize { get; set; }


    }


}
