using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Account;
using YuanliCore.CameraLib;
using YuanliCore.Data;
using YuanliCore.Interface;

namespace WLS3200Gen2
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

        private Machine machine;
        private MachineSetting machineSetting;
        private bool isSimulate;
        private MainRecipe mainRecipe = new MainRecipe();
        private string machineSettingPath, processSettingPath;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            isSimulate = false;

            //   machineSetting.Load();
            if (!Directory.Exists(systemPath))
                Directory.CreateDirectory(systemPath);



            machineSettingPath = $"{systemPath}\\MachineSetting.json";

            if (!File.Exists(machineSettingPath))
            {
                machineSetting = new MachineSetting();
                //  machineSetting.LoadPortCount = LoadPortQuantity.Single; //全智是單Port
                machineSetting.IsSimulate = true;
                machineSetting.Save(machineSettingPath);
            }
            else
            {
                machineSetting = AbstractRecipe.Load<MachineSetting>(machineSettingPath);
            }

            isSimulate = machineSetting.IsSimulate;
            if (isSimulate)
            {
                IsStepMacro = true;
                IsStepAligner = true;
                IsStepLocate = true;
                IsStepDetection = true;
            }

            processSettingPath = $"{systemPath}\\ProcessSettingPath.json";
            if (!File.Exists(processSettingPath))
            {
                processSetting = new ProcessSetting();
                processSetting.Save(processSettingPath);
            }
            else
            {
                processSetting = AbstractRecipe.Load<ProcessSetting>(processSettingPath);
            }

            machine = new Machine(isSimulate, machineSetting);


            Account = UserAccount.Load();
            Account.CurrentAccount.PropertyChanged += CurrentAccountChanged;

            //預設為最高權限使用者
            Account.CurrentAccount.Right = RightsModel.Administrator;

            //流程內加入委派
            machine.ChangeRecipe += ChangeRecipe;
            machine.WriteLog += WriteLog;
            machine.MacroReady += MacroOperate;
            machine.MacroDoneReady += MacroDoneOperate;
            machine.AlignmentReady += AlignmentOperate;
            machine.SetWaferStatus += UpdateCassetteUI;
        }

        private MainRecipe ChangeRecipe()
        {

            return mainRecipe;
        }

        private void UpdateCassetteUI(Wafer wafer)
        {
            var processStation = ProcessStations.Where(p => p.CassetteIndex == wafer.CassetteIndex).FirstOrDefault();
            processStation = wafer.ProcessStatus;
        }



        private void CurrentAccountChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                var account = sender as Account;
                //    if (account.Name != "")
                //        MachineInformation.SetMachineInfo($"使用者登入：{account.Name}  權限：{account.Right}");
            }
        }

        private void WriteLog(YuanliCore.Logger.LogType logType,string message)
        {
            switch (logType)
            {
                case YuanliCore.Logger.LogType.PROCESS:
                    LogMessage = $"[PROCESS][{ Account.CurrentAccount.Name}] {message}  ";
                    break;
                case YuanliCore.Logger.LogType.TRIG:
                    LogMessage = $"[TRIG][{ Account.CurrentAccount.Name}] {message}  ";
                    break;
                case YuanliCore.Logger.LogType.ERROR:
                    LogMessage = $"[ERROR][{ Account.CurrentAccount.Name}] {message}  ";
                    break;
                case YuanliCore.Logger.LogType.ALARM:
                    LogMessage = $"[Alarm][{ Account.CurrentAccount.Name}] {message}  ";
                    break;
                default:
                    break;
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