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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YuanliCore.Views.CanvasShapes;

namespace WLS3200Gen2
{
    /// <summary>
    /// MeasureWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MeasureWindow : Window, INotifyPropertyChanged
    {
        private WriteableBitmap mainImage;
        private double manualdistance, manualarea;
        private ObservableCollection<ROIShape> manualdrawings = new ObservableCollection<ROIShape>();
        public MeasureWindow(BitmapSource bitmapSource)
        {
            InitializeComponent();
            MainImage = new WriteableBitmap(bitmapSource);
        }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearShapeManualAction { get; set; }
        /// <summary>
        /// 新增 手動量測Shape
        /// </summary>
        public ICommand AddShapeManualAction { get; set; }
        /// <summary>
        /// Fit 縮放到最適合的大小
        /// </summary>
        public ICommand ZoomFitManualAction { get; set; }
        public double ManualDistance { get => manualdistance; set => SetValue(ref manualdistance, value); }
        public double ManualArea { get => manualarea; set => SetValue(ref manualarea, value); }
        public WriteableBitmap MainImage { get => mainImage; set => SetValue(ref mainImage, value); }
        /// <summary>
        /// 手動量測工具
        /// </summary>
        public ObservableCollection<ROIShape> ManualDrawings { get => manualdrawings; set => SetValue(ref manualdrawings, value); }

        public ICommand ManualCaliperCommand => new RelayCommand<string>((param) =>
        {
            try
            {
                var w = MainImage.Width;
                var h = MainImage.Height;
                ROIShape shape = null;
                ClearShapeManualAction.Execute(ManualDrawings);

                switch (param)
                {
                    case "Ruler": //劃出直線

                        shape = new ROILine
                        {
                            X1 = w / 2 - 200,
                            Y1 = h / 2,
                            X2 = w / 2 + 200,
                            Y2 = h / 2,
                            StrokeThickness = 10,
                            Stroke = System.Windows.Media.Brushes.Red,
                            IsInteractived = true
                        };
                        break;

                    case "Rect"://劃出可旋轉的矩形
                        shape = new ROIRotatedRect
                        {
                            X = w / 2,
                            Y = h / 2,
                            LengthX = 100,
                            LengthY = 100,
                            StrokeThickness = 5,
                            Stroke = System.Windows.Media.Brushes.Red,
                            IsInteractived = true
                        };
                        break;

                    case "Circle"://劃出可旋轉的矩形
                        shape = new ROICircle
                        {
                            X = w / 2,
                            Y = h / 2,
                            Radius = 100,
                            StrokeThickness = 5,
                            Stroke = System.Windows.Media.Brushes.Red,
                            IsInteractived = true
                        };
                        break;

                }



                AddShapeManualAction.Execute(shape);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        });

        public ICommand ManualCaliperCalculateCommand => new RelayCommand(() =>
        {
            //需要 Binding 到UI 顯示的數值
            double distance = 0;
            double area = 0;
            double width = 0;
            double height = 0;
            var shape = ManualDrawings.FirstOrDefault();
            if (shape != null)
            {
                if (shape is ROILine)
                {
                    ROILine line = shape as ROILine;

                    width = Math.Abs(line.X2 - line.X1); //最大外接矩形的寬
                    height = Math.Abs(line.Y2 - line.Y1);
                    ManualDistance = Math.Round(Math.Sqrt(width * width + height * height), 3);


                }
                else if (shape is ROIRotatedRect)
                {
                    ROIRotatedRect rect = shape as ROIRotatedRect;
                    width = rect.LengthX * 2;
                    height = rect.LengthY * 2;
                    ManualArea = Math.Round(width * height, 3);


                }
                else if (shape is ROICircle)
                {
                    ROICircle circle = shape as ROICircle;
                    width = circle.Radius * 2;
                    height = circle.Radius * 2;
                    ManualArea = Math.Round(Math.PI * Math.Pow(circle.Radius, 2), 3);


                }

            }
        });
        public ICommand CancelCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
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
