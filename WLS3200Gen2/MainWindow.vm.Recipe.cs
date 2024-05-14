using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.UserControls;
using YuanliCore.Account;
using YuanliCore.AffineTransform;
using YuanliCore.CameraLib;
using YuanliCore.Data;
using YuanliCore.ImageProcess.Match;
using YuanliCore.Interface;
using YuanliCore.Model;
using YuanliCore.Model.UserControls;
using YuanliCore.Views.CanvasShapes;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {
        private CogMatcher matcher = new CogMatcher(); //使用Vision pro 實體
        private PatmaxParams matchParam = new PatmaxParams(0);

        private ObservableCollection<WaferUIData> loadPort1Wafers = new ObservableCollection<WaferUIData>();
        private int loadPort1WaferSelect;
        private ObservableCollection<WaferUIData> loadPort2Wafers = new ObservableCollection<WaferUIData>();
        private ObservableCollection<LocateParam> locateParamList = new ObservableCollection<LocateParam>();
        private ObservableCollection<DetectionPoint> detectionPointList = new ObservableCollection<DetectionPoint>();

        private BitmapSource locateSampleImage1;
        private BitmapSource locateSampleImage2;
        private BitmapSource locateSampleImage3;

        private LocateParam locateParam1 = new LocateParam(101);//Locate pattern 從100號開始
        private LocateParam locateParam2 = new LocateParam(102);//Locate pattern 從100號開始
        private LocateParam locateParam3 = new LocateParam(103);//Locate pattern 從100號開始
        private ObservableCollection<ROIShape> drawings = new ObservableCollection<ROIShape>();
        private ObservableCollection<ROIShape> mapDrawings = new ObservableCollection<ROIShape>();
        private ObservableCollection<ROIShape> homeMapDrawings = new ObservableCollection<ROIShape>();
        private Action<CogMatcher> sampleFind;
        private Action<Point> alignMarkMove;
        private LocateMode selectMode;
        private double alignOffsetX, alignOffsetY;

        private ITransform transForm; //紀錄 從設計座標轉換成對位後座標的 公式
        private int moveIndexX, moveIndexY, detectionIndexX, detectionIndexY;
        private bool isLocate;
        private int selectDetectionPointList;
        private bool isMainHomePageSelect, isMainRecipePageSelect, isMainSettingPageSelect, isMainToolsPageSelect, isMainSecurityPageSelect;
        private bool isLoadwaferPageSelect, isLocatePageSelect, isDetectionPageSelect;
        private bool isLoadwaferOK, isLocateOK, isDetectionOK; //判斷各設定頁面是否滿足條件 ，  才能切換到下一頁
        private System.Windows.Point mousePixcel;
        private ROIShape selectShape;
        private bool isLoadwaferComplete, isLocateComplete, isDetectionComplete;

        private double alignerMicroAngle, alignerWaferIDAngle;
        private double macroTopStartPitchX, macroTopStartRollY, macroTopStartYawT, macroBackStartPos;
        private int macroTopLeftLightValue, macroTopRightLightValue, macroBackLeftLightValue, macroBackRightLightValue;
        private Model.ArmStation lastArmStation = Model.ArmStation.Cassette1;
        private string topMoveContent = "TopMove", backMoveContent = "BackMove";
        private readonly object lockObjEFEMTrans = new object();
        private bool isCanWorkEFEMTrans = true;
        private bool isDie, isDieSub, isDieInSideAll, isPosition, isRecipeAlignment = false;
        /// <summary>
        /// 畫圖的參數
        /// </summary>
        private double showSize_X, showSize_Y, offsetDraw, strokeThickness = 1, crossThickness = 1;
        /// <summary>
        /// 切換到 主畫面 首頁頁面
        /// </summary>
        public bool IsMainHomePageSelect
        {
            get
            {
                if (isMainHomePageSelect == true)
                {
                    ShowHomeMapImgae(mainRecipe.DetectRecipe);
                }
                return isMainHomePageSelect;
            }
            set => SetValue(ref isMainHomePageSelect, value);
        }
        /// <summary>
        /// 切換到 主畫面Recipe 設定頁面
        /// </summary>
        public bool IsMainRecipePageSelect
        {
            get
            {
                if (!isInitialComplete) return isMainRecipePageSelect; //ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯

                if (isMainRecipePageSelect)
                    LoadRecipePage();
                else if (!isMainRecipePageSelect)
                    UnLoadRecipePage();
                return isMainRecipePageSelect;
            }
            set => SetValue(ref isMainRecipePageSelect, value);
        }
        public bool IsMainToolsPageSelect
        {
            get
            {
                if (!isInitialComplete) return isMainToolsPageSelect; //ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯

                if (isMainToolsPageSelect)
                    LoadToolsPage();
                else if (!isMainToolsPageSelect)
                    UnLoadToolsPage();
                return isMainToolsPageSelect;
            }
            set => SetValue(ref isMainToolsPageSelect, value);
        }
        /// <summary>
        /// 切換到 主畫面設定頁面
        /// </summary>
        public bool IsMainSettingPageSelect
        {
            get
            {
                if (!isInitialComplete) return isMainSettingPageSelect; //ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯

                if (isMainSettingPageSelect)
                    LoadSettingPage();
                else if (!isMainSettingPageSelect)
                    UnLoadSettingPage();
                return isMainSettingPageSelect;
            }
            set => SetValue(ref isMainSettingPageSelect, value);
        }

        /// <summary>
        /// 切換到 安全性操作頁面
        /// </summary>
        public bool IsMainSecurityPageSelect
        {
            get
            {
                if (!isInitialComplete) return isMainSecurityPageSelect; //ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
                if (isMainSecurityPageSelect)
                    LoadSecurityPage();
                else if (!isMainSecurityPageSelect)
                    UnLoadSecurityPage();
                return isMainSecurityPageSelect;
            }
            set => SetValue(ref isMainSecurityPageSelect, value);
        }

        /// <summary>
        /// 切換到 取料頁
        /// </summary>
        public bool IsLoadwaferPageSelect
        {
            get
            {

                return isLoadwaferPageSelect;
            }
            set => SetValue(ref isLoadwaferPageSelect, value);
        }
        /// <summary>
        ///  切換到 對位頁
        /// </summary>
        public bool IsLocatePageSelect
        {
            get
            {
                if (!isInitialComplete) return isLocatePageSelect;//ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
                if (isLocatePageSelect)
                    LoadLoactePage();
                return isLocatePageSelect;
            }
            set => SetValue(ref isLocatePageSelect, value);
        }
        /// <summary>
        /// 切換到 新增檢測座標頁
        /// </summary>
        public bool IsDetectionPageSelect
        {
            get
            {
                if (!isInitialComplete) return isDetectionPageSelect;//ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
                if (!IsLocateComplete) isDetectionPageSelect = false;

                if (isDetectionPageSelect)
                    SetLocateParamToRecipe();
                return isDetectionPageSelect;
            }
            set => SetValue(ref isDetectionPageSelect, value);
        }
        /// <summary>
        /// 是否可運作EFEM
        /// </summary>
        public bool IsCanWorkEFEMTrans { get => isCanWorkEFEMTrans; set => SetValue(ref isCanWorkEFEMTrans, value); }

        public double AlignerMicroAngle { get => alignerMicroAngle; set => SetValue(ref alignerMicroAngle, value); }
        public double AlignerWaferIDAngle { get => alignerWaferIDAngle; set => SetValue(ref alignerWaferIDAngle, value); }
        public double MacroTopStartPitchX { get => macroTopStartPitchX; set => SetValue(ref macroTopStartPitchX, value); }
        public double MacroTopStartRollY { get => macroTopStartRollY; set => SetValue(ref macroTopStartRollY, value); }
        public double MacroTopStartYawT { get => macroTopStartYawT; set => SetValue(ref macroTopStartYawT, value); }
        public string TopMoveContent { get => topMoveContent; set => SetValue(ref topMoveContent, value); }
        public string BackMoveContent { get => backMoveContent; set => SetValue(ref backMoveContent, value); }

        public int MacroTopLeftLightValue { get => macroTopLeftLightValue; set => SetValue(ref macroTopLeftLightValue, value); }
        public int MacroTopRightLightValue { get => macroTopRightLightValue; set => SetValue(ref macroTopRightLightValue, value); }

        public double MacroBackStartPos { get => macroBackStartPos; set => SetValue(ref macroBackStartPos, value); }

        public int MacroBackLeftLightValue { get => macroBackLeftLightValue; set => SetValue(ref macroBackLeftLightValue, value); }
        public int MacroBackRightLightValue { get => macroBackRightLightValue; set => SetValue(ref macroBackRightLightValue, value); }

        /// <summary>
        /// 
        /// </summary>
        public Model.ArmStation RecipeLastArmStation { get => lastArmStation; set => SetValue(ref lastArmStation, value); }

        /// <summary>
        /// Load wafer已完成 (locate頁面功能需要判斷)
        /// </summary>
        public bool IsLoadwaferComplete { get => isLoadwaferComplete; set => SetValue(ref isLoadwaferComplete, value); }
        /// <summary>
        /// locate已完成 (Detection頁面功能需要判斷)
        /// </summary>
        public bool IsLocateComplete { get => isLocateComplete; set => SetValue(ref isLocateComplete, value); }
        public bool IsDetectionComplete { get => isDetectionComplete; set => SetValue(ref isDetectionComplete, value); }


        public BitmapSource LocateSampleImage1 { get => locateSampleImage1; set => SetValue(ref locateSampleImage1, value); }
        public BitmapSource LocateSampleImage2 { get => locateSampleImage2; set => SetValue(ref locateSampleImage2, value); }
        public BitmapSource LocateSampleImage3 { get => locateSampleImage3; set => SetValue(ref locateSampleImage3, value); }
        public ObservableCollection<WaferUIData> LoadPort1Wafers { get => loadPort1Wafers; set => SetValue(ref loadPort1Wafers, value); }
        public int LoadPort1WaferSelect
        {
            get
            {
                if (!isInitialComplete) return loadPort1WaferSelect;

                if (RecipeLastArmStation == Model.ArmStation.Cassette1 || RecipeLastArmStation == Model.ArmStation.Cassette2)
                {

                }
                return loadPort1WaferSelect;
            }
            set
            {
                if (RecipeLastArmStation == Model.ArmStation.Cassette1 || RecipeLastArmStation == Model.ArmStation.Cassette2)
                {
                    SetValue(ref loadPort1WaferSelect, value);
                }
            }
        }
        public ObservableCollection<LocateParam> LocateParamList { get => locateParamList; set => SetValue(ref locateParamList, value); }
        //  public ObservableCollection<WaferUIData> LoadPort2Wafers { get => loadPort2Wafers; set => SetValue(ref loadPort2Wafers, value); }
        public ObservableCollection<DetectionPoint> DetectionPointList { get => detectionPointList; set => SetValue(ref detectionPointList, value); }
        public int SelectDetectionPointList { get => selectDetectionPointList; set => SetValue(ref selectDetectionPointList, value); }
        public LocateParam LocateParam1 { get => locateParam1; set => SetValue(ref locateParam1, value); }
        public LocateParam LocateParam2 { get => locateParam2; set => SetValue(ref locateParam2, value); }
        public LocateParam LocateParam3 { get => locateParam3; set => SetValue(ref locateParam3, value); }
        public LocateMode SelectMode { get => selectMode; set => SetValue(ref selectMode, value); }
        public double AlignOffsetX { get => alignOffsetX; set => SetValue(ref alignOffsetX, value); }
        public double AlignOffsetY { get => alignOffsetY; set => SetValue(ref alignOffsetY, value); }
        //判斷有沒有做過 對位，主要卡控所有的座標都要建立在對位後的 才會是對的
        public bool IsLocate { get => isLocate; set => SetValue(ref isLocate, value); }
        public Action<CogMatcher> SampleFind { get => sampleFind; set => SetValue(ref sampleFind, value); }
        public Action<Point> AlignMarkMove { get => alignMarkMove; set => SetValue(ref alignMarkMove, value); }



        public int MoveIndexX { get => moveIndexX; set => SetValue(ref moveIndexX, value); }
        public int MoveIndexY { get => moveIndexY; set => SetValue(ref moveIndexY, value); }
        public int DetectionIndexX { get => detectionIndexX; set => SetValue(ref detectionIndexX, value); }
        public int DetectionIndexY { get => detectionIndexY; set => SetValue(ref detectionIndexY, value); }

        /// <summary>
        /// 滑鼠在影像內 Pixcel 座標
        /// </summary>
        public System.Windows.Point MousePixcel { get => mousePixcel; set => SetValue(ref mousePixcel, value); }


        /// <summary>
        /// 取得或設定 shape 
        /// </summary>
        public ObservableCollection<ROIShape> Drawings { get => drawings; set => SetValue(ref drawings, value); }
        public ObservableCollection<ROIShape> HomeMapDrawings { get => homeMapDrawings; set => SetValue(ref homeMapDrawings, value); }
        public ObservableCollection<ROIShape> MapDrawings { get => mapDrawings; set => SetValue(ref mapDrawings, value); }

        public bool IsDie { get => isDie; set => SetValue(ref isDie, value); }
        public bool IsDieSub { get => isDieSub; set => SetValue(ref isDieSub, value); }
        public bool IsDieInSideAll { get => isDieInSideAll; set => SetValue(ref isDieInSideAll, value); }
        public bool IsPosition { get => isPosition; set => SetValue(ref isPosition, value); }

        //Recipe進入會執行
        private void LoadRecipePage()
        {
            if (RightsModel.Operator == Account.CurrentAccount.Right || RightsModel.Visitor == Account.CurrentAccount.Right || Machinestatus == YuanliCore.Machine.Base.MachineStates.RUNNING)
            {
                IsMainRecipePageSelect = false;
                IsMainHomePageSelect = true;
                return;
            }

            LoadPort1Wafers.Clear();
            foreach (var item in ProcessStations)
            {
                var w = new WaferUIData();
                w.SN = item.CassetteIndex.ToString();
                //只要不是空的 就是有片
                if (item.MacroBack != WaferProcessStatus.None || item.MacroTop != WaferProcessStatus.None || item.Micro != WaferProcessStatus.None)
                    w.WaferStates = WaferProcessStatus.NotSelect;
                LoadPort1Wafers.Add(w);
            }

            if (RecipeLastArmStation == Model.ArmStation.Cassette1 || RecipeLastArmStation == Model.ArmStation.Cassette2)
            {
                if (IsLoadport1)
                {
                    RecipeLastArmStation = Model.ArmStation.Cassette1;
                }
                else if (IsLoadport2)
                {
                    RecipeLastArmStation = Model.ArmStation.Cassette2;
                }
            }

            //始終會切回到第一頁 LoadWafer 頁
            IsLoadwaferPageSelect = true;
            WriteLog("Enter the RecipePage");
        }
        //離開recipe頁面會執行
        private void UnLoadRecipePage()
        {
            var index1 = new Point(LocateParam1.IndexX, LocateParam1.IndexY);
            var pos1 = new Point(LocateParam1.GrabPositionX, LocateParam1.GrabPositionY);
            //數值都是0 是UI初始化 ， 所以不做動作 
            if (index1.X == 0 && index1.Y == 0 && pos1.X == 0 && pos1.Y == 0) return;
            SetLocateParamToRecipe();
            SetLoadWaferToRecipe();
            SetDetectionToRecipe();
        }
        private void LoadToolsPage()
        {
            if (RightsModel.Operator == Account.CurrentAccount.Right || RightsModel.Visitor == Account.CurrentAccount.Right)
            {
                IsMainToolsPageSelect = false;
                IsMainHomePageSelect = true;
                return;
            }

            IsLoadwaferPageSelect = true;
            WriteLog("Enter the ToolsPage");
        }
        private void UnLoadToolsPage()
        {

        }
        //Recipe進入會執行
        private void LoadSettingPage()
        {



            if (RightsModel.Operator == Account.CurrentAccount.Right || RightsModel.Visitor == Account.CurrentAccount.Right)
            {
                IsMainSettingPageSelect = false;
                IsMainHomePageSelect = true;
                return;
            }

            TableWaferCatchPositionX = machineSetting.TableWaferCatchPosition.X;
            TableWaferCatchPositionY = machineSetting.TableWaferCatchPosition.Y;
            TableWaferCatchPositionZ = machineSetting.TableWaferCatchPositionZ;
            TableWaferCatchPositionR = machineSetting.TableWaferCatchPositionR;
            RobotAxisLoadPort1TakePosition = machineSetting.RobotAxisLoadPort1TakePosition;
            RobotAxisLoadPort2TakePosition = machineSetting.RobotAxisLoadPort2TakePosition;
            RobotAxisAligner1TakePosition = machineSetting.RobotAxisAlignTakePosition;
            RobotAxisMacroTakePosition = machineSetting.RobotAxisMacroTakePosition;
            RobotAxisMicroTakePosition = machineSetting.RobotAxisMicroTakePosition;
            if (machineSetting.MicroscopeLensDefault != null)
            {
                MicroscopeLensDefault = machineSetting.MicroscopeLensDefault.ToArray();
            }
            WriteLog("Enter the SettingPage");
        }
        //離開recipe頁面會執行
        private void UnLoadSettingPage()
        {

        }



        //進入locate頁會執行
        private void LoadLoactePage()
        {

            //始終會切回到第一頁 LoadWafer 頁      
            //   IsLoadwaferPageSelect = true;
            //   IsLocatePageSelect = false;

        }






        public ICommand MappingEditCommand => new RelayCommand(async () =>
       {
           try
           {
               //var dies = mainRecipe.DetectRecipe.WaferMap.Dies.ToList(); // 將Dies轉換為列表以便索引
               //var foundDie = dies.FirstOrDefault(d => d.IndexX == MoveIndexX && d.IndexY == MoveIndexY);
               //int indexOfFoundDie = 0;
               //if (foundDie != null)
               //{
               //    indexOfFoundDie = dies.IndexOf(foundDie);
               //}
               Die die = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == MoveIndexX && d.IndexY == MoveIndexY).FirstOrDefault();

               MapDieColorChange(mainRecipe.DetectRecipe.WaferMap, die, Brushes.Purple);
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
           }

       });

        public ICommand MappingTestCommand => new RelayCommand(async () =>
        {
            try
            {

                Stopwatch stopwatch = new Stopwatch();
                MappingCanvasWindow win = new MappingCanvasWindow(300, 300);
                stopwatch.Start();
                if (mainRecipe.DetectRecipe.MapImage != null)
                    win.MappingTable = new WriteableBitmap(mainRecipe.DetectRecipe.MapImage.ToBitmapSource());
                var t2 = stopwatch.ElapsedMilliseconds;
                win.ShowDialog();

                stopwatch.Restart();
                //  win.MappingTable.Save("D:\\1234");             
                //   BitmapImage bitmap = new BitmapImage(new Uri("D:\\1234.bmp", UriKind.RelativeOrAbsolute));
                //   var mapImage = new WriteableBitmap(bitmap);

                mainRecipe.DetectRecipe.MapImage = win.MappingTable.ToByteFrame();
                var t1 = stopwatch.ElapsedMilliseconds;


                try
                {
                    machine.MicroDetection.Microscope.HomeAsync();

                    double aberationNow = 0;
                    machine.MicroDetection.Microscope.AFOff();

                    machine.MicroDetection.Microscope.ChangeLightAsync(44);

                    machine.MicroDetection.Microscope.ChangeApertureAsync(700);

                    double zNow = machine.MicroDetection.Microscope.AFPEL;

                    aberationNow = machine.MicroDetection.Microscope.AFNEL;

                    machine.MicroDetection.Microscope.SetSearchRange(584120, 100000);

                    machine.MicroDetection.AxisZ.MoveAsync(25000);
                }
                catch (Exception)
                {

                    throw;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        });

        public ICommand TableTestCommand => new RelayCommand(async () =>
        {
            try
            {
                SetLocateParamToRecipe();
                await machine.MicroDetection.AxisZ.MoveToAsync(250000);
                machine.MicroDetection.Camera.Stop();//var dis = machine.MicroDetection.Camera.Grab();
                var dis = machine.MicroDetection.Camera.Grab();
                machine.MicroDetection.AxisZ.MoveToAsync(machineSetting.TableWaferCatchPositionZ);//CenterX


                machine.MicroDetection.AxisX.MoveToAsync(255295);//255295 446000
                machine.MicroDetection.AxisY.MoveToAsync(199208);//199208 -22081

                //Robot.SetSpeedPercentCommand(Convert.ToInt32(RobotStaus.SpeedPercent));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        private PauseTokenSource ptsTest = new PauseTokenSource();
        private CancellationTokenSource ctsTest = new CancellationTokenSource();
        public ICommand DetectionTestCommand => new RelayCommand(async () =>
        {
            try
            {
                MainRecipe recipe = mainRecipe;
                ProcessSetting processSetting = new ProcessSetting();
                processSetting.IsAutoFocus = true;
                processSetting.IsAutoSave = true;
                Wafer currentWafer = new Wafer(1);
                await machine.MicroDetection.Run(recipe, processSetting, currentWafer, ptsTest, ctsTest);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand DetectionPauseTestCommand => new RelayCommand(async () =>
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
        public ICommand LoadMappingCommand => new RelayCommand(async () =>
        {
            string SINF_Path = "";
            System.Windows.Forms.OpenFileDialog dlg_image = new System.Windows.Forms.OpenFileDialog();
            dlg_image.Filter = "TXT files (*.txt)|*.txt|SINF files (*.sinf)|*.sinf";
            dlg_image.InitialDirectory = SINF_Path;
            if (dlg_image.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SINF_Path = dlg_image.FileName;
                if (SINF_Path != "")
                {
                    var m_Sinf = new SinfWaferMapping(true, false);
                    (m_Sinf.Dies, m_Sinf.WaferSize) = m_Sinf.ReadWaferFile(SINF_Path, true, false);

                    mainRecipe.DetectRecipe.WaferMap = new SinfWaferMapping(true, false);
                    mainRecipe.DetectRecipe.WaferMap = m_Sinf;

                    ClearHomeMapShapeAction.Execute(true);
                    await ShowMappingDrawings(mainRecipe.DetectRecipe.WaferMap.Dies, mainRecipe.DetectRecipe.BincodeList, mainRecipe.DetectRecipe.WaferMap.ColumnCount, mainRecipe.DetectRecipe.WaferMap.RowCount, 3000);
                    await SaveHomeMapImgae(mainRecipe.DetectRecipe.WaferMap);
                    ShowDetectionHomeMapImgae(mainRecipe.DetectRecipe);
                    ShowDetectionMapImgae(mainRecipe.DetectRecipe);
                }
            }
            else
            {
                SINF_Path = "";
                ClearMapShapeAction.Execute(true);
            }
        });

        public ICommand EditMappingCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                SINFMapGenerateWindow sINFMapGenerateWindow = new SINFMapGenerateWindow();
                //sINFMapGenerateWindow.WaferMap = mainRecipe.DetectRecipe.WaferMap;
                sINFMapGenerateWindow.ShowDialog();
                //if (sINFMapGenerateWindow.Sinf != null && sINFMapGenerateWindow.Sinf.Dies != null && sINFMapGenerateWindow.Sinf.Dies.Length > 0)
                //{
                //    if (mainRecipe.DetectRecipe.WaferMap == null)
                //    {
                //        mainRecipe.DetectRecipe.WaferMap = new SinfWaferMapping("", true, false);
                //    }
                //    mainRecipe.DetectRecipe.WaferMap.Dies = sINFMapGenerateWindow.Sinf.Dies;
                //    mainRecipe.DetectRecipe.WaferMap.ColumnCount = sINFMapGenerateWindow.Sinf.ColumnCount;
                //    mainRecipe.DetectRecipe.WaferMap.RowCount = sINFMapGenerateWindow.Sinf.RowCount;
                //    MapImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                //}
            }
            catch (Exception)
            {

                throw;
            }
        });
        /// <summary>
        /// 顯示主畫面Map圖片
        /// </summary>
        /// <param name="detectionRecipe"></param>
        public void ShowHomeMapImgae(DetectionRecipe detectionRecipe)
        {
            try
            {
                if (detectionRecipe.WaferMap != null && detectionRecipe.WaferMap.MapImage != null)
                {
                    HomeMapImage = new WriteableBitmap(mainRecipe.DetectRecipe.WaferMap.MapImage.ToBitmapSource());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void ShowDetectionMapImgae(DetectionRecipe detectionRecipe)
        {
            try
            {

                if (detectionRecipe.DetectionPoints == null) return;
                foreach (var item in detectionRecipe.DetectionPoints)
                {
                    Die die = detectionRecipe.WaferMap.Dies.Where(d => d.IndexX == item.IndexX && d.IndexY == item.IndexY).FirstOrDefault();
                    if (die != null)
                    {
                        MapDieColorChange(detectionRecipe.WaferMap, die, Brushes.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void ClearDetectionMapImgae(DetectionRecipe detectionRecipe)
        {
            try
            {
                foreach (var item in detectionRecipe.DetectionPoints)
                {
                    Die die = detectionRecipe.WaferMap.Dies.Where(d => d.IndexX == item.IndexX && d.IndexY == item.IndexY).FirstOrDefault();
                    if (die != null)
                    {
                        MapDieColorReturn(detectionRecipe.WaferMap, die, detectionRecipe.BincodeList);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public void MapDieColorChange(WaferMapping waferMapping, Die die, Brush brush)
        {
            try
            {
                if (showSize_X <= 0 || showSize_Y <= 0)
                {
                    return;
                }
                var clearPoint = new Point(die.OperationPixalX / showSize_X + offsetDraw, die.OperationPixalY / showSize_Y + offsetDraw);
                ROIShape tempselectShape = MapDrawings.Select(shape =>
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
                    //RemoveMapShapeAction.Execute(tempselectShape);
                    tempselectShape.Fill = brush;
                    tempselectShape.CenterCrossBrush = brush;
                }
                else
                {
                    AddMapShapeAction.Execute(new ROIRotatedRect
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
        public void MapDieColorReturn(WaferMapping waferMapping, Die die, IEnumerable<BincodeInfo> bincodeListDefault)
        {
            try
            {
                var clearPoint = new Point(die.OperationPixalX / showSize_X + offsetDraw, die.OperationPixalY / showSize_Y + offsetDraw);

                ROIShape tempselectShape = MapDrawings.Select(shape =>
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
                    //RemoveMapShapeAction.Execute(tempselectShape);
                    tempselectShape.Fill = drawFill;
                    tempselectShape.CenterCrossBrush = drawFill;
                }
                else
                {
                    AddMapShapeAction.Execute(new ROIRotatedRect
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





        public async Task SaveHomeMapImgae(WaferMapping waferMap)
        {
            try
            {
                SaveMappingAction.Execute($"{systemPath}\\Map.bmp");
                var bitmapImage = new BitmapImage();
                string path = $"{systemPath}\\Map.bmp"; // "C://Users//zhengye_lin//Desktop//333output.bmp"
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(path);//333output.bmp
                bitmapImage.EndInit();
                waferMap.MapImage = bitmapImage.FormatConvertTo(PixelFormats.Bgr24).ToByteFrame();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task ShowMappingDrawings(Die[] dies, IEnumerable<BincodeInfo> bincodeListDefault, int columnCount, int rowCount, int mappingImageDrawSize)
        {
            try
            {
                MapImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                ClearMapShapeAction.Execute(true);
                double dieSizeX = dies[0].DieSize.Width;
                double dieSizeY = dies[0].DieSize.Height;
                offsetDraw = mappingImageDrawSize / 150;
                double scale = 1;
                scale = Math.Max((columnCount + 2.5) * dieSizeX, (rowCount + 2.5) * dieSizeY) / (mappingImageDrawSize - offsetDraw * 2);
                scale = Math.Max(columnCount * dieSizeX, rowCount * dieSizeY) / (mappingImageDrawSize - offsetDraw * 2);
                strokeThickness = Math.Min(dieSizeX / 2 / scale, dieSizeX / 2 / scale) / 4;
                crossThickness = Math.Min(dieSizeX / 2 / scale, dieSizeX / 2 / scale) / 4;
                showSize_X = (columnCount * dieSizeX) / (mappingImageDrawSize - offsetDraw * 2);
                showSize_Y = (rowCount * dieSizeY) / (mappingImageDrawSize - offsetDraw * 2);


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
                int count = 0;
                foreach (var item in dies)
                {
                    count++;
                    Brush drawStroke = Brushes.Black;
                    Brush drawFill = Brushes.Gray;
                    //判斷要用什麼顏色
                    foreach (var item2 in bincodeListDefault)
                    {
                        if (item.BinCode == item2.Code)
                        {
                            drawFill = item2.Color;
                        }
                    }
                    if (count >= 50)
                    {
                        await Task.Delay(10);
                        count = 0;
                    }
                    AddMapShapeAction.Execute(new ROIRotatedRect
                    {
                        Stroke = drawStroke,
                        StrokeThickness = strokeThickness,
                        Fill = drawFill,
                        X = item.OperationPixalX / showSize_X + offsetDraw,
                        Y = item.OperationPixalY / showSize_Y + offsetDraw,
                        LengthX = item.DieSize.Width / 2.5 / showSize_X,
                        LengthY = item.DieSize.Height / 2.5 / showSize_Y,
                        IsInteractived = true,
                        IsMoveEnabled = false,
                        IsResizeEnabled = false,
                        IsRotateEnabled = false,
                        CenterCrossLength = crossThickness,
                        CenterCrossBrush = drawFill,
                        ToolTip = "X:" + (item.IndexX) + " Y:" + (item.IndexY) + " X:" + item.MapTransX + " Y:" + item.MapTransY
                    });
                }
                await Task.Delay(500);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ICommand EditSampleCommand => new RelayCommand(() =>
        {
            try
            {

                matcher.RunParams = matchParam;
                matcher.EditParameter(MainImage);

                matchParam = (PatmaxParams)matcher.RunParams;
                if (matchParam.PatternImage != null)
                    LocateSampleImage1 = matchParam.PatternImage.ToBitmapSource();

                //  UpdateRecipe();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand AlignerCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                IsCanWorkEFEMTrans = false;
                switch (key)
                {
                    case "NotchAngle":
                        await machine.Feeder.AlignerL.FixWafer();
                        if (machine.Feeder.AlignerL.IsLockOK == false)
                        {
                            await Task.Delay(1000);
                            if (machine.Feeder.AlignerL.IsLockOK == false)
                            {
                                throw new Exception("AlignerFixWafer Error!!");
                            }
                        }
                        await machine.Feeder.AlignerL.Run(AlignerMicroAngle);
                        IsCanWorkEFEMTrans = true;
                        break;
                    case "WaferIDAngle":
                        await machine.Feeder.AlignerL.FixWafer();
                        if (machine.Feeder.AlignerL.IsLockOK == false)
                        {
                            await Task.Delay(1000);
                            if (machine.Feeder.AlignerL.IsLockOK == false)
                            {
                                throw new Exception("AlignerFixWafer Error!!");
                            }
                        }
                        await machine.Feeder.AlignerL.Run(AlignerWaferIDAngle);
                        IsCanWorkEFEMTrans = true;
                        break;
                    case "WaferIDTest":
                        IsCanWorkEFEMTrans = true;
                        break;
                    default:
                        IsCanWorkEFEMTrans = true;
                        break;
                }
            }
            catch (Exception ex)
            {
                IsCanWorkEFEMTrans = true;
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand EFEMTransToLoadPortCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                        {
                            IsCanWorkEFEMTrans = false;
                            //是否執行移動片子訊息
                            string mesage = "";
                            if (IsLoadport1)
                            {
                                mesage = EFEMTransWaferMessage(RecipeLastArmStation, Model.ArmStation.Cassette1);
                            }
                            else if (IsLoadport2)
                            {
                                mesage = EFEMTransWaferMessage(RecipeLastArmStation, Model.ArmStation.Cassette2);
                            }
                            else
                            {
                                throw new Exception("EFEMTransCommand Error!");
                            }
                            var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {

                                //片子上一個狀態先記錄起來
                                Model.ArmStation oldArmStation = RecipeLastArmStation;
                                //確認手臂有無片
                                if (EFEMTransWaferBeforeCheckRobotHaveWafer() == false)
                                {
                                    throw new FlowException("EFEMTransCommand Error!Robot Have Wafer!");
                                }
                                //將Wafer取出，退到安全位置
                                EFEMTransWaferPick(oldArmStation);
                                Wafer currentWafer;
                                //將片子放下去
                                if (IsLoadport1)
                                {
                                    RecipeLastArmStation = Model.ArmStation.Cassette1;
                                    currentWafer = new Wafer(ProcessStations[LoadPort1WaferSelect].CassetteIndex);
                                    machine.Feeder.UnLoadWaferToCassette(currentWafer, true).Wait();
                                }
                                else if (IsLoadport2)
                                {
                                    RecipeLastArmStation = Model.ArmStation.Cassette2;
                                    currentWafer = new Wafer(ProcessStations[LoadPort1WaferSelect].CassetteIndex);
                                    machine.Feeder.UnLoadWaferToCassette(currentWafer, false).Wait();
                                }
                                else
                                {
                                    throw new Exception("EFEMTransCommand Error!");
                                }
                            }
                            IsCanWorkEFEMTrans = true;
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                });
            }
            catch (FlowException ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        public ICommand EFEMTransToAlignerCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {

                        if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                        {
                            IsCanWorkEFEMTrans = false;
                            //是否執行移動片子訊息
                            string mesage = EFEMTransWaferMessage(RecipeLastArmStation, Model.ArmStation.Align);
                            var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                //片子上一個狀態先記錄起來
                                Model.ArmStation oldArmStation = RecipeLastArmStation;
                                //確認手臂有無片
                                if (EFEMTransWaferBeforeCheckRobotHaveWafer() == false)
                                {
                                    throw new FlowException("EFEMTransCommand Error!Robot Have Wafer!");
                                }
                                //將Wafer取出，退到安全位置
                                EFEMTransWaferPick(oldArmStation);
                                //將片子放下去
                                RecipeLastArmStation = Model.ArmStation.Align;
                                //如果是從Micro送過來就先不要Home，因為要怎麼進怎麼出
                                if (oldArmStation != Model.ArmStation.Micro)
                                {
                                    machine.Feeder.AlignerL.Home().Wait();
                                }
                                machine.Feeder.WaferStandByToAligner().Wait();
                                machine.Feeder.AlignerL.FixWafer().Wait();
                                if (oldArmStation == Model.ArmStation.Micro)
                                {
                                    machine.Feeder.AlignerL.Home().Wait();
                                }
                                //如果是從Micro送過來就先不要Home，因為要怎麼進怎麼出
                                if (machine.Feeder.AlignerL.IsLockOK == false)
                                {
                                    throw new FlowException("EFEMTransCommand 異常!WaferToAligner Aligner真空異常!!");
                                }
                            }
                            IsCanWorkEFEMTrans = true;
                        }
                    }
                    catch (FlowException ex)
                    {

                        throw ex;
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                });
            }
            catch (FlowException ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        public ICommand EFEMTransToMacroCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                        {
                            IsCanWorkEFEMTrans = false;
                            //是否執行移動片子訊息
                            string mesage = EFEMTransWaferMessage(RecipeLastArmStation, Model.ArmStation.Macro);
                            var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                //片子上一個狀態先記錄起來
                                Model.ArmStation oldArmStation = RecipeLastArmStation;
                                //確認手臂有無片
                                if (EFEMTransWaferBeforeCheckRobotHaveWafer() == false)
                                {
                                    throw new FlowException("EFEMTransCommand Error!Robot Have Wafer!");
                                }
                                //將Wafer取出，退到安全位置
                                EFEMTransWaferPick(oldArmStation);
                                //將片子放下去
                                RecipeLastArmStation = Model.ArmStation.Macro;
                                machine.Feeder.Macro.FixWafer();
                                machine.Feeder.WaferStandByToMacroAsync().Wait();
                                if (machine.Feeder.Macro.IsLockOK == false)
                                {
                                    throw new Exception("EFEMTransCommand 異常!WaferToMacro Macro真空異常!!");
                                }
                            }
                            IsCanWorkEFEMTrans = true;
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        /// <summary>
        /// 是否執行移動片子訊息
        /// </summary>
        /// <param name="lastArmStation"></param>
        /// <param name="newArmStation"></param>
        /// <returns></returns>
        private string EFEMTransWaferMessage(Model.ArmStation lastArmStation, Model.ArmStation newArmStation)
        {
            try
            {
                string mesage = "";
                string newPlace = "";
                //machine.Feeder.Robot.SetSpeedPercentCommand(50);
                switch (newArmStation)
                {
                    case Model.ArmStation.Cassette1:
                        newPlace = "Cassette1";
                        break;
                    case Model.ArmStation.Cassette2:
                        newPlace = "Cassette2";
                        break;
                    case Model.ArmStation.Align:
                        newPlace = "Aligner";
                        break;
                    case Model.ArmStation.Macro:
                        newPlace = "Macro";
                        break;
                    case Model.ArmStation.Micro:
                        newPlace = "Micro";
                        break;
                    default:
                        throw new Exception("EFEMTransCommand Error!");
                }
                if (lastArmStation == newArmStation)
                {
                    throw new Exception("Wafer Is In " + newPlace + "!!");
                }
                switch (lastArmStation)
                {
                    case Model.ArmStation.Align:
                        mesage = "Pick Wafer Aligner To " + newPlace + "?";
                        break;
                    case Model.ArmStation.Macro:
                        mesage = "Pick Wafer Macro To " + newPlace + "?";
                        break;
                    case Model.ArmStation.Micro:
                        mesage = "Pick Wafer Micro To " + newPlace + "?";
                        break;
                    case Model.ArmStation.Cassette1:
                        mesage = "Pick Wafer Cassette1 To " + newPlace + "?";
                        break;
                    case Model.ArmStation.Cassette2:
                        mesage = "Pick Wafer Cassette2 To " + newPlace + "?";
                        break;
                    default:
                        throw new Exception("EFEMTransCommand Error!");
                }
                return mesage;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 確認手臂有無片
        /// </summary>
        private bool EFEMTransWaferBeforeCheckRobotHaveWafer()
        {
            try
            {
                machine.Feeder.Robot.FixWafer().Wait();
                Task.Delay(1000).Wait();
                if (machine.Feeder.Robot.IsLockOK)
                {
                    return false;
                }
                else
                {
                    machine.Feeder.Robot.ReleaseWafer().Wait();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 將Wafer取出，退到安全位置
        /// </summary>
        /// <param name="oldArmStation"></param>
        private void EFEMTransWaferPick(Model.ArmStation oldArmStation)
        {
            try
            {
                switch (oldArmStation)
                {
                    case Model.ArmStation.Cassette1:
                        Wafer currentWafer = new Wafer(ProcessStations[LoadPort1WaferSelect].CassetteIndex);
                        if (machineSetting.LoadPortCount == Model.LoadPortQuantity.Single)
                        {
                            machine.Feeder.WaferLoadPortToStandBy(currentWafer.CassetteIndex, Model.ArmStation.Cassette1).Wait();
                        }
                        else
                        {
                            RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort1TakePosition).Wait();
                            machine.Feeder.WaferLoadPortToStandBy(currentWafer.CassetteIndex, Model.ArmStation.Cassette1).Wait();
                        }
                        break;
                    case Model.ArmStation.Cassette2:
                        currentWafer = new Wafer(ProcessStations[LoadPort1WaferSelect].CassetteIndex);
                        if (machineSetting.LoadPortCount == Model.LoadPortQuantity.Single)
                        {
                            machine.Feeder.WaferLoadPortToStandBy(currentWafer.CassetteIndex, Model.ArmStation.Cassette2).Wait();
                        }
                        else
                        {
                            RobotAxis.MoveToAsync(machineSetting.RobotAxisLoadPort2TakePosition).Wait();
                            machine.Feeder.WaferLoadPortToStandBy(currentWafer.CassetteIndex, Model.ArmStation.Cassette2).Wait();
                        }

                        break;
                    case Model.ArmStation.Align:
                        //machine.Feeder.AlignerL.Home().Wait();
                        machine.Feeder.AlignerL.ReleaseWafer().Wait();
                        machine.Feeder.WaferAlignerToStandBy().Wait();
                        break;
                    case Model.ArmStation.Macro:
                        machine.Feeder.WaferMacroToStandBy().Wait();
                        break;
                    case Model.ArmStation.Micro:
                        machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition).Wait();
                        machine.MicroDetection.TableVacuum.Off();
                        machine.Feeder.WaferMicroToStandBy().Wait();
                        break;
                    default:
                        throw new FlowException("EFEMTransCommand 異常!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public ICommand EFEMTransToMicroCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                        {
                            IsCanWorkEFEMTrans = false;
                            //是否執行移動片子訊息
                            string mesage = EFEMTransWaferMessage(RecipeLastArmStation, Model.ArmStation.Micro);
                            var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                            if (result == MessageBoxResult.Yes)
                            {
                                //片子上一個狀態先記錄起來
                                Model.ArmStation oldArmStation = RecipeLastArmStation;
                                //確認手臂有無片
                                if (EFEMTransWaferBeforeCheckRobotHaveWafer() == false)
                                {
                                    throw new FlowException("EFEMTransCommand Error!Robot Have Wafer!");
                                }
                                //將Wafer取出，退到安全位置
                                EFEMTransWaferPick(oldArmStation);
                                //將片子放下去
                                RecipeLastArmStation = Model.ArmStation.Micro;
                                Task micro = machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition);
                                if (machineSetting.LoadPortCount == Model.LoadPortQuantity.Single)
                                {
                                    await Task.WhenAll(micro);
                                }
                                else
                                {
                                    Task robot = machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
                                    await Task.WhenAll(micro, robot);
                                }
                                machine.MicroDetection.TableVacuum.On();
                                await machine.Feeder.LoadToMicroAsync();
                                if (machine.MicroDetection.IsTableVacuum.IsSignal == false)
                                {
                                    throw new Exception("EFEMTransCommand 異常!WaferToMicro Micro真空異常!!");
                                }
                            }
                            IsCanWorkEFEMTrans = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        public ICommand MacroCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                if (RecipeLastArmStation == Model.ArmStation.Macro)
                {
                    IsCanWorkEFEMTrans = false;
                    switch (key)
                    {
                        case "TopMove":
                            if (TopMoveContent == "TopMove")
                            {
                                if (machine.Feeder.Macro.IsInnerUsing)
                                {
                                    EFEMtionRecipe eFEMtionRecipe = new EFEMtionRecipe();
                                    eFEMtionRecipe.MacroTopStartPitchX = MacroTopStartPitchX;// - machine.Feeder.Macro.InnerPitchXPosition;
                                    eFEMtionRecipe.MacroTopStartRollY = MacroTopStartRollY;// - machine.Feeder.Macro.InnerRollYPosition;
                                    eFEMtionRecipe.MacroTopStartYawT = MacroTopStartYawT;// - machine.Feeder.Macro.InnerYawTPosition;
                                    await machine.Feeder.TurnWafer(eFEMtionRecipe);
                                    //TopMoveContent = "Home";
                                }
                                else
                                {
                                    MessageBox.Show("Wafer Not In Top!!");
                                }
                            }
                            else
                            {
                                if (machine.Feeder.Macro.IsInnerUsing)
                                {
                                    await machine.Feeder.Macro.InnerRingPitchXMoveToAsync(0);
                                    await machine.Feeder.Macro.InnerRingRollYMoveToAsync(0);
                                    await machine.Feeder.Macro.InnerRingYawTMoveToAsync(0);
                                    TopMoveContent = "TopMove";
                                }
                                else
                                {
                                    MessageBox.Show("Wafer Not In Top!!");
                                }
                            }

                            break;
                        case "BackMove":
                            if (TopMoveContent == "TopMove")
                            {
                                if (machine.Feeder.Macro.IsOuterUsing)
                                {
                                    double moveOffset = MacroBackStartPos;// - machine.Feeder.Macro.OuterRollYPosition;
                                    await machine.Feeder.TurnBackWafer(moveOffset);
                                    //TopMoveContent = "Home";
                                }
                                else
                                {
                                    MessageBox.Show("Wafer Not In Back!!");
                                }
                            }
                            else
                            {
                                if (machine.Feeder.Macro.IsOuterUsing)
                                {
                                    double moveOffset = MacroBackStartPos;// - machine.Feeder.Macro.OuterRollYPosition;
                                    await machine.Feeder.TurnBackWafer(0);
                                    TopMoveContent = "TopMove";
                                }
                                else
                                {
                                    MessageBox.Show("Wafer Not In Back!!");
                                }
                            }

                            break;
                        default:
                            break;
                    }
                    IsCanWorkEFEMTrans = true;
                }
                else
                {
                    MessageBox.Show("Wafer Not In Macro!!");
                }
            }
            catch (Exception)
            {

                throw;
            }
        });


        public ICommand LocateSampleCommand => new RelayCommand<string>(async key =>
        {



        });
        public ICommand ParamConfirmCommand => new RelayCommand(() =>
        {
            SetLocateParamToRecipe();
        });

        public ICommand LocateRunCommand => new RelayCommand(async () =>
        {
            try
            {
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    IsLocateComplete = false;
                    //經過這個方法後，會將map中所有的Die  Index座標轉換成當下樣本建立時的實際機台座標(pattern中心)
                    SetLocateParamToRecipe();
                    //將die的map座標都轉換成 實際機台座標(解決片子更換後位置不對的問題 )
                    transForm = await machine.MicroDetection.Alignment(mainRecipe.DetectRecipe.AlignRecipe);

                    List<Point> designPos = new List<Point>();
                    designPos.Add(new Point(238050, 39330));
                    designPos.Add(new Point(51750, 39330));
                    designPos.Add(new Point(51750, 246330));
                    List<Point> targetPos = new List<Point>();
                    targetPos.Add(new Point(388259.446941595, 4377.94456233873));
                    targetPos.Add(new Point(201781.399127985, 4520.27402669518));
                    targetPos.Add(new Point(201632.191064522, 211707.69584316));
                    //transForm = new CogAffineTransform(designPos, targetPos);

                    isRecipeAlignment = true;
                    //將所有的Die 轉換成實際片子座標(如果建立樣本時的wafer 與 LocateRun 是一起建立的  那座標會一樣 ，主要Locate目的是針對換wafer以後要重新對位
                    //如果有需要調整檢測座標 ，需要重新做對位  ，對位後會重新建立新的map全部die座標 ，為了給後續檢測座標設定使用 

                    //依序轉換完對位前座標  ，轉換成對位後座標 塞回機械座標


                    foreach (Die die in mainRecipe.DetectRecipe.WaferMap.Dies)
                    {
                        Point pos = transForm.TransPoint(new Point(die.PosX, die.PosY));
                        die.PosX = pos.X;
                        die.PosY = pos.Y;
                    }
                    IsLocateComplete = true;
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }



        });
        public ICommand GrabCommand => new RelayCommand(async () =>
        {
            try
            {
                BitmapSource bmp = machine.MicroDetection.Camera.GrabAsync();
                DetectionPoint point = new DetectionPoint();
                Wafer currentWafer = new Wafer(1);
                string nowTime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') +
                                DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0');
                string titleIdx = "1";
                DetectionRecord(bmp, point, currentWafer, nowTime, titleIdx);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand TableZContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "Z-":
                        await TableZ.MoveToAsync(TableZ.PositionNEL);
                        break;
                    case "Z+":
                        await TableZ.MoveToAsync(TableZ.PositionPEL);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand TableZMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                TableZ.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand LocatedMoveDieCommand => new RelayCommand(async () =>
        {
            try
            {
                if (isRecipeAlignment == false) return;//還沒對位，MAP點位不能移動
                //挑選出 對應index 的Die
                YuanliCore.Data.Die[] dies = mainRecipe.DetectRecipe.WaferMap.Dies;
                YuanliCore.Data.Die die = dies.Where(d => d.IndexX == MoveIndexX && d.IndexY == MoveIndexY).FirstOrDefault();
                if (die == null) throw new Exception("No This Die");
                //設計座標轉換對位後座標
                Point transPos = transForm.TransPoint(new Point(die.MapTransX, die.MapTransY));

                await machine.MicroDetection.TableMoveToAsync(new Point(transPos.X, transPos.Y));
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand SelectMappingDieCommand => new RelayCommand(async () =>
        {
            try
            {
                ROIShape tempselectShape = MapDrawings.Select(shape =>
                {
                    var rectBegin = shape.LeftTop;
                    var rectEnd = shape.RightBottom;
                    var rect = new Rect(rectBegin, rectEnd);
                    if (rect.Contains(MapMousePixcel))
                        return shape;
                    else
                        return null;
                }).Where(s => s != null).FirstOrDefault();


                if (tempselectShape != null)
                {
                    if (this.selectShape != null)
                    {
                        this.selectShape.Stroke = System.Windows.Media.Brushes.Black;
                    }
                    tempselectShape.Stroke = System.Windows.Media.Brushes.Red;
                    this.selectShape = tempselectShape;

                    //從點選的ShapeROI  找出對應的die
                    //int listIndex = MapDrawings.IndexOf(selectShape);
                    //YuanliCore.Data.Die die = mainRecipe.DetectRecipe.WaferMap.Dies[listIndex];
                    string tip = selectShape.ToolTip.ToString();
                    string[] line = tip.Split(' ');
                    int idxX = Convert.ToInt32(line[0].Split(':')[1]);
                    int idxY = Convert.ToInt32(line[1].Split(':')[1]);
                    MoveIndexX = idxX;
                    MoveIndexY = idxY;

                    if (isRecipeAlignment == false) return;//還沒對位，MAP點位不能移動
                    //挑選出 對應index 的Die
                    YuanliCore.Data.Die[] dies = mainRecipe.DetectRecipe.WaferMap.Dies;
                    YuanliCore.Data.Die die = dies.Where(d => d.IndexX == MoveIndexX && d.IndexY == MoveIndexY).FirstOrDefault();
                    if (die == null) throw new Exception("No This Die");
                    //設計座標轉換對位後座標
                    Point transPos = transForm.TransPoint(new Point(die.MapTransX, die.MapTransY));

                    await machine.MicroDetection.TableMoveToAsync(transPos);
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        });
        public ICommand AddDetectionCommand => new RelayCommand(async () =>
        {
            try
            {
                //從點選的ShapeROI  找出對應的die
                //int listIndex = MapDrawings.IndexOf(selectShape);
                //YuanliCore.Data.Die die = mainRecipe.DetectRecipe.WaferMap.Dies[listIndex];
                string tip = selectShape.ToolTip.ToString();
                string[] line = tip.Split(' ');
                int idxX = Convert.ToInt32(line[0].Split(':')[1]);
                int idxY = Convert.ToInt32(line[1].Split(':')[1]);


                DetectionPoint point = new DetectionPoint();
                point.IndexX = idxX;
                point.IndexY = idxY;

                var newPos = transForm.TransInvertPoint(new Point(TablePosX, TablePosY));

                point.Position = new Point(Math.Ceiling(newPos.X), Math.Ceiling(newPos.Y));
                point.MicroscopeLightValue = Microscope.LightValue;
                point.MicroscopeApertureValue = Microscope.ApertureValue;
                point.LensIndex = Microscope.LensIndex;
                point.MicroscopePosition = Microscope.Position;//machineSetting.MicroscopeLensDefault.ElementAt(Microscope.LensIndex).AutoFocusPosition;
                point.MicroscopeAberationPosition = Microscope.AberationPosition; //machineSetting.MicroscopeLensDefault.ElementAt(Microscope.LensIndex).AberationPosition;
                point.CubeIndex = Microscope.CubeIndex;
                point.Filter1Index = Microscope.Filter1Index;
                point.Filter2Index = Microscope.Filter2Index;
                point.Filter3Index = Microscope.Filter3Index;
                DetectionPointList.Add(point);
                Die die = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == idxX && d.IndexY == idxY).FirstOrDefault();
                if (die != null)
                {
                    MapDieColorChange(mainRecipe.DetectRecipe.WaferMap, die, Brushes.Yellow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        });

        public ICommand RemoveDetectionCommand => new RelayCommand(async () =>
        {
            try
            {
                if (DetectionPointList.Count > SelectDetectionPointList)
                {
                    int idxX = DetectionPointList[SelectDetectionPointList].IndexX;
                    int idxY = DetectionPointList[SelectDetectionPointList].IndexY;
                    int count = DetectionPointList.Count(d => d.IndexX == idxX && d.IndexY == idxY);
                    DetectionPointList.RemoveAt(SelectDetectionPointList);
                    Die die = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == idxX && d.IndexY == idxY).FirstOrDefault();
                    if (die != null && count == 1)
                    {
                        MapDieColorReturn(mainRecipe.DetectRecipe.WaferMap, die, mainRecipe.DetectRecipe.BincodeList);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand DetectionPointListDoubleClickCommand => new RelayCommand(async () =>
        {
            try
            {
                //顯示
                int idxX = DetectionPointList[SelectDetectionPointList].IndexX;
                int idxY = DetectionPointList[SelectDetectionPointList].IndexY;
                Point SelectMousePixel = new Point(mainRecipe.DetectRecipe.WaferMap.Dies.FirstOrDefault(die => die.IndexX == idxX && die.IndexY == idxY).OperationPixalX / showSize_X + offsetDraw,
                mainRecipe.DetectRecipe.WaferMap.Dies.FirstOrDefault(die => die.IndexX == idxX && die.IndexY == idxY).OperationPixalY / showSize_Y + offsetDraw);

                //SelectMousePixel = transForm.TransInvertPoint(DetectionPointList[SelectDetectionPointList].Position);

                ROIShape tempselectShape = MapDrawings.Select(shape =>
                {
                    var rectBegin = shape.LeftTop;
                    var rectEnd = shape.RightBottom;
                    var rect = new Rect(rectBegin, rectEnd);
                    if (rect.Contains(SelectMousePixel))
                        return shape;
                    else
                        return null;
                }).Where(s => s != null).FirstOrDefault();
                if (tempselectShape != null)
                {
                    if (this.selectShape != null)
                    {
                        this.selectShape.Stroke = System.Windows.Media.Brushes.Black;
                    }
                    tempselectShape.Stroke = System.Windows.Media.Brushes.Red;
                    this.selectShape = tempselectShape;

                    //從點選的ShapeROI  找出對應的die
                    int listIndex = MapDrawings.IndexOf(selectShape);
                    YuanliCore.Data.Die die = mainRecipe.DetectRecipe.WaferMap.Dies[listIndex];
                    MoveIndexX = die.IndexX;
                    MoveIndexY = die.IndexY;
                }
                if (isRecipeAlignment == false) return;//還沒對位，MAP點位不能移動
                //移動
                var transPos = transForm.TransPoint(new Point(DetectionPointList[SelectDetectionPointList].Position.X, DetectionPointList[SelectDetectionPointList].Position.Y));
                await machine.MicroDetection.TableMoveToAsync(transPos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        private void SampleFindAction(CogMatcher matcher)
        {
            ClearShapeAction.Execute(Drawings);

            IEnumerable<MatchResult> resultPoint = matcher.Find(MainImage.ToByteFrame());

            foreach (var item in resultPoint)
            {
                var center = new ROICross
                {
                    X = item.Center.X,
                    Y = item.Center.Y,
                    Size = 50,
                    StrokeThickness = 5,
                    Stroke = System.Windows.Media.Brushes.Red,
                    IsInteractived = false
                };
                AddShapeAction.Execute(center);

                var offsetPixalX = MainImage.PixelWidth / 2 - item.Center.X;
                var offsetPixalY = MainImage.PixelHeight / 2 - item.Center.Y;
                var offsetDisX = offsetPixalX * 1.095;//1.095
                var offsetDisY = offsetPixalY * 1.095;//1.095

            }



        }

        private async void SampleMoveAction(Point pos)
        {
            await machine.MicroDetection.TableMoveToAsync(pos);
        }
        private void SetLocateParamToRecipe()
        {
            try
            {
                List<LocateParam> datas = new List<LocateParam>();
                //需要做出一個轉換矩陣 對應index 與 機台座標
                var index1 = new Point(LocateParam1.IndexX, LocateParam1.IndexY);
                var index2 = new Point(LocateParam2.IndexX, LocateParam2.IndexY);
                var index3 = new Point(LocateParam3.IndexX, LocateParam3.IndexY);



                var posDesign1 = new Point(LocateParam1.DesignPositionX, LocateParam1.DesignPositionY);
                var posDesign2 = new Point(LocateParam2.DesignPositionX, LocateParam2.DesignPositionY);
                var posDesign3 = new Point(LocateParam3.DesignPositionX, LocateParam3.DesignPositionY);
                if (mainRecipe.DetectRecipe.WaferMap != null)
                {

                    posDesign1.X = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == index1.X && d.IndexY == index1.Y).FirstOrDefault().MapTransX;
                    posDesign1.Y = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == index1.X && d.IndexY == index1.Y).FirstOrDefault().MapTransY;
                    LocateParam1.DesignPositionX = posDesign1.X;
                    LocateParam1.DesignPositionY = posDesign1.Y;


                    posDesign2.X = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == index2.X && d.IndexY == index2.Y).FirstOrDefault().MapTransX;
                    posDesign2.Y = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == index2.X && d.IndexY == index2.Y).FirstOrDefault().MapTransY;
                    LocateParam2.DesignPositionX = posDesign2.X;
                    LocateParam2.DesignPositionY = posDesign2.Y;

                    posDesign3.X = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == index3.X && d.IndexY == index3.Y).FirstOrDefault().MapTransX;
                    posDesign3.Y = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == index3.X && d.IndexY == index3.Y).FirstOrDefault().MapTransY;
                    LocateParam3.DesignPositionX = posDesign3.X;
                    LocateParam3.DesignPositionY = posDesign3.Y;

                    datas.Add(LocateParam1);
                    datas.Add(LocateParam2);
                    datas.Add(LocateParam3);
                }
                LocateParam locateParam = new LocateParam(0)
                {
                    MicroscopeLightValue = Microscope.LightValue,
                    MicroscopeApertureValue = Microscope.ApertureValue,
                    LensIndex = Microscope.LensIndex,
                    MicroscopePosition = Microscope.Position,
                    MicroscopeAberationPosition = Microscope.AberationPosition,
                    CubeIndex = Microscope.CubeIndex,
                    Filter1Index = Microscope.Filter1Index,
                    Filter2Index = Microscope.Filter2Index,
                    Filter3Index = Microscope.Filter3Index
                };
                if (LocateParamList.Count == 0)
                {
                    LocateParamList.Add(locateParam);
                }
                else
                {
                    LocateParamList.RemoveAt(0);
                    LocateParamList.Add(locateParam);
                }

                for (int i = 0; i < datas.Count; i++)
                {
                    datas[i].MicroscopeLightValue = LocateParamList[0].MicroscopeLightValue;
                    datas[i].MicroscopeApertureValue = LocateParamList[0].MicroscopeApertureValue;
                    datas[i].LensIndex = LocateParamList[0].LensIndex;
                    datas[i].MicroscopePosition = LocateParamList[0].MicroscopePosition;
                    datas[i].MicroscopeAberationPosition = LocateParamList[0].MicroscopeAberationPosition;
                    datas[i].CubeIndex = LocateParamList[0].CubeIndex;
                    datas[i].Filter1Index = LocateParamList[0].Filter1Index;
                    datas[i].Filter2Index = LocateParamList[0].Filter2Index;
                    datas[i].Filter3Index = LocateParamList[0].Filter3Index;
                }
                var pos1 = new Point(LocateParam1.GrabPositionX, LocateParam1.GrabPositionY);
                var pos2 = new Point(LocateParam2.GrabPositionX, LocateParam2.GrabPositionY);
                var pos3 = new Point(LocateParam3.GrabPositionX, LocateParam3.GrabPositionY);

                var indexs = new Point[] { index1, index2, index3 };
                var posDesign = new Point[] { posDesign1, posDesign2, posDesign3 };
                var poss = new Point[] { pos1, pos2, pos3 };
                ////if (index1.X == 0 && index1.Y == 0) return;//正常沒過LOCATE是無法進行到這步的 ，暫時卡控 讓設備不出錯
                //var transform = new CogAffineTransform(posDesign, poss);//(posDesign, poss);
                ////依序轉換完INDEX  塞回機械座標
                //foreach (YuanliCore.Data.Die die in mainRecipe.DetectRecipe.WaferMap.Dies)
                //{
                //    Point posMap = new Point(die.MapTransX, die.MapTransY);
                //    Point pos = transform.TransPoint(posMap);
                //    die.PosX = pos.X;
                //    die.PosY = pos.Y;
                //    Point posInvert = transform.TransInvertPoint(pos);
                //    if ((die.IndexX == 0 && die.IndexY == 56) || (die.IndexX == 112 && die.IndexY == 56) || (die.IndexX == 113 && die.IndexY == 56) || (die.IndexX == 0 && die.IndexY == 66))
                //    {
                //        int ss2 = 0;
                //    }
                //}
                ////this.transForm = new CogAffineTransform(posDesign, poss);

                mainRecipe.DetectRecipe.AlignRecipe.AlignmentMode = SelectMode;
                mainRecipe.DetectRecipe.AlignRecipe.OffsetX = AlignOffsetX;
                mainRecipe.DetectRecipe.AlignRecipe.OffsetY = AlignOffsetY;
                mainRecipe.DetectRecipe.AlignRecipe.FiducialDatas = datas.ToArray();


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }
        private void SetRecipeToLoadWaferParam()
        {
            AlignerMicroAngle = mainRecipe.EFEMRecipe.AlignerMicroAngle;
            AlignerWaferIDAngle = mainRecipe.EFEMRecipe.AlignerWaferIDAngle;

            MacroTopStartPitchX = mainRecipe.EFEMRecipe.MacroTopStartPitchX;
            MacroTopStartRollY = mainRecipe.EFEMRecipe.MacroTopStartRollY;
            MacroTopStartYawT = mainRecipe.EFEMRecipe.MacroTopStartYawT;

            MacroTopLeftLightValue = mainRecipe.EFEMRecipe.MacroTopLeftLightValue;
            MacroTopRightLightValue = mainRecipe.EFEMRecipe.MacroTopRightLightValue;

            MacroBackStartPos = mainRecipe.EFEMRecipe.MacroBackStartPos;

            MacroBackLeftLightValue = mainRecipe.EFEMRecipe.MacroBackLeftLightValue;
            MacroBackRightLightValue = mainRecipe.EFEMRecipe.MacroBackRightLightValue;
        }
        private void SetLoadWaferToRecipe()
        {
            mainRecipe.EFEMRecipe.AlignerMicroAngle = AlignerMicroAngle;
            mainRecipe.EFEMRecipe.AlignerWaferIDAngle = AlignerWaferIDAngle;

            mainRecipe.EFEMRecipe.MacroTopStartPitchX = MacroTopStartPitchX;
            mainRecipe.EFEMRecipe.MacroTopStartRollY = MacroTopStartRollY;
            mainRecipe.EFEMRecipe.MacroTopStartYawT = MacroTopStartYawT;

            mainRecipe.EFEMRecipe.MacroTopLeftLightValue = MacroTopLeftLightValue;
            mainRecipe.EFEMRecipe.MacroTopRightLightValue = MacroTopRightLightValue;

            mainRecipe.EFEMRecipe.MacroBackStartPos = MacroBackStartPos;

            mainRecipe.EFEMRecipe.MacroBackLeftLightValue = MacroBackLeftLightValue;
            mainRecipe.EFEMRecipe.MacroBackRightLightValue = MacroBackRightLightValue;
        }
        private void SetRecipeToLocateParam()
        {
            DetectionRecipe detectionRecipe = mainRecipe.DetectRecipe;
            if (detectionRecipe.AlignRecipe.FiducialDatas != null && detectionRecipe.AlignRecipe.FiducialDatas[0] != null)
                LocateParam1 = detectionRecipe.AlignRecipe.FiducialDatas[0];
            if (detectionRecipe.AlignRecipe.FiducialDatas != null && detectionRecipe.AlignRecipe.FiducialDatas[1] != null)
                LocateParam2 = detectionRecipe.AlignRecipe.FiducialDatas[1];
            if (detectionRecipe.AlignRecipe.FiducialDatas != null && detectionRecipe.AlignRecipe.FiducialDatas[2] != null)
                LocateParam3 = detectionRecipe.AlignRecipe.FiducialDatas[2];

            SelectMode = detectionRecipe.AlignRecipe.AlignmentMode;

            AlignOffsetX = detectionRecipe.AlignRecipe.OffsetX;
            AlignOffsetY = detectionRecipe.AlignRecipe.OffsetY;
        }
        private void SetRecipeToDetectionParam()
        {
            DetectionPointList = (ObservableCollection<DetectionPoint>)mainRecipe.DetectRecipe.DetectionPoints;
        }
        /// <summary>
        /// 將檢測座標存入recipe
        /// </summary>
        private void SetDetectionToRecipe()
        {
            mainRecipe.DetectRecipe.DetectionPoints = DetectionPointList;
        }
    }

    public class WaferUIData : INotifyPropertyChanged
    {
        private WaferProcessStatus waferStates;
        public WaferProcessStatus WaferStates { get => waferStates; set => SetValue(ref waferStates, value); }//{ get; set; }

        private int sNWidth = 20;
        public int SNWidth { get => sNWidth; set => SetValue(ref sNWidth, value); }//{ get; set; }
        public string SN { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            T oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
        }

        protected virtual void OnPropertyChanged<T>(string name, T oldValue, T newValue)
        {
            // oldValue 和 newValue 目前沒有用到，代爾後需要再實作。
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
