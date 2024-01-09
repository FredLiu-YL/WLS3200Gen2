using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using WLS3200Gen2;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Component.Adlink;
using WLS3200Gen2.UserControls;
using YuanliCore.Interface;
using YuanliCore.Motion;

namespace Test
{
    public partial class MainViewModel
    {
        private Task taskRefresh1 = Task.CompletedTask;
        private bool isRefresh;
        private ILoadPort loadPort;
        private IAligner aligner;
        private ObservableCollection<WaferUIData> loadPort1Wafers = new ObservableCollection<WaferUIData>();
        private Axis tableX;
        private AxisConfig tableXConfig;
        IMotionController motionController;
        private DigitalInput[] digitalInputs;
        private DigitalOutput[] digitalOutputs;


        public LoadPortUI loadPortUIShow = new LoadPortUI();

        public AlignerUI alignerUIShow = new AlignerUI();
        public Axis TableX { get => tableX; set => SetValue(ref tableX, value); }
        public AxisConfig TableXConfig { get => tableXConfig; set => SetValue(ref tableXConfig, value); }
        public DigitalInput[] DigitalInputs { get => digitalInputs; set => SetValue(ref digitalInputs, value); }
        public DigitalOutput[] DigitalOutputs { get => digitalOutputs; set => SetValue(ref digitalOutputs, value); }

        public ICommand LoadedCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                //loadPort1Wafers = new ObservableCollection<WaferUIData>();
                //LoadPort = new HirataLoadPort_RS232("COM2");
                //LoadPort.Initial();

