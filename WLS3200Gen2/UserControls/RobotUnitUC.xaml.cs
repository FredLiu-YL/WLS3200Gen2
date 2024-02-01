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
        private bool isRobotEnabled = true;

        private string address1, address2, address3, address4, address5, speed;
        public RobotUnitUC()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty RobotProperty = DependencyProperty.Register(nameof(Robot), typeof(IRobot), typeof(RobotUnitUC),
                                                                                      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty RobotStausProperty = DependencyProperty.Register(nameof(RobotStaus), typeof(RobotUI), typeof(RobotUnitUC),
                                                                                   new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public IRobot Robot
        {
            get => (IRobot)GetValue(RobotProperty);
            set => SetValue(RobotProperty, value);
        }
        public RobotUI RobotStaus
        {
            get => (RobotUI)GetValue(RobotStausProperty);
            set => SetValue(RobotStausProperty, value);
        }
        public bool IsRobotEnabled
        {
            get => isRobotEnabled;
            set => SetValue(ref isRobotEnabled, value);
        }
        public string Address1
        {
            get => address1;
            set => SetValue(ref address1, value);
        }
        public string Address2
        {
            get => address2;
            set => SetValue(ref address2, value);
        }
        public string Address3
        {
            get => address3;
            set => SetValue(ref address3, value);
        }
        public string Address4
        {
            get => address4;
            set => SetValue(ref address4, value);
        }
        public string Address5
        {
            get => address5;
            set => SetValue(ref address5, value);
        }
        public string Speed
        {
            get => speed;
            set => SetValue(ref speed, value);
        }

        public ICommand RobotMove => new RelayCommand<string>(async key =>
        {
            try
            {
                IsRobotEnabled = false;
                await Task.Run(async () =>
                {
                    switch (key)
                    {
                        case "0":
                            Robot.Home();
                            break;
                        case "1":
                            Robot.MovAddress(Convert.ToInt32(Address1), 0);
                            break;
                        case "2":
                            Robot.MovAddress(Convert.ToInt32(Address2), 0);
                            break;
                        case "3":
                            Robot.MovAddress(Convert.ToInt32(Address3), 0);
                            break;
                        case "4":
                            Robot.MovAddress(Convert.ToInt32(Address4), 0);
                            break;
                        case "5":
                            Robot.MovAddress(Convert.ToInt32(Address5), 0);
                            break;
                        default:
                            break;
                    }
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsRobotEnabled = true;
            }
        })
        {

        };

        public ICommand FixRelease => new RelayCommand<string>(async key =>
        {
            try
            {
                IsRobotEnabled = false;
                await Task.Run(async () =>
                {
                    switch (key)
                    {
                        case "0":
                            Robot.FixWafer();
                            break;
                        case "1":
                            Robot.ReleaseWafer();
                            break;

                        default:
                            break;
                    }
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsRobotEnabled = true;
            }
        });
        public ICommand RobotSet => new RelayCommand<string>(async key =>
        {
            try
            {
                IsRobotEnabled = false;
                await Task.Run(async () =>
                {
                    Robot.SetSpeedPercentCommand(Convert.ToInt32(RobotStaus.SpeedPercent));
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsRobotEnabled = true;
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
        private int speedPercent;
        private bool isHavePiece;
        private bool isLockOK;

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
        /// <summary>
        /// 軸的速度百分比
        /// </summary>
        public int SpeedPercent
        {
            get => speedPercent;
            set => SetValue(ref speedPercent, value);
        }
        /// <summary>
        /// 有無料
        /// </summary>
        public bool IsHavePiece
        {
            get => isHavePiece;
            set => SetValue(ref isHavePiece, value);
        }
        /// <summary>
        /// 有無lock住
        /// </summary>
        public bool IsLockOK
        {
            get => isLockOK;
            set => SetValue(ref isLockOK, value);
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
