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

        private BincodeInfo[] mapbincodeList;
        private int selectList, column, row;
        private Die[] dieArray;
        private WriteableBitmap mappingTable;
        private Action<MappingOperate> create;
        private Action<int, int, Brush, Brush> setRectangle;



        public Action<int, int, Brush, Brush> SetRectangle { get => setRectangle; set => SetValue(ref setRectangle, value); }
        public WriteableBitmap MappingTable { get => mappingTable; set => SetValue(ref mappingTable, value); }
        public BincodeInfo[] MapBincodeList { get => mapbincodeList; set => SetValue(ref mapbincodeList, value); }
        public Die[] DieArray { get => dieArray; set => SetValue(ref dieArray, value); }


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
        public ICommand TestLoadRecipePageCommand => new RelayCommand(() =>
        {
            try
            {
                //從Recipe 帶值
                //    MapBincodeList= mainRecipe.DetectRecipe.BincodeList.ToArray();
                //     DieArray = mainRecipe.DetectRecipe.WaferMap.Dies;

                var dummy = CreateDummyBincode(50, 50);//建出假資料
                MapBincodeList = dummy.mapBincodes;
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

                MapBincodeList = bincodeInfos.ToArray();
                DieArray = mainRecipe.DetectRecipe.WaferMap.Dies;// dummy.dice;


                MappingOp?.Invoke(MappingOperate.Create); //產生圖片


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
