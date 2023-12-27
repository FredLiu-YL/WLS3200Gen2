using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Interface;

namespace YuanliCore.Model
{
    public class AlignmentRecipe
    {

        public LocateMode AlignmentMode { get; set; }
        public FiducialData[] fiducialDatas { get; set; }
        public Point[] DetectionPosition { get; set; }



    }


    public  class FiducialData
    {
        public Frame<byte[]> SampleImage { get; set; }

        public Point  DesignPosition { get; set; }

        public Point GrabPosition { get; set; }
    }
}
