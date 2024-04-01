using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WLS3200Gen2.Model;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// LampUnitUC.xaml 的互動邏輯
    /// </summary>
    public partial class LampUnitUC : UserControl, INotifyPropertyChanged
    {
        private static readonly DependencyProperty LampControlProperty = DependencyProperty.Register(nameof(LampControl), typeof(ILampControl), typeof(LampUnitUC),
                                                                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty LampUIShowProperty = DependencyProperty.Register(nameof(LampUIShow), typeof(LampUI), typeof(LampUnitUC),
                                                                                     new FrameworkPropertyMetadata(new LampUI(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private int intensitySliderValue;
        public LampUnitUC()
        {
            InitializeComponent();
        }
        public ILampControl LampControl
        {
            get => (ILampControl)GetValue(LampControlProperty);
            set => SetValue(LampControlProperty, value);
        }
        public LampUI LampUIShow
        {
            get => (LampUI)GetValue(LampUIShowProperty);
            set => SetValue(LampUIShowProperty, value);
        }

        public int IntensitySliderValue
        {
            get
            {
                return intensitySliderValue;
            }
            set
            {
                LampControl.ChangeLightAsync(value).Wait();
                SetValue(ref intensitySliderValue, value);
            }
        }
        
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
    public class LampUI : INotifyPropertyChanged
    {
        private int lightValue;
        /// <summary>
        /// 目前光強度
        /// </summary>
        public int LightValue
        {
            get => lightValue;
            set => SetValue(ref lightValue, value);
        }
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
