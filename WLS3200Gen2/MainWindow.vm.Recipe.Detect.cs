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

        private BincodeInfo[] mapbincodeList;
        private BincodeInfo[] mapbincodeListHome;
        private int selectList, column, row;
        private Die[] dieArray;
        private Die[] dieArrayHome;
        private WriteableBitmap mappingHomeTable;
        private WriteableBitmap mappingTable;
        private Action<MappingOperate> createHome;
        private Action<MappingOperate> create;
        private Action<int, int, Brush, Brush> setHomeRectangle;
        private Action<int, int, Brush, Brush> setRectangle;
        private System.Windows.Point newHomeMapMousePixcel, newMapMousePixcel;
        private List<RectangleInfo> rectanglesHome = new List<RectangleInfo>();
        private List<RectangleInfo> rectangles = new List<RectangleInfo>();
        private Point homeMapDrawSize, recipeMapDrawSize;
        private double homeMapDrawCuttingline = 3000, recipeMapDrawCuttingline = 3000;
        private Point homeMapToPixelScale, recipeMapToPixelScale;
        /// <summary>
        /// Map顯示Die的狀況
        /// </summary>
        private List<RectangleInfo> tempRecipeRectangles = new List<RectangleInfo>();
        /// <summary>
        /// 紀錄單片下BinCode後的狀況
        /// </summary>
        private List<RectangleInfo> tempHomeLogAssignRectangles = new List<RectangleInfo>();
        private ObservableCollection<RectangleInfo> selectRectangles = new ObservableCollection<RectangleInfo>();
        private IEnumerable<RectangleInfo> selectHomeRectangle;
        private IEnumerable<RectangleInfo> selectRecipeRectangle;

        public Action<int, int, Brush, Brush> SetHomeRectangle { get => setHomeRectangle; set => SetValue(ref setHomeRectangle, value); }
        public Action<int, int, Brush, Brush> SetRectangle { get => setRectangle; set => SetValue(ref setRectangle, value); }
        public WriteableBitmap MappingHomeTable { get => mappingHomeTable; set => SetValue(ref mappingHomeTable, value); }
        public WriteableBitmap MappingTable { get => mappingTable; set => SetValue(ref mappingTable, value); }
        public BincodeInfo[] MapBincodeListHome { get => mapbincodeListHome; set => SetValue(ref mapbincodeListHome, value); }
        public BincodeInfo[] MapBincodeList { get => mapbincodeList; set => SetValue(ref mapbincodeList, value); }
        public Die[] DieArrayHome { get => dieArrayHome; set => SetValue(ref dieArrayHome, value); }
        public Die[] DieArray { get => dieArray; set => SetValue(ref dieArray, value); }
        public System.Windows.Point NewHomeMapMousePixcel { get => newHomeMapMousePixcel; set => SetValue(ref newHomeMapMousePixcel, value); }
        public System.Windows.Point NewMapMousePixcel { get => newMapMousePixcel; set => SetValue(ref newMapMousePixcel, value); }
        public List<RectangleInfo> RectanglesHome
        {
            get => rectanglesHome;
            set => SetValue(ref rectanglesHome, value);
        }
        public List<RectangleInfo> Rectangles
        {
            get => rectangles;
            set => SetValue(ref rectangles, value);
        }
        public ObservableCollection<RectangleInfo> SelectRectangles
        {
            get => selectRectangles;
            set => SetValue(ref selectRectangles, value);
        }
        public Action<MappingOperate> MappingHomeOp { get => createHome; set => SetValue(ref createHome, value); }
        public Action<MappingOperate> MappingRecipeOp { get => create; set => SetValue(ref create, value); }
        public Point HomeMapDrawSize { get => homeMapDrawSize; set => SetValue(ref homeMapDrawSize, value); }
        public Point RecipeMapDrawSize { get => recipeMapDrawSize; set => SetValue(ref recipeMapDrawSize, value); }
        public double HomeMapDrawCuttingline { get => homeMapDrawCuttingline; set => SetValue(ref homeMapDrawCuttingline, value); }
        public double RecipeMapDrawCuttingline { get => recipeMapDrawCuttingline; set => SetValue(ref recipeMapDrawCuttingline, value); }


        private (Die[] dice, BincodeInfo[] mapBincodes) CreateDummyBincode(int col, int row)
        {
            List<BincodeInfo> bincodeInfos = new List<BincodeInfo>();
            List<Die> dies = new List<Die>();
            object lockObj = new object();
            int width = col; // 矩形寬度
            int height = row; // 矩形高度
            int centerX = col / 2; // 中心點 X 座標
            int centerY = row / 2; // 中心點 Y 座標
            int radius = col / 2; // 圓形半徑
            int radiusSquared = radius * radius; // 半徑的平方

            Parallel.For(0, col, x =>
            {
                Parallel.For(0, row, y =>
                {

                    // 計算矩形中心點到圓心的距離的平方
                    int dx = x - centerX;
                    int dy = y - centerY;
                    int distanceSquared = dx * dx + dy * dy;

                    // 如果距離的平方小於等於半徑的平方，則在圓形內
                    if (distanceSquared <= radiusSquared)
                    {
                        Die die = new Die();
                        die.IndexX = x;
                        die.IndexY = y;
                        if (x == 7)
                            die.BinCode = "A1";
                        else
                            die.BinCode = "A0";

                        lock (lockObj)
                        {
                            dies.Add(die);
                        }
                    }

                });

            });


            var info1 = new BincodeInfo();
            info1.Code = "A0";
            info1.Color = Brushes.Green;
            bincodeInfos.Add(info1);

            var info2 = new BincodeInfo();
            info2.Code = "A1";
            info2.Color = Brushes.Red;
            bincodeInfos.Add(info2);

            return (dies.ToArray(), bincodeInfos.ToArray());


        }
        /// <summary>
        /// Home端畫面互動
        /// </summary>
        public ICommand DoubleClickHomeNewMappingDieCommand => new RelayCommand(async () =>
        {
            try
            {
                //是否在運行中的Micro檢查
                if (isRunningMicroDetection == false) return;

                Rectangle nowSelectRange = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 5,
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(nowSelectRange, NewHomeMapMousePixcel.X);
                Canvas.SetTop(nowSelectRange, NewHomeMapMousePixcel.Y);
                ChangeHomeMappingSelect(nowSelectRange);

                //移動
                Rect rect = new Rect(Canvas.GetLeft(nowSelectRange), Canvas.GetTop(nowSelectRange), nowSelectRange.Width, nowSelectRange.Height);
                RectangleInfo tempselectRects = RectanglesHome.Where(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                   || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight)).FirstOrDefault();

                if (tempselectRects != null)
                {
                    int index = RectanglesHome.IndexOf(tempselectRects);
                    var moveDie = mainRecipe.DetectRecipe.WaferMap.Dies.FirstOrDefault(n => n.IndexX == RectanglesHome[index].Col && n.IndexY == RectanglesHome[index].Row);
                    var transPos = machine.MicroDetection.TransForm.TransPoint(new Point(moveDie.MapTransX, moveDie.MapTransY));
                    await machine.MicroDetection.TableMoveToAsync(transPos);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        private void ChangeHomeMappingSelect(Rectangle nowSelectRange)
        {
            try
            {
                Rect rect = new Rect(Canvas.GetLeft(nowSelectRange), Canvas.GetTop(nowSelectRange), nowSelectRange.Width, nowSelectRange.Height);
                IEnumerable<RectangleInfo> tempselectRects = new List<RectangleInfo>();
                tempselectRects = RectanglesHome.Where(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                   || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight));
                if (tempselectRects != null)
                {
                    if (selectHomeRectangle != null)
                    {
                        foreach (var item in selectHomeRectangle)
                        {
                            int index = RectanglesHome.IndexOf(item);
                            if (index >= 0)
                            {
                                SetHomeRectangle?.Invoke(RectanglesHome[index].Col, RectanglesHome[index].Row, tempHomeLogAssignRectangles[index].Fill, tempRecipeRectangles[index].Fill);
                            }
                        }
                    }
                    foreach (var item in tempselectRects)
                    {
                        int index = RectanglesHome.IndexOf(item);
                        if (index >= 0)
                        {
                            var ss = RectanglesHome[index].Fill;
                            SetHomeRectangle?.Invoke(RectanglesHome[index].Col, RectanglesHome[index].Row, tempHomeLogAssignRectangles[index].Fill, Brushes.Red);
                        }
                    }
                    selectHomeRectangle = tempselectRects;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void ChangeRecipeMappingSelect(Rectangle nowSelectRange)
        {
            try
            {
                Rect rect = new Rect(Canvas.GetLeft(nowSelectRange), Canvas.GetTop(nowSelectRange), nowSelectRange.Width, nowSelectRange.Height);
                IEnumerable<RectangleInfo> tempselectRects = new List<RectangleInfo>();
                tempselectRects = Rectangles.Where(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                   || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight));
                if (tempselectRects != null)
                {
                    if (this.selectRecipeRectangle != null)
                    {
                        foreach (var item in selectRecipeRectangle)
                        {
                            int index = Rectangles.IndexOf(item);
                            var index2 = tempRecipeRectangles
                                .Select((rect2, idx) => new { rect2, idx })
                                .FirstOrDefault(x => x.rect2.Col == item.Col && x.rect2.Row == item.Row).idx;

                            if (index >= 0)
                            {
                                SetRectangle?.Invoke(Rectangles[index].Col, Rectangles[index].Row, Rectangles[index].Fill, tempRecipeRectangles[index2].Fill);
                            }
                        }
                    }
                    foreach (var item in tempselectRects)
                    {
                        int index = Rectangles.IndexOf(item);
                        if (index >= 0)
                        {
                            SetRectangle?.Invoke(Rectangles[index].Col, Rectangles[index].Row, Rectangles[index].Fill, Brushes.Red);
                        }
                    }
                    selectRecipeRectangle = tempselectRects;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Recipe端畫面互動
        /// </summary>
        public ICommand DoubleClickRecipeNewMappingDieCommand => new RelayCommand(async () =>
        {
            try
            {
                Rectangle nowSelectRange = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 5,
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(nowSelectRange, NewMapMousePixcel.X);
                Canvas.SetTop(nowSelectRange, NewMapMousePixcel.Y);

                ChangeRecipeMappingSelect(nowSelectRange);

                //還沒對位，MAP點位不能移動
                if (isRecipeAlignment == false) return;
                Rect rect = new Rect(Canvas.GetLeft(nowSelectRange), Canvas.GetTop(nowSelectRange), nowSelectRange.Width, nowSelectRange.Height);
                RectangleInfo tempselectRects = Rectangles.Where(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                   || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight)).FirstOrDefault();

                //移動
                if (tempselectRects != null)
                {
                    var nowMoveDie = mainRecipe.DetectRecipe.WaferMap.Dies.Where(n => n.IndexX == tempselectRects.Col && n.IndexY == tempselectRects.Row).FirstOrDefault();
                    var transPos = transForm.TransPoint(new Point(nowMoveDie.MapTransX, nowMoveDie.MapTransY));
                    await machine.MicroDetection.TableMoveToAsync(transPos);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });


        public ICommand TestLoadRecipePageCommand => new RelayCommand(() =>
        {
            try
            {
                //從Recipe 帶值
                //    MapBincodeList= mainRecipe.DetectRecipe.BincodeList.ToArray();
                //     DieArray = mainRecipe.DetectRecipe.WaferMap.Dies;

                var dummy = CreateDummyBincode(50, 50);//建出假資料
                MapBincodeListHome = dummy.mapBincodes;
                MapBincodeList = dummy.mapBincodes;
                DieArrayHome = dummy.dice;
                DieArray = dummy.dice;

                List<BincodeInfo> bincodeInfos = new List<BincodeInfo>();
                var info1 = new BincodeInfo();
                info1.Code = "000";
                info1.Color = Brushes.Green;
                bincodeInfos.Add(info1);
                var info2 = new BincodeInfo();
                info2.Code = "099";
                info2.Color = Brushes.Red;
                bincodeInfos.Add(info2);
                MapBincodeListHome = bincodeInfos.ToArray();
                MapBincodeList = bincodeInfos.ToArray();
                DieArrayHome = mainRecipe.DetectRecipe.WaferMap.Dies;// dummy.dice;
                DieArray = mainRecipe.DetectRecipe.WaferMap.Dies;// dummy.dice;
                MappingRecipeOp?.Invoke(MappingOperate.Create); //產生圖片
                tempRecipeRectangles = new List<RectangleInfo>();
                foreach (var item in Rectangles)
                {
                    tempRecipeRectangles.Add(new RectangleInfo(item.CenterX, item.CenterY, item.Width, item.Height)
                    {
                        Col = item.Col,
                        Row = item.Row,
                        Fill = item.Fill
                    });
                    tempHomeLogAssignRectangles.Add(new RectangleInfo(item.CenterX, item.CenterY, item.Width, item.Height)
                    {
                        Col = item.Col,
                        Row = item.Row,
                        Fill = item.Fill
                    });
                }
                selectHomeRectangle = null;
                selectRecipeRectangle = null;

                ShowDetectionRecipeNewMapImgae(mainRecipe.DetectRecipe);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });

        public ICommand TestDectCommand => new RelayCommand(() =>
        {
            int column = 50;
            int row = 60;
            SetRectangle?.Invoke(column, row, Brushes.Coral, Brushes.OrangeRed);


        });


        public ICommand TestDect2Command => new RelayCommand(() =>
        {

            MappingRecipeOp?.Invoke(MappingOperate.Clear);
        });
        public ICommand TestDect3Command => new RelayCommand(() =>
        {
            MappingRecipeOp?.Invoke(MappingOperate.Fit);
        });


        /// <summary>
        /// 重新設定檢查點資訊
        /// </summary>
        public void ResetDetectionRunningPointList()
        {
            try
            {
                if (DetectionHomePointList == null)
                {
                    DetectionHomePointList = new ObservableCollection<DetectionPoint>();
                }
                DetectionHomePointList.Clear();
                foreach (var item in DetectionPointList)
                {
                    DetectionHomePointList.Add(new DetectionPoint()
                    {
                        IndexX = item.IndexX,
                        IndexY = item.IndexY,
                        Position = item.Position,
                        Code = "",
                        LensIndex = item.LensIndex,
                        CubeIndex = item.CubeIndex,
                        Filter1Index = item.Filter1Index,
                        Filter2Index = item.Filter2Index,
                        Filter3Index = item.Filter3Index,
                        MicroscopeLightValue = item.MicroscopeLightValue,
                        MicroscopeApertureValue = item.MicroscopeApertureValue,
                        MicroscopePosition = item.MicroscopePosition,
                        MicroscopeAberationPosition = item.MicroscopeAberationPosition
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 重新設定Map暫存資訊
        /// </summary>
        private void ResetTempAssign()
        {
            try
            {
                tempHomeLogAssignRectangles = new List<RectangleInfo>();

                foreach (var item in RectanglesHome)
                {
                    tempHomeLogAssignRectangles.Add(new RectangleInfo(item.CenterX, item.CenterY, item.Width, item.Height)
                    {
                        Col = item.Col,
                        Row = item.Row,
                        Fill = item.Fill
                    });
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 顯示Home端Map圖
        /// </summary>
        /// <param name="detectionRecipe"></param>
        public void ShowHomeNewMapImage(DetectionRecipe detectionRecipe)
        {
            try
            {
                List<BincodeInfo> bincodeInfos = new List<BincodeInfo>();
                if (detectionRecipe.BincodeList != null)
                {
                    foreach (var item in detectionRecipe.BincodeList)
                    {
                        bincodeInfos.Add(new BincodeInfo() { Code = item.Code, Color = item.Color, Assign = item.Assign, Describe = item.Describe });
                    }
                }
                if (bincodeInfos.Count <= 0)
                {
                    var info1 = new BincodeInfo();
                    info1.Code = "000";
                    info1.Color = Brushes.Green;
                    bincodeInfos.Add(info1);
                    var info2 = new BincodeInfo();
                    info2.Code = "099";
                    info2.Color = Brushes.Red;
                    bincodeInfos.Add(info2);
                    detectionRecipe.BincodeList = bincodeInfos;
                }


                MapBincodeListHome = bincodeInfos.ToArray();
                DieArrayHome = detectionRecipe.WaferMap.Dies;// dummy.dice;
                MappingHomeOp?.Invoke(MappingOperate.Create); //產生圖片

                tempRecipeRectangles = new List<RectangleInfo>();
                tempHomeLogAssignRectangles = new List<RectangleInfo>();

                foreach (var item in RectanglesHome)
                {
                    tempRecipeRectangles.Add(new RectangleInfo(item.CenterX, item.CenterY, item.Width, item.Height)
                    {
                        Col = item.Col,
                        Row = item.Row,
                        Fill = item.Fill
                    });
                    tempHomeLogAssignRectangles.Add(new RectangleInfo(item.CenterX, item.CenterY, item.Width, item.Height)
                    {
                        Col = item.Col,
                        Row = item.Row,
                        Fill = item.Fill
                    });
                }
                foreach (var item in detectionRecipe.DetectionPoints)
                {
                    Die die = detectionRecipe.WaferMap.Dies.Where(d => d.IndexX == item.IndexX && d.IndexY == item.IndexY).FirstOrDefault();
                    if (die != null)
                    {
                        var result = tempRecipeRectangles
                         .Select((rect, idx) => new { rect, idx })
                         .FirstOrDefault(x => x.rect.Col == die.IndexX && x.rect.Row == die.IndexY);
                        tempRecipeRectangles[result.idx].Fill = Brushes.Yellow;
                    }
                }
                selectHomeRectangle = null;
                selectRecipeRectangle = null;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 重新顯示Home端Map圖Detection點位
        /// </summary>
        /// <param name="detectionRecipe"></param>
        public void ShowDetectionHomeNewMapImgae(DetectionRecipe detectionRecipe)
        {
            try
            {
                if (detectionRecipe.DetectionPoints == null) return;
                MappingHomeOp?.Invoke(MappingOperate.Clear); //清除劃過的圖片
                foreach (var item in detectionRecipe.DetectionPoints)
                {
                    Die die = detectionRecipe.WaferMap.Dies.Where(d => d.IndexX == item.IndexX && d.IndexY == item.IndexY).FirstOrDefault();
                    if (die != null)
                    {
                        HomeNewMapAssignDieColorChange(die, null, null);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// HomeMap變色，更改LogBinCode顏色
        /// </summary>
        /// <param name="changeDie"></param>
        /// <param name="fill"></param>
        /// <param name="stroke"></param>
        private void HomeNewMapAssignDieColorChange(Die changeDie, Brush fill, Brush stroke)
        {
            try
            {
                var result = tempRecipeRectangles
                                  .Select((rect, idx) => new { rect, idx })
                                  .FirstOrDefault(x => x.rect.Col == changeDie.IndexX && x.rect.Row == changeDie.IndexY);
                if (result != null)
                {
                    //若是null就是不用Assign，拿現在的狀態
                    if (fill == null)
                    {
                        fill = tempHomeLogAssignRectangles[result.idx].Fill;
                    }
                    else
                    {
                        tempHomeLogAssignRectangles[result.idx].Fill = fill;
                    }
                    //若是null就是要顯示原本Recipe，不然其實就是紅色選擇到的
                    if (stroke == null)
                    {
                        stroke = tempRecipeRectangles[result.idx].Fill;
                    }
                    SetHomeRectangle?.Invoke(RectanglesHome[result.idx].Col, RectanglesHome[result.idx].Row, fill, stroke);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 顯示Recipe端Map圖
        /// </summary>
        /// <param name="detectionRecipe"></param>
        public void ShowRecipeNewMapImage(DetectionRecipe detectionRecipe)
        {
            try
            {
                List<BincodeInfo> bincodeInfos = new List<BincodeInfo>();
                if (detectionRecipe.BincodeList != null)
                {
                    foreach (var item in detectionRecipe.BincodeList)
                    {
                        bincodeInfos.Add(new BincodeInfo() { Code = item.Code, Color = item.Color, Assign = item.Assign, Describe = item.Describe });
                    }

                }
                if (bincodeInfos.Count <= 0)
                {
                    var info1 = new BincodeInfo();
                    info1.Code = "000";
                    info1.Color = Brushes.Green;
                    bincodeInfos.Add(info1);
                    var info2 = new BincodeInfo();
                    info2.Code = "099";
                    info2.Color = Brushes.Red;
                    bincodeInfos.Add(info2);
                }
                MapBincodeList = bincodeInfos.ToArray();
                DieArray = detectionRecipe.WaferMap.Dies;// dummy.dice;
                MappingRecipeOp?.Invoke(MappingOperate.Create); //產生圖片
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 重新顯示Recipe端Map圖Detection點位
        /// </summary>
        /// <param name="detectionRecipe"></param>
        public void ShowDetectionRecipeNewMapImgae(DetectionRecipe detectionRecipe)
        {
            try
            {
                if (detectionRecipe.DetectionPoints == null) return;
                MappingRecipeOp?.Invoke(MappingOperate.Clear); //清除劃過的圖片
                foreach (var item in detectionRecipe.DetectionPoints)
                {
                    Die die = detectionRecipe.WaferMap.Dies.Where(d => d.IndexX == item.IndexX && d.IndexY == item.IndexY).FirstOrDefault();
                    if (die != null)
                    {
                        RecipeMapDieColorChange(false, die, Brushes.Yellow);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 更換tempRecipeRectangles顏色， 讓RecipeMap上的Die變成DefectList顏色(brush黃色是選擇要檢查的Die,brush==null代表要改回來)
        /// </summary>
        /// <param name="die"></param>
        /// <param name="brush"></param>
        private void RecipeMapDieColorChange(bool isDoubleClickSelected, Die die, Brush stroke)
        {
            try
            {
                var result = Rectangles
                                  .Select((rect, idx) => new { rect, idx })
                                  .FirstOrDefault(x => x.rect.Col == die.IndexX && x.rect.Row == die.IndexY);
                var result2 = tempRecipeRectangles
                                 .Select((rect, idx) => new { rect, idx })
                                 .FirstOrDefault(x => x.rect.Col == die.IndexX && x.rect.Row == die.IndexY);
                if (result != null && result2 != null)
                {
                    //若是要Die按兩下變色的功能
                    if (isDoubleClickSelected)
                    {
                        if (stroke == null)
                        {
                            stroke = tempRecipeRectangles[result2.idx].Fill;
                        }
                    }
                    else
                    {
                        if (stroke == null)
                        {
                            tempRecipeRectangles[result2.idx].Fill = Rectangles[result.idx].Fill;
                        }
                        else
                        {
                            tempRecipeRectangles[result2.idx].Fill = stroke;
                        }
                    }
                    SetRectangle?.Invoke(Rectangles[result.idx].Col, Rectangles[result.idx].Row, Rectangles[result.idx].Fill, tempRecipeRectangles[result2.idx].Fill);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
