using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YuanliCore.Interface;
using YuanliCore.Model;

namespace WLS3200Gen2.Model.Recipe
{
    public class DetectionRecipe
    {
        /// <summary>
        /// 檢查要下的Bincode清單
        /// </summary>
        public IEnumerable<BincodeInfo> BincodeList { get; set; }
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

        /// <summary>
        /// 暫放  等待WaferMapping  修正後應該存入WaferMapping  目前測試350*350可以 ，400*400檔案太大讀取會有問題
        /// </summary>
        public Frame<byte[]> MapImage { get; set; }
    }

    public class DetectionPoint : INotifyPropertyChanged
    {
        private int indexHeader = 0, indexX, indexY;
        private Point position;
        private String code, subProgramName;
        private int lensIndex, cubeIndex, filter1Index, filter2Index, filter3Index,
                    microscopeLightValue, microscopeApertureValue;
        private double microscopePosition, microscopeAberationPosition;
        /// <summary>
        /// 標題的Index
        /// </summary>
        public int IndexHeader { get => indexHeader; set => SetValue(ref indexHeader, value); }
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
        /// 顯示目前的code是多少
        /// </summary>
        public string Code { get => code; set => SetValue(ref code, value); }
        /// <summary>
        /// 鏡頭倍率
        /// </summary>
        public int LensIndex { get => lensIndex; set => SetValue(ref lensIndex, value); }
        /// <summary>
        /// Cube在哪一槽
        /// </summary>
        public int CubeIndex { get => cubeIndex; set => SetValue(ref cubeIndex, value); }
        /// <summary>
        /// 第一道濾片
        /// </summary>
        public int Filter1Index { get => filter1Index; set => SetValue(ref filter1Index, value); }
        /// <summary>
        /// 第二道濾片
        /// </summary>
        public int Filter2Index { get => filter2Index; set => SetValue(ref filter2Index, value); }
        /// <summary>
        /// 第三道濾片
        /// </summary>
        public int Filter3Index { get => filter3Index; set => SetValue(ref filter3Index, value); }
        /// <summary>
        /// 光強度
        /// </summary>
        public int MicroscopeLightValue { get => microscopeLightValue; set => SetValue(ref microscopeLightValue, value); }
        /// <summary>
        /// 光圈
        /// </summary>
        public int MicroscopeApertureValue { get => microscopeApertureValue; set => SetValue(ref microscopeApertureValue, value); }
        /// <summary>
        /// 目前Z軸位置
        /// </summary>
        public double MicroscopePosition { get => microscopePosition; set => SetValue(ref microscopePosition, value); }
        /// <summary>
        /// 準焦位置
        /// </summary>
        public double MicroscopeAberationPosition { get => microscopeAberationPosition; set => SetValue(ref microscopeAberationPosition, value); }
        /// <summary>
        /// SubDie模式名稱
        /// </summary>
        public string SubProgramName { get => subProgramName; set => SetValue(ref subProgramName, value); }

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
