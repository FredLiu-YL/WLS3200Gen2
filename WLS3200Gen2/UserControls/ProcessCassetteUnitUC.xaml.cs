using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using YuanliCore.Data;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// CassetteUC.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class CassetteUnitUC : UserControl, INotifyPropertyChanged
    {

        private Brush macroTopColor = Brushes.LightSkyBlue; //預設是藍色
        private Brush macrobackpColor = Brushes.LightSkyBlue;
        private Brush microColor = Brushes.LightSkyBlue;
        public CassetteUnitUC()
        {
            InitializeComponent();

        }
        public static readonly DependencyProperty CassetteIndexProperty = DependencyProperty.Register(nameof(CassetteIndex), typeof(int), typeof(CassetteUnitUC),
                                                                          new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public static readonly DependencyProperty MacroTopProperty = DependencyProperty.Register(nameof(MacroTop), typeof(WaferProcessStatus), typeof(CassetteUnitUC),
                                                                                 new FrameworkPropertyMetadata(WaferProcessStatus.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty MacroBackProperty = DependencyProperty.Register(nameof(MacroBack), typeof(WaferProcessStatus), typeof(CassetteUnitUC),
                                                                               new FrameworkPropertyMetadata(WaferProcessStatus.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty WaferIDProperty = DependencyProperty.Register(nameof(WaferID), typeof(WaferProcessStatus), typeof(CassetteUnitUC),
                                                                               new FrameworkPropertyMetadata(WaferProcessStatus.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MicroProperty = DependencyProperty.Register(nameof(Micro), typeof(WaferProcessStatus), typeof(CassetteUnitUC),
                                                                           new FrameworkPropertyMetadata(WaferProcessStatus.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public int CassetteIndex
        {
            get => (int)GetValue(CassetteIndexProperty);
            set => SetValue(CassetteIndexProperty, value);
        }



        /// <summary>
        /// 巨觀檢查站晶面 狀態
        /// </summary>
        public WaferProcessStatus MacroTop
        {
            get => (WaferProcessStatus)GetValue(MacroTopProperty);
            set => SetValue(MacroTopProperty, value);
        }
        /// <summary>
        /// 巨觀檢查站晶背後 狀態
        /// </summary>
        public WaferProcessStatus MacroBack
        {
            get => (WaferProcessStatus)GetValue(MacroBackProperty);
            set => SetValue(MacroBackProperty, value);
        }
        /// <summary>
        /// 讀取WAFERID站
        /// </summary>
        public WaferProcessStatus WaferID
        {
            get => (WaferProcessStatus)GetValue(WaferIDProperty);
            set => SetValue(WaferIDProperty, value);
        }

        /// <summary>
        /// 微觀檢查站
        /// </summary>
        public WaferProcessStatus Micro
        {
            get => (WaferProcessStatus)GetValue(MicroProperty);
            set => SetValue(MicroProperty, value);
        }




        public ICommand Top_Command => new RelayCommand(async () =>
        {
            try
            {

                if (MacroTop == WaferProcessStatus.Select)
                    MacroTop = WaferProcessStatus.NotSelect;
                else
                    MacroTop = WaferProcessStatus.Select;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand Back_Command => new RelayCommand(async () =>
        {
            try
            {

                if (MacroBack == WaferProcessStatus.Select)
                    MacroBack = WaferProcessStatus.NotSelect;
                else if (MacroBack == WaferProcessStatus.NotSelect)
                    MacroBack = WaferProcessStatus.Select;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand WaferIDCommand => new RelayCommand(async () =>
        {
            try
            {
                if (WaferID == WaferProcessStatus.Select)
                    WaferID = WaferProcessStatus.NotSelect;
                else if (WaferID == WaferProcessStatus.NotSelect)
                    WaferID = WaferProcessStatus.Select;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand Micro_Command => new RelayCommand(async () =>
        {
            try
            {
                if (Micro == WaferProcessStatus.Select)
                    Micro = WaferProcessStatus.NotSelect;
                else if (Micro == WaferProcessStatus.NotSelect)
                    Micro = WaferProcessStatus.Select;

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




    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WaferProcessStatus status)
            {
                switch (status)
                {
                    case WaferProcessStatus.None:
                        return Brushes.LightGray;

                    case WaferProcessStatus.NotSelect:
                        return Brushes.LightSkyBlue;

                    case WaferProcessStatus.Select:
                        return Brushes.Gold;

                    case WaferProcessStatus.Pass:
                        return Brushes.GreenYellow;

                    case WaferProcessStatus.Reject:
                        return Brushes.Red;

                    case WaferProcessStatus.InProgress:
                        return Brushes.Goldenrod;

                    case WaferProcessStatus.Complate:
                        return Brushes.Lime;

                }
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
