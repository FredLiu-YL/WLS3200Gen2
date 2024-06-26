﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.Command;
using WLS3200Gen2;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Component;
using WLS3200Gen2.Model.Component.Adlink;
using WLS3200Gen2.Model.Module;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.UserControls;
using YuanliCore.CameraLib;
using YuanliCore.Interface;
using YuanliCore.Model.Interface;
using YuanliCore.Model.Interface.Component;
using YuanliCore.Model.Microscope;
using YuanliCore.Motion;
using YuanliCore.Views.CanvasShapes;

namespace Test
{
    public partial class MainViewModel
    {
        private Task taskRefresh1 = Task.CompletedTask;
        private bool isRefresh;
        private ILoadPort loadPort;
        private IAligner aligner;
        private IMacro macro;
        private IRobot robot;
        private IMicroscope micro;
        private ILampControl lampControl;
        private IReader reader;
        private ObservableCollection<WaferUIData> loadPort1Wafers = new ObservableCollection<WaferUIData>();
        private Axis tableX;
        private Axis tableY;
        private Axis tableR;
        private Axis tableZ;
        private Axis robotAxis;
        private AxisConfig tableXConfig;
        private IMotionController motionController;
        private DigitalInput[] digitalInputs;
        private DigitalOutput[] digitalOutputs;
        private MacroStatus macroStatus = new MacroStatus();
        //private MacroDetection macroDetection1;
        private LoadPortQuantity loadPortCount = LoadPortQuantity.Single;


        //public MacroDetection MacroDetection1 { get => macroDetection1; set => SetValue(ref macroDetection1, value); }

        private LoadPortUI loadPortUIShow = new LoadPortUI();

        private AlignerUI alignerUIShow = new AlignerUI();

        private RobotUI robotStaus = new RobotUI();

        private YuanliCore.Model.MicroscopeParam microscopeParam = new YuanliCore.Model.MicroscopeParam();

        private AFAberationUI aFAberation = new AFAberationUI();

        private LampUI lampControlParam = new LampUI();

        public Axis TableX { get => tableX; set => SetValue(ref tableX, value); }
        public Axis TableY { get => tableY; set => SetValue(ref tableY, value); }
        public Axis TableR { get => tableR; set => SetValue(ref tableR, value); }
        public Axis TableZ { get => tableZ; set => SetValue(ref tableZ, value); }
        public Axis RobotAxis { get => robotAxis; set => SetValue(ref robotAxis, value); }
        public AxisConfig TableXConfig { get => tableXConfig; set => SetValue(ref tableXConfig, value); }
        public DigitalInput[] DigitalInputs { get => digitalInputs; set => SetValue(ref digitalInputs, value); }
        public DigitalOutput[] DigitalOutputs { get => digitalOutputs; set => SetValue(ref digitalOutputs, value); }
        public ObservableCollection<WaferUIData> LoadPort1Wafers { get => loadPort1Wafers; set => SetValue(ref loadPort1Wafers, value); }
        public ILoadPort LoadPort { get => loadPort; set => SetValue(ref loadPort, value); }
        public LoadPortUI LoadPortUIShow { get => loadPortUIShow; set => SetValue(ref loadPortUIShow, value); }
        public IAligner Aligner { get => aligner; set => SetValue(ref aligner, value); }
        public AlignerUI AlignerUIShow { get => alignerUIShow; set => SetValue(ref alignerUIShow, value); }
        public IMacro Macro { get => macro; set => SetValue(ref macro, value); }

        public MacroStatus MacroStatus { get => macroStatus; set => SetValue(ref macroStatus, value); }


        public IRobot Robot { get => robot; set => SetValue(ref robot, value); }
        public RobotUI RobotStaus { get => robotStaus; set => SetValue(ref robotStaus, value); }


        public IMicroscope Micro { get => micro; set => SetValue(ref micro, value); }
        public YuanliCore.Model.MicroscopeParam MicroscopeParam { get => microscopeParam; set => SetValue(ref microscopeParam, value); }

        public AFAberationUI AFAberation { get => aFAberation; set => SetValue(ref aFAberation, value); }
        public ILampControl LampControl { get => lampControl; set => SetValue(ref lampControl, value); }
        public LampUI LampControlParam { get => lampControlParam; set => SetValue(ref lampControlParam, value); }

