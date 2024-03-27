using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using YuanliCore.Model.UserControls;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// LoadPortUnitUC.xaml 的互動邏輯
    /// </summary>
    public partial class LoadPortUnitUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty LoadPortProperty = DependencyProperty.Register(nameof(LoadPort), typeof(ILoadPort), typeof(LoadPortUnitUC),
                                                                                         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty LoadPortWafersProperty = DependencyProperty.Register(nameof(LoadPortWafers), typeof(ObservableCollection<WaferUIData>), typeof(LoadPortUnitUC),
                                                                                       new FrameworkPropertyMetadata(new ObservableCollection<WaferUIData>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty LoadPortUIShowProperty = DependencyProperty.Register(nameof(LoadPortUIShow), typeof(LoadPortUI), typeof(LoadPortUnitUC),
                                                                                     new FrameworkPropertyMetadata(new LoadPortUI(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsLoadPortParamReadOnlyProperty = DependencyProperty.Register(nameof(IsLoadPortParamReadOnly), typeof(bool), typeof(LoadPortUnitUC),
                                                                                     new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private bool isLoadPortEnabled = true;
        public LoadPortUnitUC()
        {
            InitializeComponent();
        }
        public ILoadPort LoadPort
        {
            get => (ILoadPort)GetValue(LoadPortProperty);
            set => SetValue(LoadPortProperty, value);
        }
        //暫時拿來顯示用   實際上只需要有Loadpot資訊就好
        public ObservableCollection<WaferUIData> LoadPortWafers
        {
            get => (ObservableCollection<WaferUIData>)GetValue(LoadPortWafersProperty);
            set => SetValue(LoadPortWafersProperty, value);
        }
        public LoadPortUI LoadPortUIShow
        {
            get => (LoadPortUI)GetValue(LoadPortUIShowProperty);
            set => SetValue(LoadPortUIShowProperty, value);
        }
        public bool IsLoadPortEnabled
        {
            get => isLoadPortEnabled;
            set => SetValue(ref isLoadPortEnabled, value);
        }
        public bool IsLoadPortParamReadOnly
        {
            get => (bool)GetValue(IsLoadPortParamReadOnlyProperty);
            set => SetValue(IsLoadPortParamReadOnlyProperty, value);
        }
        public ICommand OpenCassetteLoad => new RelayCommand(async () =>
        {
            try
            {
                /*  IsLoadPortEnabled = false;
                  await LoadPort.Load();
                  LoadPortWafers.Clear();
                  int i = 1;
                  if (LoadPort.Slot != null)
                  {
                      foreach (var item in LoadPort.Slot)
                      {
                          if (item == null)
                          {
                              LoadPortWafers.Add(new WaferUIData { WaferStates = ExistStates.None, SN = (i + 1).ToString() });
                          }
                          else if (item == true)
                          {
                              LoadPortWafers.Add(new WaferUIData { WaferStates = ExistStates.Exist, SN = (i + 1).ToString() });
                          }
                          else
                          {
                              LoadPortWafers.Add(new WaferUIData { WaferStates = ExistStates.Error, SN = (i + 1).ToString() });
                          }
                          i++;
                      }
                  }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoadPortEnabled = true;
            }
        });
        public ICommand UnLoad => new RelayCommand(async () =>
        {
            try
            {
                IsLoadPortEnabled = false;
                await LoadPort.Home();
                LoadPortWafers.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoadPortEnabled = true;
            }
        });
        public ICommand AlarmReset => new RelayCommand(async () =>
        {
            try
            {
                IsLoadPortEnabled = false;
                await LoadPort.AlarmReset();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoadPortEnabled = true;
            }
        });
        public ICommand ParamSet => new RelayCommand(async () =>
        {
            try
            {
                IsLoadPortEnabled = false;
                LoadPortParam loadPortParam = new LoadPortParam();
                loadPortParam.WaferThickness = LoadPortUIShow.WaferThickness;
                loadPortParam.CassettePitch = LoadPortUIShow.CassettePitch;
                loadPortParam.StarOffset = LoadPortUIShow.StarOffset;
                loadPortParam.WaferPitchTolerance = LoadPortUIShow.WaferPitchTolerance;
                loadPortParam.WaferPositionTolerance = LoadPortUIShow.WaferPositionTolerance;
                await LoadPort.SetParam(loadPortParam);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsLoadPortEnabled = true;
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
    public class LoadPortUI : INotifyPropertyChanged
    {
        private bool isMapping;
        private bool?[] slot;
        private string errorStatus;
        private string deviceStatus;
        private string errorCode;
        private bool isCassettePutOK;
        private bool isClamp;
        private bool isSwitchDoor;
        private bool isVaccum;
        private bool isDoorOpen;
        private bool isSensorCheckDoorOpen;
        private bool isDock;

        private int waferThickness;
        private int cassettePitch;
        private int starOffset;
        private int waferPitchTolerance;
        private int waferPositionTolerance;
        /// <summary>
        /// 是否有Mapping資訊
        /// </summary>
        public bool IsMapping
        {
            get => isMapping;
            set => SetValue(ref isMapping, value);
        }
        /// <summary>
        /// null:沒片子 true:有片子 false:片子異常
        /// </summary>
        public bool?[] Slot
        {
            get => slot;
            set => SetValue(ref slot, value);
        }
        /// <summary>
        /// Error的狀態 正常、可復原的錯誤、不可復原的錯誤
        /// </summary>
        public string ErrorStatus
        {
            get => errorStatus;
            set => SetValue(ref errorStatus, value);
        }
        /// <summary>
        /// LoadPort目前運作的位置
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
        /// Cassette放置是否正確
        /// </summary>
        public bool IsCassettePutOK
        {
            get => isCassettePutOK;
            set => SetValue(ref isCassettePutOK, value);
        }
        /// <summary>
        /// Cassette是否夾住
        /// </summary>
        public bool IsClamp
        {
            get => isClamp;
            set => SetValue(ref isClamp, value);
        }
        /// <summary>
        /// 轉開門機構，是否有轉開
        /// </summary>
        public bool IsSwitchDoor
        {
            get => isSwitchDoor;
            set => SetValue(ref isSwitchDoor, value);
        }
        /// <summary>
        /// 吸附門真空，是否開啟
        /// </summary>
        public bool IsVaccum
        {
            get => isVaccum;
            set => SetValue(ref isVaccum, value);
        }
        /// <summary>
        /// 門是否打開了
        /// </summary>
        public bool IsDoorOpen
        {
            get => isDoorOpen;
            set => SetValue(ref isDoorOpen, value);
        }
        /// <summary>
        /// Sensor確認門是否打開
        /// </summary>
        public bool IsSensorCheckDoorOpen
        {
            get => isSensorCheckDoorOpen;
            set => SetValue(ref isSensorCheckDoorOpen, value);
        }
        /// <summary>
        ///  移動Cassette前進後退的平台，是否前進到可以運作的位置
        /// </summary>
        public bool IsDock
        {
            get => isDock;
            set => SetValue(ref isDock, value);
        }
        /// <summary>
        /// Wafer厚薄度(um)
        /// </summary>
        public int WaferThickness
        {
            get => waferThickness;
            set => SetValue(ref waferThickness, value);
        }
        /// <summary>
        /// Cassette間距(um)
        /// </summary>
        public int CassettePitch
        {
            get => cassettePitch;
            set => SetValue(ref cassettePitch, value);
        }
        /// <summary>
        /// Cassette間距(um)
        /// </summary>
        public int StarOffset
        {
            get => starOffset;
            set => SetValue(ref starOffset, value);
        }
        /// <summary>
        /// Wafer間距容忍值
        /// </summary>
        public int WaferPitchTolerance
        {
            get => waferPitchTolerance;
            set => SetValue(ref waferPitchTolerance, value);
        }
        /// <summary>
        /// Wafer位置容忍值
        /// </summary>
        public int WaferPositionTolerance
        {
            get => waferPositionTolerance;
            set => SetValue(ref waferPositionTolerance, value);
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
