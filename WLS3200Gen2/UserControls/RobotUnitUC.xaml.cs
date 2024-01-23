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
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Module;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// RobotUnitUC.xaml 的互動邏輯
    /// </summary>
    public partial class RobotUnitUC : UserControl, INotifyPropertyChanged
    {
        public RobotUnitUC()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty RobotProperty = DependencyProperty.Register(nameof(Robot), typeof(IRobot), typeof(RobotUnitUC),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty RobotUIShowProperty = DependencyProperty.Register(nameof(RobotUIShow), typeof(RobotUI), typeof(RobotUnitUC),
                                                                                   new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public IRobot Robot
        {
            get => (IRobot)GetValue(RobotProperty);
            set => SetValue(RobotProperty, value);
        }
        public RobotUI RobotUIShow
        {
            get => (RobotUI)GetValue(RobotUIShowProperty);
            set => SetValue(RobotUIShowProperty, value);
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
    public class RobotUI : INotifyPropertyChanged
    {
        private string mode;
        private bool isStopSignal;
        private bool isEStopSignal;
        private bool isCommandDoneSignal;
        private bool isMovDoneSignal;
        private bool isRunning;
        private string errorCode;
        private int errorXYZWRC;


        /// <summary>
        /// 目前手臂的運作模式
        /// </summary>
        public string Mode
        {
            get => mode;
            set => SetValue(ref mode, value);
        }
        /// <summary>
        /// 停止訊號
        /// </summary>
        public bool IsStopSignal
        {
            get => isStopSignal;
            set => SetValue(ref isStopSignal, value);
        }
        /// <summary>
        /// 急停訊號
        /// </summary>
        public bool IsEStopSignal
        {
            get => isEStopSignal;
            set => SetValue(ref isEStopSignal, value);
        }
        /// <summary>
        /// 指令訊號完成
        /// </summary>
        public bool IsCommandDoneSignal
        {
            get => isCommandDoneSignal;
            set => SetValue(ref isCommandDoneSignal, value);
        }
        /// <summary>
        /// 移動完成訊號完成
        /// </summary>
        public bool IsMovDoneSignal
        {
            get => isMovDoneSignal;
            set => SetValue(ref isMovDoneSignal, value);
        }
        /// <summary>
        /// 是否還在運作
        /// </summary>
        public bool IsRunning
        {
            get => isRunning;
            set => SetValue(ref isRunning, value);
        }
        /// <summary>
        /// 發生什麼樣的異常
        /// </summary>
        public string ErrorCode
        {
            get => errorCode;
            set => SetValue(ref errorCode, value);
        }
        /// <summary>
        /// XYZWRC軸狀態 0:正常 1~3要看對應ErrorCode(例如:001000 代表Z軸有問題 對應的問題要查表)
        /// </summary>
        public int ErrorXYZWRC
        {
            get => errorXYZWRC;
            set => SetValue(ref errorXYZWRC, value);
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