                //Aligner = new HirataAligner_RS232("COM32");
                //Aligner.Initial();
                MotionInit();
                if (LoadPort != null)
                {
                    LoadPortParam loadPortParam = new LoadPortParam();
                    loadPortParam = await LoadPort.GetParam();
                    loadPortUIShow.WaferThickness = loadPortParam.WaferThickness;
                    loadPortUIShow.CassettePitch = loadPortParam.CassettePitch;
                    loadPortUIShow.StarOffset = loadPortParam.StarOffset;
                    loadPortUIShow.WaferPitchTolerance = loadPortParam.WaferPitchTolerance;
                    loadPortUIShow.WaferPositionTolerance = loadPortParam.WaferPositionTolerance;
                    isRefresh = true;
                }
                taskRefresh1 = Task.Run(RefreshStatus);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ClosingCommand => new RelayCommand(() =>
        {
            try
            {
                LoadPort.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });

        public void MotionInit()
        {
            try
            {
                OutputSwitchs = new bool[64];
                for (int i = 0; i < OutputSwitchs.Length; i++)
                {
                    OutputSwitchs[i] = new bool();
                    OutputSwitchs[i] = false;
                }

                List<AxisConfig> axisConfig = new List<AxisConfig>();
                for (int i = 0; i <= 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            AxisConfig axisXConfig = new AxisConfig();
                            axisXConfig.AxisName = "AxisX";
                            axisXConfig.AxisID = 0;
                            axisXConfig.MoveVel = new VelocityParams(1000000, 0.5);
                            axisXConfig.HomeVel = new VelocityParams(100000, 0.8);
                            axisXConfig.HomeMode = HomeModes.ORGAndIndex;
                            axisConfig.Add(axisXConfig);
                            break;
                        case 1:
                            AxisConfig axisYConfig = new AxisConfig();
                            axisYConfig.AxisName = "AxisY";
                            axisYConfig.AxisID = 1;
                            axisYConfig.MoveVel = new VelocityParams(1000000, 0.5);
                            axisYConfig.HomeVel = new VelocityParams(100000, 0.5);
                            axisYConfig.HomeMode = HomeModes.ORGAndIndex;
                            axisConfig.Add(axisYConfig);
                            break;
                        case 2:
                            AxisConfig axisZInfo = new AxisConfig();
                            axisZInfo.AxisName = "AxisZ";
                            axisZInfo.AxisID = 2;
                            axisZInfo.MoveVel = new VelocityParams(50000, 0.2);
                            axisZInfo.HomeVel = new VelocityParams(50000, 0.5);
                            axisZInfo.HomeMode = HomeModes.EL;
                            axisZInfo.HomeDirection = HomeDirection.Backward;
                            axisConfig.Add(axisZInfo);
                            break;
                        case 3:
                            AxisConfig axisRInfo = new AxisConfig();
                            axisRInfo.AxisName = "AxisR";
                            axisRInfo.AxisID = 3;
                            axisRInfo.MoveVel = new VelocityParams(45000, 0.2);
                            axisRInfo.HomeVel = new VelocityParams(4500, 0.2);
                            axisRInfo.HomeMode = HomeModes.ORG;
                            axisConfig.Add(axisRInfo);

                            break;
                        case 4:
                            AxisConfig axisRobotInfo = new AxisConfig();
                            axisRobotInfo.AxisName = "RobotAxis";
                            axisRobotInfo.AxisID = 4;
                            axisRobotInfo.MoveVel = new VelocityParams(3000000, 0.2);
                            axisRobotInfo.HomeVel = new VelocityParams(300000, 0.2);
                            axisRobotInfo.HomeMode = HomeModes.ORGAndIndex;
                            axisConfig.Add(axisRobotInfo);
                            break;
                    }
                }
                //var doNames = new string[] { "do1", "do2", "do3", "di1", "di2", "di3", "di1", "di2", "di3" };
                //var diNames = new string[] { "di1", "di2", "di3", "di1", "di2", "di3", "di1", "di2", "di3" };
                var doNames = new string[64];
                var diNames = new string[32];

                motionController = new Adlink7856(axisConfig, doNames, diNames);
                motionController.InitializeCommand();

                DigitalOutputs = motionController.OutputSignals.ToArray();
                DigitalInputs = motionController.InputSignals.ToArray();
                TableX = motionController.Axes[0];

                AxisConfig axis_TableXConfig = new AxisConfig();
                axis_TableXConfig.MoveVel = motionController.Axes[0].AxisVelocity;
                axis_TableXConfig.HomeVel = motionController.Axes[0].HomeVelocity;
                TableXConfig = axis_TableXConfig;

                DigitalOutput[] outputs = DigitalOutputs;

                OuterRingRollXServo = outputs[52];
                OuterRingRollXOrg = outputs[53];
                OuterRingRollXForward = outputs[48];
                OuterRingRollXBackward = outputs[49];
                OuterRingRollXStop = outputs[50];
                OuterRingRollXAlarmReset = outputs[51];

                OuterRingPitchYServo = outputs[58];
                OuterRingPitchYOrg = outputs[59];
                OuterRingPitchYForward = outputs[54];
                OuterRingPitchYBackward = outputs[55];
                OuterRingPitchYStop = outputs[56];
                OuterRingPitchYAlarmReset = outputs[57];

                OuterRingYawZServo = outputs[36];
                OuterRingYawZOrg = outputs[37];
                OuterRingYawZForward = outputs[32];
                OuterRingYawZBackward = outputs[33];
                OuterRingYawZStop = outputs[34];
                OuterRingYawZAlarmReset = outputs[35];

                OuterRingLiftMotorStart = outputs[21];
                OuterRingLiftMotorM0 = outputs[22];
                OuterRingLiftMotorM1 = outputs[23];
                OuterRingLiftMotorAlarmReset = outputs[19];
                OuterRingLiftMotorServoOff = outputs[20];
                OuterRingLiftMotorStop = outputs[18];

                InnerRingYawZServo = outputs[43];
                InnerRingYawZOrg = outputs[42];
                InnerRingYawZForward = outputs[38];
                InnerRingYawZBackward = outputs[39];
                InnerRingYawZStop = outputs[40];
                InnerRingYawZAlarmReset = outputs[41];

                InnerRingLiftMotorStart = outputs[29];
                InnerRingLiftMotorM0 = outputs[30];
                InnerRingLiftMotorM1 = outputs[31];
                InnerRingLiftMotorAlarmReset = outputs[27];
                InnerRingLiftMotorServoOff = outputs[28];
                InnerRingLiftMotorStop = outputs[26];
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private async Task RefreshStatus()
        {
            try
            {

                while (isRefresh)
                {
                    if (LoadPort != null)
                    {
                        LoadPortStatus loadPortStatus = new LoadPortStatus();
                        loadPortStatus = await LoadPort.GetStatus();
                        loadPortUIShow.ErrorStatus = loadPortStatus.ErrorStatus;
                        loadPortUIShow.DeviceStatus = loadPortStatus.DeviceStatus;
                        loadPortUIShow.ErrorCode = loadPortStatus.ErrorCode;
                        loadPortUIShow.IsCassettePutOK = loadPortStatus.IsCassettePutOK;
                        loadPortUIShow.IsClamp = loadPortStatus.IsClamp;
                        loadPortUIShow.IsSwitchDoor = loadPortStatus.IsSwitchDoor;
                        loadPortUIShow.IsVaccum = loadPortStatus.IsVaccum;
                        loadPortUIShow.IsDoorOpen = loadPortStatus.IsDoorOpen;
                        loadPortUIShow.IsSensorCheckDoorOpen = loadPortStatus.IsSensorCheckDoorOpen;
                        loadPortUIShow.IsDock = loadPortStatus.IsDock;
                    }
                    if (Aligner != null)
                    {
                        AlignerStatus alignerStatus = new AlignerStatus();
                        alignerStatus = await Aligner.GetStatus();
                        AlignerUIShow.DeviceStatus = alignerStatus.DeviceStatus;
                        AlignerUIShow.ErrorCode = alignerStatus.ErrorCode;
                        AlignerUIShow.NotchStatus = alignerStatus.NotchStatus;
                        AlignerUIShow.IsWafer = alignerStatus.IsWafer;
                        AlignerUIShow.IsOrg = alignerStatus.IsOrg;
                        AlignerUIShow.IsVaccum = alignerStatus.IsVaccum;
                    }




                    await Task.Delay(300);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public DigitalOutput OuterRingRollXServo { get; set; }
        public DigitalOutput OuterRingRollXOrg { get; set; }
        public DigitalOutput OuterRingRollXForward { get; set; }
        public DigitalOutput OuterRingRollXBackward { get; set; }
        public DigitalOutput OuterRingRollXStop { get; set; }
        public DigitalOutput OuterRingRollXAlarmReset { get; set; }

        public DigitalOutput OuterRingPitchYServo { get; set; }
        public DigitalOutput OuterRingPitchYOrg { get; set; }
        public DigitalOutput OuterRingPitchYForward { get; set; }
        public DigitalOutput OuterRingPitchYBackward { get; set; }
        public DigitalOutput OuterRingPitchYStop { get; set; }
        public DigitalOutput OuterRingPitchYAlarmReset { get; set; }

        public DigitalOutput OuterRingYawZServo { get; set; }
        public DigitalOutput OuterRingYawZOrg { get; set; }
        public DigitalOutput OuterRingYawZForward { get; set; }
        public DigitalOutput OuterRingYawZBackward { get; set; }
        public DigitalOutput OuterRingYawZStop { get; set; }
        public DigitalOutput OuterRingYawZAlarmReset { get; set; }

        public DigitalOutput OuterRingLiftMotorStart { get; set; }
        public DigitalOutput OuterRingLiftMotorM0 { get; set; }
        public DigitalOutput OuterRingLiftMotorM1 { get; set; }
        public DigitalOutput OuterRingLiftMotorAlarmReset { get; set; }
        public DigitalOutput OuterRingLiftMotorServoOff { get; set; }
        public DigitalOutput OuterRingLiftMotorStop { get; set; }




        public DigitalOutput InnerRingYawZServo { get; set; }
        public DigitalOutput InnerRingYawZOrg { get; set; }
        public DigitalOutput InnerRingYawZForward { get; set; }
        public DigitalOutput InnerRingYawZBackward { get; set; }
        public DigitalOutput InnerRingYawZStop { get; set; }
        public DigitalOutput InnerRingYawZAlarmReset { get; set; }

        public DigitalOutput InnerRingLiftMotorStart { get; set; }
        public DigitalOutput InnerRingLiftMotorM0 { get; set; }
        public DigitalOutput InnerRingLiftMotorM1 { get; set; }
        public DigitalOutput InnerRingLiftMotorAlarmReset { get; set; }
        public DigitalOutput InnerRingLiftMotorServoOff { get; set; }
        public DigitalOutput InnerRingLiftMotorStop { get; set; }

        public ICommand OpenCCommand => new RelayCommand<string>(async key =>
        {

            try
            {
                await Task.Run(async () =>
                {
                    if (LoadPort != null)
                    {
                        LoadPortParam loadPortParam = new LoadPortParam();
                        loadPortParam = await LoadPort.GetParam();
                        loadPortUIShow.WaferThickness = loadPortParam.WaferThickness;
                        loadPortUIShow.CassettePitch = loadPortParam.CassettePitch;
                        loadPortUIShow.StarOffset = loadPortParam.StarOffset;
                        loadPortUIShow.WaferPitchTolerance = loadPortParam.WaferPitchTolerance;
                        loadPortUIShow.WaferPositionTolerance = loadPortParam.WaferPositionTolerance;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }

        });



        public ObservableCollection<WaferUIData> LoadPort1Wafers { get => loadPort1Wafers; set => SetValue(ref loadPort1Wafers, value); }
        public ILoadPort LoadPort { get => loadPort; set => SetValue(ref loadPort, value); }
        public LoadPortUI LoadPortUIShow { get => loadPortUIShow; set => SetValue(ref loadPortUIShow, value); }


        public IAligner Aligner { get => aligner; set => SetValue(ref aligner, value); }
        public AlignerUI AlignerUIShow { get => alignerUIShow; set => SetValue(ref alignerUIShow, value); }


        public ICommand OutputSwitchCommand => new RelayCommand<string>((par) =>
        {
            try
            {
                motionController.SetOutputCommand(0, true);

                //if (OutputSwitchs[0] == false)
                //{
                //    OutputSwitchs[0] = true;
                //}
                //else
                //{
                //    OutputSwitchs[0] = false;
                //}
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });

        private bool[] outputSwitchs;
        public bool[] OutputSwitchs { get => outputSwitchs; set => SetValue(ref outputSwitchs, value); }
    }
}
