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

        public ICommand OpenCassetteLoad => new RelayCommand(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    LoadPort.Load();
                    LoadPortWafers.Clear();
                    int i = 1;
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

                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand UnLoad => new RelayCommand(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    LoadPort.Home();
                    LoadPortWafers.Clear();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand AlarmReset => new RelayCommand(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    LoadPort.AlarmReset();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand ParamSet => new RelayCommand(async () =>
        {
            try
            {
                await Task.Run(() =>
                {
                    LoadPort.SetParam();
                });
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
