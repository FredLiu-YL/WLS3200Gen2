﻿using GalaSoft.MvvmLight.Command;
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
        private System.Windows.Point mapmousePixcel, newHomeMapMousePixcel, newMapMousePixcel;
        private List<RectangleInfo> rectanglesHome = new List<RectangleInfo>();
        private List<RectangleInfo> rectangles = new List<RectangleInfo>();
        /// <summary>
        /// Map顯示Die的狀況
        /// </summary>
        private List<RectangleInfo> tempRecipeRectangles = new List<RectangleInfo>();
        /// <summary>
        /// 下BinCode後的狀況
        /// </summary>
        private List<RectangleInfo> tempHomeAssignRectangles = new List<RectangleInfo>();
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
        public Action<MappingOperate> MappingOp { get => create; set => SetValue(ref create, value); }

        


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
                Rectangle selectRange = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 5,
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(selectRange, NewHomeMapMousePixcel.X);
                Canvas.SetTop(selectRange, NewHomeMapMousePixcel.Y);
                var rect = new Rect(Canvas.GetLeft(selectRange), Canvas.GetTop(selectRange), selectRange.Width, selectRange.Height);
                IEnumerable<RectangleInfo> tempselectRects = new List<RectangleInfo>();
                tempselectRects = Rectangles.Where(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                   || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight));
                if (tempselectRects != null)
                {
                    if (selectHomeRectangle != null)
                    {
                        foreach (var item in selectHomeRectangle)
                        {
                            int index = Rectangles.IndexOf(item);
                            if (index >= 0)
                            {
                                SetHomeRectangle?.Invoke(Rectangles[index].Col, Rectangles[index].Row, tempHomeAssignRectangles[index].Fill, tempRecipeRectangles[index].Fill);
                            }
                        }
                    }
                    foreach (var item in tempselectRects)
                    {
                        int index = Rectangles.IndexOf(item);
                        if (index >= 0)
                        {
                            var ss = Rectangles[index].Fill;
                            SetHomeRectangle?.Invoke(Rectangles[index].Col, Rectangles[index].Row, tempHomeAssignRectangles[index].Fill, Brushes.Red);
                        }
                    }
                    selectHomeRectangle = tempselectRects;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });
        /// <summary>
        /// Recipe端畫面互動
        /// </summary>
        public ICommand DoubleClickRecipeNewMappingDieCommand => new RelayCommand(async () =>
        {
            try
            {
                Rectangle selectRange = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 5,
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(selectRange, NewMapMousePixcel.X);
                Canvas.SetTop(selectRange, NewMapMousePixcel.Y);
                var rect = new Rect(Canvas.GetLeft(selectRange), Canvas.GetTop(selectRange), selectRange.Width, selectRange.Height);
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
                            if (index >= 0)
                            {
                                SetRectangle?.Invoke(Rectangles[index].Col, Rectangles[index].Row, Rectangles[index].Fill, tempRecipeRectangles[index].Fill);
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
                MappingOp?.Invoke(MappingOperate.Create); //產生圖片
                tempRecipeRectangles = new List<RectangleInfo>();
                foreach (var item in Rectangles)
                {
                    tempRecipeRectangles.Add(new RectangleInfo(item.CenterX, item.CenterY, item.Width, item.Height)
                    {
                        Col = item.Col,
                        Row = item.Row,
                        Fill = item.Fill
                    });
                    tempHomeAssignRectangles.Add(new RectangleInfo(item.CenterX, item.CenterY, item.Width, item.Height)
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

            MappingOp?.Invoke(MappingOperate.Clear);
        });
        public ICommand TestDect3Command => new RelayCommand(() =>
        {
            MappingOp?.Invoke(MappingOperate.Fit);
        });
    }

}
