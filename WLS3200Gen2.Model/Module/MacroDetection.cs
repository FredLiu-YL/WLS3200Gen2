using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.CameraLib;
using YuanliCore.Interface;
using YuanliCore.Model;
using YuanliCore.Model.Interface;
using YuanliCore.Motion;
using static WLS3200Gen2.Model.Component.HannDeng_Macro;

namespace WLS3200Gen2.Model
{
    public class MacroDetection : INotifyPropertyChanged
    {
        private ICamera camera;
        private PauseTokenSource pauseToken;
        private CancellationTokenSource cancelToken;
        private Subject<(BitmapSource, bool)> subject = new Subject<(BitmapSource, bool)>();
        private IDisposable camlive;
      

        public MacroDetection()
        {
          
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

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }


   
}