        public IReader Reader { get => reader; set => SetValue(ref reader, value); }
        //////////////////////////////////
        private WriteableBitmap mappingImage;
        private ObservableCollection<ROIShape> mappingDrawings = new ObservableCollection<ROIShape>();
        private Point mappingMousePixel;
        private bool mappingIsMoveEnable;
        private bool isEditBinGBEnable;
        public WriteableBitmap MappingImage { get => mappingImage; set => SetValue(ref mappingImage, value); }
        public ObservableCollection<ROIShape> MappingDrawings { get => mappingDrawings; set => SetValue(ref mappingDrawings, value); }
        public System.Windows.Point MappingMousePixel { get => mappingMousePixel; set => SetValue(ref mappingMousePixel, value); }
        public bool MappingIsMoveEnable
        {
            get => mappingIsMoveEnable;
            set
            {
                IsEditBinGBEnable = !value;
                SetValue(ref mappingIsMoveEnable, value);
            }
        }
        public bool IsEditBinGBEnable { get => isEditBinGBEnable; set => SetValue(ref isEditBinGBEnable, value); }
        public ICommand AddShapeMappingAction { get; set; }
        public ICommand ClearShapeMappingAction { get; set; }
        public ICommand RemoveShapeMappingAction { get; set; }

        private double nowPos;
        public double NowPos { get => nowPos; set => SetValue(ref nowPos, value); }

        private bool isOutputSwitchEnable = true;
        public bool IsOutputSwitchEnable { get => isOutputSwitchEnable; set => SetValue(ref isOutputSwitchEnable, value); }

        public WriteableBitmap MainImage { get => mainImage; set => SetValue(ref mainImage, value); }
        private WriteableBitmap mainImage;
        private IDisposable camlive;
        private ICamera MachineCamera = null;

