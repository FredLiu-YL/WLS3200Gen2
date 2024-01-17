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
        /// <summary>
        /// 
        /// </summary>
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

        public Size DieSize { get; set; }
    }
}
