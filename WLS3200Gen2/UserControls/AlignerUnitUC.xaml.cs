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
using GalaSoft.MvvmLight.Command;
using WLS3200Gen2.Model;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// AlignerUnitUC.xaml 的互動邏輯
    /// </summary>
    public partial class AlignerUnitUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty AlignerProperty = DependencyProperty.Register(nameof(Aligner), typeof(IAligner), typeof(AlignerUnitUC),
                                                                                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty AlignerUIShowProperty = DependencyProperty.Register(nameof(AlignerUIShow), typeof(AlignerUI), typeof(AlignerUnitUC),
                                                                                  new FrameworkPropertyMetadata(new AlignerUI(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private double degree;

        private bool isAlignerEnabled = true;
        public AlignerUnitUC()
        {
            InitializeComponent();
        }
        public IAligner Aligner
        {
            get => (IAligner)GetValue(AlignerProperty);
            set => SetValue(AlignerProperty, value);
        }
        public AlignerUI AlignerUIShow
        {
            get => (AlignerUI)GetValue(AlignerUIShowProperty);
            set => SetValue(AlignerUIShowProperty, value);
        }
        public double Degree
        {
            get => degree;
            set => SetValue(ref degree, value);
        }
        public bool IsAlignerEnabled
        {
            get => isAlignerEnabled;
            set => SetValue(ref isAlignerEnabled, value);
        }

        public ICommand Home => new RelayCommand(async () =>
        {
            try
            {
                IsAlignerEnabled = false;
                await Aligner.Home();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsAlignerEnabled = true;
            }
        });
        public ICommand VaccumOn => new RelayCommand(async () =>
        {
            try
            {
                IsAlignerEnabled = false;
                await Aligner.Vaccum(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsAlignerEnabled = true;
            }
        });
        public ICommand VaccumOff => new RelayCommand(async () =>
        {
            try
            {
                IsAlignerEnabled = false;
                await Aligner.Vaccum(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsAlignerEnabled = true;
            }
        });
        public ICommand Run => new RelayCommand(async () =>
        {
            try
            {
                IsAlignerEnabled = false;
                await Aligner.Run(Degree);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsAlignerEnabled = true;
            }
        });
        public ICommand AlarmReset => new RelayCommand(async () =>
        {
            try
            {
                IsAlignerEnabled = false;
                await Aligner.AlarmReset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsAlignerEnabled = true;
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
    public class AlignerUI : INotifyPropertyChanged
    {
        private string deviceStatus;
        private string errorCode;
        private string notchStatus;
        private bool isWafer;
        private bool isOrg;
        private bool isVaccum;
        /// <summary>
        /// Aligner目前運作的位置
        /// </summary>
        public string DeviceStatus
        {
            get => deviceStatus;
            set => SetValue(ref deviceStatus, value);
        }
        /// <summary>
        /// 異常狀態Str:00-FF
        /// </summary>
        public string ErrorCode
        {
            get => errorCode;
            set => SetValue(ref errorCode, value);
        }
        /// <summary>
        /// AlignerNotch偵測狀態
        /// </summary>
        public string NotchStatus
        {
            get => notchStatus;
            set => SetValue(ref notchStatus, value);
        }
        /// <summary>
        /// 是否有Wafer
        /// </summary>
        public bool IsWafer
        {
            get => isWafer;
            set => SetValue(ref isWafer, value);
        }
        /// <summary>
        /// 是否在原點
        /// </summary>
        public bool IsOrg
        {
            get => isOrg;
            set => SetValue(ref isOrg, value);
        }
        /// <summary>
        /// Wafer真空，是否建立
        /// </summary>
        public bool IsVaccum
        {
            get => isVaccum;
            set => SetValue(ref isVaccum, value);
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
