using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

    public class DetectionPoint : INotifyPropertyChanged
    {
        private int indexX, indexY;
        private double offsetX, offsetY;
        private Point position;

        /// <summary>
        /// 檢測位置的Index 
        /// </summary>
        public int IndexX { get => indexX; set => SetValue(ref indexX, value); }
        public int IndexY { get => indexY; set => SetValue(ref indexY, value); }

        public double OffsetX { get => offsetX; set => SetValue(ref offsetX, value); }
        public double OffsetY { get => offsetY; set => SetValue(ref offsetY, value); }

        /// <summary>
        /// 檢測位置的座標
        /// </summary>
        public Point Position { get => position; set => SetValue(ref position, value); }


        /// <summary>
        /// 目前光強度
        /// </summary>
        public int MicroscopeLightValue { get; set; }
        /// <summary>
        /// 目前光圈
        /// </summary>
        public int MicroscopeApertureValue { get; set; }


        /// <summary>
        /// 目前Z軸位置
        /// </summary>
        public int MicroscopePosition { get; set; }

        /// <summary>
        /// 準焦位置
        /// </summary>
        public double MicroscopeAberationPosition { get; set; }



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


    public enum LocateMode
    {

        Pattern,
        Edge


    }
}
