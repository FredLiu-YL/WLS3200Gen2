using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuanliCore.ImageProcess.Match;
using YuanliCore.Interface;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// LocateUC.xaml 的互動邏輯
    /// </summary>
    public partial class LocateUC : UserControl, INotifyPropertyChanged
    {
        private CogMatcher matcher = new CogMatcher(); //使用Vision pro 實體
        /*  private PatmaxParams matchParam1 = new PatmaxParams(1);
          private PatmaxParams matchParam2 = new PatmaxParams(2);
          private PatmaxParams matchParam3 = new PatmaxParams(3);*/
        private BitmapSource locateSampleImage1;
        private BitmapSource locateSampleImage2;
        private BitmapSource locateSampleImage3;

        private double locateGrabPosX1;
        private double locateGrabPosX2;
        private double locateGrabPosX3;
        private double locateGrabPosY1;
        private double locateGrabPosY2;
        private double locateGrabPosY3;


        private int locateIndexX1;
        private int locateIndexY1;
        private int locateIndexX2;
        private int locateIndexY2;
        private int locateIndexX3;
        private int locateIndexY3;

        private bool islocateEdgeMode;
        private bool islocatePatternMode;
        private int locateModeIndex;


        public static readonly DependencyProperty MainImageProperty = DependencyProperty.Register(nameof(MainImage), typeof(BitmapSource), typeof(LocateUC),
                                                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MatchParam1Property = DependencyProperty.Register(nameof(MatchParam1), typeof(LocateParam), typeof(LocateUC),
                                                                             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty MatchParam2Property = DependencyProperty.Register(nameof(MatchParam2), typeof(LocateParam), typeof(LocateUC),
                                                                             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MatchParam3Property = DependencyProperty.Register(nameof(MatchParam3), typeof(LocateParam), typeof(LocateUC),
                                                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public BitmapSource MainImage
        {
            get => (BitmapSource)GetValue(MainImageProperty);
            set => SetValue(MainImageProperty, value);
        }

        public LocateParam MatchParam1
        {
            get => (LocateParam)GetValue(MatchParam1Property);
            set => SetValue(MatchParam1Property, value);
        }
        public LocateParam MatchParam2
        {
            get => (LocateParam)GetValue(MatchParam2Property);
            set => SetValue(MatchParam2Property, value);
        }
        public LocateParam MatchParam3
        {
            get => (LocateParam)GetValue(MatchParam3Property);
            set => SetValue(MatchParam3Property, value);
        }


        public double LocateGrabPosX1 { get => locateGrabPosX1; set => SetValue(ref locateGrabPosX1, value); }
        public double LocateGrabPosX2 { get => locateGrabPosX2; set => SetValue(ref locateGrabPosX2, value); }
        public double LocateGrabPosX3 { get => locateGrabPosX3; set => SetValue(ref locateGrabPosX3, value); }
        public double LocateGrabPosY1 { get => locateGrabPosY1; set => SetValue(ref locateGrabPosY1, value); }
        public double LocateGrabPosY2 { get => locateGrabPosY2; set => SetValue(ref locateGrabPosY2, value); }
        public double LocateGrabPosY3 { get => locateGrabPosY3; set => SetValue(ref locateGrabPosY3, value); }


        public int LocateIndexX1 { get => locateIndexX1; set => SetValue(ref locateIndexX1, value); }
        public int LocateIndexX2 { get => locateIndexX2; set => SetValue(ref locateIndexX2, value); }
        public int LocateIndexX3 { get => locateIndexX3; set => SetValue(ref locateIndexX3, value); }
        public int LocateIndexY1 { get => locateIndexY1; set => SetValue(ref locateIndexY1, value); }
        public int LocateIndexY2 { get => locateIndexY2; set => SetValue(ref locateIndexY2, value); }
        public int LocateIndexY3 { get => locateIndexY3; set => SetValue(ref locateIndexY3, value); }


        public BitmapSource LocateSampleImage1 { get => locateSampleImage1; set => SetValue(ref locateSampleImage1, value); }
        public BitmapSource LocateSampleImage2 { get => locateSampleImage2; set => SetValue(ref locateSampleImage2, value); }
        public BitmapSource LocateSampleImage3 { get => locateSampleImage3; set => SetValue(ref locateSampleImage3, value); }
        public bool IsLocateEdgeMode
        {
            get
            {
                if (islocateEdgeMode)
                    LocateModeIndex = 0;
                return islocateEdgeMode;

            }
            set => SetValue(ref islocateEdgeMode, value);
        }
        public bool IsLocatePatternMode
        {
            get
            {
                if (islocatePatternMode)
                    LocateModeIndex = 1;
                return islocatePatternMode;

            }
            set => SetValue(ref islocatePatternMode, value);
        }
        public int LocateModeIndex { get => locateModeIndex; set => SetValue(ref locateModeIndex, value); }


        public LocateUC()
        {
            InitializeComponent();
        }

        public ICommand EditSampleCommand => new RelayCommand<string>(async key =>
        {
            try
            {

                switch (key)
                {
                    case "Sample1":
                        matcher.RunParams = MatchParam1.MatchParam;
                        matcher.EditParameter(MainImage);

                        MatchParam1.MatchParam = (PatmaxParams)matcher.RunParams;
                        if (MatchParam1.MatchParam.PatternImage != null)
                            LocateSampleImage1 = MatchParam1.MatchParam.PatternImage.ToBitmapSource();
                        break;
                    case "Sample2":
                        matcher.RunParams = MatchParam2.MatchParam;
                        matcher.EditParameter(MainImage);

                        MatchParam2.MatchParam = (PatmaxParams)matcher.RunParams;
                        if (MatchParam2.MatchParam.PatternImage != null)
                            LocateSampleImage2 = MatchParam2.MatchParam.PatternImage.ToBitmapSource();
                        break;
                    case "Sample3":
                        matcher.RunParams = MatchParam3.MatchParam;
                        matcher.EditParameter(MainImage);
                        MatchParam3.MatchParam = (PatmaxParams)matcher.RunParams;
                        if (MatchParam3.MatchParam.PatternImage != null)
                            LocateSampleImage3 = MatchParam3.MatchParam.PatternImage.ToBitmapSource();
                        break;
                    default:
                        break;
                }





                //  UpdateRecipe();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand LocateSampleCommand => new RelayCommand<string>(async key =>
        {
            /*  ClearShapeAction.Execute(Drawings);
              resultPoint = matcher.Find(Image.ToByteFrame());

              foreach (var item in resultPoint)
              {
                  var center = new ROICross
                  {
                      X = item.Center.X,
                      Y = item.Center.Y,
                      Size = 5,
                      StrokeThickness = 2,
                      Stroke = Brushes.Red,
                      IsInteractived = false
                  };
                  AddShapeAction.Execute(center);

              }*/

        });




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

    public class LocateParam
    {
        public Point DesignPosition { get; set; }
        public Point Index { get; set; }

        public PatmaxParams MatchParam { get; set; }

    }

}
