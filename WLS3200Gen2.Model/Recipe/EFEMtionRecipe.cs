using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Recipe
{
    public class EFEMtionRecipe : INotifyPropertyChanged
    {
        private double alignerMicroAngle;
        private double alignerWaferIDAngle;
        private double alignerLoadPortAngle;
        private double macroTopStartPitchX;
        private double macroTopStartRollY;
        private double macroTopStartYawT;
        private double macroBackStartPos;

        public double AlignerMicroAngle { get => alignerMicroAngle; set => SetValue(ref alignerMicroAngle, value); }
        public double AlignerWaferIDAngle { get => alignerWaferIDAngle; set => SetValue(ref alignerWaferIDAngle, value); }
        public double AlignerLoadPortAngle { get => alignerLoadPortAngle; set => SetValue(ref alignerLoadPortAngle, value); }
        public double MacroTopStartPitchX { get => macroTopStartPitchX; set => SetValue(ref macroTopStartPitchX, value); }
        public double MacroTopStartRollY { get => macroTopStartRollY; set => SetValue(ref macroTopStartRollY, value); }
        public double MacroTopStartYawT { get => macroTopStartYawT; set => SetValue(ref macroTopStartYawT, value); }

        public double MacroBackStartPos { get => macroBackStartPos; set => SetValue(ref macroBackStartPos, value); }

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
