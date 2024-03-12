using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        /// 對位成功後 ，原點位置偏移量X
        /// </summary>
        public double OffsetX { get; set; }
        /// <summary>
        /// 對位成功後 ，原點位置偏移量Y
        /// </summary>
        public double OffsetY { get; set; }
        /// <summary>
        /// 對位方式  
        /// </summary>
        public LocateMode AlignmentMode { get; set; }
        
        /// <summary>
        /// 定位點資訊
        /// </summary>
        public LocateParam[] FiducialDatas { get; set; }





    }


    /* public class FiducialData
     {
         public Frame<byte[]> SampleImage { get; set; }

         public PatmaxParams Param { get; set; }

         public Point DesignPosition { get; set; }

         public Point GrabPosition { get; set; }

     }*/

    public class LocateParam : INotifyPropertyChanged
    {
        private double grabPositionX, grabPositionY, designPositionX, designPositionY;
        private int indexX, indexY;
        private Frame<byte[]> sampleImage;

        public LocateParam(int number)
        {

            MatchParam = new PatmaxParams(number);
        }
        /// <summary>
        /// 樣本圖
        /// </summary>
        public Frame<byte[]> SampleImage { get => sampleImage; set => SetValue(ref sampleImage, value); }

        //正常拍照座標跟設計座標應該分開 ，暫時先一起  有需要再分
        public double GrabPositionX { get => grabPositionX; set => SetValue(ref grabPositionX, value); }
        public double GrabPositionY { get => grabPositionY; set => SetValue(ref grabPositionY, value); }
        public double DesignPositionX { get => designPositionX; set => SetValue(ref designPositionX, value); }
        public double DesignPositionY { get => designPositionY; set => SetValue(ref designPositionY, value); }


        public int IndexX { get => indexX; set => SetValue(ref indexX, value); }
        public int IndexY { get => indexY; set => SetValue(ref indexY, value); }

        /// <summary>
        /// Cognex 資料存在這裡 需要獨立序列化 id 在100-199  用於檔案區隔  。從100開始是因為檔案名稱搜尋1開頭比較簡單
        /// </summary>
        public PatmaxParams MatchParam { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            T oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
        }
        protected virtual void OnPropertyChanged<T>(string name, T oldValue, T newValue)
        {
            // oldValue 和 newValue 目前沒有用到，代爾後需要再實作。
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
