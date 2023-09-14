using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WLS3200Gen2
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        private string appName;
        private Mutex mutex = null;

        public App()
        {
            appName = Assembly.GetExecutingAssembly().GetName().Name;
            mutex = new Mutex(true, $"Global\\{appName}", out bool createNew);

            //檢查是否同名Mutex已存在(表示另一份程式正在執行)
            if (!mutex.WaitOne(0, false))
            {
                MessageBox.Show($"{appName} was already in execution.");
                Current.Shutdown();
                return;
            }

            DispatcherUnhandledException += DispatcherUnhandledExceptionHandler;
        }
        protected override void OnExit(ExitEventArgs e)
        {
            if (mutex != null) mutex.ReleaseMutex();
            base.OnExit(e);
        }
        private void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string error1 = $"{e.Exception.Message}{Environment.NewLine}Application will be close.";
            string error2 = e.Exception.InnerException?.Message;

            MessageBox.Show(
                $"{error1}{Environment.NewLine}{error2}",
                "需處理的錯誤",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
           
        }

    }
}
