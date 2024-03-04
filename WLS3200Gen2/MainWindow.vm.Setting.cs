using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WLS3200Gen2.Model;
using WLS3200Gen2.UserControls;

namespace WLS3200Gen2
{
    public partial class MainViewModel 
    {
        private IRobot robot;
        private RobotUI robotStaus = new RobotUI();
        private ILoadPort loadPort;
        private LoadPortUI loadPortUIShow = new LoadPortUI();
        private ILoadPort loadPort2;
        private LoadPortUI loadPort2UIShow = new LoadPortUI();
        private IAligner aligner;
        private AlignerUI alignerUIShow = new AlignerUI();
        public IRobot Robot { get => robot; set => SetValue(ref robot, value); }
        public RobotUI RobotStaus { get => robotStaus; set => SetValue(ref robotStaus, value); }
        public ILoadPort LoadPort { get => loadPort; set => SetValue(ref loadPort, value); }
        public LoadPortUI LoadPortUIShow { get => loadPortUIShow; set => SetValue(ref loadPortUIShow, value); }
        public ILoadPort LoadPort2 { get => loadPort2; set => SetValue(ref loadPort2, value); }
        public LoadPortUI LoadPort2UIShow { get => loadPort2UIShow; set => SetValue(ref loadPort2UIShow, value); }
        public IAligner Aligner { get => aligner; set => SetValue(ref aligner, value); }
        public AlignerUI AlignerUIShow { get => alignerUIShow; set => SetValue(ref alignerUIShow, value); }

        public ICommand SaveSettingCommand => new RelayCommand(() =>
        {
            try
            {
                machineSetting.Save(machineSettingPath);
                Customers.Clear();
     
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
