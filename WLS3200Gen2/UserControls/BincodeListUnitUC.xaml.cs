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
    /// BincodeListUnitUC.xaml 的互動邏輯
    /// </summary>
    public partial class BincodeListUnitUC : UserControl, INotifyPropertyChanged
    {
        private bool isAssign = true;
        public BincodeListUnitUC()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(nameof(Code), typeof(string), typeof(BincodeListUnitUC),
                                                                         new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty DescribeProperty = DependencyProperty.Register(nameof(Describe), typeof(string), typeof(BincodeListUnitUC),
                                                                                 new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty AssignProperty = DependencyProperty.Register(nameof(Assign), typeof(string), typeof(BincodeListUnitUC),
                                                                               new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Brush), typeof(BincodeListUnitUC),
                                                                               new FrameworkPropertyMetadata(Brushes.Green, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }
        public string Describe
        {
            get => (string)GetValue(DescribeProperty);
            set => SetValue(DescribeProperty, value);
        }
        public string Assign
        {
            get => (string)GetValue(AssignProperty);
            set => SetValue(AssignProperty, value);
        }
        public Brush Color
        {
            get => (Brush)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }
        public bool IsAssign
        {
            get => isAssign;
            set => SetValue(ref isAssign, value);
        }
        public ICommand AssignCommand => new RelayCommand(async () =>
        {
            try
            {
                if (IsAssign)
                {
                    IsAssign = false;
                    if (Assign == Describe)
                    {
                        Assign = "";
                        await Task.Delay(50);
                    }
                    else
                    {
                        Assign = Describe;
                        await Task.Delay(50);
                    }
                    IsAssign = true;
                }
            }
            catch (Exception ex)
            {

                throw ex;
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
