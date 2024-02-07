using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WLS3200Gen2
{
    public partial class MainViewModel 
    {
        public ICommand SaveSettingCommand => new RelayCommand(() =>
        {
            try
            {
                machineSetting.Save(machineSettingPath);

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
