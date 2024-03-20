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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// ProcessMenuUC.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessMenuUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsAutoSaveProperty = DependencyProperty.Register(nameof(IsAutoSave), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsWaferCirclePhotoProperty = DependencyProperty.Register(nameof(IsWaferCirclePhoto), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsDieAllPhotoProperty = DependencyProperty.Register(nameof(IsDieAllPhoto), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ProcessMenuUC()
        {
            InitializeComponent();
        }
        public bool IsAutoSave
        {
            get => (bool)GetValue(IsAutoSaveProperty);
            set => SetValue(IsAutoSaveProperty, value);
        }
        public bool IsWaferCirclePhoto
        {
            get => (bool)GetValue(IsWaferCirclePhotoProperty);
            set => SetValue(IsWaferCirclePhotoProperty, value);
        }
        public bool IsDieAllPhoto
        {
            get => (bool)GetValue(IsDieAllPhotoProperty);
            set => SetValue(IsDieAllPhotoProperty, value);
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
