﻿using GalaSoft.MvvmLight.Command;
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
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model.Recipe;
using WLS3200Gen2.UserControls;
using YuanliCore.AffineTransform;
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
        private double moveIndexX, moveIndexY;


        public BitmapSource LocateSampleImage1 { get => locateSampleImage1; set => SetValue(ref locateSampleImage1, value); }
        public BitmapSource LocateSampleImage2 { get => locateSampleImage2; set => SetValue(ref locateSampleImage2, value); }
        public BitmapSource LocateSampleImage3 { get => locateSampleImage3; set => SetValue(ref locateSampleImage3, value); }
        public ObservableCollection<WaferUIData> LoadPort1Wafers { get => loadPort1Wafers; set => SetValue(ref loadPort1Wafers, value); }
        public ObservableCollection<WaferUIData> LoadPort2Wafers { get => loadPort2Wafers; set => SetValue(ref loadPort2Wafers, value); }

        public LocateParam LocateParam1 { get => locateParam1; set => SetValue(ref locateParam1, value); }
        public LocateParam LocateParam2 { get => locateParam2; set => SetValue(ref locateParam2, value); }
        public LocateParam LocateParam3 { get => locateParam3; set => SetValue(ref locateParam3, value); }
        public LocateMode SelectMode { get => selectMode; set => SetValue(ref selectMode, value); }

        public ExistStates TestStates { get => testStates; set => SetValue(ref testStates, value); }

        public double AlignOffsetX { get => alignOffsetX; set => SetValue(ref alignOffsetX, value); }
        public double AlignOffsetY { get => alignOffsetY; set => SetValue(ref alignOffsetY, value); }

        public Action<CogMatcher> SampleFind { get => sampleFind; set => SetValue(ref sampleFind, value); }



        public double MoveIndexX { get => moveIndexX; set => SetValue(ref moveIndexX, value); }
        public double MoveIndexY { get => moveIndexY; set => SetValue(ref moveIndexY, value); }


        /// <summary>
        /// 滑鼠在影像內 Pixcel 座標
        /// </summary>
        public System.Windows.Point MousePixcel { get; set; }


        /// <summary>
        /// 取得或設定 shape 
        /// </summary>
        public ObservableCollection<ROIShape> Drawings { get => drawings; set => SetValue(ref drawings, value); }

        public ObservableCollection<ROIShape> MapDrawings { get => mapDrawings; set => SetValue(ref mapDrawings, value); }

        public ICommand LoadWaferCommand => new RelayCommand<string>(async key =>
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

        public ICommand LoadMappingCommand => new RelayCommand<string>(async key =>
        {

            mainRecipe.DetectRecipe.WaferMap = new WaferMapping();

            foreach (var item in mainRecipe.DetectRecipe.WaferMap.Dies)
            {


            }


            /* var center = new ROICross
             {
                 X = item.Center.X,
                 Y = item.Center.Y,
                 Size = 5,
                 StrokeThickness = 2,
                 Stroke = System.Windows.Media.Brushes.Red,
                 IsInteractived = false
             };
             AddShapeAction.Execute(center);*/
        });

        public ICommand EditSampleCommand => new RelayCommand<string>(async key =>
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
        public ICommand ParamConfirmCommand => new RelayCommand(async () =>
        {


            SetLocateParamToRecipe();


        });
        public ICommand LocateRunCommand => new RelayCommand(async () =>
        {
            SetLocateParamToRecipe();

            transForm = await machine.MicroDetection.Alignment(mainRecipe.DetectRecipe.AlignRecipe);


        });
        public ICommand LocatedMoveDieCommand => new RelayCommand(async () =>
        {
            //挑選出 對應index 的Die
            YuanliCore.Data.Die[] dies = mainRecipe.DetectRecipe.WaferMap.Dies;
            YuanliCore.Data.Die die = dies.Where(d => d.IndexX == MoveIndexX && d.IndexY == MoveIndexY).FirstOrDefault();

            //設計座標轉換對位後座標
            Point transPos = transForm.TransPoint(new Point(die.PosX, die.PosY));

            await machine.MicroDetection.TableMoveToAsync(transPos);

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

            var pos1 = new Point(LocateParam1.GrabPositionX, LocateParam1.GrabPositionY);
            var pos2 = new Point(LocateParam2.GrabPositionX, LocateParam2.GrabPositionY);
            var pos3 = new Point(LocateParam3.GrabPositionX, LocateParam3.GrabPositionY);

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
