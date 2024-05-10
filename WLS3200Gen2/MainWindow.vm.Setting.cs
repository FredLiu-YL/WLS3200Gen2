using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.UserControls;
using YuanliCore.Motion;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private RobotUI robotStaus = new RobotUI();
        private LoadPortUI loadPortUIShow = new LoadPortUI();
        private LoadPortUI loadPort2UIShow = new LoadPortUI();
        private AlignerUI alignerUIShow = new AlignerUI();
        private LampUI lampControl1Param = new LampUI();
        private LampUI lampControl2Param = new LampUI();
        private Point tableWaferCatchPosition;
        private double tableWaferCatchPositionX, tableWaferCatchPositionY, tableWaferCatchPositionZ, tableWaferCatchPositionR;
        private double robotAxisStandbyPosition, robotAxisLoadPort1TakePosition, robotAxisLoadPort2TakePosition;
        private double robotAxisAligner1TakePosition, robotAxisAligner2TakePosition, robotAxisMacroTakePosition, robotAxisMicroTakePosition;
        private MicroscopeLens[] microscopeLensDefault = new MicroscopeLens[6];
        //public IRobot Robot { get => robot; set => SetValue(ref robot, value); }
        public RobotUI RobotStaus { get => robotStaus; set => SetValue(ref robotStaus, value); }
        public LoadPortUI LoadPortUIShow { get => loadPortUIShow; set => SetValue(ref loadPortUIShow, value); }
        public LoadPortUI LoadPort2UIShow { get => loadPort2UIShow; set => SetValue(ref loadPort2UIShow, value); }
        public AlignerUI AlignerUIShow { get => alignerUIShow; set => SetValue(ref alignerUIShow, value); }
        public LampUI LampControl1Param { get => lampControl1Param; set => SetValue(ref lampControl1Param, value); }
        public LampUI LampControl2Param { get => lampControl2Param; set => SetValue(ref lampControl2Param, value); }
        public double TableWaferCatchPositionX { get => tableWaferCatchPositionX; set => SetValue(ref tableWaferCatchPositionX, value); }
        public double TableWaferCatchPositionY { get => tableWaferCatchPositionY; set => SetValue(ref tableWaferCatchPositionY, value); }
        public double TableWaferCatchPositionZ { get => tableWaferCatchPositionZ; set => SetValue(ref tableWaferCatchPositionZ, value); }
        public double TableWaferCatchPositionR { get => tableWaferCatchPositionR; set => SetValue(ref tableWaferCatchPositionR, value); }
        public double RobotAxisLoadPort1TakePosition { get => robotAxisLoadPort1TakePosition; set => SetValue(ref robotAxisLoadPort1TakePosition, value); }
        public double RobotAxisLoadPort2TakePosition { get => robotAxisLoadPort2TakePosition; set => SetValue(ref robotAxisLoadPort2TakePosition, value); }
        public double RobotAxisAligner1TakePosition { get => robotAxisAligner1TakePosition; set => SetValue(ref robotAxisAligner1TakePosition, value); }
        public double RobotAxisAligner2TakePosition { get => robotAxisAligner2TakePosition; set => SetValue(ref robotAxisAligner2TakePosition, value); }
        public double RobotAxisMacroTakePosition { get => robotAxisMacroTakePosition; set => SetValue(ref robotAxisMacroTakePosition, value); }
        public double RobotAxisMicroTakePosition { get => robotAxisMicroTakePosition; set => SetValue(ref robotAxisMicroTakePosition, value); }
        public MicroscopeLens[] MicroscopeLensDefault { get => microscopeLensDefault; set => SetValue(ref microscopeLensDefault, value); }

        public ICommand SaveSettingCommand => new RelayCommand(() =>
        {
            try
            {
                //machineSetting.TableXConfig.AxisName = "AxisX";
                //machineSetting.TableXConfig.AxisID = 0;
                //machineSetting.TableXConfig.Ratio = 10;
                //machineSetting.TableXConfig.MoveVel = new VelocityParams(100000, 0.5);
                //machineSetting.TableXConfig.HomeVel = new VelocityParams(10000, 0.8);
                //machineSetting.TableXConfig.HomeMode = HomeModes.ORGAndIndex;

                //machineSetting.TableYConfig.AxisName = "AxisY";
                //machineSetting.TableYConfig.AxisID = 1;
                //machineSetting.TableYConfig.Ratio = 10;
                //machineSetting.TableYConfig.MoveVel = new VelocityParams(100000, 0.5);
                //machineSetting.TableYConfig.HomeVel = new VelocityParams(10000, 0.5);
                //machineSetting.TableYConfig.HomeMode = HomeModes.ORGAndIndex;

                //machineSetting.TableZConfig = new AxisConfig();

                //machineSetting.TableZConfig.AxisName = "AxisZ";
                //machineSetting.TableZConfig.AxisID = 2;
                //machineSetting.TableZConfig.Ratio = 1;
                //machineSetting.TableZConfig.MoveVel = new VelocityParams(50000, 0.2);
                //machineSetting.TableZConfig.HomeVel = new VelocityParams(50000, 0.5);
                //machineSetting.TableZConfig.HomeMode = HomeModes.EL;

                //machineSetting.TableRConfig.AxisName = "AxisR";
                //machineSetting.TableRConfig.AxisID = 3;
                //machineSetting.TableRConfig.Ratio = 1;
                //machineSetting.TableRConfig.MoveVel = new VelocityParams(45000, 0.2);
                //machineSetting.TableRConfig.HomeVel = new VelocityParams(4500, 0.2);
                //machineSetting.TableRConfig.HomeMode = HomeModes.ORG;

                //machineSetting.RobotAxisConfig.AxisName = "RobotAxis";
                //machineSetting.RobotAxisConfig.AxisID = 4;
                //machineSetting.RobotAxisConfig.Ratio = 10;
                //machineSetting.RobotAxisConfig.MoveVel = new VelocityParams(300000, 0.2);
                //machineSetting.RobotAxisConfig.HomeVel = new VelocityParams(30000, 0.2);
                //machineSetting.RobotAxisConfig.HomeMode = HomeModes.ORGAndIndex;


                machineSetting.TableWaferCatchPosition = new Point(TableWaferCatchPositionX, TableWaferCatchPositionY);
                machineSetting.TableWaferCatchPositionZ = TableWaferCatchPositionZ;
                machineSetting.TableWaferCatchPositionR = TableWaferCatchPositionR;
                machineSetting.RobotAxisLoadPort1TakePosition = RobotAxisLoadPort1TakePosition;
                machineSetting.RobotAxisLoadPort2TakePosition = RobotAxisLoadPort2TakePosition;
                machineSetting.RobotAxisAlignTakePosition = RobotAxisAligner1TakePosition;
                machineSetting.RobotAxisMacroTakePosition = RobotAxisMacroTakePosition;
                machineSetting.RobotAxisMicroTakePosition = RobotAxisMicroTakePosition;


                machineSetting.TableXConfig = TableXConfig;
                machineSetting.TableYConfig = TableYConfig;
                machineSetting.TableZConfig = TableZConfig;
                machineSetting.TableRConfig = TableRConfig;
                machineSetting.RobotAxisConfig = RobotAxisConfig;
                machineSetting.MicroscopeLensDefault = MicroscopeLensDefault;
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

        public ICommand DefaultBincodeEditCommand => new RelayCommand(() =>
        {
            try
            {
                if (machineSetting.BincodeListDefault == null)
                    machineSetting.BincodeListDefault = new ObservableCollection<BincodeInfo>();

                BincodeSettingWindow settingWindow = new BincodeSettingWindow(machineSetting.BincodeListDefault);


                settingWindow.ShowDialog();

                machineSetting.BincodeListDefault = settingWindow.BincodeList;
                machineSetting.Save(machineSettingPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
    }

}
