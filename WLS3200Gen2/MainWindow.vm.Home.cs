using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private bool isRunning =false;
        public bool IsRunning { get => isRunning; set => SetValue(ref isRunning, value); }
        public BitmapSource Icon { get => icon; set => SetValue(ref icon, value); }

        public ICommand RunCommand => new RelayCommand(async () =>
        {
            try
            {
                IsRunning = true;
                await machine.ProcessRun();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                
            }
        });
        public ICommand PauseCommand => new RelayCommand(async () =>
        {
            try
            {
              
                IsRunning = false;

            
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        public ICommand StopCommand => new RelayCommand(async () =>
        {
            try
            {
                await machine.ProcessStop();
                IsRunning = false;


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
    }
}
