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
        private PatmaxParams matchParam = new PatmaxParams(0);

        private BitmapSource locateSampleImage1;
        private BitmapSource locateSampleImage2;
        private BitmapSource locateSampleImage3;

        private bool islocateEdgeMode;
        private bool islocatePatternMode;
        private int locateModeIndex;
        public BitmapSource MainImage;
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
}
