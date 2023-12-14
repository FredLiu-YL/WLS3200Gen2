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

        public static readonly DependencyProperty ExistStateProperty = DependencyProperty.Register(nameof(ExistState), typeof(ExistStates), typeof(ExistNotifyUC),
                                                                                new FrameworkPropertyMetadata(ExistStates.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SNProperty = DependencyProperty.Register(nameof(SN), typeof(string), typeof(ExistNotifyUC),
                                                                              new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public ExistStates ExistState
        {
            get => (ExistStates)GetValue(ExistStateProperty);
            set => SetValue(ExistStateProperty, value);
        }

        public string SN
        {
            get => (string)GetValue(SNProperty);
            set => SetValue(SNProperty, value);
        }

        public ICommand SelectCommand => new RelayCommand(async () =>
        {
            try
            {
                if (ExistState == ExistStates.Exist)
                    ExistState = ExistStates.Select;
               else if(ExistState == ExistStates.Select)
                    ExistState = ExistStates.Exist;
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


    public enum ExistStates
    {
        None,
        Exist,
        Select,


    }

    public class BackColorConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((ExistStates)value)
            {
                case ExistStates.None:
                    return Brushes.DimGray;

                case ExistStates.Exist:
                    return Brushes.Lime;

                case ExistStates.Select:
                    return Brushes.Gold;

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
