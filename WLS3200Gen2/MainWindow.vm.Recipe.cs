﻿using GalaSoft.MvvmLight.Command;
using Nito.AsyncEx;
using System;
using System.Collections;
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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        private ObservableCollection<DetectionPoint> detectionHomePointList = new ObservableCollection<DetectionPoint>();
        private ObservableCollection<DetectionPoint> detectionPointList = new ObservableCollection<DetectionPoint>();

        private BitmapSource locateSampleImage1;
        private BitmapSource locateSampleImage2;
        private BitmapSource locateSampleImage3;

        private LocateParam locateParam1 = new LocateParam(101);//Locate pattern 從100號開始
        private LocateParam locateParam2 = new LocateParam(102);//Locate pattern 從100號開始
        private LocateParam locateParam3 = new LocateParam(103);//Locate pattern 從100號開始
        private ObservableCollection<ROIShape> drawings = new ObservableCollection<ROIShape>();
        private ObservableCollection<ROIShape> manualdrawings = new ObservableCollection<ROIShape>();

        private ObservableCollection<ROIShape> mapDrawings = new ObservableCollection<ROIShape>();
        private Action<CogMatcher> sampleFind;
        private Action<Point> alignMarkMove;
        private LocateMode selectMode;
        private double alignOffsetX, alignOffsetY;

        private ITransform transForm; //紀錄 從設計座標轉換成對位後座標的 公式
        private int moveIndexX, moveIndexY, detectionIndexX, detectionIndexY;
        private double locateX, locateY;
        private bool isLocate;
        private int selectDetectionHomePointList;
        private ObservableCollection<int> selectRecipeDetectionPointList = new ObservableCollection<int>();
        private bool isFirstRecipePageSelect = true, isRecipeMapShow;
        private bool isMainHomePageSelect, isMainRecipePageSelect, isMainSettingPageSelect, isMainToolsPageSelect, isMainSecurityPageSelect;
        private bool isLoadwaferPageSelect, isMacroPageSelect, isAlignerPageSelect, isLocatePageSelect, isDetectionPageSelect;
        private bool isLoadwaferOK, isLocateOK, isDetectionOK; //判斷各設定頁面是否滿足條件 ，  才能切換到下一頁
        private string mapAdd = "MAP ADD";
        private System.Windows.Point recipeMousePixcel;
        private ROIShape selectShape;
        private bool isStepLoadwafer = true, isStepMacro, isStepAligner, isStepLocate, isStepDetection;

        private double alignerMicroAngle, alignerWaferIDAngle;
        private double macroTopStartPitchX, macroTopStartRollY, macroTopStartYawT, macroBackStartPos;
        private int macroTopLeftLightValue, macroTopRightLightValue, macroBackLeftLightValue, macroBackRightLightValue;
        private Model.ArmStation lastArmStation = Model.ArmStation.Cassette1;
        private string topMoveContent = "TopMove", backMoveContent = "BackMove";
        private readonly object lockObjEFEMTrans = new object();
        private bool isCanWorkEFEMTrans = true;
        private bool isDie, isDieSub, isDieInSideAll, isPosition;
        /// <summary>
        /// recipe頁面是否變更Die
        /// </summary>
        private bool isRecipeShowNowDie = false;
        /// <summary>
        /// 在Recipe頁面是否已經對完位
        /// </summary>
        private bool isRecipeAlignment = false;
        /// <summary>
        /// 是否正在MapAdd模式
        /// </summary>
        private bool isMapAdding = false;
        /// <summary>
        /// MapAdd防彈跳
        /// </summary>
        private bool isMapEnd = true;
        /// <summary>
        /// RecipeMap可以做事情
        /// </summary>
        private bool isRecipeMapCanEdit = true;
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
                if (!isInitialComplete) return isMainHomePageSelect;
                if (isMainHomePageSelect)
                {
                    WriteLog(YuanliCore.Logger.LogType.TRIG, "Enter the HomePage");
                    if (isRunCommand == false)
                    {
                        ResetTempAssign();
                        ResetDetectionRunningPointList();
                    }
                    ShowDetectionHomeNewMapImgae(mainRecipe.DetectRecipe);
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
                if (!isInitialComplete)
                {
                    isMainRecipePageSelect = false;
                    IsMainHomePageSelect = true;
                    return isMainRecipePageSelect;
                }  //ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯

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
                if (!isInitialComplete)
                {
                    isMainToolsPageSelect = false;
                    IsMainHomePageSelect = true;
                    return isMainToolsPageSelect;
                }  //ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
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
                if (!isInitialComplete)
                {
                    isMainSettingPageSelect = false;
                    IsMainHomePageSelect = true;
                    return isMainSettingPageSelect;
                }  //ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯

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
                if (!isInitialComplete)
                {
                    isMainSecurityPageSelect = false;
                    IsMainHomePageSelect = true;
                    return isMainSecurityPageSelect;
                }  //ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
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
                if (!isInitialComplete) return isLocatePageSelect;//ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
                return isLoadwaferPageSelect;
            }
            set => SetValue(ref isLoadwaferPageSelect, value);
        }
        public bool IsMacroPageSelect
        {
            get
            {
                if (!isInitialComplete) return isMacroPageSelect;//ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
                if (!IsStepMacro) isMacroPageSelect = false;
                return isMacroPageSelect;
            }
            set => SetValue(ref isMacroPageSelect, value);
        }
        public bool IsAlignerPageSelect
        {
            get
            {
                if (!isInitialComplete) return isAlignerPageSelect;//ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
                if (!IsStepAligner) isAlignerPageSelect = false;
                return isAlignerPageSelect;
            }
            set => SetValue(ref isAlignerPageSelect, value);
        }

        /// <summary>
        ///  切換到 對位頁
        /// </summary>
        public bool IsLocatePageSelect
        {
            get
            {
                if (!isInitialComplete) return isLocatePageSelect;//ui初始化會進來一次  所以在沒有完成初始化之前不做下面邏輯
                if (!IsStepLocate) isLocatePageSelect = false;
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
                if (!IsStepDetection) isDetectionPageSelect = false;

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
        public bool IsMapEnd { get => isMapEnd; set => SetValue(ref isMapEnd, value); }
        public bool IsRecipeMapCanEdit { get => isRecipeMapCanEdit; set => SetValue(ref isRecipeMapCanEdit, value); }
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
        public bool IsStepLoadwafer { get => isStepLoadwafer; set => SetValue(ref isStepLoadwafer, value); }
        /// <summary>
        /// Macro 設定完成
        /// </summary>
        public bool IsStepMacro { get => isStepMacro; set => SetValue(ref isStepMacro, value); }
        /// <summary>
        /// Aligner 設定完成，且Notch已經轉到Micro位置
        /// </summary>
        public bool IsStepAligner { get => isStepAligner; set => SetValue(ref isStepAligner, value); }
        /// <summary>
        /// locate已完成 (Detection頁面功能需要判斷)
        /// </summary>
        public bool IsStepLocate { get => isStepLocate; set => SetValue(ref isStepLocate, value); }
        /// <summary>
        /// 
        /// </summary>
        public bool IsStepDetection { get => isStepDetection; set => SetValue(ref isStepDetection, value); }


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
        /// <summary>
        /// 需要檢查的點(即將要log紀錄的資訊)
        /// </summary>
        public ObservableCollection<DetectionPoint> DetectionHomePointList { get => detectionHomePointList; set => SetValue(ref detectionHomePointList, value); }
        public int SelectDetectionHomePointList { get => selectDetectionHomePointList; set => SetValue(ref selectDetectionHomePointList, value); }
        /// <summary>
        /// 需要檢查的點(Recipe)
        /// </summary>
        public ObservableCollection<DetectionPoint> DetectionPointList { get => detectionPointList; set => SetValue(ref detectionPointList, value); }
        public ObservableCollection<int> SelectRecipeDetectionPointList { get => selectRecipeDetectionPointList; set => SetValue(ref selectRecipeDetectionPointList, value); }
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


        public double LocateX { get => locateX; set => SetValue(ref locateX, value); }
        public double LocateY { get => locateY; set => SetValue(ref locateY, value); }
        public int MoveIndexX { get => moveIndexX; set => SetValue(ref moveIndexX, value); }
        public int MoveIndexY { get => moveIndexY; set => SetValue(ref moveIndexY, value); }
        public int DetectionIndexX { get => detectionIndexX; set => SetValue(ref detectionIndexX, value); }
        public int DetectionIndexY { get => detectionIndexY; set => SetValue(ref detectionIndexY, value); }

        /// <summary>
        /// 滑鼠在影像內 Pixcel 座標
        /// </summary>
        public System.Windows.Point RecipeMousePixcel { get => recipeMousePixcel; set => SetValue(ref recipeMousePixcel, value); }
        /// <summary>
        /// MapAdd標題名稱
        /// </summary>
        public String MapAdd { get => mapAdd; set => SetValue(ref mapAdd, value); }
        /// <summary>
        /// 取得或設定 shape 
        /// </summary>
        public ObservableCollection<ROIShape> Drawings { get => drawings; set => SetValue(ref drawings, value); }
        /// <summary>
        /// 手動量測工具
        /// </summary>
        public ObservableCollection<ROIShape> ManualDrawings { get => manualdrawings; set => SetValue(ref manualdrawings, value); }

        public bool IsDie { get => isDie; set => SetValue(ref isDie, value); }
        public bool IsDieSub { get => isDieSub; set => SetValue(ref isDieSub, value); }
        public bool IsDieInSideAll { get => isDieInSideAll; set => SetValue(ref isDieInSideAll, value); }
        public bool IsPosition { get => isPosition; set => SetValue(ref isPosition, value); }

        //Recipe進入會執行
        private void LoadRecipePage()
        {
            if (RightsModel.Operator == Account.CurrentAccount.Right || RightsModel.Visitor == Account.CurrentAccount.Right ||
                Machinestatus == YuanliCore.Machine.Base.MachineStates.RUNNING || isHome == false || Machinestatus != YuanliCore.Machine.Base.MachineStates.IDLE)
            {
                IsMainRecipePageSelect = false;
                IsMainHomePageSelect = true;
                return;
            }

            //如果是第一次進來RecipePageSelect ZoomFitManualAction
            if (isFirstRecipePageSelect)
            {
                Task.Run(async () =>
                {
                    //這裡的Await是要重新刷新UI才可以讓ZoomFitManualAction有效果
                    await Task.Delay(50);
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        try
                        {
                            ZoomRcipeFitManualAction.Execute(Drawings);
                            isFirstRecipePageSelect = false;
                        }
                        catch (Exception ex)
                        {
                        }
                    }));
                });
            }
            //為什麼要在這邊判斷，因為要先切到Recipe頁面再來畫圖才會出現
            if (isRecipeMapShow == false && mainRecipe.DetectRecipe.WaferMap != null)
            {
                //開一個Task是因為要等畫面切完之後，再來變化
                Task.Run(async () =>
                {
                    //這裡的Await是要重新刷新UI才可以讓ZoomFitManualAction有效果
                    await Task.Delay(50);
                    isRecipeMapShow = true;
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        try
                        {
                            ShowRecipeNewMapImage(mainRecipe.DetectRecipe);
                            ShowDetectionRecipeNewMapImgae(mainRecipe.DetectRecipe);
                        }
                        catch (Exception ex)
                        {
                        }
                    }));
                });
            }
            else
            {
                ShowDetectionRecipeNewMapImgae(mainRecipe.DetectRecipe);
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
            isRecipeShowNowDie = true;
            //始終會切回到第一頁 LoadWafer 頁
            IsLoadwaferPageSelect = true;
            WriteLog(YuanliCore.Logger.LogType.TRIG, "Enter the RecipePage");
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
            isRecipeShowNowDie = false;
        }
        private void LoadToolsPage()
        {
            if (RightsModel.AdvancedOperator == Account.CurrentAccount.Right ||
                isHome == false || Machinestatus != YuanliCore.Machine.Base.MachineStates.IDLE)
            {
                IsMainToolsPageSelect = false;
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
            IsLoadwaferPageSelect = true;
            WriteLog(YuanliCore.Logger.LogType.TRIG, "Enter the ToolsPage");
        }
        private void UnLoadToolsPage()
        {

        }
        //Recipe進入會執行
        private void LoadSettingPage()
        {
            if (RightsModel.Operator == Account.CurrentAccount.Right || RightsModel.Visitor == Account.CurrentAccount.Right ||
                Machinestatus != YuanliCore.Machine.Base.MachineStates.IDLE)
            {
                IsMainSettingPageSelect = false;
                IsMainHomePageSelect = true;
                return;
            }
            LogPath = machineSetting.LogPath;
            ResultPath = machineSetting.ResultPath;

            TableWaferCatchPositionX = machineSetting.TableWaferCatchPosition.X;
            TableWaferCatchPositionY = machineSetting.TableWaferCatchPosition.Y;
            TableWaferCatchPositionZ = machineSetting.TableWaferCatchPositionZ;
            TableWaferCatchPositionR = machineSetting.TableWaferCatchPositionR;
            TableCenterX = machineSetting.TableCenterPosition.X;
            TableCenterY = machineSetting.TableCenterPosition.Y;
            RobotAxisLoadPort1TakePosition = machineSetting.RobotAxisLoadPort1TakePosition;
            RobotAxisLoadPort2TakePosition = machineSetting.RobotAxisLoadPort2TakePosition;
            RobotAxisAligner1TakePosition = machineSetting.RobotAxisAlignTakePosition;
            RobotAxisMacroTakePosition = machineSetting.RobotAxisMacroTakePosition;
            RobotAxisMicroTakePosition = machineSetting.RobotAxisMicroTakePosition;
            AlignerMicroOffset = machineSetting.AlignerMicroOffset;
            AlignerUnLoadOffset = machineSetting.AlignerUnLoadOffset;

            InnerRingPitchXPositionPEL = machineSetting.InnerRingPitchXPositionPEL;
            InnerRingPitchXPositionNEL = machineSetting.InnerRingPitchXPositionNEL;
            InnerRingRollYPositionPEL = machineSetting.InnerRingRollYPositionPEL;
            InnerRingRollYPositionNEL = machineSetting.InnerRingRollYPositionNEL;
            InnerRingYawTPositionPEL = machineSetting.InnerRingYawTPositionPEL;
            InnerRingYawTPositionNEL = machineSetting.InnerRingYawTPositionNEL;
            OuterRingRollYPositionPEL = machineSetting.OuterRingRollYPositionPEL;
            OuterRingRollYPositionNEL = machineSetting.OuterRingRollYPositionNEL;

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
            }
            WriteLog(YuanliCore.Logger.LogType.TRIG, "Enter the SettingPage");
        }
        //離開recipe頁面會執行
        private void UnLoadSettingPage()
        {
            try
            {
                //machineSetting.TableWaferCatchPosition = new Point(TableWaferCatchPositionX, TableWaferCatchPositionY);
                //machineSetting.TableWaferCatchPositionZ = TableWaferCatchPositionZ;
                //machineSetting.TableWaferCatchPositionR = TableWaferCatchPositionR;
                //machineSetting.TableCenterPosition = new Point(TableCenterX, TableCenterY);
                //machineSetting.RobotAxisLoadPort1TakePosition = RobotAxisLoadPort1TakePosition;
                //machineSetting.RobotAxisLoadPort2TakePosition = RobotAxisLoadPort2TakePosition;
                //machineSetting.RobotAxisAlignTakePosition = RobotAxisAligner1TakePosition;
                //machineSetting.RobotAxisMacroTakePosition = RobotAxisMacroTakePosition;
                //machineSetting.RobotAxisMicroTakePosition = RobotAxisMicroTakePosition;
                //machineSetting.AlignerMicroOffset = AlignerMicroOffset;
                //machineSetting.AlignerUnLoadOffset = AlignerUnLoadOffset;
                //machineSetting.LogPath = LogPath;
                //machineSetting.ResultPath = ResultPath;
                //machineSetting.InnerRingPitchXPositionPEL = InnerRingPitchXPositionPEL;
                //machineSetting.InnerRingPitchXPositionNEL = InnerRingPitchXPositionNEL;
                //machineSetting.InnerRingRollYPositionPEL = InnerRingRollYPositionPEL;
                //machineSetting.InnerRingRollYPositionNEL = InnerRingRollYPositionNEL;
                //machineSetting.InnerRingYawTPositionPEL = InnerRingYawTPositionPEL;
                //machineSetting.InnerRingYawTPositionNEL = InnerRingYawTPositionNEL;
                //machineSetting.OuterRingRollYPositionPEL = OuterRingRollYPositionPEL;
                //machineSetting.OuterRingRollYPositionNEL = OuterRingRollYPositionNEL;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        //進入locate頁會執行
        private void LoadLoactePage()
        {

            //始終會切回到第一頁 LoadWafer 頁      
            //   IsLoadwaferPageSelect = true;
            //   IsLocatePageSelect = false;

        }






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

                var cc = machineSetting.AlignerCOM;
                var cc2 = TableXConfig.AxisID;
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
        //MoveToPickZ
        public ICommand MoveToPickZCommand => new RelayCommand(async () =>
        {
            try
            {
                await machine.MicroDetection.AxisZ.MoveToAsync(machineSetting.TableWaferCatchPositionZ);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand MoveToCenterCommand => new RelayCommand(async () =>
        {
            try
            {
                Point pos = transForm.TransPoint(mainRecipe.DetectRecipe.WaferMap.MapCenterPoint);
                await machine.MicroDetection.TableMoveToAsync(pos);
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
                //MainRecipe recipe = mainRecipe;
                //ProcessSetting processSetting = new ProcessSetting();
                //processSetting.IsAutoFocus = true;
                //processSetting.IsAutoSave = true;
                //Wafer currentWafer = new Wafer(1);
                //var cc = machine.MicroDetection.Microscope.AberationPosition;
                //await machine.MicroDetection.Run(currentWafer, recipe, processSetting, ptsTest, ctsTest);
                SubDieEditWindow subDieEditWindow = new SubDieEditWindow(MainImage);
                subDieEditWindow.ShowDialog();
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

                    ShowHomeNewMapImage(mainRecipe.DetectRecipe);
                    ShowDetectionHomeNewMapImgae(mainRecipe.DetectRecipe);
                    ResetDetectionRunningPointList();

                    ShowRecipeNewMapImage(mainRecipe.DetectRecipe);
                    ShowDetectionRecipeNewMapImgae(mainRecipe.DetectRecipe);

                    transForm = OrgTransform(mainRecipe.DetectRecipe.WaferMap.MapCenterPoint, machineSetting.TableCenterPosition);
                }
            }
            else
            {
                SINF_Path = "";
            }
        });
        public ICommand RecipeCanvasDoubleClickCommand => new RelayCommand(async () =>
        {
            try
            {
                //在特定情況下才可以動作
                if (IsCanWorkEFEMTrans && isRecipeCanvasCanMove)
                {
                    var relativeX = (RecipeMousePixcel.X - MainImage.PixelWidth / 2) * MicroscopeLensDefault[machine.MicroDetection.Microscope.LensIndex].RatioX;
                    var relativeY = (RecipeMousePixcel.Y - MainImage.PixelHeight / 2) * MicroscopeLensDefault[machine.MicroDetection.Microscope.LensIndex].RatioY;
                    await machine.MicroDetection.TableMoveToAndOnceAsync(new Point(relativeX, relativeY));
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand GrabSaveAsCommand => new RelayCommand(() =>
        {
            try
            {
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog
                {
                    Filter = "Bmp Files (*.bmp)|*.bmp|All Files (*.*)|*.*"
                };
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    BitmapSource bmp = machine.MicroDetection.Camera.GrabAsync();
                    bmp.ToBitmap().Save(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand RecipeBincodeEditCommand => new RelayCommand(() =>
        {
            try
            {
                if (BincodeList == null)
                    BincodeList = new ObservableCollection<BincodeInfo>();

                BincodeSettingWindow settingWindow = new BincodeSettingWindow(BincodeList);
                settingWindow.ShowDialog();
                BincodeList = settingWindow.BincodeList;
                if (mainRecipe.DetectRecipe.BincodeList != null)
                {
                    mainRecipe.DetectRecipe.BincodeList = BincodeList;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });



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
                    case "WaferIDGrab":
                        await machine.Feeder.Reader.ReadAsync();
                        ResultImage = new WriteableBitmap(machine.Feeder.Reader.Image.ToBitmapSource());
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
                WriteLog(YuanliCore.Logger.LogType.TRIG, "RecipePage Wafer To LoadPort");
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
                    MessageBox.Show("WaferToLoadPort Error! No LoadPort!!", "Info", MessageBoxButton.YesNo);
                    return;
                }
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                await Task.Run(() =>
                {
                    try
                    {
                        if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                        {
                            IsCanWorkEFEMTrans = false;
                            //是否執行移動片子訊息

                            if (result == MessageBoxResult.Yes)
                            {
                                WriteLog(YuanliCore.Logger.LogType.PROCESS, "RecipePage Wafer To LoadPort Start");
                                //片子上一個狀態先記錄起來
                                Model.ArmStation oldArmStation = RecipeLastArmStation;
                                //確認手臂有無片
                                if (EFEMTransWaferBeforeCheckRobotHaveWafer())
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
                                    throw new FlowException("EFEMTransCommand Error!");
                                }
                            }
                            IsStepMacro = true;
                            IsStepAligner = false;
                            IsStepLocate = false;
                            IsStepDetection = false;
                            IsCanWorkEFEMTrans = true;
                            WriteLog(YuanliCore.Logger.LogType.PROCESS, "RecipePage Wafer To LoadPort End");
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
                isInitialComplete = false;
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        public ICommand EFEMTransToAlignerCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "RecipePage Wafer To Aligner");
                //是否執行移動片子訊息
                string mesage = EFEMTransWaferMessage(RecipeLastArmStation, Model.ArmStation.Align);
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);

                await Task.Run(() =>
                {
                    if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                    {
                        IsCanWorkEFEMTrans = false;
                        if (result == MessageBoxResult.Yes)
                        {
                            WriteLog(YuanliCore.Logger.LogType.PROCESS, "RecipePage Wafer To Aligner Start");
                            //片子上一個狀態先記錄起來
                            Model.ArmStation oldArmStation = RecipeLastArmStation;
                            //確認手臂有無片
                            if (EFEMTransWaferBeforeCheckRobotHaveWafer())
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
                            IsStepMacro = true;
                            IsStepAligner = true;
                            IsStepLocate = false;
                            IsStepDetection = false;
                            WriteLog(YuanliCore.Logger.LogType.PROCESS, "RecipePage Wafer To Aligner End");
                        }
                        IsCanWorkEFEMTrans = true;
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
                isInitialComplete = false;
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        public ICommand EFEMTransToMacroCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                WriteLog(YuanliCore.Logger.LogType.TRIG, "RecipePage Wafer To Macro");
                string mesage = EFEMTransWaferMessage(RecipeLastArmStation, Model.ArmStation.Macro);
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                await Task.Run(() =>
                {
                    try
                    {
                        if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                        {
                            IsCanWorkEFEMTrans = false;
                            //是否執行移動片子訊息
                            if (result == MessageBoxResult.Yes)
                            {
                                WriteLog(YuanliCore.Logger.LogType.PROCESS, "RecipePage Wafer To Macro Start");
                                //片子上一個狀態先記錄起來
                                Model.ArmStation oldArmStation = RecipeLastArmStation;
                                //確認手臂有無片
                                if (EFEMTransWaferBeforeCheckRobotHaveWafer())
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
                                    throw new FlowException("EFEMTransCommand 異常!WaferToMacro Macro真空異常!!");
                                }
                                IsStepMacro = true;
                                IsStepAligner = false;
                                IsStepLocate = false;
                                IsStepDetection = false;
                                WriteLog(YuanliCore.Logger.LogType.PROCESS, "RecipePage Wafer To Macro End");
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
                isInitialComplete = false;
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
                    return true;
                }
                else
                {
                    machine.Feeder.Robot.ReleaseWafer().Wait();
                    return false;
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
                WriteLog(YuanliCore.Logger.LogType.TRIG, "RecipePage Wafer To Micro");
                string mesage = EFEMTransWaferMessage(RecipeLastArmStation, Model.ArmStation.Micro);
                var result = MessageBox.Show(mesage, "Info", MessageBoxButton.YesNo);
                await Task.Run(async () =>
                {
                    try
                    {
                        if (IsCanWorkEFEMTrans && Machinestatus == YuanliCore.Machine.Base.MachineStates.IDLE)
                        {
                            IsCanWorkEFEMTrans = false;
                            //是否執行移動片子訊息
                            if (result == MessageBoxResult.Yes)
                            {
                                WriteLog(YuanliCore.Logger.LogType.PROCESS, "RecipePage Wafer To Micro Start");
                                //片子上一個狀態先記錄起來
                                Model.ArmStation oldArmStation = RecipeLastArmStation;
                                //確認手臂有無片
                                if (EFEMTransWaferBeforeCheckRobotHaveWafer())
                                {
                                    throw new FlowException("EFEMTransCommand Error!Robot Have Wafer!");
                                }
                                //將Wafer取出，退到安全位置
                                EFEMTransWaferPick(oldArmStation);
                                //將片子放下去
                                RecipeLastArmStation = Model.ArmStation.Micro;
                                Task micro = machine.MicroDetection.TableMoveToAsync(machineSetting.TableWaferCatchPosition);
                                Task microZ = machine.MicroDetection.AxisZ.MoveToAsync(machineSetting.TableWaferCatchPositionZ);
                                if (machineSetting.LoadPortCount == Model.LoadPortQuantity.Single)
                                {
                                    await Task.WhenAll(micro, microZ);
                                }
                                else
                                {
                                    Task robot = machine.Feeder.RobotAxis.MoveToAsync(machineSetting.RobotAxisMicroTakePosition);
                                    await Task.WhenAll(micro, microZ, robot);
                                }
                                machine.MicroDetection.TableVacuum.On();
                                Wafer station = new Wafer(1);
                                await machine.Feeder.LoadToMicroAsync(station);
                                if (machine.MicroDetection.IsTableVacuum.IsSignal == false)
                                {
                                    throw new FlowException("EFEMTransCommand 異常!WaferToMicro Micro真空異常!!");
                                }
                                IsStepAligner = true;
                                IsStepMacro = true;
                                IsStepAligner = true;
                                IsStepLocate = true;
                                IsStepDetection = false;
                                WriteLog(YuanliCore.Logger.LogType.PROCESS, "RecipePage Wafer To Micro End");
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
                isInitialComplete = false;
                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
            }
        });
        public ICommand SetLightValueCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                switch (key)
                {
                    case "SetToTop":
                        MacroTopLeftLightValue = LampControl1Param.LightValue;
                        MacroTopRightLightValue = LampControl2Param.LightValue;
                        break;
                    case "SetToBack":
                        MacroBackLeftLightValue = LampControl1Param.LightValue;
                        MacroBackRightLightValue = LampControl2Param.LightValue;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
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
            SetMicroScopeValueToLocateParamList();
            SetLocateParamToRecipe();
        });
        private void SetMicroScopeValueToLocateParamList()
        {
            try
            {
                LocateParam locateParam = new LocateParam(0)
                {
                    MicroscopeLightValue = Microscope.LightValue,
                    MicroscopeApertureValue = Microscope.ApertureValue,
                    LensIndex = Microscope.LensIndex,
                    MicroscopePosition = Microscope.Position,
                    MicroscopeAberationPosition = Microscope.Position,
                    CubeIndex = Microscope.CubeIndex,
                    Filter1Index = Microscope.Filter1Index,
                    Filter2Index = Microscope.Filter2Index,
                    Filter3Index = Microscope.Filter3Index
                };
                LocateParamList.Clear();
                LocateParamList.Add(locateParam);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public ICommand LocateParamListDoubleClickCommand => new RelayCommand(async () =>
        {
            try
            {
                //設定顯微鏡參數
                await Microscope.ChangeLightAsync(LocateParamList[0].MicroscopeLightValue);
                await Microscope.ChangeApertureAsync(LocateParamList[0].MicroscopeApertureValue);
                await Microscope.ChangeLensAsync(LocateParamList[0].LensIndex);
                await Microscope.ChangeCubeAsync(LocateParamList[0].CubeIndex);
                await Microscope.ChangeFilter1Async(LocateParamList[0].Filter1Index);
                await Microscope.ChangeFilter2Async(LocateParamList[0].Filter2Index);
                await Microscope.ChangeFilter3Async(LocateParamList[0].Filter3Index);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        private CogAffineTransform OrgTransform(Point waferMapCenter, Point workHolderCenter)
        {
            try
            {
                isRecipeAlignment = false;
                List<Point> designPos = new List<Point>();
                List<Point> targetPos = new List<Point>();
                designPos.Add(new Point(waferMapCenter.X, waferMapCenter.Y));
                targetPos.Add(new Point(workHolderCenter.X, workHolderCenter.Y));

                designPos.Add(new Point(waferMapCenter.X + 1000, waferMapCenter.Y));
                targetPos.Add(new Point(workHolderCenter.X + 1000, workHolderCenter.Y));

                designPos.Add(new Point(waferMapCenter.X, waferMapCenter.Y + 1000));
                targetPos.Add(new Point(workHolderCenter.X, workHolderCenter.Y + 1000));

                var transform = new CogAffineTransform(designPos, targetPos);

                isRecipeAlignment = true;
                return transform;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ICommand LocateRunCommand => new RelayCommand(async () =>
        {
            try
            {
                if (IsCanWorkEFEMTrans)
                {
                    IsCanWorkEFEMTrans = false;
                    IsStepDetection = false;
                    //經過這個方法後，會將map中所有的Die  Index座標轉換成當下樣本建立時的實際機台座標(pattern中心)
                    SetLocateParamToRecipe();
                    //將die的map座標都轉換成 實際機台座標(解決片子更換後位置不對的問題 )

                    transForm = await machine.MicroDetection.Alignment(mainRecipe.DetectRecipe.AlignRecipe, machineSetting.MicroscopeLensDefault.ElementAt(Microscope.LensIndex));
                    isRecipeAlignment = true;
                    //將所有的Die 轉換成實際片子座標(如果建立樣本時的wafer 與 LocateRun 是一起建立的  那座標會一樣 ，主要Locate目的是針對換wafer以後要重新對位
                    //如果有需要調整檢測座標 ，需要重新做對位  ，對位後會重新建立新的map全部die座標 ，為了給後續檢測座標設定使用 

                    //依序轉換完對位前座標  ，轉換成對位後座標 塞回機械座標

                    updateRecipeMapTransform = transForm;
                    foreach (Die die in mainRecipe.DetectRecipe.WaferMap.Dies)
                    {
                        Point pos = transForm.TransPoint(new Point(die.PosX, die.PosY));
                        die.PosX = pos.X;
                        die.PosY = pos.Y;
                    }
                    IsStepDetection = true;
                    IsCanWorkEFEMTrans = true;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                IsCanWorkEFEMTrans = true;
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
                Die[] dies = mainRecipe.DetectRecipe.WaferMap.Dies;
                Die die = dies.Where(d => d.IndexX == MoveIndexX && d.IndexY == MoveIndexY).FirstOrDefault();
                if (die == null) throw new Exception("No This Die");
                //設計座標轉換對位後座標
                Point transPos = transForm.TransPoint(new Point(die.MapTransX + mainRecipe.DetectRecipe.AlignRecipe.OffsetX, die.MapTransY + mainRecipe.DetectRecipe.AlignRecipe.OffsetY));

                await machine.MicroDetection.TableMoveToAsync(new Point(transPos.X, transPos.Y));


                //
                var mapPoint = new Point(die.MapTransX, die.MapTransY);
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

                SetFocusCenter?.Invoke(tempselectRects.Col, tempselectRects.Row);
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
                int idxX = MicroCheckNowIndexX;
                int idxY = MicroCheckNowIndexY;


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
                mainRecipe.DetectRecipe.DetectionPoints = DetectionPointList;
                Die die = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == idxX && d.IndexY == idxY).FirstOrDefault();
                if (die != null)
                {
                    RecipeMapDieColorChange(false, die, Brushes.Yellow);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand MapAddDetectionCommand => new RelayCommand(async () =>
        {
            try
            {
                if (MapAdd == "MAP ADD")
                {
                    IsRecipeMapCanEdit = false;
                    IsMapEnd = false;
                    isMapAdding = true;
                    MappingRecipeOp?.Invoke(MappingOperate.Clear);
                    await Task.Delay(50);
                    mainRecipe.DetectRecipe.DetectionPoints = DetectionPointList;
                    ShowRecipeMapImgaeAddType(mainRecipe.DetectRecipe);
                    MappingRecipeOp?.Invoke(MappingOperate.StartAdd);
                    MapAdd = "END";
                    IsMapEnd = true;
                }
                else
                {
                    IsMapEnd = false;
                    MappingRecipeOp?.Invoke(MappingOperate.EndAdd);
                    await Task.Delay(50);
                    //將SelectRectangles全部加填進去，然後用當前的參數
                    DetectionPoint pointParam = new DetectionPoint();
                    pointParam.MicroscopeLightValue = Microscope.LightValue;
                    pointParam.MicroscopeApertureValue = Microscope.ApertureValue;
                    pointParam.LensIndex = Microscope.LensIndex;
                    pointParam.MicroscopePosition = Microscope.Position;//machineSetting.MicroscopeLensDefault.ElementAt(Microscope.LensIndex).AutoFocusPosition;
                    pointParam.MicroscopeAberationPosition = Microscope.AberationPosition; //machineSetting.MicroscopeLensDefault.ElementAt(Microscope.LensIndex).AberationPosition;
                    pointParam.CubeIndex = Microscope.CubeIndex;
                    pointParam.Filter1Index = Microscope.Filter1Index;
                    pointParam.Filter2Index = Microscope.Filter2Index;
                    pointParam.Filter3Index = Microscope.Filter3Index;
                    DetectionPointList.Clear();
                    foreach (var item in SelectRectangles)
                    {
                        int idxX = item.Col;
                        int idxY = item.Row;
                        var haveSameXY = DetectionPointList.FirstOrDefault(d => d.IndexX == idxX && d.IndexY == idxY);
                        Die die = mainRecipe.DetectRecipe.WaferMap.Dies.FirstOrDefault(d => d.IndexX == idxX && d.IndexY == idxY);
                        if (item.Fill == Brushes.Yellow && die != null)
                        {
                            DetectionPoint point = new DetectionPoint();
                            point.MicroscopeLightValue = pointParam.MicroscopeLightValue;
                            point.MicroscopeApertureValue = pointParam.MicroscopeApertureValue;
                            point.LensIndex = pointParam.LensIndex;
                            point.MicroscopePosition = pointParam.MicroscopePosition;
                            point.MicroscopeAberationPosition = pointParam.MicroscopeAberationPosition;
                            point.CubeIndex = pointParam.CubeIndex;
                            point.Filter1Index = pointParam.Filter1Index;
                            point.Filter2Index = pointParam.Filter2Index;
                            point.Filter3Index = pointParam.Filter3Index;

                            point.IndexX = idxX;
                            point.IndexY = idxY;
                            //var newPos = transForm.TransInvertPoint(new Point(die.MapTransX, die.MapTransY));
                            point.Position = new Point(Math.Ceiling(die.MapTransX), Math.Ceiling(die.MapTransY));
                            DetectionPointList.Add(point);
                        }
                    }
                    MapAdd = "MAP ADD";
                    mainRecipe.DetectRecipe.DetectionPoints = DetectionPointList;
                    ShowDetectionRecipeNewMapImgae(mainRecipe.DetectRecipe);
                    IsMapEnd = true;
                    isMapAdding = false;
                    IsRecipeMapCanEdit = true;
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
                int selectRecipeIndex = -1;
                //顯示
                if (SelectRecipeDetectionPointList.Count >= 1)
                {
                    selectRecipeIndex = SelectRecipeDetectionPointList[0];
                }
                if (selectRecipeIndex >= 0)
                {
                    if (DetectionPointList.Count > selectRecipeIndex)
                    {
                        int idxX = DetectionPointList[selectRecipeIndex].IndexX;
                        int idxY = DetectionPointList[selectRecipeIndex].IndexY;
                        int count = DetectionPointList.Count(d => d.IndexX == idxX && d.IndexY == idxY);
                        DetectionPointList.RemoveAt(selectRecipeIndex);
                        Die die = mainRecipe.DetectRecipe.WaferMap.Dies.Where(d => d.IndexX == idxX && d.IndexY == idxY).FirstOrDefault();
                        if (die != null && count == 1)
                        {
                            RecipeMapDieColorChange(false, die, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand EditDetectionCommand => new RelayCommand(async () =>
        {
            try
            {
                //mainRecipe.DetectRecipe.DetectionPoints = DetectionPointList;
                int selectRecipeIndex = -1;
                //顯示
                if (SelectRecipeDetectionPointList.Count >= 1)
                {
                    selectRecipeIndex = SelectRecipeDetectionPointList[0];
                }
                if (selectRecipeIndex >= 0)
                {
                    ObservableCollection<int> nowSelectRecipeDetectionPointList = SelectRecipeDetectionPointList;
                    //設定初始值=-1

                    DetectionPoint editInitDetectionPoint = InitialDetectionPoint(nowSelectRecipeDetectionPointList);
                    DetectionEditWindow detectionEditWindow = new DetectionEditWindow(editInitDetectionPoint, Microscope);
                    detectionEditWindow.ShowDialog();

                    if (detectionEditWindow.DetectionPointList != null && detectionEditWindow.DetectionPointList.Count > 0)
                    {
                        UpdateDetectionPointList(nowSelectRecipeDetectionPointList, detectionEditWindow.DetectionPointList);
                    }
                    detectionEditWindow = null;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        private void UpdateDetectionPointList(ObservableCollection<int> nowSelectRecipeDetectionPointList, ObservableCollection<DetectionPoint> updateDetectionPoint)
        {
            try
            {
                if (updateDetectionPoint[0].LensIndex != -1)
                {
                    foreach (var item in nowSelectRecipeDetectionPointList)
                    {
                        DetectionPointList[item].LensIndex = updateDetectionPoint[0].LensIndex;
                    }
                }
                if (updateDetectionPoint[0].CubeIndex != -1)
                {
                    foreach (var item in nowSelectRecipeDetectionPointList)
                    {
                        DetectionPointList[item].CubeIndex = updateDetectionPoint[0].CubeIndex;
                    }
                }
                if (updateDetectionPoint[0].Filter1Index != -1)
                {
                    foreach (var item in nowSelectRecipeDetectionPointList)
                    {
                        DetectionPointList[item].Filter1Index = updateDetectionPoint[0].Filter1Index;
                    }
                }
                if (updateDetectionPoint[0].Filter2Index != -1)
                {
                    foreach (var item in nowSelectRecipeDetectionPointList)
                    {
                        DetectionPointList[item].Filter2Index = updateDetectionPoint[0].Filter2Index;
                    }
                }
                if (updateDetectionPoint[0].Filter3Index != -1)
                {
                    foreach (var item in nowSelectRecipeDetectionPointList)
                    {
                        DetectionPointList[item].Filter3Index = updateDetectionPoint[0].Filter3Index;
                    }
                }
                if (updateDetectionPoint[0].MicroscopeLightValue != -1)
                {
                    foreach (var item in nowSelectRecipeDetectionPointList)
                    {
                        DetectionPointList[item].MicroscopeLightValue = updateDetectionPoint[0].MicroscopeLightValue;
                    }
                }
                if (updateDetectionPoint[0].MicroscopeApertureValue != -1)
                {
                    foreach (var item in nowSelectRecipeDetectionPointList)
                    {
                        DetectionPointList[item].MicroscopeApertureValue = updateDetectionPoint[0].MicroscopeApertureValue;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private DetectionPoint InitialDetectionPoint(ObservableCollection<int> nowSelectRecipeDetectionPointList)
        {
            try
            {
                int selectRecipeIndex = SelectRecipeDetectionPointList[0];
                DetectionPoint editInitDetectionPoint = new DetectionPoint();
                editInitDetectionPoint.LensIndex = DetectionPointList[selectRecipeIndex].LensIndex;
                editInitDetectionPoint.CubeIndex = DetectionPointList[selectRecipeIndex].CubeIndex;
                editInitDetectionPoint.Filter1Index = DetectionPointList[selectRecipeIndex].Filter1Index;
                editInitDetectionPoint.Filter2Index = DetectionPointList[selectRecipeIndex].Filter2Index;
                editInitDetectionPoint.Filter3Index = DetectionPointList[selectRecipeIndex].Filter3Index;
                editInitDetectionPoint.MicroscopeLightValue = DetectionPointList[selectRecipeIndex].MicroscopeLightValue;
                editInitDetectionPoint.MicroscopeApertureValue = DetectionPointList[selectRecipeIndex].MicroscopeApertureValue;
                editInitDetectionPoint.MicroscopePosition = DetectionPointList[selectRecipeIndex].MicroscopePosition;
                editInitDetectionPoint.MicroscopeAberationPosition = DetectionPointList[selectRecipeIndex].MicroscopeAberationPosition;
                editInitDetectionPoint.SubProgramName = DetectionPointList[selectRecipeIndex].SubProgramName;

                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].LensIndex != DetectionPointList[item].LensIndex)
                    {
                        editInitDetectionPoint.LensIndex = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].CubeIndex != DetectionPointList[item].CubeIndex)
                    {
                        editInitDetectionPoint.CubeIndex = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].Filter1Index != DetectionPointList[item].Filter1Index)
                    {
                        editInitDetectionPoint.Filter1Index = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].Filter2Index != DetectionPointList[item].Filter2Index)
                    {
                        editInitDetectionPoint.Filter2Index = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].Filter3Index != DetectionPointList[item].Filter3Index)
                    {
                        editInitDetectionPoint.Filter3Index = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].MicroscopeLightValue != DetectionPointList[item].MicroscopeLightValue)
                    {
                        editInitDetectionPoint.MicroscopeLightValue = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].MicroscopeApertureValue != DetectionPointList[item].MicroscopeApertureValue)
                    {
                        editInitDetectionPoint.MicroscopeApertureValue = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].MicroscopePosition != DetectionPointList[item].MicroscopePosition)
                    {
                        editInitDetectionPoint.MicroscopePosition = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].MicroscopeAberationPosition != DetectionPointList[item].MicroscopeAberationPosition)
                    {
                        editInitDetectionPoint.MicroscopeAberationPosition = -1;
                        break;
                    }
                }
                foreach (int item in nowSelectRecipeDetectionPointList)
                {
                    if (DetectionPointList[selectRecipeIndex].SubProgramName != DetectionPointList[item].SubProgramName)
                    {
                        editInitDetectionPoint.SubProgramName = "-1";
                        break;
                    }
                }

                return editInitDetectionPoint;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ICommand DetectionPointListDoubleClickCommand => new RelayCommand(async () =>
        {
            try
            {
                if (IsRecipeMapCanEdit == false)
                {
                    return;
                }
                int selectRecipeIndex = -1;
                //顯示
                if (SelectRecipeDetectionPointList.Count >= 1)
                {
                    selectRecipeIndex = SelectRecipeDetectionPointList[0];
                }
                if (selectRecipeIndex >= 0)
                {

                    int idxX = DetectionPointList[selectRecipeIndex].IndexX;
                    int idxY = DetectionPointList[selectRecipeIndex].IndexY;
                    var showDie = mainRecipe.DetectRecipe.WaferMap.Dies.FirstOrDefault(die => die.IndexX == idxX && die.IndexY == idxY);
                    Point mapPoint = new Point(showDie.MapTransX,
                                               showDie.MapTransY);

                    recipeMapToPixelScale = MapPointToMapImagePixelScale(RecipeMapDrawSize.X, RecipeMapDrawSize.Y, HomeMapDrawCuttingline);
                    var die00 = mainRecipe.DetectRecipe.WaferMap.Dies[0];
                    var transMapMousePixcelX = 0.0;
                    var transMapMousePixcelY = 0.0;
                    if (die00.MapTransX == die00.OperationPixalX)
                    {
                        transMapMousePixcelX = recipeMapToPixelScale.X * mapPoint.X + RecipeMapDrawSize.X / 2;
                    }
                    else
                    {
                        transMapMousePixcelX = recipeMapToPixelScale.X * (die00.MapTransX + die00.OperationPixalX - mapPoint.X) + RecipeMapDrawSize.X / 2;
                    }

                    if (die00.MapTransY == die00.OperationPixalY)
                    {
                        transMapMousePixcelY = recipeMapToPixelScale.Y * mapPoint.Y + RecipeMapDrawSize.Y / 2;
                    }
                    else
                    {
                        transMapMousePixcelY = recipeMapToPixelScale.Y * (die00.MapTransY + die00.OperationPixalY - mapPoint.Y) + RecipeMapDrawSize.Y / 2;
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

                    ChangeRecipeMappingSelect(nowSelectRange);

                    //還沒對位，MAP點位不能移動
                    if (isRecipeAlignment == false) return;
                    //移動
                    var transPos = transForm.TransPoint(new Point(DetectionPointList[selectRecipeIndex].Position.X + mainRecipe.DetectRecipe.AlignRecipe.OffsetX, DetectionPointList[selectRecipeIndex].Position.Y + mainRecipe.DetectRecipe.AlignRecipe.OffsetY));
                    await machine.MicroDetection.TableMoveToAsync(transPos);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        /// <summary>
        /// 顯示目前平台位置相對應到畫面上的Wafer點位
        /// </summary>
        /// <param name="nowTablePosX"></param>
        /// <param name="nowTablePosY"></param>
        private void ShowNowTablePosInRecipeWaferMapDie(double nowTablePosX, double nowTablePosY, ITransform transForm)
        {
            try
            {
                Point mapPoint = transForm.TransInvertPoint(new Point(nowTablePosX, nowTablePosY));
                recipeMapToPixelScale = MapPointToMapImagePixelScale(RecipeMapDrawSize.X, RecipeMapDrawSize.Y, RecipeMapDrawCuttingline);
                var die00 = mainRecipe.DetectRecipe.WaferMap.Dies[0];
                var transMapMousePixcelX = 0.0;
                var transMapMousePixcelY = 0.0;
                if (die00.MapTransX == die00.OperationPixalX)
                {
                    transMapMousePixcelX = recipeMapToPixelScale.X * mapPoint.X + RecipeMapDrawSize.X / 2;
                }
                else
                {
                    transMapMousePixcelX = recipeMapToPixelScale.X * (die00.MapTransX + die00.OperationPixalX - mapPoint.X) + RecipeMapDrawSize.X / 2;
                }

                if (die00.MapTransY == die00.OperationPixalY)
                {
                    transMapMousePixcelY = recipeMapToPixelScale.Y * mapPoint.Y + RecipeMapDrawSize.Y / 2;
                }
                else
                {
                    transMapMousePixcelY = recipeMapToPixelScale.Y * (die00.MapTransY + die00.OperationPixalY - mapPoint.Y) + RecipeMapDrawSize.Y / 2;
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
                ChangeRecipeMappingSelect(nowSelectRange);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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
                LocateX = offsetPixalX * machineSetting.CamerasPixelTable.X;//1.095
                LocateY = offsetPixalY * machineSetting.CamerasPixelTable.Y;//1.095
            }



        }

        private async void SampleMoveAction(Point pos)
        {
            try
            {
                await machine.MicroDetection.TableMoveToAsync(pos);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            LocateParam locateParam = new LocateParam(0)
            {
                MicroscopeLightValue = LocateParam1.MicroscopeLightValue,
                MicroscopeApertureValue = LocateParam1.MicroscopeApertureValue,
                LensIndex = LocateParam1.LensIndex,
                MicroscopePosition = LocateParam1.MicroscopePosition,
                MicroscopeAberationPosition = LocateParam1.MicroscopePosition,
                CubeIndex = LocateParam1.CubeIndex,
                Filter1Index = LocateParam1.Filter1Index,
                Filter2Index = LocateParam1.Filter2Index,
                Filter3Index = LocateParam1.Filter3Index
            };
            LocateParamList.Clear();
            LocateParamList.Add(locateParam);


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
    public static class DataGridSelectedIndexBehavior
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(DataGridSelectedIndexBehavior),
                new PropertyMetadata(null, OnSelectedItemsChanged));

        public static void SetSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        public static IList GetSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if (e.OldValue is IList oldList)
                {
                    dataGrid.SelectionChanged -= OnDataGridSelectionChanged;
                }

                if (e.NewValue is IList newList)
                {
                    dataGrid.SelectionChanged += OnDataGridSelectionChanged;
                }
            }
        }

        private static void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                var selectedItems = GetSelectedItems(dataGrid);
                if (selectedItems == null) return;

                selectedItems.Clear();
                foreach (var item in dataGrid.SelectedItems)
                {
                    selectedItems.Add(dataGrid.Items.IndexOf(item));
                }
            }
        }
    }
    public static class DataGridSelectedItemsBehavior
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(DataGridSelectedItemsBehavior),
                new PropertyMetadata(null, OnSelectedItemsChanged));

        public static void SetSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        public static IList GetSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if (e.OldValue is IList oldList)
                {
                    dataGrid.SelectionChanged -= OnDataGridSelectionChanged;
                }

                if (e.NewValue is IList newList)
                {
                    dataGrid.SelectionChanged += OnDataGridSelectionChanged;
                }
            }
        }

        private static void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                IList selectedItems = GetSelectedItems(dataGrid);
                if (selectedItems == null) return;

                selectedItems.Clear();
                foreach (var item in dataGrid.SelectedItems)
                {
                    selectedItems.Add(item);
                }
            }
        }
    }
}
