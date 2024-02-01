using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.UserControls;
using YuanliCore.AffineTransform;
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
        private ObservableCollection<WaferUIData> loadPort2Wafers = new ObservableCollection<WaferUIData>();
        private ObservableCollection<DetectionPoint> detectionPointList = new ObservableCollection<DetectionPoint>();

        private BitmapSource locateSampleImage1;
        private BitmapSource locateSampleImage2;
        private BitmapSource locateSampleImage3;

        private LocateParam locateParam1 = new LocateParam(101);//Locate pattern 從100號開始
        private LocateParam locateParam2 = new LocateParam(102);//Locate pattern 從100號開始
        private LocateParam locateParam3 = new LocateParam(103);//Locate pattern 從100號開始
        private ObservableCollection<ROIShape> drawings = new ObservableCollection<ROIShape>();
        private ObservableCollection<ROIShape> mapDrawings = new ObservableCollection<ROIShape>();
        private Action<CogMatcher> sampleFind;
        private LocateMode selectMode;
        private double alignOffsetX, alignOffsetY;
        private ExistStates testStates;
        private ITransform transForm; //紀錄 從設計座標轉換成對位後座標的 公式
        private int moveIndexX, moveIndexY, detectionIndexX, detectionIndexY;
        private bool isLocate;
        private int selectDetectionPointList;
        private bool isMainRecipePageSelect, isMainSettingPageSelect;
        private bool isLoadwaferPageSelect, isLocatePageSelect, isDetectionPageSelect;
        private bool isLoadwaferOK, isLocateOK, isDetectionOK; //判斷各設定頁面是否滿足條件 ，  才能切換到下一頁
        private System.Windows.Point mousePixcel;
        private ROIShape selectShape;
        private bool isLoadwaferComplete, isLocateComplete, isDetectionComplete;

        /// <summary>
        /// 切換到 主畫面Recipe 設定頁面
        /// </summary>
        public bool IsMainRecipePageSelect
        {
            get
            {
                if (isMainRecipePageSelect)
                    LoadRecipePage();
                else if (!isMainRecipePageSelect)
                    UnLoadRecipePage();
                return isMainRecipePageSelect;
            }
            set => SetValue(ref isMainRecipePageSelect, value);
        }
        /// <summary>
        /// 切換到 主畫面設定頁面
        /// </summary>
        public bool IsMainSettingPageSelect
        {
            get
            {
                if (isMainSettingPageSelect)
                    LoadSettingPage();
                else if (!isMainSettingPageSelect)
                    UnLoadSettingPage();
                return isMainSettingPageSelect;
            }
            set => SetValue(ref isMainSettingPageSelect, value);
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
                if (isDetectionPageSelect)
                    SetLocateParamToRecipe();
                return isDetectionPageSelect;
            }
            set => SetValue(ref isDetectionPageSelect, value);
        }

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
        public ObservableCollection<WaferUIData> LoadPort2Wafers { get => loadPort2Wafers; set => SetValue(ref loadPort2Wafers, value); }
        public ObservableCollection<DetectionPoint> DetectionPointList { get => detectionPointList; set => SetValue(ref detectionPointList, value); }
        public int SelectDetectionPointList { get => selectDetectionPointList; set => SetValue(ref selectDetectionPointList, value); }
        public LocateParam LocateParam1 { get => locateParam1; set => SetValue(ref locateParam1, value); }
        public LocateParam LocateParam2 { get => locateParam2; set => SetValue(ref locateParam2, value); }
        public LocateParam LocateParam3 { get => locateParam3; set => SetValue(ref locateParam3, value); }
        public LocateMode SelectMode { get => selectMode; set => SetValue(ref selectMode, value); }

        public ExistStates TestStates { get => testStates; set => SetValue(ref testStates, value); }

        public double AlignOffsetX { get => alignOffsetX; set => SetValue(ref alignOffsetX, value); }
        public double AlignOffsetY { get => alignOffsetY; set => SetValue(ref alignOffsetY, value); }
        //判斷有沒有做過 對位，主要卡控所有的座標都要建立在對位後的 才會是對的
        public bool IsLocate { get => isLocate; set => SetValue(ref isLocate, value); }
        public Action<CogMatcher> SampleFind { get => sampleFind; set => SetValue(ref sampleFind, value); }



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

        public ObservableCollection<ROIShape> MapDrawings { get => mapDrawings; set => SetValue(ref mapDrawings, value); }


        public ICommand LoadRecipePageCommand => new RelayCommand(() =>
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

        //Recipe進入會執行
        private void LoadRecipePage()
        {
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

            //將檢測座標存入recipe
            mainRecipe.DetectRecipe.DetectionPoints = DetectionPointList;
        }
        //Recipe進入會執行
        private void LoadSettingPage()
        {

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
            IsLoadwaferPageSelect = true;
            IsLocatePageSelect = false;

        }

        public ICommand TestLoadRecipePageCommand => new RelayCommand(() =>
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


        public ICommand LoadWaferCommand => new RelayCommand(() =>
        {

            LoadPort2Wafers.Add(
                new WaferUIData
                {
                    WaferStates = ExistStates.Exist
                });
            LoadPort2Wafers.Add(
          new WaferUIData
          {
              WaferStates = ExistStates.Exist
          });


            LoadPort2Wafers[4].WaferStates = ExistStates.Error;

            TestStates = ExistStates.Exist;
        });




        public ICommand MappingEditCommand => new RelayCommand(() =>
       {
           try
           {
               BincodeSettingWindow settingWindow = new BincodeSettingWindow();
               settingWindow.ShowDialog();



           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
           }

       });
        public ICommand LoadMappingCommand => new RelayCommand(() =>
        {
            MapImage = new WriteableBitmap(15000, 15000, 96, 96, machine.MicroDetection.Camera.PixelFormat, null);
            ClearMapShapeAction.Execute(MapDrawings);

            List<Die> dielist = new List<Die>();
            for (int x = 1; x <= 100; x++)
            {
                for (int y = 1; y <= 100; y++)
                {
                    var temp = new Die
                    {
                        IndexX = x,
                        IndexY = y,
                        PosX = 1000 + x * 100,
                        PosY = 1000 + y * 100,
                        DieSize = new Size(40, 40)
                    };

                    dielist.Add(temp);
                }
            }

            //模擬  編輯完MAP圖後 資料存回mainRecipe內
            mainRecipe.DetectRecipe.WaferMap = new SinfWaferMapping("");
            mainRecipe.DetectRecipe.WaferMap.Dies = dielist.ToArray();

            //將MAP圖資訊 轉換成顯示資訊
            mainRecipe.DetectRecipe.WaferMap.ReadWaferFile("");
            foreach (var item in mainRecipe.DetectRecipe.WaferMap.Dies)
            {

                var center = new ROIRotatedRect
                {
                    X = item.PosX,
                    Y = item.PosY,
                    LengthX = item.DieSize.Width,
                    LengthY = item.DieSize.Height,
                    StrokeThickness = 2,
                    Stroke = System.Windows.Media.Brushes.LightGreen,
                    IsInteractived = false,
                    ToolTip = $"X={item.IndexX} , Y={item.IndexY}",
                    IsCenterShow = false
                };
                AddMapShapeAction.Execute(center);
            }
        });

        public ICommand EditMappingCommand => new RelayCommand<string>(async key =>
        {
            try
            {
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

                //        mainRecipe.DetectRecipe.WaferMap = new SinfWaferMapping("");
                //        mainRecipe.DetectRecipe.WaferMap.Dies = m_Sinf.Dies.ToArray();

                //        ShowMappingDrawings(mainRecipe.DetectRecipe.WaferMap.Dies, mainRecipe.DetectRecipe.WaferMap.ColumnCount, mainRecipe.DetectRecipe.WaferMap.RowCount, 3000);

                //    }
                //}
                //else
                //{
                //    SINF_Path = "";
                //}



                SINFMapGenerateWindow sINFMapGenerateWindow = new SINFMapGenerateWindow();
                sINFMapGenerateWindow.ShowDialog();
                if (sINFMapGenerateWindow.Sinf.Dies != null && sINFMapGenerateWindow.Sinf.Dies.Length > 0)
                {
                    mainRecipe.DetectRecipe.WaferMap.Dies = sINFMapGenerateWindow.Sinf.Dies.ToArray();
                    ShowMappingDrawings(mainRecipe.DetectRecipe.WaferMap.Dies, mainRecipe.DetectRecipe.WaferMap.ColumnCount, mainRecipe.DetectRecipe.WaferMap.RowCount, 3000);
                }


            }
            catch (Exception)
            {

                throw;
            }
        });


        public void ShowMappingDrawings(Die[] dies, int columnCount, int rowCount, int mappingImageDrawSize)
        {
            try
            {
                ClearMapShapeAction.Execute(true);
                double showSize_X;
                double showSize_Y;
                double dieSizeX = dies[0].DieSize.Width;
                double dieSizeY = dies[0].DieSize.Height;
                double offsetDraw = mappingImageDrawSize / 150;
                double scale = 1;
                scale = Math.Max((columnCount + 2.5) * dieSizeX, (rowCount + 2.5) * dieSizeY) / (mappingImageDrawSize - offsetDraw * 2);
                double strokeThickness = 1;
                double crossThickness = 1;
                strokeThickness = Math.Min(dieSizeX / 2 / scale, dieSizeX / 2 / scale) / 4;
                crossThickness = Math.Min(dieSizeX / 2 / scale, dieSizeX / 2 / scale) / 4;

                showSize_X = (columnCount * dieSizeX) / (mappingImageDrawSize - offsetDraw * 2);
                showSize_Y = (rowCount * dieSizeY) / (mappingImageDrawSize - offsetDraw * 2);
                foreach (var item in dies)
                {
                    Brush drawStroke = Brushes.Black;
                    Brush drawFill = Brushes.Gray;
                    //判斷要用什麼顏色
                    //foreach (var item2 in collection)
                    //{

                    //}
                    AddMapShapeAction.Execute(new ROIRotatedRect
                    {
                        Stroke = drawStroke,
                        StrokeThickness = strokeThickness,
                        Fill = drawFill,
                        X = (item.PosX + item.DieSize.Width * 0.5) / showSize_X + offsetDraw,
                        Y = (item.PosY + item.DieSize.Height * 0.5) / showSize_Y + offsetDraw,
                        LengthX = item.DieSize.Width / 2.5 / showSize_X,
                        LengthY = item.DieSize.Height / 2.5 / showSize_Y,
                        IsInteractived = true,
                        IsMoveEnabled = false,
                        IsResizeEnabled = false,
                        IsRotateEnabled = false,
                        CenterCrossLength = crossThickness,
                        CenterCrossBrush = drawFill,
                        ToolTip = "X:" + item.IndexX + " Y:" + item.IndexY + " X:" + item.PosX + " Y:" + item.PosY
                    });
                }

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
                //經過這個方法後，會將map中所有的Die  Index座標轉換成當下樣本建立時的實際機台座標(pattern中心)
                SetLocateParamToRecipe();
                //將die的map座標都轉換成 實際機台座標(解決片子更換後位置不對的問題 )
                transForm = await machine.MicroDetection.Alignment(mainRecipe.DetectRecipe.AlignRecipe);

                //將所有的Die 轉換成實際片子座標(如果建立樣本時的wafer 與 LocateRun 是一起建立的  那座標會一樣 ，主要Locate目的是針對換wafer以後要重新對位
                //如果有需要調整檢測座標 ，需要重新做對位  ，對位後會重新建立新的map全部die座標 ，為了給後續檢測座標設定使用 

                //依序轉換完對位前座標  ，轉換成對位後座標 塞回機械座標
                foreach (Die die in mainRecipe.DetectRecipe.WaferMap.Dies)
                {
                    Point pos = transForm.TransPoint(new Point(die.PosX, die.PosY));
                    die.PosX = pos.X;
                    die.PosY = pos.Y;
                }


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
                //挑選出 對應index 的Die
                YuanliCore.Data.Die[] dies = mainRecipe.DetectRecipe.WaferMap.Dies;
                YuanliCore.Data.Die die = dies.Where(d => d.IndexX == MoveIndexX && d.IndexY == MoveIndexY).FirstOrDefault();
                if (die == null) throw new Exception("");
                //設計座標轉換對位後座標
                Point transPos = transForm.TransPoint(new Point(die.PosX, die.PosY));

                await machine.MicroDetection.TableMoveToAsync(transPos);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        public ICommand SelectMappingDieCommand => new RelayCommand(() =>
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
                        this.selectShape.Stroke = System.Windows.Media.Brushes.LightGreen;

                    }

                    tempselectShape.Stroke = System.Windows.Media.Brushes.Red;
                    this.selectShape = tempselectShape;

                    //從點選的ShapeROI  找出對應的die
                    int listIndex = MapDrawings.IndexOf(selectShape);
                    YuanliCore.Data.Die die = mainRecipe.DetectRecipe.WaferMap.Dies[listIndex];
                    MoveIndexX = die.IndexX;
                    MoveIndexY = die.IndexY;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }


        });
        public ICommand AddDetectionCommand => new RelayCommand(() =>
        {
            try
            {
                //從點選的ShapeROI  找出對應的die
                int listIndex = MapDrawings.IndexOf(selectShape);
                YuanliCore.Data.Die die = mainRecipe.DetectRecipe.WaferMap.Dies[listIndex];

                DetectionPoint point = new DetectionPoint();
                point.IndexX = die.IndexX;
                point.IndexY = die.IndexY;
                point.OffsetX = 0;
                point.OffsetY = 0;
                point.Position = new Point(die.PosX, die.PosY);
                DetectionPointList.Add(point);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
        });

        public ICommand RemoveDetectionCommand => new RelayCommand(() =>
        {



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
                    Size = 5,
                    StrokeThickness = 2,
                    Stroke = System.Windows.Media.Brushes.Red,
                    IsInteractived = false
                };
                AddShapeAction.Execute(center);

            }



        }
        private void SetLocateParamToRecipe()
        {
            List<LocateParam> datas = new List<LocateParam>();

            datas.Add(LocateParam1);
            datas.Add(LocateParam2);
            datas.Add(LocateParam3);

            //需要做出一個轉換矩陣 對應index 與 機台座標
            var index1 = new Point(LocateParam1.IndexX, LocateParam1.IndexY);
            var index2 = new Point(LocateParam2.IndexX, LocateParam2.IndexY);
            var index3 = new Point(LocateParam3.IndexX, LocateParam3.IndexY);

            var pos1 = new Point(LocateParam1.DesignPositionX, LocateParam1.DesignPositionY);
            var pos2 = new Point(LocateParam2.DesignPositionX, LocateParam2.DesignPositionY);
            var pos3 = new Point(LocateParam3.DesignPositionX, LocateParam3.DesignPositionY);

            var indexs = new Point[] { index1, index2, index3 };
            var poss = new Point[] { pos1, pos2, pos3 };
            var transform = new CogAffineTransform(indexs, poss);

            //依序轉換完INDEX  塞回機械座標
            foreach (YuanliCore.Data.Die die in mainRecipe.DetectRecipe.WaferMap.Dies)
            {
                Point pos = transform.TransPoint(new Point(die.IndexX, die.IndexY));
                die.PosX = pos.X;
                die.PosY = pos.Y;
            }


            mainRecipe.DetectRecipe.AlignRecipe.AlignmentMode = SelectMode;
            mainRecipe.DetectRecipe.AlignRecipe.OffsetX = AlignOffsetX;
            mainRecipe.DetectRecipe.AlignRecipe.OffsetY = AlignOffsetY;
            mainRecipe.DetectRecipe.AlignRecipe.FiducialDatas = datas.ToArray();

        }

        private void SetRecipeToLocateParam(DetectionRecipe detectionRecipe)
        {
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
    }

    public class WaferUIData : INotifyPropertyChanged
    {
        private ExistStates waferStates;
        public ExistStates WaferStates { get => waferStates; set => SetValue(ref waferStates, value); }//{ get; set; }

        private int sNWidth;
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
