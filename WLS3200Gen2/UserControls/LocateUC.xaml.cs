using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using WLS3200Gen2.Model.Recipe;
using YuanliCore.ImageProcess.Match;
using YuanliCore.Interface;
using YuanliCore.Model;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// LocateUC.xaml 的互動邏輯
    /// </summary>
    public partial class LocateUC : UserControl, INotifyPropertyChanged
    {
        private CogMatcher tempMatcher = new CogMatcher(); //使用Vision pro 實體
        /*  private PatmaxParams matchParam1 = new PatmaxParams(1);
          private PatmaxParams matchParam2 = new PatmaxParams(2);
          private PatmaxParams matchParam3 = new PatmaxParams(3);*/
        private BitmapSource locateSampleImage1;
        private BitmapSource locateSampleImage2;
        private BitmapSource locateSampleImage3;


      //  private double offsetX, offsetY;


        private LocateMode modeForUI = LocateMode.Pattern;

        private int locateModeIndex;


        public static readonly DependencyProperty MainImageProperty = DependencyProperty.Register(nameof(MainImage), typeof(BitmapSource), typeof(LocateUC),
                                                                                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MatchParam1Property = DependencyProperty.Register(nameof(MatchParam1), typeof(LocateParam), typeof(LocateUC),
                                                                             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty MatchParam2Property = DependencyProperty.Register(nameof(MatchParam2), typeof(LocateParam), typeof(LocateUC),
                                                                             new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MatchParam3Property = DependencyProperty.Register(nameof(MatchParam3), typeof(LocateParam), typeof(LocateUC),
                                                                          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty MatchFindProperty = DependencyProperty.Register(nameof(MatchFind), typeof(Action<CogMatcher>), typeof(LocateUC),
                                                                        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty AlignMarkMoveProperty = DependencyProperty.Register(nameof(AlignMarkMove), typeof(Action<Point>), typeof(LocateUC),
                                                                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        
        public static readonly DependencyProperty CurrentPosXProperty = DependencyProperty.Register(nameof(CurrentPosX), typeof(double), typeof(LocateUC),
                                                                   new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CurrentPosYProperty = DependencyProperty.Register(nameof(CurrentPosY), typeof(double), typeof(LocateUC),
                                                                 new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public static readonly DependencyProperty SelectModeProperty = DependencyProperty.Register(nameof(SelectMode), typeof(LocateMode), typeof(LocateUC),
                                                          new FrameworkPropertyMetadata(LocateMode.Pattern, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnOffsetChanged)));

        //public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(nameof(Offset), typeof(Vector), typeof(LocateUC),
        //                                          new FrameworkPropertyMetadata(new Vector(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnOffsetChanged)));

        public static readonly DependencyProperty OffsetXProperty = DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(LocateUC),
                                        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty OffsetYProperty = DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(LocateUC),
                                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
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

        public double CurrentPosX
        {
            get => (double)GetValue(CurrentPosXProperty);
            set => SetValue(CurrentPosXProperty, value);
        }
        public double CurrentPosY
        {
            get => (double)GetValue(CurrentPosYProperty);
            set => SetValue(CurrentPosYProperty, value);
        }

      /*  public Vector Offset
        {
            get => (Vector)GetValue(OffsetProperty);
            set=> SetValue(OffsetProperty, value);
        }*/

        public Action<CogMatcher> MatchFind
        {
            get => (Action<CogMatcher>)GetValue(MatchFindProperty);
            set => SetValue(MatchFindProperty, value);
        }
        public Action<Point> AlignMarkMove
        {
            get => (Action<Point>)GetValue(AlignMarkMoveProperty);
            set => SetValue(AlignMarkMoveProperty, value);
        }
        public LocateMode SelectMode
        {
            get => (LocateMode)GetValue(SelectModeProperty);
            set => SetValue(SelectModeProperty, value);
        }


        public LocateMode ModeForUI
        {
            get
            {
                if (modeForUI == LocateMode.Edge)
                    LocateModeIndex = 1;
                else if (modeForUI == LocateMode.Pattern)
                    LocateModeIndex = 0;
                SelectMode = modeForUI;
                return modeForUI;
            }
            set => SetValue(ref modeForUI, value);
        }
        /*  {

              get {
                  if (mode == LocateMode.Edge)
                      LocateModeIndex = 0;
                  else if (mode == LocateMode.Pattern)
                      LocateModeIndex = 1;

                  return mode; }
              set => SetValue(ref mode, value);

          }*/



        public double OffsetX
        {
            get => (double)GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }
        public double OffsetY
        {
            get => (double)GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }





        public BitmapSource LocateSampleImage1 { get => locateSampleImage1; set => SetValue(ref locateSampleImage1, value); }
        public BitmapSource LocateSampleImage2 { get => locateSampleImage2; set => SetValue(ref locateSampleImage2, value); }
        public BitmapSource LocateSampleImage3 { get => locateSampleImage3; set => SetValue(ref locateSampleImage3, value); }

        public int LocateModeIndex { get => locateModeIndex; set => SetValue(ref locateModeIndex, value); }


        public LocateUC()
        {
            InitializeComponent();
        }



        public ICommand ClosingCommand => new RelayCommand(() =>
       {
           //  UpdateParam();

       });

        public ICommand LoadedCommand => new RelayCommand(() =>
        {

        });
        public ICommand EditSampleCommand => new RelayCommand<string>(async key =>
        {
            try
            {

                switch (key)
                {
                    case "Sample1":

                        tempMatcher.RunParams = MatchParam1.MatchParam;
                        tempMatcher.EditParameter(MainImage);

                        MatchParam1.MatchParam = (PatmaxParams)tempMatcher.RunParams;
                        if (MatchParam1.MatchParam.PatternImage != null)
                            MatchParam1.SampleImage = MatchParam1.MatchParam.PatternImage;
                        break;
                    case "Sample2":
                        tempMatcher.RunParams = MatchParam2.MatchParam;
                        tempMatcher.EditParameter(MainImage);

                        MatchParam2.MatchParam = (PatmaxParams)tempMatcher.RunParams;
                        if (MatchParam2.MatchParam.PatternImage != null)
                            MatchParam2.SampleImage = MatchParam2.MatchParam.PatternImage;
                        break;
                    case "Sample3":
                        tempMatcher.RunParams = MatchParam3.MatchParam;
                        tempMatcher.EditParameter(MainImage);
                        MatchParam3.MatchParam = (PatmaxParams)tempMatcher.RunParams;
                        if (MatchParam3.MatchParam.PatternImage != null)
                            MatchParam3.SampleImage = MatchParam3.MatchParam.PatternImage;
                        break;
                    default:
                        break;
                }


                //  UpdateParam();


                //  UpdateRecipe();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand LocateSampleCommand => new RelayCommand<string>(async key =>
        {


            switch (key)
            {
                case "Sample1":
                    tempMatcher.RunParams = MatchParam1.MatchParam;

                    break;
                case "Sample2":
                    tempMatcher.RunParams = MatchParam2.MatchParam;

                    break;
                case "Sample3":
                    tempMatcher.RunParams = MatchParam3.MatchParam;

                    break;
                default:
                    break;
            }
            //  UpdateParam();
            MatchFind?.Invoke(tempMatcher);

         
        });





        public ICommand MoveToSampleCommand => new RelayCommand<string>(async key =>
        {
            Point movePos =new Point();

            switch (key)
            {
                case "Sample1":
                    movePos = new Point(MatchParam1.GrabPositionX, MatchParam1.GrabPositionY);

                    break;
                case "Sample2":
                    movePos = new Point(MatchParam2.GrabPositionX, MatchParam2.GrabPositionY);

                    break;
                case "Sample3":
                    movePos = new Point(MatchParam3.GrabPositionX, MatchParam3.GrabPositionY);
                    break;
                default:
                    break;
            }
         
            AlignMarkMove?.Invoke(movePos);

          

        });

        public ICommand GetPositionCommand => new RelayCommand<string>(async key =>
        {
            switch (key)
            {
                case "set1":
                    MatchParam1.GrabPositionX = CurrentPosX;
                    MatchParam1.GrabPositionY = CurrentPosY;
                    //MatchParam1.DesignPositionX = CurrentPosX;
                    //MatchParam1.DesignPositionY = CurrentPosY;
                    break;
                case "set2":
                    MatchParam2.GrabPositionX = CurrentPosX;
                    MatchParam2.GrabPositionY = CurrentPosY;
                    //MatchParam2.DesignPositionX = CurrentPosX;
                    //MatchParam2.DesignPositionY = CurrentPosY;

                    break;
                case "set3":
                    MatchParam3.GrabPositionX = CurrentPosX;
                    MatchParam3.GrabPositionY = CurrentPosY;
                    //MatchParam3.DesignPositionX = CurrentPosX;
                    //MatchParam3.DesignPositionY = CurrentPosY;

                    break;
                default:
                    break;
            }

            //   UpdateParam();
        });
        private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as LocateUC;
            dp.SetOffset();
        }
        private void SetOffset()
        {
            ModeForUI = SelectMode;
            if (modeForUI == LocateMode.Edge)
                LocateModeIndex = 1;
            else if (modeForUI == LocateMode.Pattern)
                LocateModeIndex = 0;
          


        }
        /* public void UpdateParam()
         {

             if (MatchParam1 == null || MatchParam2 == null) return;
             //正常拍照座標跟設計座標應該分開 ，暫時先一起  有需要再分
             MatchParam1.GrabPosition = new Point(LocateGrabPosX1, LocateGrabPosY1);
             MatchParam2.GrabPosition = new Point(LocateGrabPosX2, LocateGrabPosY2);
             MatchParam3.GrabPosition = new Point(LocateGrabPosX3, LocateGrabPosY3);

             MatchParam1.DesignPosition = new Point(LocateGrabPosX1, LocateGrabPosY1);
             MatchParam2.DesignPosition = new Point(LocateGrabPosX2, LocateGrabPosY2);
             MatchParam3.DesignPosition = new Point(LocateGrabPosX3, LocateGrabPosY3);

             MatchParam1.Index = new Point(LocateIndexX1, LocateIndexY1);
             MatchParam2.Index = new Point(LocateIndexX2, LocateIndexY2);
             MatchParam3.Index = new Point(LocateIndexX3, LocateIndexY3);


         }*/


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



    public class LocateModePatternConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((LocateMode)value == LocateMode.Pattern)
                return true;
            else
                return false;
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return LocateMode.Pattern;
            else
                return LocateMode.Edge;


        }
    }

    public class LocateModeEdgeConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if ((LocateMode)value == LocateMode.Edge)
                return true;
            else
                return false;
        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return LocateMode.Edge;
            else
                return LocateMode.Pattern;


        }
    }


    public class ImageBMPConver : IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var bmp = value as Frame<byte[]>;
            return bmp.ToBitmapSource();


        }

        //当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var bmp = value as BitmapSource;

            return bmp.ToByteFrame();
        }
    }
}
