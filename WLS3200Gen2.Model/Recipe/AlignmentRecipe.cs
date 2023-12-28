using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.ImageProcess.Match;
using YuanliCore.Interface;

namespace YuanliCore.Model
{
    public class AlignmentRecipe
    {
        /// <summary>
        /// 對位成功後 ，原點位置偏移量
        /// </summary>
        public Vector Offset { get; set; }
        /// <summary>
        /// 對位方式  
        /// </summary>
        public LocateMode AlignmentMode { get; set; }
        /// <summary>
        /// 定位點資訊
        /// </summary>
        public FiducialData[] fiducialDatas { get; set; }





    }


    public class FiducialData
    {
        public Frame<byte[]> SampleImage { get; set; }

        public PatmaxParams Param { get; set; }

        public Point DesignPosition { get; set; }

        public Point GrabPosition { get; set; }
    }
}
