using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Model.Interface;

namespace WLS3200Gen2
{
    /// <summary>
    /// DetectionEditWindow.xaml 的互動邏輯
    /// </summary>
    public partial class DetectionEditWindow : Window, INotifyPropertyChanged
    {
        private IMicroscope Microscope { get; set; }
        private ObservableCollection<DetectionPoint> detectionPointList = new ObservableCollection<DetectionPoint>();
        public ObservableCollection<DetectionPoint> DetectionPointList { get => detectionPointList; set => SetValue(ref detectionPointList, value); }
        public DetectionEditWindow(DetectionPoint setectionPoint, IMicroscope microscope)
        {
            InitializeComponent();
            DetectionPointList.Clear();
            DetectionPointList.Add(setectionPoint);
            Microscope = microscope;

        }
        public ICommand GetNowParamCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                DetectionPointList[0].LensIndex = Microscope.LensIndex;
                DetectionPointList[0].CubeIndex = Microscope.CubeIndex;
                DetectionPointList[0].Filter1Index = Microscope.Filter1Index;
                DetectionPointList[0].Filter2Index = Microscope.Filter2Index;
                DetectionPointList[0].Filter3Index = Microscope.Filter3Index;
                DetectionPointList[0].MicroscopeLightValue = Microscope.LightValue;
                DetectionPointList[0].MicroscopeApertureValue = Microscope.ApertureValue;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ConfirmCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand CancelCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                DetectionPointList = null;
                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });

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
