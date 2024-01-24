using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace YuanliCore.Data
{
    public class Die
    {
        public int IndexX { get; set; }
        public int IndexY { get; set; }
        /// <summary>
        /// 機械座標x
        /// </summary>
        public double PosX { get; set; }
        /// <summary>
        /// 機械座標Y
        /// </summary>
        public double PosY { get; set; }
        /// <summary>
        /// Die的BinCode資訊
        /// </summary>
        public string BinCode { get; set; }
        /// <summary>
        /// Die尺寸
        /// </summary>
        public Size DieSize { get; set; }


    }
}
