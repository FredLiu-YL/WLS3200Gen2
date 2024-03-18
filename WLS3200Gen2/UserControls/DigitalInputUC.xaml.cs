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
    /// DigitalInputUC.xaml 的互動邏輯
    /// </summary>
    public partial class DigitalInputUC : UserControl, INotifyPropertyChanged
    {
        private bool isGetStatus = false;
        private bool[] uIInputSignals = new bool[32];
        private System.Windows.Threading.DispatcherTimer dispatcherTimerFeedback;
        public DigitalInputUC()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty InputSignalsProperty = DependencyProperty.Register("InputSignals", typeof(DigitalInput[]), typeof(DigitalInputUC), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public DigitalInput[] InputSignals
        {
            get => (DigitalInput[])GetValue(InputSignalsProperty);
            set => SetValue(InputSignalsProperty, value);
        }
        
        public bool[] UIInputSignals
        {
            get => uIInputSignals;
            set => SetValue(ref uIInputSignals, value);
        }
        private void DigitalInput_loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //var c = Thread.CurrentThread;
                //StartAxisStatus();
                if (InputSignals == null) return;
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
        private void DigitalInput_Unloaded(object sender, RoutedEventArgs e)
        {
            isGetStatus = false;

            if (dispatcherTimerFeedback != null)
            {
                dispatcherTimerFeedback.Stop();
                dispatcherTimerFeedback.Tick -= new EventHandler(dispatcherTimerFeedback_Tick);
            }
        }
        /// <summary>
        /// Input狀態
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
                bool[] signals = new bool[32];
                for (int i = 0; i < signals.Length; i++)
                {
                    signals[i] = InputSignals[i].IsSignal;
                }
                UIInputSignals = signals;
            }
            catch (Exception ex)
            {

                throw ex;
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
}
