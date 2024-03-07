using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
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
using YuanliCore.Data;

namespace YuanliCore.Model.UserControls
{
    /// <summary>
    /// ExistNotifyUC.xaml 的互動邏輯
    /// </summary>
    public partial class ExistNotifyUC : UserControl, INotifyPropertyChanged
    {


        public ExistNotifyUC()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ExistStateProperty = DependencyProperty.Register(nameof(ExistState), typeof(WaferProcessStatus), typeof(ExistNotifyUC),
                                                                                new FrameworkPropertyMetadata(WaferProcessStatus.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SNProperty = DependencyProperty.Register(nameof(SN), typeof(string), typeof(ExistNotifyUC),
                                                                              new FrameworkPropertyMetadata("12", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SNWidthProperty = DependencyProperty.Register(nameof(SNWidth), typeof(int), typeof(ExistNotifyUC),
                                                                             new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public WaferProcessStatus ExistState
        {
            get => (WaferProcessStatus)GetValue(ExistStateProperty);
            set => SetValue(ExistStateProperty, value);
        }

        public string SN
        {
            get => (string)GetValue(SNProperty);
            set => SetValue(SNProperty, value);
        }

        public int SNWidth
        {
            get => (int)GetValue(SNWidthProperty);
            set => SetValue(SNWidthProperty, value);
        }
        public ICommand SelectCommand => new RelayCommand(async () =>
        {
            try
            {
                //關閉內部自動切換顏色功能
                /*
                if (ExistState == WaferProcessStatus.NotSelect)
                    ExistState = WaferProcessStatus.Select;
                else if(ExistState == WaferProcessStatus.Select)
                    ExistState = WaferProcessStatus.NotSelect;
                */

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
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


  /*  public enum ExistStates
    {
        None,
        Exist,
        Select,
        Running,
        Complete,
        Error


    }*/

    public class BackColorConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((WaferProcessStatus)value)
            {
                case WaferProcessStatus.None:
                    return Brushes.DimGray;

                case WaferProcessStatus.NotSelect:
                    return Brushes.RoyalBlue;

                case WaferProcessStatus.Select:
                    return Brushes.LawnGreen;

                case WaferProcessStatus.Complate:
                    return Brushes.ForestGreen;

                case WaferProcessStatus.InProgress:
                    return Brushes.MediumTurquoise;

               

                default:
                    return Brushes.DarkRed;

            }



        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
