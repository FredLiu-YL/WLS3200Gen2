using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YuanliCore.Model;

namespace WLS3200Gen2.Model.Recipe
{
    public class DetectionRecipe
    {
        public IEnumerable<DetectionPoint> DetectionPoints { get; set; }
        /// <summary>
        /// 對位參數
        /// </summary>
        public AlignmentRecipe AlignRecipe { get; set; } = new AlignmentRecipe();
        /// <summary>
        /// MAP資訊
        /// </summary>
        public WaferMapping WaferMap { get; set; }

    }

    public class DetectionPoint
    {
        /// <summary>
        /// 檢測位置的描述 (子Die 、Index )
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 檢測位置的座標
        /// </summary>
        public Point Position { get; set; }



    }


    public enum LocateMode
    {

        Pattern,
        Edge


    }
}
