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
        private bool isGetStatus = false;
        private bool[] uIOutputSignals = new bool[64];
        private System.Windows.Threading.DispatcherTimer dispatcherTimerFeedback;
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
        public bool[] UIOutputSignals
        {
            get => uIOutputSignals;
            set => SetValue(ref uIOutputSignals, value);
        }
        private void DigitalOutput_loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //var c = Thread.CurrentThread;
                //StartAxisStatus();
                if (OutputSignals == null) return;
                dispatcherTimerFeedback = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimerFeedback.Tick += new EventHandler(dispatcherTimerFeedback_Tick);
                dispatcherTimerFeedback.Interval = TimeSpan.FromMilliseconds(200);
                dispatcherTimerFeedback.Start();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke((Action)delegate
                {
                    MessageBox.Show(ex.Message);
                });
            }

        }

        private void DigitalOutput_Unloaded(object sender, RoutedEventArgs e)
        {
            isGetStatus = false;

            if (dispatcherTimerFeedback != null)
            {
                dispatcherTimerFeedback.Stop();
                dispatcherTimerFeedback.Tick -= new EventHandler(dispatcherTimerFeedback_Tick);
            }
        }
        /// <summary>
        /// Output狀態
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimerFeedback_Tick(object sender, EventArgs e)
        {
            Refresh();
        }

        private async Task Refresh()
        {
            try
            {
               
                bool[] signals = new bool[UIOutputSignals.Length];
                for (int i = 0; i < signals.Length; i++)
                {
                    signals[i] = OutputSignals[i].IsSwitchOn;
                }
                if (isClick) return;
                UIOutputSignals = signals;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public ICommand OutputSwitchCommand => new RelayCommand<string>((par) =>
       {
           try
           {
               int id = Convert.ToInt32(par);
               if (isClick) return;//防止連點
               isClick = true;

               if (OutputSignals[id].IsSwitchOn == false)
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
