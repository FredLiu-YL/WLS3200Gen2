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
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuanliCore.Interface;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// DigitalOutputUC.xaml 的互動邏輯
    /// </summary>
    public partial class DigitalOutputUC : UserControl, INotifyPropertyChanged
    {
        private bool isClick;
        public DigitalOutputUC()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty OutputSignalsProperty = DependencyProperty.Register("OutputSignals", typeof(DigitalOutput[]), typeof(DigitalOutputUC), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public DigitalOutput[] OutputSignals
        {
            get => (DigitalOutput[])GetValue(OutputSignalsProperty);
            set => SetValue(OutputSignalsProperty, value);
        }


        public ICommand OutputSwitchCommand => new RelayCommand<string>((par) =>
       {
           try
           {
               int id = Convert.ToInt32(par);
               if (isClick) return;//防止連點
               isClick = true;

               if (OutputSignals[id].IsSwitchOn == true )
               {
                   OutputSignals[id].On();
               }
               else
               {
                   OutputSignals[id].Off();
               }
               isClick = false;
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
    

}
