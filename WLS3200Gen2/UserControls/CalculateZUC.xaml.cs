using GalaSoft.MvvmLight.Command;
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

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// CalculateZ.xaml 的互動邏輯
    /// </summary>
    public partial class CalculateZUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TableZProperty = DependencyProperty.Register(nameof(TableZPos), typeof(double), typeof(CalculateZUC),
                                                                                       new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private double z1 = 0, z2 = 0, zDiff = 0;
        public CalculateZUC()
        {
            InitializeComponent();
        }
        public double TableZPos
        {
            get => (double)GetValue(TableZProperty);
            set => SetValue(TableZProperty, value);
        }
        public double Z1 { get => z1; set => SetValue(ref z1, value); }
        public double Z2 { get => z2; set => SetValue(ref z2, value); }
        public double ZDiff { get => zDiff; set => SetValue(ref zDiff, value); }
        public ICommand Z1SetCommand => new RelayCommand(async () =>
        {
            try
            {
                Z1 = TableZPos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand Z2SetCommand => new RelayCommand(async () =>
        {
            try
            {
                Z2 = TableZPos;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand ZDiffCommand => new RelayCommand(async () =>
        {
            try
            {
                ZDiff = Z1 - Z2;
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
