using GalaSoft.MvvmLight.Command;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.UserControls;
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
        private bool isRunning = false;
        private bool isRunCommand = false;
        private ProcessSetting processSetting;

        private Visibility informationUIVisibility, workholderUCVisibility;

        private int tabControlSelectedIndex; // 0:Process Infomation   1:Alignment  2:Micro  3 :Macro
        private bool isOperateUI = true;

        private double manualPosX, manualPosY;
        private MachineStates machinestatus = MachineStates.IDLE;
        private WaferProcessStatus macroJudgeOperation;
        private WaferProcessStatus microJudgeOperation = WaferProcessStatus.Pass;
        private bool isLoadport1, isLoadport2;
        private IMicroscope microscope;
        private YuanliCore.Model.MicroscopeParam microscopeParam = new YuanliCore.Model.MicroscopeParam();
        private IMacro macro;
        private IAligner aligner;
        private ILoadPort loadPort1;
        private IRobot robot;
        private MacroStatus macroStatus = new MacroStatus();
        private bool isAutoSave, isAutoFocus;

        public bool IsRunning { get => isRunning; set => SetValue(ref isRunning, value); }
        /// <summary>
        /// 在很多情況下 流程進行到一半需要人為操作 ，此時需要卡控不必要按鈕鎖住
        /// </summary>
        public bool IsOperateUI { get => isOperateUI; set => SetValue(ref isOperateUI, value); }

        public string LogMessage { get => logMessage; set => SetValue(ref logMessage, value); }
        public Visibility ProcessVisibility { get => processVisibility; set => SetValue(ref processVisibility, value); }
        public ProcessSetting ProcessSetting { get => processSetting; set => SetValue(ref processSetting, value); }

        public Visibility processVisibility = Visibility.Visible;
        public Visibility InformationUCVisibility { get => informationUIVisibility; set => SetValue(ref informationUIVisibility, value); }
        public Visibility WorkholderUCVisibility { get => workholderUCVisibility; set => SetValue(ref workholderUCVisibility, value); }
        public int TabControlSelectedIndex { get => tabControlSelectedIndex; set => SetValue(ref tabControlSelectedIndex, value); }
        public double ManualPosX { get => manualPosX; set => SetValue(ref manualPosX, value); }
        public double ManualPosY { get => manualPosY; set => SetValue(ref manualPosY, value); }
        public MachineStates Machinestatus { get => machinestatus; set => SetValue(ref machinestatus, value); }
        public ObservableCollection<ProcessStation> ProcessStations { get => processStations; set => SetValue(ref processStations, value); }

        public bool IsLoadport1 { get => isLoadport1; set => SetValue(ref isLoadport1, value); }
        public bool IsLoadport2 { get => isLoadport2; set => SetValue(ref isLoadport2, value); }
        public IMicroscope Microscope { get => microscope; set => SetValue(ref microscope, value); }
        public YuanliCore.Model.MicroscopeParam MicroscopeParam { get => microscopeParam; set => SetValue(ref microscopeParam, value); }
        public IMacro Macro { get => macro; set => SetValue(ref macro, value); }
        public MacroStatus MacroStatus { get => macroStatus; set => SetValue(ref macroStatus, value); }
        public IAligner Aligner { get => aligner; set => SetValue(ref aligner, value); }
        public ILoadPort LoadPort1 { get => loadPort1; set => SetValue(ref loadPort1, value); }
        public IRobot Robot { get => robot; set => SetValue(ref robot, value); }
        public bool IsAutoSave { get => isAutoSave; set => SetValue(ref isAutoSave, value); }
        public bool IsAutoFocus { get => isAutoFocus; set => SetValue(ref isAutoFocus, value); }
        public ICommand RunCommand => new RelayCommand(async () =>
        {
            try
            {
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

                if (isRunCommand == false)
                {
                    isRunCommand = true;




                    IsRunning = true;
                    WriteLog("Process Start");
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

                    ProcessSetting.Save(processSettingPath);//ProcessSetting存檔 
                    await machine.ProcessRunAsync(ProcessSetting);

                    SwitchStates(MachineStates.IDLE);

                    isRunCommand = false;
                    IsRunning = false;
                }
                else
                {
                    await machine.ProcessResume();
                }

            }
            catch (Exception ex)
            {
                WriteLog(ex.Message);
                MessageBox.Show(ex.Message);
                isRunCommand = false;

                SwitchStates(MachineStates.Alarm);
            }
            finally
            {
                WriteLog("Process Finish");

            }
        });


        public ICommand PauseCommand => new RelayCommand(async () =>
        {
            try
            {

                IsRunning = false;
                // ProcessVisibility = Visibility.Hidden;

                SwitchStates(MachineStates.PAUSED);
                await machine.ProcessPause();


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
                IsRunning = false;

                SwitchStates(MachineStates.RUNNING);
                await machine.ProcessResume();
                ProcessVisibility = Visibility.Visible;
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
                ProcessVisibility = Visibility.Visible;
                SwitchStates(MachineStates.IDLE);

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
                    var recipename = win.FileName;
                    mainRecipe.Load(path, recipename);
                    ShowHomeMapImgae(mainRecipe.DetectRecipe);
                    SetRecipeToLoadWaferParam(mainRecipe.EFEMRecipe);
                    SetRecipeToLocateParam(mainRecipe.DetectRecipe);
                    SetRecipeToDetectionParam(mainRecipe.DetectRecipe);
                    //ShowDetectionHomeMapImgae(mainRecipe.DetectRecipe);
                    //if (mainRecipe.DetectRecipe.WaferMap != null)
                    //    await ShowMappingDrawings(mainRecipe.DetectRecipe.WaferMap.Dies, mainRecipe.DetectRecipe.BincodeList, mainRecipe.DetectRecipe.WaferMap.ColumnCount, mainRecipe.DetectRecipe.WaferMap.RowCount, 3000);
                    //ShowDetectionMapImgae(mainRecipe.DetectRecipe);
                    WriteLog("Load Recipe :" + recipename);
                }

            }
            catch (Exception ex)
            {

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
                    mainRecipe.RecipeSave(path, recipename);

                    WriteLog("Save Recipe :" + recipename);
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

                SwitchStates(MachineStates.RUNNING);

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
            }
        });

        public ICommand TESTCommand => new RelayCommand(async () =>
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
        public ICommand ManualReFindCommand => new RelayCommand(async () =>
       {
           try
           {
               await machine.MicroDetection.FindFiducial(MainImage, TablePosX, TablePosY);

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
        public ICommand VolumeOffCommand => new RelayCommand(async () =>
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
        public ICommand TEST1Command => new RelayCommand(async () =>
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
        private PauseTokenSource ptsTest = new PauseTokenSource();
        private CancellationTokenSource ctsTest = new CancellationTokenSource();
        public ICommand TestCommand => new RelayCommand(async () =>
        {
            try
            {
                //List<MicroscopeLens> microscopeLenses = new List<MicroscopeLens>();
                //for (int i = 0; i <= 5; i++)
                //{
                //    MicroscopeLens microscopeLens = new MicroscopeLens();
                //    microscopeLenses.Add(microscopeLens);
                //}
                //machineSetting.MicroscopeLensDefault = microscopeLenses;


                MainRecipe recipe = mainRecipe;
                Wafer currentWafer = new Wafer(1);
                currentWafer.Dies = recipe.DetectRecipe.WaferMap.Dies;
                await machine.MicroDetection.Run(recipe.DetectRecipe, processSetting, currentWafer, ptsTest, ctsTest);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand Test1Command => new RelayCommand(async () =>
        {
            try
            {
                ptsTest.IsPaused = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public void ShowDetectionHomeMapImgae(DetectionRecipe detectionRecipe)
        {
            try
            {
                if (detectionRecipe.DetectionPoints == null) return;
                foreach (var item in detectionRecipe.DetectionPoints)
                {
                    Die die = detectionRecipe.WaferMap.Dies.Where(d => d.IndexX == item.IndexX && d.IndexY == item.IndexY).FirstOrDefault();
                    if (die != null)
                    {
                        HomeMapDieColorChange(detectionRecipe.WaferMap, die, Brushes.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void HomeMapDieColorChange(WaferMapping waferMapping, Die die, Brush brush)
        {
            try
            {
                var clearPoint = new Point(die.OperationPixalX / showSize_X + offsetDraw, die.OperationPixalY / showSize_Y + offsetDraw);
                ROIShape tempselectShape = HomeMapDrawings.Select(shape =>
                {
                    var rectBegin = shape.LeftTop;
                    var rectEnd = shape.RightBottom;
                    var rect = new Rect(rectBegin, rectEnd);
                    if (rect.Contains(clearPoint))
                        return shape;
                    else
                        return null;
                }).Where(s => s != null).FirstOrDefault();

                if (tempselectShape != null)
                {
                    //RemoveHomeMapShapeAction.Execute(tempselectShape);
                    tempselectShape.Fill = brush;
                    tempselectShape.CenterCrossBrush = brush;
                }
                else
                {
                    AddHomeMapShapeAction.Execute(new ROIRotatedRect
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = strokeThickness,
                        Fill = brush,
                        X = die.OperationPixalX / showSize_X + offsetDraw,
                        Y = die.OperationPixalY / showSize_Y + offsetDraw,
                        LengthX = die.DieSize.Width / 2.5 / showSize_X,
                        LengthY = die.DieSize.Height / 2.5 / showSize_Y,
                        IsInteractived = true,
                        IsMoveEnabled = false,
                        IsResizeEnabled = false,
                        IsRotateEnabled = false,
                        CenterCrossLength = crossThickness,
                        CenterCrossBrush = brush,
                        ToolTip = "X:" + (die.IndexX) + " Y:" + (die.IndexY) + " X:" + die.MapTransX + " Y:" + die.MapTransY
                    });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void HomeMapDieColorReturn(WaferMapping waferMapping, Die die, IEnumerable<BincodeInfo> bincodeListDefault)
        {
            try
            {
                var clearPoint = new Point(die.OperationPixalX / showSize_X + offsetDraw, die.OperationPixalY / showSize_Y + offsetDraw);
                ROIShape tempselectShape = HomeMapDrawings.Select(shape =>
                {
                    var rectBegin = shape.LeftTop;
                    var rectEnd = shape.RightBottom;
                    var rect = new Rect(rectBegin, rectEnd);
                    if (rect.Contains(clearPoint))
                        return shape;
                    else
                        return null;
                }).Where(s => s != null).FirstOrDefault();

                if (bincodeListDefault == null)
                {
                    BincodeInfo[] pBinCodes = new BincodeInfo[2];
                    pBinCodes[0] = new BincodeInfo();
                    pBinCodes[1] = new BincodeInfo();
                    pBinCodes[0].Code = "000";
                    pBinCodes[0].Describe = "OK";
                    pBinCodes[0].Color = Brushes.Green;
                    pBinCodes[1].Code = "099";
                    pBinCodes[1].Describe = "NG";
                    pBinCodes[1].Color = Brushes.Red;
                    bincodeListDefault = pBinCodes;
                }
                Brush drawFill = Brushes.Gray;
                //判斷要用什麼顏色
                foreach (var item2 in bincodeListDefault)
                {
                    if (die.BinCode == item2.Code)
                    {
                        drawFill = item2.Color;
                    }
                }

                if (tempselectShape != null)
                {
                    //RemoveHomeMapShapeAction.Execute(tempselectShape);
                    tempselectShape.Fill = drawFill;
                    tempselectShape.CenterCrossBrush = drawFill;
                }
                else
                {
                    AddHomeMapShapeAction.Execute(new ROIRotatedRect
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = strokeThickness,
                        Fill = drawFill,
                        X = die.OperationPixalX / showSize_X + offsetDraw,
                        Y = die.OperationPixalY / showSize_Y + offsetDraw,
                        LengthX = die.DieSize.Width / 2.5 / showSize_X,
                        LengthY = die.DieSize.Height / 2.5 / showSize_Y,
                        IsInteractived = true,
                        IsMoveEnabled = false,
                        IsResizeEnabled = false,
                        IsRotateEnabled = false,
                        CenterCrossLength = crossThickness,
                        CenterCrossBrush = drawFill,
                        ToolTip = "X:" + (die.IndexX) + " Y:" + (die.IndexY) + " X:" + die.MapTransX + " Y:" + die.MapTransY
                    });
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
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

        private async Task<WaferProcessStatus> MacroOperate(PauseTokenSource pts, CancellationTokenSource cts)
        {
            machine.ProcessPause();//暫停

            //切到Macro 頁面
            TabControlSelectedIndex = 3;
            IsOperateUI = false;
            cts.Token.ThrowIfCancellationRequested();
            await pts.Token.WaitWhilePausedAsync(cts.Token);
            //切到Infomation頁面
            TabControlSelectedIndex = 0;
            IsOperateUI = true;
            return macroJudgeOperation;
        }
        private async Task<WaferProcessStatus> MicroOperate(PauseTokenSource pts, CancellationTokenSource cts)
        {
            pts.IsPaused = true;
            //machine.ProcessPause();//暫停

            //切到Micro 頁面
            TabControlSelectedIndex = 3;
            IsOperateUI = false;
            cts.Token.ThrowIfCancellationRequested();
            await pts.Token.WaitWhilePausedAsync(cts.Token);
            //切到Infomation頁面
            TabControlSelectedIndex = 0;
            IsOperateUI = true;
            return microJudgeOperation;
        }
        private async Task<String> WaferIDOperate(PauseTokenSource pts, CancellationTokenSource cts)
        {
            pts.IsPaused = true;
            //machine.ProcessPause();//暫停

            //切到Micro 頁面
            TabControlSelectedIndex = 1;
            IsOperateUI = false;
            cts.Token.ThrowIfCancellationRequested();
            await pts.Token.WaitWhilePausedAsync(cts.Token);
            //切到Infomation頁面
            TabControlSelectedIndex = 0;
            IsOperateUI = true;
            return "";
        }
    }
}
