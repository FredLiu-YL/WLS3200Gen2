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
        private string lensName;
        private double ratioX, ratioY, offsetPixelX, offsetPixelY, autoFocusPosition, aberationPosition;
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
