using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WLS3200Gen2;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Component;
using YuanliCore.Model.UserControls;

namespace Test.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public partial class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// 

        private ILoadPort loadPort;
        private ObservableCollection<WaferUIData> loadPort1Wafers = new ObservableCollection<WaferUIData>();
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            loadPort1Wafers = new ObservableCollection<WaferUIData>();

            //LoadPort = new HirataLoadPort_RS232("COM2");
            //LoadPort.Initial();
            
            //LoadPort = new HirataLoadPort_RS232("COM2");
            //LoadPort.Initial();

            loadPort = new DummyLoadPort();
        }
        public ICommand LoadedCommand => new RelayCommand<string>(key =>
        {
         
        });



        public ICommand OpenCCommand => new RelayCommand<string>(async key =>
        {

            try
            {
                await Task.Run(() =>
              {
                  LoadPort.Load();
                  LoadPort1Wafers.Clear();
                  int i = 1;
                  foreach (var item in LoadPort.Slot)
                  {
                      if (item == null)
                      {
                          LoadPort1Wafers.Add(new WaferUIData { WaferStates = ExistStates.None, SN = (i + 1).ToString() });
                      }
                      else if (item == true)
                      {
                          LoadPort1Wafers.Add(new WaferUIData { WaferStates = ExistStates.Exist, SN = (i + 1).ToString() });
                      }
                      else
                      {
                          LoadPort1Wafers.Add(new WaferUIData { WaferStates = ExistStates.Error, SN = (i + 1).ToString() });
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

  
        public ObservableCollection<WaferUIData> LoadPort1Wafers { get => loadPort1Wafers; set => SetValue(ref loadPort1Wafers, value); }

      
        public ILoadPort LoadPort { get => loadPort; set => SetValue(ref loadPort, value); }

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