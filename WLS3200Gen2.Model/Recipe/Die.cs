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
        /// 使用者操作的索引座標X
        /// </summary>
        public int IndexX { get; set; }
        /// <summary>
        /// 使用者操作的索引座標Y
        /// </summary>
        public int IndexY { get; set; }
        /// <summary>
        /// 使用者介面的圖像座標X
        /// </summary>
        public double OperationPixalX { get; set; }
        /// <summary>
        /// 使用者介面的圖像座標Y
        /// </summary>
        public double OperationPixalY { get; set; }
        /// <summary>
        /// 轉換後同象限座標X(因機台座標與Index象限可能不一致，轉換後同象限)
        /// </summary>
        public double MapTransX { get; set; }
        /// <summary>
        /// 轉換後同象限座標Y(因機台座標與Index象限可能不一致，轉換後同象限)
        /// </summary>
        public double MapTransY { get; set; }
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

        public Die Copy()
        {
            return new Die()
            {
                IndexX = this.IndexX,
                IndexY = this.IndexY,
                OperationPixalX = this.OperationPixalX,
                OperationPixalY = this.OperationPixalY,
                MapTransX = this.MapTransX,
                MapTransY = this.MapTransY,
                PosX = this.PosX,
                PosY = this.PosY,
                BinCode = this.BinCode,
                DieSize = this.DieSize,
            };
        }
    }
}
