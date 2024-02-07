using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Account;
using YuanliCore.CameraLib;
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
        string machineSettingPath;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            isSimulate = true;

            //   machineSetting.Load();
          
          

            machineSetting = new MachineSetting();
            machineSettingPath = $"{systemPath}\\MachineSetting.json";
            if (!File.Exists(machineSettingPath))
            {
                machineSetting.Save(machineSettingPath);
            }

            MachineSetting processSetting = AbstractRecipe.Load<MachineSetting>(machineSettingPath);
         
            
            machine = new Machine(isSimulate, machineSetting);
        

            Account = UserAccount.Load();
            Account.CurrentAccount.PropertyChanged += CurrentAccountChanged;

            //預設為最高權限使用者
            Account.CurrentAccount.Right = RightsModel.Administrator;
            machine.ChangeRecipe += ChangeRecipe;
            machine.WriteLog += WriteLog;

 


        }

        private MainRecipe ChangeRecipe()
        {

            return new MainRecipe();
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

        private void WriteLog(string message)
        {

            LogMessage = $"[{ Account.CurrentAccount.Name}] {message}  ";
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