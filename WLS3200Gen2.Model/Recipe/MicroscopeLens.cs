using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Recipe
{
    public class MicroscopeLens : INotifyPropertyChanged
    {
        private string lensName = "";
        private double ratioX, ratioY, offsetPixelX, offsetPixelY, autoFocusPosition, aberationPosition;
        private int bFIntensity, bFApeture, bFAftbl, dFIntensity, dFApeture, dFAftbl;
        /// <summary>
        /// 鏡頭名稱
        /// </summary>
        public string LensName { get => lensName; set => SetValue(ref lensName, value); }
        /// <summary>
        /// X比例um/pixal
        /// </summary>
        public double RatioX { get => ratioX; set => SetValue(ref ratioX, value); }
        /// <summary>
        /// Y比例um/pixal
        /// </summary>
        public double RatioY { get => ratioY; set => SetValue(ref ratioY, value); }
        /// <summary>
        /// X比例um/pixal
        /// </summary>
        public double OffsetPixelX { get => offsetPixelX; set => SetValue(ref offsetPixelX, value); }
        /// <summary>
        /// Y比例um/pixal
        /// </summary>
        public double OffsetPixelY { get => offsetPixelY; set => SetValue(ref offsetPixelY, value); }

        /// <summary>
        /// 自動對焦高度
        /// </summary>
        public double AutoFocusPosition { get => autoFocusPosition; set => SetValue(ref autoFocusPosition, value); }
        /// <summary>
        /// 自動對焦數值
        /// </summary>
        public double AberationPosition { get => aberationPosition; set => SetValue(ref aberationPosition, value); }
        /// <summary>
        /// 明視野光亮度
        /// </summary>
        public int BFIntensity { get => bFIntensity; set => SetValue(ref bFIntensity, value); }
        /// <summary>
        /// 明視野光圈
        /// </summary>
        public int BFApeture { get => bFApeture; set => SetValue(ref bFApeture, value); }
        /// <summary>
        /// 明視野自動對焦參數組
        /// </summary>
        public int BFAftbl { get => bFAftbl; set => SetValue(ref bFAftbl, value); }
        /// <summary>
        /// 暗視野光亮度
        /// </summary>
        public int DFIntensity { get => dFIntensity; set => SetValue(ref dFIntensity, value); }
        /// <summary>
        /// 暗視野光圈
        /// </summary>
        public int DFApeture { get => dFApeture; set => SetValue(ref dFApeture, value); }
        /// <summary>
        /// 暗視野自動對焦參數組
        /// </summary>
        public int DFAftbl { get => dFAftbl; set => SetValue(ref dFAftbl, value); }

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
