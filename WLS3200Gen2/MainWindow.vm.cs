using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using YuanliCore.Account;
using YuanliCore.Motion;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private UserAccount account;
        public Axis TableX { get => tableX; set => SetValue(ref tableX, value); }
        public Axis TableY { get => tableY; set => SetValue(ref tableY, value); }
        public string Version { get => version; set => SetValue(ref version, value); }
        public UserAccount Account { get => account; set => SetValue(ref account, value); }

        private string version;

        private Axis tableX;

        private Axis tableY;

        public ICommand WindowLoadedCommand => new RelayCommand(async () =>
        {
            try
            {
                machine.Initial();
                machine.Home();
                TableX = machine.MicroDetection.AxisX;
                TableY = machine.MicroDetection.AxisY;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        public ICommand WindowClosingCommand => new RelayCommand(() =>
       {
           try
           {


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