        public ICommand LoadedCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                MappingImage = new WriteableBitmap(15000, 15000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                LoadPort1Wafers = new ObservableCollection<WaferUIData>();

                //////////////////////Initial//////////////////////
                //LampControl = new StrongLampRS232("COM25");
                //LampControl.Initial();
                loadPortCount = LoadPortQuantity.Pair;
                if (loadPortCount == LoadPortQuantity.Single)
                {
                    BXUCB aa = new BXUCB("COM4", false);
                    aa.Error += (e) =>
                    {
                        throw e;
                    };
                    Micro = aa;
                }
                else
                {
                    BXUCB aa = new BXUCB("COM24", false);
                    aa.Error += (e) =>
                    {
                        throw e;
                    };
                    Micro = aa;
                }

                Micro.Initial();


                bool IsMotionInitOK = false;
                IsMotionInitOK = MotionInit();

                if (IsMotionInitOK == true)
                {
                    int idx = 2;//0
                    TableX = motionController.Axes[idx];
                    //TableY = motionController.Axes[1];
                    //TableZ = motionController.Axes[2];
                    //TableW = motionController.Axes[3];
                    //RobotAxis = motionController.Axes[4];

                    AxisConfig axis_TableXConfig = new AxisConfig();
                    axis_TableXConfig.MoveVel = motionController.Axes[idx].AxisVelocity;
                    axis_TableXConfig.HomeVel = motionController.Axes[idx].HomeVelocity;
                    TableXConfig = axis_TableXConfig;

                    DigitalOutputs = motionController.OutputSignals.ToArray();
                    DigitalInputs = motionController.InputSignals.ToArray();

                    Macro = new HannDeng_Macro(DigitalOutputs, DigitalInputs);
                }

                //await Micro.HomeAsync();

                Reader = new CognexWaferID();
                Reader.Initial();

                if (loadPortCount == LoadPortQuantity.Single)
                {
                    //MachineCamera = new YuanliCore.CameraLib.IDS.UeyeCamera();
                    //MachineCamera.Open();
                    //CameraLive();
                }
                   

                //Robot = new HirataRobot_RS232("COM5", 10, 2);
                //Robot.Initial();


                //Aligner = new HirataAligner_RS232("COM32");
                //Aligner.Initial();

                //LoadPort = new HirataLoadPort_RS232("COM2");
                //LoadPort.Initial();
                //if (LoadPort != null)
                //{
                //    LoadPortParam loadPortParam = new LoadPortParam();
                //    loadPortParam = await LoadPort.GetParam();
                //    LoadPortUIShow.WaferThickness = loadPortParam.WaferThickness;
                //    LoadPortUIShow.CassettePitch = loadPortParam.CassettePitch;
                //    LoadPortUIShow.StarOffset = loadPortParam.StarOffset;
                //    LoadPortUIShow.WaferPitchTolerance = loadPortParam.WaferPitchTolerance;
                //    LoadPortUIShow.WaferPositionTolerance = loadPortParam.WaferPositionTolerance;
                //    isRefresh = true;
                //}



                //先個軸測試完成( X Y Z W R)復歸再來測試以下這些

                //await Robot.Home();
                //await loadPort.Home();
                //await Aligner.Home();
                //await Macro.Home();

                //Task robotAxisHome = RobotAxis.HomeAsync();
                //await Task.WhenAll(robotAxisHome);


                //await TableZ.HomeAsync();

                //Task axisXHome = TableX.HomeAsync();

                //Task axisYHome = TableY.HomeAsync();

                //Task axisRHome = TableR.HomeAsync();

                //await Micro.HomeAsync();//Task microscopeHome1 =

                //await Task.WhenAll(axisXHome, axisYHome, axisRHome, microscopeHome1);




                taskRefresh1 = Task.Run(RefreshStatus);
                isRefresh = true;
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
                //LoadPort.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });

        public bool MotionInit()
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
                if (loadPortCount == LoadPortQuantity.Single)
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                AxisConfig axisXConfig = new AxisConfig();
                                axisXConfig.AxisName = "AxisX";
                                axisXConfig.AxisID = 0;
                                axisXConfig.Ratio = 10;
                                axisXConfig.MoveVel = new VelocityParams(50000, 0.5);
                                axisXConfig.HomeVel = new VelocityParams(10000, 0.8);
                                axisXConfig.HomeMode = HomeModes.ORGAndIndex;
                                axisConfig.Add(axisXConfig);
                                break;
                            case 1:
                                AxisConfig axisYConfig = new AxisConfig();
                                axisYConfig.AxisName = "AxisY";
                                axisYConfig.AxisID = 1;
                                axisYConfig.Ratio = 10;
                                axisYConfig.MoveVel = new VelocityParams(50000, 0.5);
                                axisYConfig.HomeVel = new VelocityParams(10000, 0.5);
                                axisYConfig.HomeMode = HomeModes.ORGAndIndex;
                                axisConfig.Add(axisYConfig);
                                break;
                            case 2:
                                AxisConfig axisZInfo = new AxisConfig();
                                axisZInfo.AxisName = "AxisZ";
                                axisZInfo.AxisID = 2;
                                axisZInfo.Ratio = 10;
                                axisZInfo.MoveVel = new VelocityParams(10000, 1);
                                axisZInfo.HomeVel = new VelocityParams(5000, 1);
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
                                axisRobotInfo.Ratio = 10;
                                axisRobotInfo.MoveVel = new VelocityParams(300000, 0.2);
                                axisRobotInfo.HomeVel = new VelocityParams(30000, 0.2);
                                axisRobotInfo.HomeMode = HomeModes.ORGAndIndex;
                                axisConfig.Add(axisRobotInfo);
                                break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i <= 4; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                AxisConfig axisXConfig = new AxisConfig();
                                axisXConfig.AxisName = "AxisX";
                                axisXConfig.AxisID = 0;
                                axisXConfig.Ratio = 10;
                                axisXConfig.MoveVel = new VelocityParams(50000, 0.5);
                                axisXConfig.HomeVel = new VelocityParams(30000, 0.8);
                                axisXConfig.HomeMode = HomeModes.ORGAndIndex;
                                axisConfig.Add(axisXConfig);
                                break;
                            case 1:
                                AxisConfig axisYConfig = new AxisConfig();
                                axisYConfig.AxisName = "AxisY";
                                axisYConfig.AxisID = 1;
                                axisYConfig.Ratio = 10;
                                axisYConfig.MoveVel = new VelocityParams(50000, 0.5);
                                axisYConfig.HomeVel = new VelocityParams(30000, 0.5);
                                axisYConfig.HomeMode = HomeModes.ORGAndIndex;
                                axisConfig.Add(axisYConfig);
                                break;
                            case 2:
                                AxisConfig axisZInfo = new AxisConfig();
                                axisZInfo.AxisName = "AxisZ";
                                axisZInfo.AxisID = 2;
                                axisZInfo.Ratio = 1;
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
                                axisRInfo.HomeVel = new VelocityParams(10000, 0.2);
                                axisRInfo.HomeMode = HomeModes.ORG;
                                axisConfig.Add(axisRInfo);

                                break;
                            case 4:
                                AxisConfig axisRobotInfo = new AxisConfig();
                                axisRobotInfo.AxisName = "RobotAxis";
                                axisRobotInfo.AxisID = 4;
                                axisRobotInfo.Ratio = 10;
                                axisRobotInfo.MoveVel = new VelocityParams(300000, 0.2);
                                axisRobotInfo.HomeVel = new VelocityParams(50000, 0.2);
                                axisRobotInfo.HomeMode = HomeModes.ORGAndIndex;
                                axisConfig.Add(axisRobotInfo);
                                break;
                        }
                    }
                }


                //var doNames = new string[] { "do1", "do2", "do3", "di1", "di2", "di3", "di1", "di2", "di3" };
                //var diNames = new string[] { "di1", "di2", "di3", "di1", "di2", "di3", "di1", "di2", "di3" };
                var doNames = new string[64];
                var diNames = new string[32];

                if (loadPortCount == LoadPortQuantity.Single)
                {
                    motionController = new Adlink7856(axisConfig, doNames, diNames, "C:\\Motion.xml");
                }
                else
                {
                    motionController = new Adlink7856(axisConfig, doNames, diNames, "C:\\WLS3200-System\\Motion.xml");
                }
                motionController.InitializeCommand();
                return true;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private async Task RefreshStatus()
        {
            try
            {
                await Task.Run(async () =>
                {
                    while (isRefresh)
                    {
                        if (LoadPort != null)
                        {
                            LoadPortStatus loadPortStatus = new LoadPortStatus();
                            loadPortStatus = await LoadPort.GetStatus();
                            LoadPortUIShow.ErrorStatus = loadPortStatus.ErrorStatus;
                            LoadPortUIShow.DeviceStatus = loadPortStatus.DeviceStatus;
                            LoadPortUIShow.ErrorCode = loadPortStatus.ErrorCode;
                            LoadPortUIShow.IsCassettePutOK = loadPortStatus.IsCassettePutOK;
                            LoadPortUIShow.IsClamp = loadPortStatus.IsClamp;
                            LoadPortUIShow.IsSwitchDoor = loadPortStatus.IsSwitchDoor;
                            LoadPortUIShow.IsVaccum = loadPortStatus.IsVaccum;
                            LoadPortUIShow.IsDoorOpen = loadPortStatus.IsDoorOpen;
                            LoadPortUIShow.IsSensorCheckDoorOpen = loadPortStatus.IsSensorCheckDoorOpen;
                            LoadPortUIShow.IsDock = loadPortStatus.IsDock;
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
                        if (Robot != null)
                        {
                            RobotStatus robotStatus = new RobotStatus();
                            robotStatus = await Robot.GetStatus();
                            RobotStaus.Mode = robotStatus.Mode;
                            RobotStaus.IsStopSignal = robotStatus.IsStopSignal;
                            RobotStaus.IsEStopSignal = robotStatus.IsEStopSignal;
                            RobotStaus.IsCommandDoneSignal = robotStatus.IsCommandDoneSignal;
                            RobotStaus.IsMovDoneSignal = robotStatus.IsMovDoneSignal;
                            RobotStaus.IsRunning = robotStatus.IsRunning;
                            RobotStaus.ErrorCode = robotStatus.ErrorCode;
                            RobotStaus.ErrorXYZWRC = Convert.ToInt32("" + robotStatus.ErrorX + robotStatus.ErrorY + robotStatus.ErrorZ + robotStatus.ErrorW + robotStatus.ErrorR + robotStatus.ErrorC);

                            RobotStaus.IsHavePiece = Robot.IsLockOK;
                            RobotStaus.IsLockOK = Robot.IsLockOK;
                            //RobotUIIShow
                        }

                        if (Micro != null)
                        {
                            NowPos = Micro.Position;
                            MicroscopeParam.Position = Convert.ToInt32(nowPos);
                            MicroscopeParam.ApertureValue = Micro.ApertureValue;
                            MicroscopeParam.LightValue = Micro.LightValue;

                            AFAberation.AberationValue = Convert.ToInt32(Micro.AberationPosition);
                        }

                        if (LampControl != null)
                        {
                            LampControlParam.LightValue = LampControl.LightValue;
                        }

                        await Task.Delay(300);
                    }
                });


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


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
                        LoadPortUIShow.WaferThickness = loadPortParam.WaferThickness;
                        LoadPortUIShow.CassettePitch = loadPortParam.CassettePitch;
                        LoadPortUIShow.StarOffset = loadPortParam.StarOffset;
                        LoadPortUIShow.WaferPitchTolerance = loadPortParam.WaferPitchTolerance;
                        LoadPortUIShow.WaferPositionTolerance = loadPortParam.WaferPositionTolerance;
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



        public ICommand OpenBXFMACommand => new RelayCommand<string>(async key =>
      {
          string ss = await Reader.ReadAsync();

          MainImage = new WriteableBitmap(Reader.Image.ToBitmapSource());
      });


        private void CameraLive()
        {
            var camera = MachineCamera;
            var dis = MachineCamera.Grab();
            try
            {


                MainImage = new WriteableBitmap(camera.Width, camera.Height, 96, 96, camera.PixelFormat, null);


                camlive = camera.Frames.ObserveLatestOn(TaskPoolScheduler.Default) //取最新的資料 ；TaskPoolScheduler.Default  表示在另外一個執行緒上執行
                             .ObserveOn(DispatcherScheduler.Current)  //將訂閱資料轉換成柱列順序丟出 ；DispatcherScheduler.Current  表示在主執行緒上執行
                             .Subscribe(frame =>
                             {

                                 var a = System.Threading.Thread.CurrentThread.ManagedThreadId;
                                 if (frame != null) MainImage.WritePixels(frame);
                                 //  Image = new WriteableBitmap(frame.Width, frame.Height, frame.dP, double dpiY, PixelFormat pixelFormat, BitmapPalette palette);
                             });
            }
            catch (Exception ex)
            {


            }

        }

        private bool isFirst = true;
        public ICommand OutputSwitchCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                IsOutputSwitchEnable = false;
                switch (key)
                {
                    case "btn1":
                        await Micro.MoveAsync(1000);
                        break;
                    case "btn2":
                        await Micro.MoveToAsync(10);
                        break;
                    case "btn0":
                        await Micro.HomeAsync();
                        //bool tt = Micro.IsAutoFocusOk;
                        double aberationNow = 0;
                        Micro.AFOff();
                        isFirst = true;
                        if (isFirst)
                        {
                            await Micro.ChangeLightAsync(44);

                            await Micro.ChangeApertureAsync(700);

                            double zNow = Micro.AFPEL;

                            aberationNow = Micro.AFNEL;

                            Micro.SetSearchRange(584120, 100000);
                            isFirst = false;
                        }

                        await Micro.AFOneShotAsync();
                        Micro.AFTrace();
                        Micro.MoveToAsync(584120);

                        //Micro.Z_PositionPEL = 851110;
                        //Micro.Z_PositionNEL = 1;
                        //Micro.Aberration_PositionPEL = 780000;
                        //Micro.Aberration_PositionNEL = 350000;
                        //await Micro.ZMoveCommand(100);
                        //await Micro.AberrationMoveCommand(100);


                        //Micro.AberrationMoveToCommand(550);//550~790

                        //motionController.SetOutputCommand(0, true);

                        //if (OutputSwitchs[0] == false)
                        //{
                        //    OutputSwitchs[0] = true;
                        //}
                        //else
                        //{
                        //    OutputSwitchs[0] = false;
                        //}


                        //string SINF_Path = "";
                        //System.Windows.Forms.OpenFileDialog dlg_image = new System.Windows.Forms.OpenFileDialog();
                        //dlg_image.Filter = "TXT files (*.txt)|*.txt|SINF files (*.sinf)|*.sinf";
                        //dlg_image.InitialDirectory = SINF_Path;
                        //if (dlg_image.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        //{
                        //    SINF_Path = dlg_image.FileName;
                        //    if (SINF_Path != "")
                        //    {
                        //        var m_Sinf = new SinfWaferMapping("");
                        //        m_Sinf.ReadWaferFile(SINF_Path);

                        //    }
                        //}
                        //else
                        //{
                        //    SINF_Path = "";
                        //}



                        //cc.Dies;


                        //bool ss = true;
                        //if (ss)
                        //{
                        //    AddShapeMappingAction.Execute(new ROICircle
                        //    {
                        //        Stroke = Brushes.Yellow,
                        //        //StrokeThickness = this.StrokeThickness,
                        //        Fill = Brushes.Transparent,//Brushes.Blue,
                        //        X = 1000,
                        //        Y = 1000,
                        //        Radius = 1000,
                        //        //LengthX = showSize_X / 3,//(dieSize.Width / 3) / showScale,
                        //        //LengthY = showSize_Y / 3,//(dieSize.Height / 2) / showScale,
                        //        IsInteractived = true,
                        //        IsMoveEnabled = false,
                        //        IsResizeEnabled = false,
                        //        IsRotateEnabled = false,
                        //        CenterCrossLength = 0,
                        //        ToolTip = "Circle"
                        //    });
                        //}
                        //else
                        //{
                        //    ClearShapeMappingAction.Execute(true);
                        //}
                        break;
                    default:
                        break;
                }


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsOutputSwitchEnable = true;
            }
        });

        private bool[] outputSwitchs;
        public bool[] OutputSwitchs { get => outputSwitchs; set => SetValue(ref outputSwitchs, value); }
    }
}
