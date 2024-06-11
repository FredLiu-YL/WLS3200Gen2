using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WLS3200Gen2.Model;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.Module;
using WLS3200Gen2.UserControls;
using YuanliCore.Account;
using YuanliCore.CameraLib;
using YuanliCore.Data;
using YuanliCore.Interface;
using YuanliCore.Machine.Base;
using YuanliCore.Model.Interface;
using YuanliCore.Model.UserControls;
using YuanliCore.Motion;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private Task taskRefresh1 = Task.CompletedTask;
        private UserAccount account;
        private string version;
        private Axis tableX, tableY, tableR, tableZ;
        private Axis robotAxis;
        private AxisConfig tableXConfig, tableYConfig, tableZConfig, tableRConfig, robotAxisConfig;
        private double tableXMaxVel, tableYMaxVel;
        private double tablePosX, tablePosY, tablePosZ, tablePosR;
        private int microCheckNowIndexX, microCheckNowIndexY;
        private ObservableCollection<BincodeInfo> bincodeList = new ObservableCollection<BincodeInfo>();

        private ObservableCollection<CassetteUnitUC> cassetteUC = new ObservableCollection<CassetteUnitUC>();
        private WriteableBitmap mainImage, mapImage;
        private DigitalInput[] digitalInputs;
        private DigitalOutput[] digitalOutputs;
        private IDisposable camlive;
        //WLS3200的文件都放在這 (Recipe、 Log setting)
        private string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\WLS3200";
        //WLS3200 生產相關資訊會存到這
        private string processDataPath;

        private bool isRefresh, isInitialComplete, isWaferInSystem;
        private LoadPortQuantity loadportQuantity = LoadPortQuantity.Pair;

        public ObservableCollection<CassetteUnitUC> CassetteUC
        {
            get => cassetteUC;
            set { SetValue(ref cassetteUC, value); }
        }
        public ICommand AddButtonAction { get; set; }
        public double TableXMaxVel { get => tableXMaxVel; set => SetValue(ref tableXMaxVel, value); }
        public double TableYMaxVel { get => tableYMaxVel; set => SetValue(ref tableYMaxVel, value); }
        public int MicroCheckNowIndexX { get => microCheckNowIndexX; set => SetValue(ref microCheckNowIndexX, value); }
        public int MicroCheckNowIndexY { get => microCheckNowIndexY; set => SetValue(ref microCheckNowIndexY, value); }
        public Axis TableX { get => tableX; set => SetValue(ref tableX, value); }
        public Axis TableY { get => tableY; set => SetValue(ref tableY, value); }
        public Axis TableR { get => tableR; set => SetValue(ref tableR, value); }
        public Axis TableZ { get => tableZ; set => SetValue(ref tableZ, value); }
        public Axis RobotAxis { get => robotAxis; set => SetValue(ref robotAxis, value); }
        public AxisConfig TableXConfig { get => tableXConfig; set => SetValue(ref tableXConfig, value); }
        public AxisConfig TableYConfig { get => tableYConfig; set => SetValue(ref tableYConfig, value); }
        public AxisConfig TableRConfig { get => tableRConfig; set => SetValue(ref tableRConfig, value); }
        public AxisConfig TableZConfig { get => tableZConfig; set => SetValue(ref tableZConfig, value); }
        public AxisConfig RobotAxisConfig { get => robotAxisConfig; set => SetValue(ref robotAxisConfig, value); }
        public DigitalInput[] DigitalInputs { get => digitalInputs; set => SetValue(ref digitalInputs, value); }
        public DigitalOutput[] DigitalOutputs { get => digitalOutputs; set => SetValue(ref digitalOutputs, value); }
        public ObservableCollection<BincodeInfo> BincodeList { get => bincodeList; set => SetValue(ref bincodeList, value); }

        //刷新座標
        public double TablePosX { get => tablePosX; set => SetValue(ref tablePosX, value); }
        public double TablePosY { get => tablePosY; set => SetValue(ref tablePosY, value); }
        public double TablePosZ { get => tablePosZ; set => SetValue(ref tablePosZ, value); }
        public double TablePosR { get => tablePosR; set => SetValue(ref tablePosR, value); }

        public string Version { get => version; set => SetValue(ref version, value); }
        public UserAccount Account { get => account; set => SetValue(ref account, value); }

        public WriteableBitmap MainImage { get => mainImage; set => SetValue(ref mainImage, value); }

        public LoadPortQuantity LoadportQuantity { get => loadportQuantity; set => SetValue(ref loadportQuantity, value); }

        private ObservableCollection<RobotAddress> customers = new ObservableCollection<RobotAddress>();
        public ObservableCollection<RobotAddress> Customers { get => customers; set => SetValue(ref customers, value); }
        /// <summary>
        /// 新增 Shape
        /// </summary>
        public ICommand AddShapeAction { get; set; }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearShapeAction { get; set; }
        /// <summary>
        /// 新增 手動量測Shape
        /// </summary>
        public ICommand AddShapeManualAction { get; set; }
        /// <summary>
        /// 清除 手動量測Shape
        /// </summary>
        public ICommand ClearShapeManualAction { get; set; }
        /// <summary>
        /// Fit 縮放到最適合的大小
        /// </summary>
        public ICommand ZoomFitManualAction { get; set; }
        /// <summary>
        /// Fit 縮放到最適合的大小
        /// </summary>
        public ICommand ZoomRcipeFitManualAction { get; set; }

        public ICommand WindowLoadedCommand => new RelayCommand(async () =>
        {
            try
            {
                ProcessVisibility = Visibility.Hidden;
                InformationUCVisibility = Visibility.Hidden;
                StopVisibility = Visibility.Hidden;
                WorkholderUCVisibility = Visibility.Collapsed;
                Assembly thisAssem = typeof(MainViewModel).Assembly;
                AssemblyName thisAssemName = thisAssem.GetName();

                Version ver = thisAssemName.Version;
                Version = $"WLS3100/3200  {ver}";
                //
                //大部分都會在這裡初始化  有些因為寫法問題必須移動到MainViewModel.cs
                //machineSetting 的讀取放在MainViewModel.cs
                //
                LogMessage = "Initial ．．．";
                machine.Initial();
                SwitchStates(MachineStates.RUNNING);
                IsCanChangeRecipe = false;
                IsOperateUI = false;
                //加入 LOG功能到各模組 一定要放在  machine.Initial()後面
                machine.MicroDetection.WriteLog += WriteLog;
                machine.Feeder.WriteLog += WriteLog;

                await Task.Delay(10);//顯示UI 
                //先將相機開啟
                machine.MicroDetection.Camera.Open();
                await Task.Delay(50);

                //isWaferInSystem = true;
                //isWaferInSystem = await machine.BeforeHomeCheck();
                //MessageBoxResult result = MessageBoxResult.Yes;
                //if (isWaferInSystem)
                //{
                //    result = MessageBox.Show("Wafer In System!! StartHome??", "StartHome", MessageBoxButton.YesNo);
                //}

                //if (result == MessageBoxResult.Yes)
                //{
                //    await machine.Home();
                //}
                //else
                //{
                //    //直接關掉程式!
                //    machine.Disopse();
                //    isRefresh = false;
                //    await Task.Delay(500);
                //    Application.Current.Shutdown();

                //}
                if (machineSetting.BincodeListDefault == null)
                {
                    BincodeInfo bincode1 = new BincodeInfo
                    {
                        Code = "000",
                        Describe = "OK",
                        Color = Brushes.Blue
                    };
                    BincodeInfo bincode2 = new BincodeInfo
                    {
                        Code = "999",
                        Describe = "Error",
                        Color = Brushes.GreenYellow
                    };
                    BincodeList.Add(bincode1);
                    BincodeList.Add(bincode2);
                }
                else
                {
                    BincodeListUpdate(machineSetting.BincodeListDefault);
                }
                bool isInitialMicroscopeLensDefaultOK = false;
                try
                {
                    if (machineSetting.MicroscopeLensDefault != null)
                    {
                        MicroscopeLensDefault = (ObservableCollection<MicroscopeLens>)machineSetting.MicroscopeLensDefault;
                        for (int i = 0; i < MicroscopeLensDefault.Count; i++)
                        {
                            if (MicroscopeLensDefault[i] == null)
                            {
                                MicroscopeLensDefault[i] = new MicroscopeLens();
                            }
                        }
                        if (MicroscopeLensDefault.Count >= 6)
                        {
                            isInitialMicroscopeLensDefaultOK = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                if (isInitialMicroscopeLensDefaultOK == false)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        MicroscopeLensDefault.Add(new MicroscopeLens());
                    }
                }
                MicroscopeParam.LensName.Clear();
                for (int i = 1; i < MicroscopeLensDefault.Count; i++)
                {
                    MicroscopeParam.LensName.Add(MicroscopeLensDefault[i].LensName);
                }

                TableX = machine.MicroDetection.AxisX;
                TableY = machine.MicroDetection.AxisY;
                TableR = machine.MicroDetection.AxisR;
                TableZ = machine.MicroDetection.AxisZ;
                RobotAxis = machine.Feeder.RobotAxis;

                Robot = machine.Feeder.Robot;
                Microscope = machine.MicroDetection.Microscope;
                Macro = machine.Feeder.Macro;
                Aligner = machine.Feeder.AlignerL;
                LoadPort1 = machine.Feeder.LoadPortL;
                LampControl1 = machine.Feeder.LampControl1;
                LampControl2 = machine.Feeder.LampControl2;

                DigitalInputs = machine.GetInputs();
                DigitalOutputs = machine.GetOutputs();

                TableXConfig = machineSetting.TableXConfig.Copy();
                TableYConfig = machineSetting.TableYConfig.Copy();
                TableRConfig = machineSetting.TableRConfig.Copy();
                TableZConfig = machineSetting.TableZConfig.Copy();
                RobotAxisConfig = machineSetting.RobotAxisConfig.Copy();

                TableXMaxVel = TableXConfig.MoveVel.MaxVel;
                TableYMaxVel = TableYConfig.MoveVel.MaxVel;
                IsAutoSave = ProcessSetting.IsAutoSave;
                IsAutoFocus = true;
                IsAutoFocus = ProcessSetting.IsAutoFocus;

                InformationUCVisibility = Visibility.Hidden;
                WorkholderUCVisibility = Visibility.Collapsed;


                WriteLog(YuanliCore.Logger.LogType.PROCESS, "Equipment Ready．．．");

                isRefresh = true;
                taskRefresh1 = Task.Run(RefreshPos);

                SampleFind = SampleFindAction;
                AlignMarkMove = SampleMoveAction;

                CameraLive();
                await Task.Delay(50);
                machine.MicroDetection.FiducialRecord += FiducialRecord;
                machine.MicroDetection.AlignManual += AlignmentOperate;
                machine.MicroDetection.AlignmentManualAdd();
                machine.MicroDetection.DetectionRecord += DetectionRecord;
                machine.MicroDetection.MicroReady += MicroOperate;
                machine.Feeder.WaferIDRecord += WaferIDRecord;
                machine.Feeder.WaferIDReady += WaferIDOperate;

                for (int i = 0; i < 25; i++)
                {
                    ToolLoadPort1ComboBox.Add((i + 1).ToString());
                }

                //這裡的Await是要重新刷新UI才可以讓ZoomFitManualAction有效果
                await Task.Delay(1000);
                ZoomFitManualAction.Execute(Drawings);

                isInitialComplete = true;

                //載入機台設定LoadPort 數量  給UI作卡控
                LoadportQuantity = machineSetting.LoadPortCount;


                if (!File.Exists("MAP1.bmp")) throw new Exception("模擬情境下需要放一張圖片到執行檔資料夾 取名MAP1.bmp");
                // 讀取BMP檔案
                BitmapImage bitmap = new BitmapImage(new Uri("MAP1.bmp", UriKind.RelativeOrAbsolute));

                Customers = new ObservableCollection<RobotAddress>()
            {
                new RobotAddress() { Name = "LoadPort1 Step1", Address = "110" },
                new RobotAddress() { Name = "LoadPort1 Step2", Address = "111" },
                new RobotAddress() { Name = "LoadPort1 Step3", Address = "112" },
                new RobotAddress() { Name = "LoadPort1 Step4", Address = "113" },
                new RobotAddress() { Name = "LoadPort1 Step5", Address = "114" },
                new RobotAddress() { Name = "LoadPort1 Step1", Address = "110" },
                new RobotAddress() { Name = "LoadPort1 Step2", Address = "111" },
                new RobotAddress() { Name = "LoadPort1 Step3", Address = "112" },
                new RobotAddress() { Name = "LoadPort1 Step4", Address = "113" },
                new RobotAddress() { Name = "LoadPort1 Step5", Address = "114" },
            };




                Customers.Add(new RobotAddress() { Name = "LoadPort1 Step1", Address = "110" });
                SwitchStates(MachineStates.IDLE);
                IsOperateUI = true;
            }
            catch (Exception ex)
            {
                SwitchStates(MachineStates.Alarm);
                MessageBox.Show(ex.ToString());
            }
            finally
            {

            }
        });

        private void BincodeListUpdate(IEnumerable<BincodeInfo> bincodeListDefault)
        {
            try
            {
                if (bincodeListDefault != null)
                {
                    foreach (var item in BincodeList)
                    {
                        try
                        {
                            item.PropertyChanged -= BincodeList_PropertyChanged;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    BincodeList.Clear();
                    foreach (BincodeInfo item in bincodeListDefault)
                    {
                        BincodeList.Add(new BincodeInfo() { Code = item.Code, Describe = item.Describe, Assign = item.Assign, Color = item.Color });
                    }
                    foreach (BincodeInfo item in BincodeList)
                    {
                        item.PropertyChanged += BincodeList_PropertyChanged;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void BincodeList_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e is PropertyChangedEventArgsExtended extendedArgs)
            {
                string propertyName = extendedArgs.PropertyName;
                object oldValue = extendedArgs.OldValue;
                object newValue = extendedArgs.NewValue;
                string code = extendedArgs.Code;
                if (propertyName == "Assign")
                {
                    if (isRunningMicroDetection)
                    {
                        Point mapPoint = machine.MicroDetection.TransForm.TransInvertPoint(new Point(TablePosX, TablePosY));
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
                            nowAssignDie = mainRecipe.DetectRecipe.WaferMap.Dies.FirstOrDefault(r => r.IndexX == tempselectRects.Col && r.IndexY == tempselectRects.Row);
                            Assign(code);
                        }
                        else
                        {
                            nowAssignDie = null;
                        }
                    }
                }
            }
        }
        private void Assign(string binCode)
        {
            try
            {
                //如果是在執行的狀態才能下BinCode
                lock (lockAssignObj)
                {
                    BincodeInfo assignBincodeInfo = mainRecipe.DetectRecipe.BincodeList.Where(r => r.Code == binCode).FirstOrDefault();
                    if (assignBincodeInfo != null && nowAssignDie != null)
                    {
                        //執行中的
                        foreach (DetectionPoint item in DetectionHomePointList)
                        {
                            if (item.IndexX == nowAssignDie.IndexX && item.IndexY == nowAssignDie.IndexY)
                            {
                                item.Code = assignBincodeInfo.Code;
                            }
                        }
                        HomeNewMapAssignDieColorChange(nowAssignDie, assignBincodeInfo.Color, Brushes.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 顯示目前平台位置相對應到畫面上的Wafer點位
        /// </summary>
        /// <param name="nowTablePosX"></param>
        /// <param name="nowTablePosY"></param>
        private void ShowNowTablePosInHomeWaferMapDie(double nowTablePosX, double nowTablePosY)
        {
            try
            {
                Point mapPoint = new Point(nowTablePosX, nowTablePosY);
                mapPoint = machine.MicroDetection.TransForm.TransInvertPoint(new Point(nowTablePosX, nowTablePosY));
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

                var transMapMousePixcel = new Point(transMapMousePixcelX, transMapMousePixcelY);

                Rectangle nowSelectRange = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 5,
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(nowSelectRange, transMapMousePixcel.X);
                Canvas.SetTop(nowSelectRange, transMapMousePixcel.Y);

                var rect = new Rect(Canvas.GetLeft(nowSelectRange), Canvas.GetTop(nowSelectRange), nowSelectRange.Width, nowSelectRange.Height);
                RectangleInfo tempselectRects = RectanglesHome.FirstOrDefault(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                 || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight));
                ChangeHomeMappingSelect(nowSelectRange);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Map位置轉換成要顯示的Pixel要乘上的Scale
        /// </summary>
        /// <param name="drawWidth"></param>
        /// <param name="drawHeight"></param>
        /// <param name="drawCuttingline"></param>
        /// <returns></returns>
        public Point MapPointToMapImagePixelScale(double drawWidth, double drawHeight, double drawCuttingline)
        {
            try
            {
                //var mapPoint11 = new Point(transDie.OperationPixalX, transDie.OperationPixalY);
                //var mapImagePoint11 = new Point((drawWidth + drawCuttingline) * 3 + startPoint.X / 2, (drawHeight + drawCuttingline) * 7 + startPoint.Y / 2);
                //var scale1 = new Point((mapImagePoint11.X - startPoint.X / 2) / mapPoint11.X, (mapImagePoint11.Y - startPoint.Y / 2) / mapPoint11.Y);
                Point startPoint = new Point(drawWidth, drawHeight);
                var transDie = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == 3 && d.IndexY == 7).FirstOrDefault();
                var scale2 = new Point((drawWidth + drawCuttingline) / transDie.DieSize.Width, (drawHeight + drawCuttingline) / transDie.DieSize.Height);
                var scale = scale2;
                return scale;
            }
            catch (Exception)
            {

                return new Point(0, 0);
            }
        }


        public ICommand WindowClosingCommand => new RelayCommand<CancelEventArgs>(async e =>
        {
            try
            {
                // 取消預設的視窗關閉行為
                e.Cancel = true;
                var result = MessageBox.Show("是否關閉程式?", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No)
                {
                    // 如果使用者選擇 "否"，則取消視窗關閉
                    e.Cancel = true;
                    return;

                }
                machine.Disopse();

                isRefresh = false;
                // 如果使用者選擇 "是"，則允許視窗關閉
                e.Cancel = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        public ICommand DragTESTCommand => new RelayCommand<DragDeltaEventArgs>(async e =>
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

        private void FiducialRecord(BitmapSource bitmap, Point? pixel, int number)
        {
            var bmp = bitmap.ToBitmap();
            // 建立Graphics物件
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bmp))
            {
                if (pixel.HasValue)//有座標表示有對位成功 ，劃出紅色十字
                {
                    // 繪製紅色直線 (橫線)
                    System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red);

                    graphics.DrawLine(pen, (float)(pixel.Value.X - 20), (float)pixel.Value.Y, (float)(pixel.Value.X + 20), (float)pixel.Value.Y);

                    // 繪製紅色直線 (豎線)
                    graphics.DrawLine(pen, (float)pixel.Value.X, (float)(pixel.Value.Y - 20), (float)pixel.Value.X, (float)(pixel.Value.Y + 20));

                    // 釋放Pen物件
                    pen.Dispose();

                }
                try
                {
                    bmp.Save(processDataPath + $"\\Fiducial-{number}.bmp");
                }
                catch (Exception)
                {
                }

            }
        }
        private void DetectionRecord(BitmapSource bitmap, DetectionPoint point)
        {
            try
            {
                string path = $"{ machine.MicroDetection.GrabSaveFolder }\\MicroPhoto\\{ machine.MicroDetection.GrabTitleIdx}_{mainRecipe.Name}_{machine.MicroDetection.GrabSavePicturTime}_{point.IndexX}_{point.IndexY}.bmp";
                bitmap.ToBitmap().Save(path);
                machine.MicroDetection.GrabTitleIdx += 1;
            }
            catch (Exception ex)
            {

            }
        }


        private void SwitchStates(MachineStates states)
        {
            Machinestatus = states;
            if (machine.StackLight != null)
                machine.StackLight.SwitchStates(states);
        }

        //Security進入會執行
        private void LoadSecurityPage()
        {
            if (RightsModel.Operator == Account.CurrentAccount.Right || RightsModel.Visitor == Account.CurrentAccount.Right)
            {
                IsMainSecurityPageSelect = false;
                IsMainHomePageSelect = true;
                return;
            }
            WriteLog(YuanliCore.Logger.LogType.TRIG, "Enter the SecurityPage");
        }


        //離開Security頁面會執行
        private void UnLoadSecurityPage()
        {
            if (isInitialComplete)
            {
                //軸 修改參數存檔
                machineSetting.TableXConfig = TableXConfig.Copy();
                machineSetting.TableYConfig = TableYConfig.Copy();
                machineSetting.TableRConfig = TableRConfig.Copy();
                machineSetting.TableZConfig = TableZConfig.Copy();
                machineSetting.RobotAxisConfig = RobotAxisConfig.Copy();

                machineSetting.Save(machineSettingPath);
            }

        }
        private void RefreshHomeMap(double tablePosX, double tablePosY)
        {
            try
            {
                lock (lockHomeMapObj)
                {
                    if (isUpdateHomeMap == false && updateHomeMapTransform != null)
                    {
                        isUpdateHomeMap = true;
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var mapPoint = updateHomeMapTransform.TransInvertPoint(new Point(tablePosX, tablePosY));
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

                            Rect rect = new Rect(Canvas.GetLeft(nowSelectRange), Canvas.GetTop(nowSelectRange), nowSelectRange.Width, nowSelectRange.Height);
                            RectangleInfo tempselectRects = RectanglesHome.FirstOrDefault(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                              || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight));
                            if (tempselectRects != null)
                            {
                                MicroCheckNowIndexX = tempselectRects.Col;
                                MicroCheckNowIndexY = tempselectRects.Row;
                            }
                            isUpdateHomeMap = false;
                        }));
                        while (isUpdateHomeMap)
                        {
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }
        private void RefreshRecipeMap(double tablePosX, double tablePosY)
        {
            try
            {
                lock (lockRecipeMapObj)
                {
                    if (isUpdateRecipeMap == false && updateRecipeMapTransform != null)
                    {
                        isUpdateRecipeMap = true;
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var mapPoint = updateRecipeMapTransform.TransInvertPoint(new Point(tablePosX, tablePosY));
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
                            ChangeRecipeMappingSelect(nowSelectRange);

                            Rect rect = new Rect(Canvas.GetLeft(nowSelectRange), Canvas.GetTop(nowSelectRange), nowSelectRange.Width, nowSelectRange.Height);
                            RectangleInfo tempselectRects = RectanglesHome.FirstOrDefault(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                              || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight));
                            if (tempselectRects != null)
                            {
                                MicroCheckNowIndexX = tempselectRects.Col;
                                MicroCheckNowIndexY = tempselectRects.Row;
                            }
                            isUpdateRecipeMap = false;
                        }));
                        while (isUpdateRecipeMap)
                        {
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }
        }

        private async Task RefreshPos()
        {
            try
            {

                while (isRefresh)
                {
                    if (isInitialComplete)
                    {
                        //var pos = atfMachine.Table_Module.GetPostion();
                        TablePosX = machine.MicroDetection.AxisX.Position;
                        TablePosY = machine.MicroDetection.AxisY.Position;
                        TablePosZ = machine.MicroDetection.AxisZ.Position;
                        TablePosR = machine.MicroDetection.AxisR.Position;
                        if (isRunningMicroDetection)
                        {
                            RefreshHomeMap(TablePosX, TablePosY);

                        }
                        if (isRecipeAlignment && isMapAdding == false)
                        {
                            RefreshRecipeMap(TablePosX, TablePosY);
                        }
                        //if (atfMachine.AFModule.AFSystem != null)
                        //    PositionZ = (int)atfMachine.AFModule.AFSystem.AxisZPosition;


                        if (isMainRecipePageSelect || isMainSecurityPageSelect || isMainHomePageSelect)
                        {
                            if (machine.MicroDetection.Microscope != null)
                            {
                                MicroscopeParam.Position = Convert.ToInt32(machine.MicroDetection.Microscope.Position);
                                MicroscopeParam.ApertureValue = machine.MicroDetection.Microscope.ApertureValue;
                                MicroscopeParam.LightValue = machine.MicroDetection.Microscope.LightValue;
                                AFAberation.AberationValue = Convert.ToInt32(machine.MicroDetection.Microscope.AberationPosition);
                            }
                            if (machine.Feeder.LampControl1 != null)
                            {
                                LampControl1Param.LightValue = machine.Feeder.LampControl1.LightValue;
                            }
                            if (machine.Feeder.LampControl2 != null)
                            {
                                LampControl2Param.LightValue = machine.Feeder.LampControl2.LightValue;
                            }
                        }
                        if (isMainToolsPageSelect)
                        {
                            IsMicroVaccum8 = digitalInputs[4].IsSignal;
                            IsMicroVaccum12 = digitalInputs[5].IsSignal;
                            IsMacroVaccum8 = digitalInputs[6].IsSignal;
                            IsMacroVaccum12 = digitalInputs[7].IsSignal;
                            IsAlignerVaccum12 = machine.Feeder.AlignerL.IsLockOK;
                        }
                        if (isMainSecurityPageSelect)
                        {
                            if (machine.Feeder.LoadPortL != null)
                            {
                                LoadPortStatus loadPortStatus = new LoadPortStatus();
                                loadPortStatus = await machine.Feeder.LoadPortL.GetStatus();
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
                            if (machine.Feeder.AlignerL != null)
                            {
                                AlignerStatus alignerStatus = new AlignerStatus();
                                alignerStatus = await machine.Feeder.AlignerL.GetStatus();
                                AlignerUIShow.DeviceStatus = alignerStatus.DeviceStatus;
                                AlignerUIShow.ErrorCode = alignerStatus.ErrorCode;
                                AlignerUIShow.NotchStatus = alignerStatus.NotchStatus;
                                AlignerUIShow.IsWafer = alignerStatus.IsWafer;
                                AlignerUIShow.IsOrg = alignerStatus.IsOrg;
                                AlignerUIShow.IsVaccum = alignerStatus.IsVaccum;
                            }
                            if (machine.Feeder.Robot != null)
                            {
                                RobotStatus robotStatus = new RobotStatus();
                                robotStatus = await machine.Feeder.Robot.GetStatus();
                                RobotStaus.Mode = robotStatus.Mode;
                                RobotStaus.IsStopSignal = robotStatus.IsStopSignal;
                                RobotStaus.IsEStopSignal = robotStatus.IsEStopSignal;
                                RobotStaus.IsCommandDoneSignal = robotStatus.IsCommandDoneSignal;
                                RobotStaus.IsMovDoneSignal = robotStatus.IsMovDoneSignal;
                                RobotStaus.IsRunning = robotStatus.IsRunning;
                                RobotStaus.ErrorCode = robotStatus.ErrorCode;
                                RobotStaus.ErrorXYZWRC = Convert.ToInt32("" + robotStatus.ErrorX + robotStatus.ErrorY + robotStatus.ErrorZ + robotStatus.ErrorW + robotStatus.ErrorR + robotStatus.ErrorC);

                                RobotStaus.IsLockOK = machine.Feeder.Robot.IsLockOK;
                                //RobotUIIShow
                            }

                            if (machine.Feeder.LampControl1 != null)
                            {
                                LampControl1Param.LightValue = machine.Feeder.LampControl1.LightValue;
                            }
                            if (machine.Feeder.LampControl2 != null)
                            {
                                LampControl2Param.LightValue = machine.Feeder.LampControl2.LightValue;
                            }
                        }
                        await Task.Delay(300);
                    }
                }

            }
            catch (Exception ex)
            {


                SwitchStates(MachineStates.Emergency);
                throw ex;
            }

        }


        private void CameraLive()
        {
            var camera = machine.MicroDetection.Camera;
            var dis = machine.MicroDetection.Camera.Grab();
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
            catch (FlowException ex2)
            {

            }
            catch (Exception ex)
            {


            }

        }
    }


    /// <summary>
    /// 如果是雙Port 就把loadport有關功能打開
    /// </summary>
    public class IsLoadPortEnableConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((LoadPortQuantity)value)
            {
                case LoadPortQuantity.Single:
                    return false;

                case LoadPortQuantity.Pair:
                    return true;

                default:
                    return false;

            }



        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    /// <summary>
    /// 如果是雙Port 就把loadport有關功能打開
    /// </summary>
    public class IsLoadPortVisibleConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            switch ((LoadPortQuantity)value)
            {
                case LoadPortQuantity.Single:
                    return Visibility.Hidden;

                case LoadPortQuantity.Pair:
                    return Visibility.Visible;

                default:
                    return Visibility.Hidden;
            }



        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
