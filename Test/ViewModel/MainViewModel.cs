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
using WLS3200Gen2.UserControls;
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
        private Task taskRefresh1 = Task.CompletedTask;
        private bool isRefresh;
        private ILoadPort loadPort;
        private IAligner aligner;
        private ObservableCollection<WaferUIData> loadPort1Wafers = new ObservableCollection<WaferUIData>();
        public LoadPortUI loadPortUIShow = new LoadPortUI();

        public AlignerUI alignerUIShow = new AlignerUI();

        public MainViewModel()
        {
            loadPort1Wafers = new ObservableCollection<WaferUIData>();
            LoadPort = new HirataLoadPort_RS232("COM2");
            LoadPort.Initial();

            Aligner = new HirataAligner_RS232("COM32");
            Aligner.Initial();
        }
        public ICommand LoadedCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                if (LoadPort != null)
                {
                    LoadPortParam loadPortParam = new LoadPortParam();
                    loadPortParam = await LoadPort.GetParam();
                    loadPortUIShow.WaferThickness = loadPortParam.WaferThickness;
                    loadPortUIShow.CassettePitch = loadPortParam.CassettePitch;
                    loadPortUIShow.StarOffset = loadPortParam.StarOffset;
                    loadPortUIShow.WaferPitchTolerance = loadPortParam.WaferPitchTolerance;
                    loadPortUIShow.WaferPositionTolerance = loadPortParam.WaferPositionTolerance;
                    isRefresh = true;
                }
                taskRefresh1 = Task.Run(RefreshStatus);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ClosingCommand => new RelayCommand(() =>
        {
            try
            {
                LoadPort.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        private async Task RefreshStatus()
        {
            try
            {

                while (isRefresh)
                {
                    if (LoadPort != null)
                    {
                        LoadPortStatus loadPortStatus = new LoadPortStatus();
                        loadPortStatus = await LoadPort.GetStatus();
                        loadPortUIShow.ErrorStatus = loadPortStatus.ErrorStatus;
                        loadPortUIShow.DeviceStatus = loadPortStatus.DeviceStatus;
                        loadPortUIShow.ErrorCode = loadPortStatus.ErrorCode;
                        loadPortUIShow.IsCassettePutOK = loadPortStatus.IsCassettePutOK;
                        loadPortUIShow.IsClamp = loadPortStatus.IsClamp;
                        loadPortUIShow.IsSwitchDoor = loadPortStatus.IsSwitchDoor;
                        loadPortUIShow.IsVaccum = loadPortStatus.IsVaccum;
                        loadPortUIShow.IsDoorOpen = loadPortStatus.IsDoorOpen;
                        loadPortUIShow.IsSensorCheckDoorOpen = loadPortStatus.IsSensorCheckDoorOpen;
                        loadPortUIShow.IsDock = loadPortStatus.IsDock;
                    }
                    if (Aligner != null)
                    {
                        AlignerStatus alignerStatus = new AlignerStatus();
                        alignerStatus = await Aligner.GetStatus();
                        AlignerUIShow.DeviceStatus = alignerStatus.DeviceStatus;
                        AlignerUIShow.ErrorCode = alignerStatus.ErrorCode;
                        AlignerUIShow.NotchStatus = alignerStatus.NotchStatus;
                        AlignerUIShow.IsWafer = alignerStatus.IsWafer;
                        AlignerUIShow.IsOrg = alignerStatus.IsOrg;
                        AlignerUIShow.IsVaccum = alignerStatus.IsVaccum;
                    }
                    await Task.Delay(300);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ICommand OpenCCommand => new RelayCommand<string>(async key =>
        {

            try
            {
                await Task.Run(async () =>
               {
                   if (LoadPort != null)
                   {
                       LoadPortParam loadPortParam = new LoadPortParam();
                       loadPortParam = await LoadPort.GetParam();
                       loadPortUIShow.WaferThickness = loadPortParam.WaferThickness;
                       loadPortUIShow.CassettePitch = loadPortParam.CassettePitch;
                       loadPortUIShow.StarOffset = loadPortParam.StarOffset;
                       loadPortUIShow.WaferPitchTolerance = loadPortParam.WaferPitchTolerance;
                       loadPortUIShow.WaferPositionTolerance = loadPortParam.WaferPositionTolerance;
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
        public LoadPortUI LoadPortUIShow { get => loadPortUIShow; set => SetValue(ref loadPortUIShow, value); }


        public IAligner Aligner { get => aligner; set => SetValue(ref aligner, value); }
        public AlignerUI AlignerUIShow { get => alignerUIShow; set => SetValue(ref alignerUIShow, value); }

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