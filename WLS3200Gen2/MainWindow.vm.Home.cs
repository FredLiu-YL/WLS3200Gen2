using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.UserControls;
using YuanliCore.CameraLib;
using YuanliCore.Data;
using YuanliCore.Interface;
using YuanliCore.Machine.Base;
using YuanliCore.Model.Interface;
using YuanliCore.Model.Microscope;
using YuanliCore.UserControls;
using YuanliCore.Views.CanvasShapes;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private ObservableCollection<ProcessStation> processStations = new ObservableCollection<ProcessStation>();
        private string logMessage;
        private string loadPortContent = "Load";
        private Visibility processVisibility = Visibility.Visible, stopVisibility = Visibility.Visible;
        private bool isHome = false;
        private bool isRunning = false;
        private bool isRunCommand = false;
        private readonly object lockAssignObj = new object();
        private readonly object lockHomeMapObj = new object();
        private readonly object lockRecipeMapObj = new object();
        private bool isUpdateHomeMap = false;
        private bool isUpdateRecipeMap = false;
        private PackIconKind pausePackIcon = PackIconKind.Home;
        private PackIconKind volumePackIcon = PackIconKind.VolumeOff;
        private ITransform updateHomeMapTransform { get; set; }
        private ITransform updateRecipeMapTransform { get; set; }
        private Die nowAssignDie;
        /// <summary>
        /// 在運行中的Wafer檢查
        /// </summary>
        private bool isRunningMicroDetection = false;

        private ProcessSetting processSetting;
        private Visibility informationUIVisibility, workholderUCVisibility;
        private WriteableBitmap homeResultImage, resultImage;
        private int tabControlSelectedIndex = 0; // 0:Process Infomation   1:Alignment  2:Micro  3 :Macro
        private bool isOperateUI = true, isCanChangeRecipe = true;
        private bool isMacroJudge = false, isMacroDone = false;
        private double manualPosX, manualPosY;
        private string waferIDManualKeyIn;
        private MachineStates machinestatus = MachineStates.IDLE;
        private WaferProcessStatus macroJudgeOperation;
        private ProcessStation macroDoneOperation = new ProcessStation(1);
        private WaferProcessStatus microJudgeOperation = WaferProcessStatus.Pass;
        private bool isLoadport1, isLoadport2;
        private IMicroscope microscope;
        private YuanliCore.Model.MicroscopeParam microscopeParam = new YuanliCore.Model.MicroscopeParam();
        private AFAberationUI aFAberation = new AFAberationUI();
        private IMacro macro;
        private IAligner aligner;
        private ILoadPort loadPort1;
        private IRobot robot;
        private ILampControl lampControl1, lampControl2;
        private MacroStatus macroStatus = new MacroStatus();
        private bool isAutoSave, isTestRun, isAutoFocus, isInnerRing, isOuterRing, isDegreeUnLoad, isSecondFlip;
        private int secondFlipPos;
        private Degree degreeUnLoad = Degree.Degree0;
        private TopContinueRotate topContinueRotate = TopContinueRotate.No;
        private string recipeName, processInfoWaferID, processInfoProgress1;
        private double manualdistance, manualarea;

        public PackIconKind PausePackIcon { get => pausePackIcon; set => SetValue(ref pausePackIcon, value); }
        public bool IsRunning { get => isRunning; set => SetValue(ref isRunning, value); }
        public PackIconKind VolumePackIcon { get => volumePackIcon; set => SetValue(ref volumePackIcon, value); }
        /// <summary>
        /// 在很多情況下 流程進行到一半需要人為操作 ，此時需要卡控不必要按鈕鎖住
        /// </summary>
        public bool IsOperateUI { get => isOperateUI; set => SetValue(ref isOperateUI, value); }
        public bool IsCanChangeRecipe { get => isCanChangeRecipe; set => SetValue(ref isCanChangeRecipe, value); }
        public bool IsMacroJudge { get => isMacroJudge; set => SetValue(ref isMacroJudge, value); }
        public bool IsMacroDone { get => isMacroDone; set => SetValue(ref isMacroDone, value); }
        public string LogMessage { get => logMessage; set => SetValue(ref logMessage, value); }
        public string LoadPortContent { get => loadPortContent; set => SetValue(ref loadPortContent, value); }
        public Visibility ProcessVisibility { get => processVisibility; set => SetValue(ref processVisibility, value); }
        public Visibility StopVisibility { get => stopVisibility; set => SetValue(ref stopVisibility, value); }
        public ProcessSetting ProcessSetting { get => processSetting; set => SetValue(ref processSetting, value); }
        public Visibility InformationUCVisibility { get => informationUIVisibility; set => SetValue(ref informationUIVisibility, value); }
        public Visibility WorkholderUCVisibility { get => workholderUCVisibility; set => SetValue(ref workholderUCVisibility, value); }
        public WriteableBitmap HomeResultImage { get => homeResultImage; set => SetValue(ref homeResultImage, value); }
        public WriteableBitmap ResultImage { get => resultImage; set => SetValue(ref resultImage, value); }
        public int TabControlSelectedIndex { get => tabControlSelectedIndex; set => SetValue(ref tabControlSelectedIndex, value); }
        public double ManualPosX { get => manualPosX; set => SetValue(ref manualPosX, value); }
        public double ManualPosY { get => manualPosY; set => SetValue(ref manualPosY, value); }
        public string WaferIDManualKeyIn { get => waferIDManualKeyIn; set => SetValue(ref waferIDManualKeyIn, value); }
        public MachineStates Machinestatus { get => machinestatus; set => SetValue(ref machinestatus, value); }
        public ObservableCollection<ProcessStation> ProcessStations { get => processStations; set => SetValue(ref processStations, value); }

        public bool IsLoadport1 { get => isLoadport1; set => SetValue(ref isLoadport1, value); }
        public bool IsLoadport2 { get => isLoadport2; set => SetValue(ref isLoadport2, value); }
        public IMicroscope Microscope { get => microscope; set => SetValue(ref microscope, value); }
        public YuanliCore.Model.MicroscopeParam MicroscopeParam { get => microscopeParam; set => SetValue(ref microscopeParam, value); }
        public AFAberationUI AFAberation { get => aFAberation; set => SetValue(ref aFAberation, value); }
        public IMacro Macro { get => macro; set => SetValue(ref macro, value); }
        public MacroStatus MacroStatus { get => macroStatus; set => SetValue(ref macroStatus, value); }
        public bool IsInnerRing { get => isInnerRing; set => SetValue(ref isInnerRing, value); }
        public bool IsOuterRing { get => isOuterRing; set => SetValue(ref isOuterRing, value); }
        public IAligner Aligner { get => aligner; set => SetValue(ref aligner, value); }
        public ILoadPort LoadPort1 { get => loadPort1; set => SetValue(ref loadPort1, value); }
        public IRobot Robot { get => robot; set => SetValue(ref robot, value); }
        public ILampControl LampControl1 { get => lampControl1; set => SetValue(ref lampControl1, value); }
        public ILampControl LampControl2 { get => lampControl2; set => SetValue(ref lampControl2, value); }
        public bool IsAutoSave { get => isAutoSave; set => SetValue(ref isAutoSave, value); }
        public bool IsTestRun { get => isTestRun; set => SetValue(ref isTestRun, value); }
        public bool IsAutoFocus { get => isAutoFocus; set => SetValue(ref isAutoFocus, value); }


        public bool IsDegreeUnLoad { get => isDegreeUnLoad; set => SetValue(ref isDegreeUnLoad, value); }
        public bool IsSecondFlip { get => isSecondFlip; set => SetValue(ref isSecondFlip, value); }
        public int SecondFlipPos { get => secondFlipPos; set => SetValue(ref secondFlipPos, value); }
        public Degree DegreeUnLoad { get => degreeUnLoad; set => SetValue(ref degreeUnLoad, value); }
        public TopContinueRotate TopContinueRotate { get => topContinueRotate; set => SetValue(ref topContinueRotate, value); }


        public String RecipeName { get => recipeName; set => SetValue(ref recipeName, value); }


        public string ProcessInfoWaferID { get => processInfoWaferID; set => SetValue(ref processInfoWaferID, value); }
        public string ProcessInfoProgress1 { get => processInfoProgress1; set => SetValue(ref processInfoProgress1, value); }

        public ICommand RunCommand => new RelayCommand(async () =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Run");
                if (IsLoadport1 == IsLoadport2)
                {
                    MessageBox.Show("Loadport Wrong choice");
                    return;

                }

                if (Machinestatus == MachineStates.Emergency)
                {
                    MessageBox.Show("Not available in emergencies", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (isHaveError)
                {
                    MessageBox.Show("Error is notr clear!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (isRunCommand == false)
                {
                    ProcessVisibility = Visibility.Hidden;
                    IsCanChangeRecipe = false;
                    isRunCommand = true;
                    IsRunning = true;
                    WriteLog(YuanliCore.Logger.LogType.PROCESS, "Process Start");
                    //產生當下時間的資料夾
                    processDataPath = CreateProcessFolder();

                    SwitchStates(MachineStates.RUNNING);
                    //判斷是使用loadport1 還是 2
                    ProcessSetting.IsLoadport1 = IsLoadport1;
                    ProcessSetting.IsLoadport2 = IsLoadport2;//暫時沒用到 都用IsLoadport1 判斷
                    //寫入每片Wafer的作業流程
                    ProcessSetting.ProcessStation = ProcessStations.ToArray();
                    //運作模式
                    ProcessSetting.IsAutoSave = IsAutoSave;
                    ProcessSetting.IsAutoFocus = IsAutoFocus;
                    ProcessSetting.IsTestRun = IsTestRun;
                    ProcessSetting.Inch = Model.Module.InchType.Inch12;
                    ProcessSetting.IsAutoFocus = IsAutoFocus;
                    ProcessSetting.IsDegreeUnLoad = IsDegreeUnLoad;
                    ProcessSetting.IsSecondFlip = IsSecondFlip;
                    ProcessSetting.SecondFlipPos = SecondFlipPos;
                    ProcessSetting.DegreeUnLoad = DegreeUnLoad;
                    ProcessSetting.TopContinueRotate = TopContinueRotate;
                    ProcessSetting.Save(processSettingPath);//ProcessSetting存檔 
                    isWaferInSystem = await machine.BeforeHomeCheck();
                    if (isWaferInSystem)
                    {
                        //WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, $"Remaining number of wafers : {waferusableCount} ");
                        MessageBox.Show("Wafer In System!! Please RemoveWafer", "Wafer In System", MessageBoxButton.YesNo);
                    }
                    else
                    {
                        TableX.AxisVelocity.MaxVel = TableXMaxVel;
                        TableY.AxisVelocity.MaxVel = TableYMaxVel;
                        TableZ.AxisVelocity.MaxVel = TableZMaxVel;
                        await machine.ProcessRunAsync(ProcessSetting);
                    }

                    SwitchStates(MachineStates.IDLE);

                    isRunCommand = false;
                    IsRunning = false;
                    IsCanChangeRecipe = true;
                    ProcessVisibility = Visibility.Visible;
                }
                else
                {
                    await machine.ProcessResume();
                    ProcessVisibility = Visibility.Hidden;
                }

            }
            catch (FlowException ex)
            {
                isHaveError = true;
                WriteLog(YuanliCore.Logger.LogType.ALARM, ex.Message);
                MessageBox.Show(ex.Message);
                isRunCommand = false;
                IsRunning = false;
                IsCanChangeRecipe = true;
                ProcessVisibility = Visibility.Visible;
                SwitchStates(MachineStates.Alarm);
            }
            catch (Exception ex)
            {
                isHaveError = true;
                WriteLog(YuanliCore.Logger.LogType.ERROR, ex.Message);
                MessageBox.Show(ex.Message);
                isRunCommand = false;
                IsRunning = false;
                IsCanChangeRecipe = true;
                ProcessVisibility = Visibility.Visible;
                SwitchStates(MachineStates.Emergency);
                isHome = false;
                isInitialComplete = false;
            }
            finally
            {
                WriteLog(YuanliCore.Logger.LogType.PROCESS, "Process Finish");
            }
        });


        public ICommand PauseCommand => new RelayCommand(async () =>
        {
            try
            {
                if (isHome)
                {
                    WriteLog(YuanliCore.Logger.LogType.TRIG, "Pause");
                    if (Machinestatus == MachineStates.Emergency)
                    {
                        MessageBox.Show("Not available in emergencies", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    IsRunning = false;
                    SwitchStates(MachineStates.PAUSED);
                    await machine.ProcessPause();
                    ProcessVisibility = Visibility.Visible;
                }
                else
                {
                    try
                    {
                        if (isHaveError)
                        {
                            MessageBox.Show("Error is notr clear!!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        SwitchStates(MachineStates.RUNNING);
                        WriteLog(YuanliCore.Logger.LogType.TRIG, "Home");
                        IsOperateUI = false;
                        IsCanChangeRecipe = false;
                        isWaferInSystem = await machine.BeforeHomeCheck();
                        MessageBoxResult result = MessageBoxResult.Yes;
                        if (isWaferInSystem)
                        {
                            result = MessageBox.Show("Wafer In System!! StartHome??", "StartHome", MessageBoxButton.YesNo);
                        }

                        if (result == MessageBoxResult.Yes)
                        {
                            WriteLog(YuanliCore.Logger.LogType.TRIG, "Home Start");

                            await machine.Home();
                            PausePackIcon = PackIconKind.Pause;
                            ProcessVisibility = Visibility.Visible;
                            StopVisibility = Visibility.Visible;
                            isHome = true;
                            IsOperateUI = true;
                            IsCanChangeRecipe = true;
                            WriteLog(YuanliCore.Logger.LogType.TRIG, "Home End");
                        }
                        SwitchStates(MachineStates.IDLE);
                    }
                    catch (Exception ex)
                    {
                        SwitchStates(MachineStates.Alarm);
                        isHaveError = true;
                        IsOperateUI = true;
                        throw ex;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand ResumeCommand => new RelayCommand(async () =>
        {
            try
            {

                InformationUCVisibility = Visibility.Visible;
                WorkholderUCVisibility = Visibility.Collapsed;
                TabControlSelectedIndex = 0;
                IsRunning = true;
                ProcessVisibility = Visibility.Hidden;
                SwitchStates(MachineStates.RUNNING);
                await machine.ProcessResume();
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
                WriteLog(YuanliCore.Logger.LogType.TRIG, "Stop");
                await machine.Abort();
                IsRunning = false;
                //SwitchStates(MachineStates.IDLE);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });

        public ICommand ReadRecipeCommand => new RelayCommand(async () =>
        {
            try
            {
                IsCanChangeRecipe = false;
                IsOperateUI = false;
                string path = $"{systemPath}\\Recipe\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                FileInfoWindow win = new FileInfoWindow(false, "WLS3200Gen2", path);
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                bool isDialogResult = (bool)win.ShowDialog();
                if (isDialogResult)
                {
                    string recipename = win.FileName;
                    RecipeName = recipename;
                    mainRecipe.Name = recipename;
                    mainRecipe.Load(path, recipename);
                    SetRecipeToLoadWaferParam();
                    SetRecipeToLocateParam();
                    SetRecipeToDetectionParam();
                    if (mainRecipe.DetectRecipe.WaferMap != null)
                    {
                        //ShowDetectionHomeMapImgae(mainRecipe.DetectRecipe);

                        ShowHomeNewMapImage(mainRecipe.DetectRecipe);
                        ShowDetectionHomeNewMapImgae(mainRecipe.DetectRecipe);

                        BincodeListUpdate(mainRecipe.DetectRecipe.BincodeList);
                        isRecipeMapShow = false;
                        transForm = OrgTransform(mainRecipe.DetectRecipe.WaferMap.MapCenterPoint, machineSetting.TableCenterPosition);
                    }
                    ResetDetectionRunningPointList();
                    WriteLog(YuanliCore.Logger.LogType.TRIG, "Load Recipe :" + recipename);
                }
                IsCanChangeRecipe = true;
                IsOperateUI = true;
                //SinfWaferMapping sinfWaferMapping = new SinfWaferMapping(true, true);
                //sinfWaferMapping = (SinfWaferMapping)mainRecipe.DetectRecipe.WaferMap;
                //sinfWaferMapping.SaveWaferFile("C://Users//USER//Documents//0603.txt", false, false);
            }
            catch (Exception ex)
            {
                IsOperateUI = true;
                IsCanChangeRecipe = true;
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand SaveRecipeCommand => new RelayCommand(() =>
        {
            try
            {

                string path = $"{systemPath}\\Recipe\\";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                }

                FileInfoWindow win = new FileInfoWindow(true, "WLS3200Gen2", path);
                win.WindowStartupLocation = WindowStartupLocation.CenterScreen;

                bool isDialogResult = (bool)win.ShowDialog();
                if (isDialogResult)
                {
                    var recipename = win.FileName;
                    // machine.BonderRecipe.Save(win.FilePathName);
                    mainRecipe.Name = recipename;
                    mainRecipe.RecipeSave(path, recipename);
                    RecipeName = recipename;
                    WriteLog(YuanliCore.Logger.LogType.TRIG, "Save Recipe :" + recipename);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        });
        public ICommand SlotMappingCommand => new RelayCommand(async () =>
        {
            try
            {
                bool?[] wafers = null;
                if (IsLoadport1 == IsLoadport2)
                {
                    throw new Exception("Loadport Wrong choice");
                }
                IsCanChangeRecipe = false;
                IsOperateUI = false;
                SwitchStates(MachineStates.RUNNING);

                if (LoadPortContent == "Load")
                {
                    if (IsLoadport1)
                    {
                        await machine.Feeder.LoadPortL.Load();
                        wafers = machine.Feeder.LoadPortL.Slot;
                    }
                    else if (IsLoadport2)
                    {
                        await machine.Feeder.LoadPortR.Load();
                        wafers = machine.Feeder.LoadPortR.Slot;
                    }
                    LoadPortContent = "UnLoad";
                }
                else
                {
                    if (IsLoadport1)
                    {
                        await machine.Feeder.LoadPortL.Home();
                        wafers = machine.Feeder.LoadPortL.Slot;
                    }
                    else if (IsLoadport2)
                    {
                        await machine.Feeder.LoadPortR.Home();
                        wafers = machine.Feeder.LoadPortR.Slot;
                    }
                    LoadPortContent = "Load";
                }

                ProcessStations.Clear();
                //陣列第一個位置是第25片 ， cassette最上面是第25片 ， 所以要從上往下排
                for (int i = 0; i < wafers.Length; i++)//陣列位置由第0個開始
                {
                    var temp = new ProcessStation(wafers.Length - i); //但陣列第一個位置 是 cassette第25片  所以 index反過來給
                    if (!wafers[i].HasValue)
                    {
                        temp.MacroTop = WaferProcessStatus.None;
                        temp.MacroBack = WaferProcessStatus.None;
                        temp.WaferID = WaferProcessStatus.None;
                        temp.Micro = WaferProcessStatus.None;
                    }
                    ProcessStations.Add(temp);
                }
                SwitchStates(MachineStates.IDLE);
                IsCanChangeRecipe = true;
                IsOperateUI = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });

        public ICommand ManualReFindCommand => new RelayCommand(async () =>
       {
           try
           {
               //找出畫面位置的 實際軸座標
               Point movePos = await machine.MicroDetection.FindFiducial(MainImage, TablePosX, TablePosY);
               ManualPosX = movePos.X;
               ManualPosY = movePos.Y;
           }
           catch (Exception ex)
           {

               MessageBox.Show(ex.Message);
           }
           finally
           {
           }
       });
        public ICommand ManualMoveCommand => new RelayCommand(async () =>
        {
            try
            {
                await Task.WhenAll(TableX.MoveToAsync(ManualPosX), TableY.MoveToAsync(ManualPosY));
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand ManualGetPosCommand => new RelayCommand(() =>
        {
            try
            {
                ManualPosX = TablePosX;
                ManualPosY = TablePosY;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand WaferIDConfirmOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                if (WaferIDManualKeyIn != "")
                {
                    await machine.ProcessResume();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand AlignmentOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                await machine.ProcessResume();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand GrabCommand => new RelayCommand(async () =>
        {
            try
            {
                BitmapSource bmp = machine.MicroDetection.Camera.GrabAsync();
                DetectionPoint point = new DetectionPoint();
                point.IndexX = MicroCheckNowIndexX;
                point.IndexY = MicroCheckNowIndexY;
                DetectionRecord(bmp, point);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand NextWaferCommand => new RelayCommand(async () =>
        {
            try
            {
                await machine.ProcessResume();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ManualCaliperCommand => new RelayCommand<string>((param) =>
        {
            try
            {
                MeasureWindow measureWindow = new MeasureWindow(machine.MicroDetection.Camera.GrabAsync());
                measureWindow.ShowDialog();
                //var w = MainImage.Width;
                //var h = MainImage.Height;
                //ROIShape shape = null;
                //ClearShapeManualAction.Execute(ManualDrawings);
                //switch (param)
                //{
                //    case "Ruler": //劃出直線

                //        shape = new ROILine
                //        {
                //            X1 = w / 2 - 200,
                //            Y1 = h / 2,
                //            X2 = w / 2 + 200,
                //            Y2 = h / 2,
                //            StrokeThickness = 5,
                //            Stroke = System.Windows.Media.Brushes.Red,
                //            IsInteractived = true
                //        };
                //        break;

                //    case "Rect"://劃出可旋轉的矩形
                //        shape = new ROIRotatedRect
                //        {
                //            X = w / 2,
                //            Y = h / 2,
                //            LengthX = 100,
                //            LengthY = 100,
                //            StrokeThickness = 5,
                //            Stroke = System.Windows.Media.Brushes.Red,
                //            IsInteractived = true
                //        };
                //        break;

                //    case "Circle"://劃出可旋轉的矩形
                //        shape = new ROICircle
                //        {
                //            X = w / 2,
                //            Y = h / 2,
                //            Radius = 100,
                //            StrokeThickness = 5,
                //            Stroke = System.Windows.Media.Brushes.Red,
                //            IsInteractived = true
                //        };
                //        break;

                //}
                //AddShapeManualAction.Execute(shape);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });


        public ICommand MacroPASSOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                macroJudgeOperation = WaferProcessStatus.Pass;
                await machine.ProcessResume();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand MacroRejectOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                macroJudgeOperation = WaferProcessStatus.Reject;
                await machine.ProcessResume();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand DoneOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                await machine.ProcessResume();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand TopAgainOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                macroDoneOperation.MacroTop = WaferProcessStatus.Select;
                await machine.ProcessResume();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand BackAgainOperateCommand => new RelayCommand(async () =>
        {
            try
            {
                macroDoneOperation.MacroBack = WaferProcessStatus.Select;
                await machine.ProcessResume();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand ResetErrorCommand => new RelayCommand(async () =>
        {
            try
            {
                MessageBoxResult result = MessageBoxResult.Yes;
                if (isHaveError)
                {
                    result = MessageBox.Show("Is Error Clear", "Error Clear", MessageBoxButton.YesNo);
                }
                if (result == MessageBoxResult.Yes)
                {
                    isHaveError = false;
                    SwitchStates(MachineStates.IDLE);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand VolumeOffCommand => new RelayCommand(async () =>
        {
            try
            {
                if (machine.StackLight != null)
                {
                    machine.StackLight.VolumeOff();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand ShowInfoCommand => new RelayCommand(async () =>
        {
            try
            {
                InfomationWidow widow = new InfomationWidow();
                widow.ShowDialog();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });
        public ICommand HomeDetectionPointListDoubleClickCommand => new RelayCommand(async () =>
        {
            try
            {
                //是否在運行中的Micro檢查
                if (isRunningMicroDetection == false) return;

                //顯示
                int idxX = DetectionHomePointList[SelectDetectionHomePointList].IndexX;
                int idxY = DetectionHomePointList[SelectDetectionHomePointList].IndexY;
                var showDie = mainRecipe.DetectRecipe.WaferMap.Dies.FirstOrDefault(die => die.IndexX == idxX && die.IndexY == idxY);
                Point mapPoint = new Point(showDie.MapTransX, showDie.MapTransY);
                var transMapMousePixcel = NowTablePosTransToHomeMapPixel(mapPoint);
                Rectangle nowSelectRange = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 5,
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(nowSelectRange, transMapMousePixcel.X);
                Canvas.SetTop(nowSelectRange, transMapMousePixcel.Y);
                ChangeHomeMappingSelect(nowSelectRange);

                //移動
                Rect rect = new Rect(Canvas.GetLeft(nowSelectRange), Canvas.GetTop(nowSelectRange), nowSelectRange.Width, nowSelectRange.Height);
                RectangleInfo tempselectRects = RectanglesHome.Where(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                   || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight)).FirstOrDefault();

                if (tempselectRects != null)
                {
                    int index = RectanglesHome.IndexOf(tempselectRects);
                    var moveDie = mainRecipe.DetectRecipe.WaferMap.Dies.FirstOrDefault(n => n.IndexX == RectanglesHome[index].Col && n.IndexY == RectanglesHome[index].Row);
                    var transPos = machine.MicroDetection.TransForm.TransPoint(
                        new Point(moveDie.MapTransX + mainRecipe.DetectRecipe.AlignRecipe.OffsetX, moveDie.MapTransY + mainRecipe.DetectRecipe.AlignRecipe.OffsetY)
                        );
                    await machine.MicroDetection.TableMoveToAsync(transPos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public Point NowTablePosTransToHomeMapPixel(Point mapPoint)
        {
            try
            {
                homeMapToPixelScale = MapPointToMapImagePixelScale(HomeMapDrawSize.X, HomeMapDrawSize.Y, HomeMapDrawCuttingline);
                var die00 = mainRecipe.DetectRecipe.WaferMap.Dies[0];
                var transMapMousePixcelX = 0.0;
                var transMapMousePixcelY = 0.0;
                if (die00.MapTransX == die00.OperationPixalX)
                {
                    transMapMousePixcelX = homeMapToPixelScale.X * mapPoint.X + HomeMapDrawSize.X / 2;
                }
                else
                {
                    transMapMousePixcelX = homeMapToPixelScale.X * (die00.MapTransX + die00.OperationPixalX - mapPoint.X) + HomeMapDrawSize.X / 2;
                }

                if (die00.MapTransY == die00.OperationPixalY)
                {
                    transMapMousePixcelY = homeMapToPixelScale.Y * mapPoint.Y + HomeMapDrawSize.Y / 2;
                }
                else
                {
                    transMapMousePixcelY = homeMapToPixelScale.Y * (die00.MapTransY + die00.OperationPixalY - mapPoint.Y) + HomeMapDrawSize.Y / 2;
                }
                return new Point(transMapMousePixcelX, transMapMousePixcelY);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void HomeMapDieColorChange(WaferMapping waferMapping, Die die, Brush brush)
        {
            //try
            //{
            //    if (showSize_X <= 0 || showSize_Y <= 0)
            //    {
            //        return;
            //    }
            //    var clearPoint = new Point(die.OperationPixalX / showSize_X + offsetDraw, die.OperationPixalY / showSize_Y + offsetDraw);
            //    ROIShape tempselectShape = HomeMapDrawings.Select(shape =>
            //    {
            //        var rectBegin = shape.LeftTop;
            //        var rectEnd = shape.RightBottom;
            //        var rect = new Rect(rectBegin, rectEnd);
            //        if (rect.Contains(clearPoint))
            //            return shape;
            //        else
            //            return null;
            //    }).Where(s => s != null).FirstOrDefault();

            //    if (tempselectShape != null)
            //    {
            //        //RemoveHomeMapShapeAction.Execute(tempselectShape);
            //        tempselectShape.Fill = brush;
            //        tempselectShape.CenterCrossBrush = brush;
            //    }
            //    else
            //    {
            //        AddHomeMapShapeAction.Execute(new ROIRotatedRect
            //        {
            //            Stroke = Brushes.Black,
            //            StrokeThickness = strokeThickness,
            //            Fill = brush,
            //            X = die.OperationPixalX / showSize_X + offsetDraw,
            //            Y = die.OperationPixalY / showSize_Y + offsetDraw,
            //            LengthX = die.DieSize.Width / 2.5 / showSize_X,
            //            LengthY = die.DieSize.Height / 2.5 / showSize_Y,
            //            IsInteractived = true,
            //            IsMoveEnabled = false,
            //            IsResizeEnabled = false,
            //            IsRotateEnabled = false,
            //            CenterCrossLength = crossThickness,
            //            CenterCrossBrush = brush,
            //            ToolTip = "X:" + (die.IndexX) + " Y:" + (die.IndexY) + " X:" + die.MapTransX + " Y:" + die.MapTransY
            //        });
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
        }
        public void HomeMapDieColorReturn(WaferMapping waferMapping, Die die, IEnumerable<BincodeInfo> bincodeListDefault)
        {
            //try
            //{
            //    var clearPoint = new Point(die.OperationPixalX / showSize_X + offsetDraw, die.OperationPixalY / showSize_Y + offsetDraw);
            //    ROIShape tempselectShape = HomeMapDrawings.Select(shape =>
            //    {
            //        var rectBegin = shape.LeftTop;
            //        var rectEnd = shape.RightBottom;
            //        var rect = new Rect(rectBegin, rectEnd);
            //        if (rect.Contains(clearPoint))
            //            return shape;
            //        else
            //            return null;
            //    }).Where(s => s != null).FirstOrDefault();

            //    if (bincodeListDefault == null)
            //    {
            //        BincodeInfo[] pBinCodes = new BincodeInfo[2];
            //        pBinCodes[0] = new BincodeInfo();
            //        pBinCodes[1] = new BincodeInfo();
            //        pBinCodes[0].Code = "000";
            //        pBinCodes[0].Describe = "OK";
            //        pBinCodes[0].Color = Brushes.Green;
            //        pBinCodes[1].Code = "099";
            //        pBinCodes[1].Describe = "NG";
            //        pBinCodes[1].Color = Brushes.Red;
            //        bincodeListDefault = pBinCodes;
            //    }
            //    Brush drawFill = Brushes.Gray;
            //    //判斷要用什麼顏色
            //    foreach (var item2 in bincodeListDefault)
            //    {
            //        if (die.BinCode == item2.Code)
            //        {
            //            drawFill = item2.Color;
            //        }
            //    }

            //    if (tempselectShape != null)
            //    {
            //        //RemoveHomeMapShapeAction.Execute(tempselectShape);
            //        tempselectShape.Fill = drawFill;
            //        tempselectShape.CenterCrossBrush = drawFill;
            //    }
            //    else
            //    {
            //        AddHomeMapShapeAction.Execute(new ROIRotatedRect
            //        {
            //            Stroke = Brushes.Black,
            //            StrokeThickness = strokeThickness,
            //            Fill = drawFill,
            //            X = die.OperationPixalX / showSize_X + offsetDraw,
            //            Y = die.OperationPixalY / showSize_Y + offsetDraw,
            //            LengthX = die.DieSize.Width / 2.5 / showSize_X,
            //            LengthY = die.DieSize.Height / 2.5 / showSize_Y,
            //            IsInteractived = true,
            //            IsMoveEnabled = false,
            //            IsResizeEnabled = false,
            //            IsRotateEnabled = false,
            //            CenterCrossLength = crossThickness,
            //            CenterCrossBrush = drawFill,
            //            ToolTip = "X:" + (die.IndexX) + " Y:" + (die.IndexY) + " X:" + die.MapTransX + " Y:" + die.MapTransY
            //        });
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
        }
        private string CreateProcessFolder()
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
            // var time = DateTime.Now.ToString("HH-mm-ss");

            var path = $"C:\\WLS3200\\{date}\\";
            if (!Directory.Exists(path))
            {

                //新增資料夾
                Directory.CreateDirectory(path);
            }
            return path;
        }
        //WaferIDConfirmOperateCommand
        private async Task<WaferProcessStatus> MacroOperate(PauseTokenSource pts, CancellationTokenSource cts, bool isTopCheck)
        {
            machine.ProcessPause();//暫停

            if (isTopCheck)
            {
                IsInnerRing = true;
                IsOuterRing = false;
            }
            else
            {
                IsInnerRing = false;
                IsOuterRing = true;
            }
            IsMacroDone = false;
            IsMacroJudge = true;
            //切到Macro 頁面
            TabControlSelectedIndex = 4;
            IsOperateUI = false;
            cts.Token.ThrowIfCancellationRequested();
            await pts.Token.WaitWhilePausedAsync(cts.Token);
            //切到Infomation頁面
            TabControlSelectedIndex = 0;
            IsOperateUI = true;
            IsInnerRing = false;
            IsOuterRing = false;
            return macroJudgeOperation;
        }
        private async Task<ProcessStation> MacroDoneOperate(PauseTokenSource pts, CancellationTokenSource cts)
        {
            machine.ProcessPause();//暫停
            IsMacroJudge = false;
            IsMacroDone = true;
            macroDoneOperation = new ProcessStation(1);
            //切到Macro 頁面
            TabControlSelectedIndex = 4;
            IsOperateUI = false;
            cts.Token.ThrowIfCancellationRequested();
            await pts.Token.WaitWhilePausedAsync(cts.Token);
            //切到Infomation頁面
            IsMacroDone = false;
            if (!(macroDoneOperation.MacroTop == WaferProcessStatus.Select || macroDoneOperation.MacroBack == WaferProcessStatus.Select))
            {
                TabControlSelectedIndex = 0;
            }
            IsOperateUI = true;
            return macroDoneOperation;
        }
        private async Task<Point> AlignmentOperate(PauseTokenSource pts, CancellationTokenSource cts, double grabPosX, double grabPosY)
        {
            if (ProcessSetting.IsTestRun)
            {
                ManualPosX = grabPosX;
                ManualPosY = grabPosY;
                return new Point(ManualPosX, ManualPosY);
            }
            else
            {
                machine.ProcessPause();//暫停

                //將原本拍照的座標設成預設值
                ManualPosX = grabPosX;
                ManualPosY = grabPosY;

                //切到Alignment 頁面
                TabControlSelectedIndex = 2;
                IsOperateUI = false;
                cts.Token.ThrowIfCancellationRequested();
                await pts.Token.WaitWhilePausedAsync(cts.Token);
                //切到Infomation頁面
                TabControlSelectedIndex = 0;
                IsOperateUI = true;
                return new Point(ManualPosX, ManualPosY);//最後UI任何操作都會寫入ManualPosX Y 的座標 ，離開之前傳回座標給流程
            }
        }
        private async Task<(WaferProcessStatus, Die[])> MicroOperate(PauseTokenSource pts, CancellationTokenSource cts, Die[] dies)
        {
            pts.IsPaused = true;
            machine.ProcessPause();//暫停
            updateHomeMapTransform = machine.MicroDetection.TransForm;
            isRunningMicroDetection = true;
            //切到Micro 頁面
            TabControlSelectedIndex = 3;
            IsOperateUI = false;
            cts.Token.ThrowIfCancellationRequested();
            await pts.Token.WaitWhilePausedAsync(cts.Token);
            isRunningMicroDetection = false;
            //切到Infomation頁面
            TabControlSelectedIndex = 0;
            IsOperateUI = true;
            try
            {
                TableX.AxisVelocity.MaxVel = TableXMaxVel;
                TableY.AxisVelocity.MaxVel = TableYMaxVel;
                TableZ.AxisVelocity.MaxVel = TableZMaxVel;
                foreach (var item in dies)
                {
                    RectangleInfo rectangleInfo = tempHomeLogAssignRectangles.FirstOrDefault(n => n.Col == item.IndexX && n.Row == item.IndexY);
                    if (rectangleInfo != null)
                    {
                        BincodeInfo bincodeInfo = BincodeList.FirstOrDefault(n => n.Color == rectangleInfo.Fill);
                        if (bincodeInfo != null)
                        {
                            item.BinCode = bincodeInfo.Code;
                        }
                    }
                }
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        ResetDetectionRunningPointList();
                        ResetTempAssign();
                        ShowDetectionHomeNewMapImgae(mainRecipe.DetectRecipe);
                    }
                    catch (Exception ex)
                    {
                    }
                }));
            }
            catch (Exception ex)
            {
            }
            microJudgeOperation = WaferProcessStatus.Pass;
            return (microJudgeOperation, dies);
        }
        private async Task<String> WaferIDOperate(PauseTokenSource pts, CancellationTokenSource cts)
        {
            if (ProcessSetting.IsTestRun)
            {
                TabControlSelectedIndex = 1;
                await Task.Delay(50);//等切過去TableIndex再來變化，不然第一次還沒初始，圖像不會顯示
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        HomeResultImage = new WriteableBitmap(machine.Feeder.Reader.Image.ToBitmapSource());
                    }
                    catch (Exception ex)
                    {
                    }
                }));
                await Task.Delay(1000);
                TabControlSelectedIndex = 0;
                return "Test";
            }
            else
            {
                pts.IsPaused = true;
                machine.ProcessPause();//暫停
                WaferIDManualKeyIn = "";
                //切到Micro 頁面
                TabControlSelectedIndex = 1;
                await Task.Delay(50);//等切過去TableIndex再來變化，不然第一次還沒初始，圖像不會顯示
                IsOperateUI = false;
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        HomeResultImage = new WriteableBitmap(machine.Feeder.Reader.Image.ToBitmapSource());
                    }
                    catch (Exception ex)
                    {
                    }
                }));
                cts.Token.ThrowIfCancellationRequested();
                await pts.Token.WaitWhilePausedAsync(cts.Token);
                //切到Infomation頁面
                TabControlSelectedIndex = 0;
                IsOperateUI = true;
                return WaferIDManualKeyIn;
            }
        }
        private void WaferIDRecord(BitmapSource bitmap)
        {
            System.Drawing.Bitmap bmp = bitmap.ToBitmap();
        }
    }
}
