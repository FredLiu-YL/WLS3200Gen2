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
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// CassetteUC.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class CassetteUnitUC : UserControl, INotifyPropertyChanged
    {
        private bool wafer_IsEnable;

        private string waferInfo;

        private WorkItem workStatus = new WorkItem();

        private bool isFirstLoaded;

        private SolidColorBrush PrimaryHueLightBrush = Application.Current.Resources["PrimaryHueLightBrush"] as SolidColorBrush;
        public CassetteUnitUC()
        {
            InitializeComponent();
        }

        public CassetteUnitUC(bool isEnable, string pwaferInfo)
        {
            InitializeComponent();

            isFirstLoaded = false;

            Wafer_IsEnable = isEnable;
            WaferInfo = pwaferInfo;
        }
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isFirstLoaded == false)
                {
                    WorkStatus.IsTop = false;
                    WorkStatus.IsBack = false;
                    WorkStatus.IsMicro = false;

                    ResourceDictionary dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.bluegrey.xaml", UriKind.RelativeOrAbsolute);

                    if (dictionary.Count > 0)
                    {
                        // 资源字典已加载
                        object blueGreyColorResource1 = dictionary["PrimaryHueLightBrush"];
                        if (blueGreyColorResource1 != null && blueGreyColorResource1 is SolidColorBrush)
                        {
                            SolidColorBrush brush = (SolidColorBrush)blueGreyColorResource1;
                            Color blueGreyColor = brush.Color;
                            Click_Off = brush;
                            // 现在你可以使用 blueGreyColor
                        }
                        // 资源字典已加载
                        object blueGreyColorResource2 = dictionary["PrimaryHueDarkBrush"]; //PrimaryHueDarkForegroundBrush
                        if (blueGreyColorResource2 != null && blueGreyColorResource2 is SolidColorBrush)
                        {
                            SolidColorBrush brush = (SolidColorBrush)blueGreyColorResource2;
                            Color blueGreyColor = brush.Color;
                            Click_On = brush;
                            // 现在你可以使用 blueGreyColor
                        }
                    }
                    WorkStatus.BackGroundTop = Click_Off;
                    WorkStatus.BackGroundBack = Click_Off;
                    WorkStatus.BackGroundMicro = Click_Off;
                    //AddButtonAction = new RelayCommand<int>(key => { AddButton(key); });
                    isFirstLoaded = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public event Action<WorkItem> WorkItemChange;
        public WorkItem WorkStatus { get => workStatus; set => SetValue(ref workStatus, value); }
        public bool Wafer_IsEnable { get => wafer_IsEnable; set => SetValue(ref wafer_IsEnable, value); }
        
        public string WaferInfo { get => waferInfo; set => SetValue(ref waferInfo, value); }

        public Brush Click_On;

        public Brush Click_Off;
        public ICommand Top_Command => new RelayCommand(async () =>
        {
            try
            {
                if (WorkStatus.IsTop == true)
                {
                    WorkStatus.IsTop = false;
                    WorkStatus.BackGroundTop = Click_Off;
                }
                else
                {
                    WorkStatus.IsTop = true;
                    WorkStatus.BackGroundTop = Click_On;
                }

                WorkItemChange?.Invoke(WorkStatus);
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
                if (WorkStatus.IsBack == true)
                {
                    WorkStatus.IsBack = false;
                    WorkStatus.BackGroundBack = Click_Off;
                }
                else
                {
                    WorkStatus.IsBack = true;
                    WorkStatus.BackGroundBack = Click_On;
                }

                WorkItemChange?.Invoke(WorkStatus);
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
                if (WorkStatus.IsMicro == true)
                {
                    WorkStatus.IsMicro = false;
                    WorkStatus.BackGroundMicro = Click_Off;
                }
                else
                {
                    WorkStatus.IsMicro = true;
                    WorkStatus.BackGroundMicro = Click_On;
                }

                WorkItemChange?.Invoke(WorkStatus);
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
    public class WorkItem : INotifyPropertyChanged
    {
        private bool isTop;

        private bool isBack;

        private bool isMicro;

        private Brush backGroundTop;

        private Brush backGroundBack;

        private Brush backGroundMicro;

        public bool IsTop { get => isTop; set => SetValue(ref isTop, value); }
        public bool IsBack { get => isBack; set => SetValue(ref isBack, value); }
        public bool IsMicro { get => isMicro; set => SetValue(ref isMicro, value); }

        public Brush BackGroundTop { get => backGroundTop; set => SetValue(ref backGroundTop, value); }
        public Brush BackGroundBack { get => backGroundBack; set => SetValue(ref backGroundBack, value); }
        public Brush BackGroundMicro { get => backGroundMicro; set => SetValue(ref backGroundMicro, value); }


        //變更顏色-

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
