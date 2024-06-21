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
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Data;

namespace WLS3200Gen2.UserControls
{

    /// <summary>
    /// ProcessCassetteMenuUC.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessCassetteMenuUC : UserControl, INotifyPropertyChanged
    {
        private bool isAllSelectTopMacro;
        private bool isAllSelectBackMacro;
        private bool isAllSelectWaferIDMacro;
        private bool isAllSelectMicroMacro;
        public ProcessCassetteMenuUC()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty ProcessStationProperty = DependencyProperty.Register(nameof(ProcessStation), typeof(ObservableCollection<ProcessStation>), typeof(ProcessCassetteMenuUC),
                                                                                  new FrameworkPropertyMetadata(new ObservableCollection<ProcessStation>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsCanSelectProperty = DependencyProperty.Register(nameof(IsCanSelect), typeof(bool), typeof(ProcessCassetteMenuUC),
                                                                                 new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<ProcessStation> ProcessStation
        {
            get => (ObservableCollection<ProcessStation>)GetValue(ProcessStationProperty);
            set => SetValue(ProcessStationProperty, value);
        }
        public bool IsCanSelect
        {
            get => (bool)GetValue(IsCanSelectProperty);
            set => SetValue(IsCanSelectProperty, value);
        }
        public ICommand AllSelectTopMacroCommand => new RelayCommand(() =>
        {
            try
            {
                if (isAllSelectTopMacro)
                {
                    foreach (var item in ProcessStation)
                    {
                        if (item.MacroTop == WaferProcessStatus.NotSelect || item.MacroTop == WaferProcessStatus.Pass)
                        {
                            item.MacroTop = WaferProcessStatus.Select;
                        }
                    }
                    isAllSelectTopMacro = false;
                }
                else
                {
                    foreach (var item in ProcessStation)
                    {
                        if (item.MacroTop == WaferProcessStatus.Select || item.MacroTop == WaferProcessStatus.Pass)
                        {
                            item.MacroTop = WaferProcessStatus.NotSelect;
                        }
                    }
                    isAllSelectTopMacro = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand AllSelectBackMacroCommand => new RelayCommand(() =>
        {
            try
            {
                if (isAllSelectBackMacro)
                {
                    foreach (var item in ProcessStation)
                    {
                        if (item.MacroBack == WaferProcessStatus.NotSelect || item.MacroBack == WaferProcessStatus.Pass)
                        {
                            item.MacroBack = WaferProcessStatus.Select;
                        }
                    }
                    isAllSelectBackMacro = false;
                }
                else
                {
                    foreach (var item in ProcessStation)
                    {
                        if (item.MacroBack == WaferProcessStatus.Select || item.MacroBack == WaferProcessStatus.Pass)
                        {
                            item.MacroBack = WaferProcessStatus.NotSelect;
                        }
                    }
                    isAllSelectBackMacro = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand AllSelectWaferIDCommand => new RelayCommand(() =>
        {
            try
            {
                if (isAllSelectWaferIDMacro)
                {
                    foreach (var item in ProcessStation)
                    {
                        if (item.WaferID == WaferProcessStatus.NotSelect || item.WaferID == WaferProcessStatus.Pass)
                        {
                            item.WaferID = WaferProcessStatus.Select;
                        }
                    }
                    isAllSelectWaferIDMacro = false;
                }
                else
                {
                    foreach (var item in ProcessStation)
                    {
                        if (item.WaferID == WaferProcessStatus.Select || item.WaferID == WaferProcessStatus.Pass)
                        {
                            item.WaferID = WaferProcessStatus.NotSelect;
                        }
                    }
                    isAllSelectWaferIDMacro = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand AllSelectMicroCommand => new RelayCommand(() =>
        {
            try
            {
                if (isAllSelectMicroMacro)
                {
                    foreach (var item in ProcessStation)
                    {
                        if (item.Micro == WaferProcessStatus.NotSelect || item.Micro == WaferProcessStatus.Pass)
                        {
                            item.Micro = WaferProcessStatus.Select;
                        }
                    }
                    isAllSelectMicroMacro = false;
                }
                else
                {
                    foreach (var item in ProcessStation)
                    {
                        if (item.Micro == WaferProcessStatus.Select || item.Micro == WaferProcessStatus.Pass)
                        {
                            item.Micro = WaferProcessStatus.NotSelect;
                        }
                    }
                    isAllSelectMicroMacro = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
