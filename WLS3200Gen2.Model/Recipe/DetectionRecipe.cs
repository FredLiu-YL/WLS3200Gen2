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
        /// <summary>
        /// 檢查點
        /// </summary>
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
        private int indexX, indexY, subDieType;
        private double offsetX, offsetY;
        private Point position;

        /// <summary>
        /// 檢測位置的Index X
        /// </summary>
        public int IndexX { get => indexX; set => SetValue(ref indexX, value); }
        /// <summary>
        /// 檢測位置的Index Y
        /// </summary>
        public int IndexY { get => indexY; set => SetValue(ref indexY, value); }
        /// <summary>
        /// 檢測位置平台的座標X Y
        /// </summary>
        public Point Position { get => position; set => SetValue(ref position, value); }
        /// <summary>
        /// 鏡頭倍率
        /// </summary>
        public int LensIndex { get; set; }
        /// <summary>
        /// Cube在哪一槽
        /// </summary>
        public int CubeIndex { get; set; }
        /// <summary>
        /// 第一道濾片
        /// </summary>
        public int Filter1Index { get; set; }
        /// <summary>
        /// 第二道濾片
        /// </summary>
        public int Filter2Index { get; set; }
        /// <summary>
        /// 第三道濾片
        /// </summary>
        public int Filter3Index { get; set; }
        /// <summary>
        /// 光強度
        /// </summary>
        public int MicroscopeLightValue { get; set; }
        /// <summary>
        /// 光圈
        /// </summary>
        public int MicroscopeApertureValue { get; set; }


        /// <summary>
        /// 目前Z軸位置
        /// </summary>
        public double MicroscopePosition { get; set; }

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
