using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;

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
        private BitmapSource icon;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            isSimulate = true;

            //  machineSetting.Load();
            icon = BitmapFrame.Create(new Uri("pack://application:,,,/WLS3200Gen2;component/YuanLi.ico"));
            machine = new Machine(isSimulate, machineSetting);

            // machine.Initial();
            // machine.Home();

            machine.ChangeRecipe += ChangeRecipe;
        }

        private MainRecipe ChangeRecipe()
        {

            return new MainRecipe();
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