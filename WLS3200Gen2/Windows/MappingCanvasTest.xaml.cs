using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// MappingCanvas.xaml 的互動邏輯
    /// </summary>
    public partial class MappingCanvasTest : UserControl, INotifyPropertyChanged
    {

        private bool isDragging;
        private Point lastMousePosition;
        private BitmapImage bitmapImage;
   
        //  private ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>();
        public MappingCanvasTest()
        {
            InitializeComponent();


            bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("D:\\10.bmp");
            bitmapImage.EndInit();
            myImage.Source = bitmapImage;

            // 初始化矩形的位置集合



            // 添加滑鼠左鍵拖曳事件
            myGrid.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            myGrid.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            myGrid.MouseMove += Canvas_MouseMove;

       
     
            DrawRectangles();
            //  DrawCross();
        }
        /*public ObservableCollection<Rectangle> Rectangles
        {
            get => rectangles;
            set => SetValue(ref rectangles, value);
        }*/


        #region MyRegion

        private const double ScaleRate = 0.1; // 縮放比率
        private ScaleTransform scaleTransform = new ScaleTransform(1, 1); // 初始縮放比例為 1

        private ObservableCollection<LineViewModel> _lines = new ObservableCollection<LineViewModel>();
        public ObservableCollection<LineViewModel> Lines
        {
            get => _lines;
            set => SetValue(ref _lines, value);
        }
        private ObservableCollection<RectangleInfo> rectangles = new ObservableCollection<RectangleInfo>();
        public ObservableCollection<RectangleInfo> Rectangles
        {
            get => rectangles;
            set => SetValue(ref rectangles, value);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (viewbox.LayoutTransform.Value.M11 > 2) return;
            // 放大因子
            double scaleDelta = 0.05;
            // 縮放
            viewbox.LayoutTransform = new ScaleTransform(viewbox.LayoutTransform.Value.M11 + scaleDelta, viewbox.LayoutTransform.Value.M22 + scaleDelta);
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            if (viewbox.LayoutTransform.Value.M11 <= 0.04) return;

            // 縮小因子
            double scaleDelta = 0;
            if (viewbox.LayoutTransform.Value.M11 > 0.3)
                scaleDelta = 0.1;
            if (viewbox.LayoutTransform.Value.M11 <= 0.3)
                scaleDelta = 0.02;
            // 縮放
            viewbox.LayoutTransform = new ScaleTransform(viewbox.LayoutTransform.Value.M11 - scaleDelta, viewbox.LayoutTransform.Value.M22 - scaleDelta);
       
        }


        private void DrawCross()
        {


            // 根據圖像大小設置十字線的位置和尺寸
            double imageSize = 1000; // 圖像大小
            double crossSize = 200; // 十字線大小

            double centerX = imageSize / 2 - crossSize / 2;
            double centerY = imageSize / 2 - crossSize / 2;

            // 添加橫線
            Lines.Add(new LineViewModel { X1 = 0, Y1 = centerY, X2 = imageSize, Y2 = centerY });

            // 添加豎線
            Lines.Add(new LineViewModel { X1 = centerX, Y1 = 0, X2 = centerX, Y2 = imageSize });
        }

        private void DrawRectangles()
        {

            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {

                     Rectangles.Add(new RectangleInfo { X = 70 + (60 * i), Y = 70 + (60 * j), Width = 50, Height = 50, Fill = Brushes.DarkSeaGreen });
                }
            }
            

          //  Rectangles.Add(new RectangleInfo { X = 200, Y =200, Width = 100, Height = 100, Fill = Brushes.Red });
          //  Rectangles.Add(new RectangleInfo { X = 400, Y = 500, Width = 50, Height = 50, Fill = Brushes.Blue });


            // 添加更多矩形資訊

            // 將集合繫結到 ItemsControl 的 ItemsSource 屬性
            //itemsControl.ItemsSource = Rectangles;
        }
        private Grid mainGrid;

        private Point dragStartPoint;

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragStartPoint = e.GetPosition(scrollViewer);
            myGrid.CaptureMouse();
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myGrid.ReleaseMouseCapture();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!myGrid.IsMouseCaptured) return;

            Point dragEndPoint = e.GetPosition(scrollViewer);
            double offsetX = dragEndPoint.X - dragStartPoint.X;
            double offsetY = dragEndPoint.Y - dragStartPoint.Y;

            scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - offsetX);
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offsetY);

            dragStartPoint = dragEndPoint;
        }
        #endregion



        /*  private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
          {
              isDragging = true;
              lastMousePosition = e.GetPosition(null);
              var x = lastMousePosition.X - bitmapImage.PixelWidth / 2;
              var y = lastMousePosition.Y - bitmapImage.PixelHeight / 2;
              Point imagePoint = new Point(lastMousePosition.X / scaleTransform.ScaleX, lastMousePosition.Y / scaleTransform.ScaleY);

              // 获取鼠标相对于图像的位置
              Point imagePoint1 = e.GetPosition(myImage);



              myImage.CaptureMouse();
          }

          private void Image_MouseMove(object sender, MouseEventArgs e)
          {
              if (isDragging)
              {
                  Point mousePosition = e.GetPosition(null);
                  Vector offset = mousePosition - lastMousePosition;

                  // 移动图像
                  translateTransform.X += offset.X * (1 / scaleTransform.ScaleX);
                  translateTransform.Y += offset.Y * (1 / scaleTransform.ScaleY);

                  // 移动图像
                     double newX = scrollViewer.HorizontalOffset - offset.X;
                     double newY = scrollViewer.VerticalOffset - offset.Y;
                     scrollViewer.ScrollToHorizontalOffset(newX);
                     scrollViewer.ScrollToVerticalOffset(newY);

                  lastMousePosition = mousePosition;


              }
          }

          private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
          {
              isDragging = false;
              myImage.ReleaseMouseCapture();
          }

          private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
          {

              {
                  double zoom = e.Delta > 0 ? 1.1 : 0.9; // 根据滚轮方向计算缩放比例
                  ScaleImage(zoom);
                  e.Handled = true;
              }
          }

          private void ScaleImage(double scale)
          {
              // 设置放大缩小的最小值和最大值
              double minScale = 0.02;
              double maxScale = 2;

              // 计算新的缩放比例
              double newScaleX = Math.Max(minScale, Math.Min(scaleTransform.ScaleX * scale, maxScale));
              double newScaleY = Math.Max(minScale, Math.Min(scaleTransform.ScaleY * scale, maxScale));

              // 应用新的缩放比例
              scaleTransform.ScaleX = newScaleX;
              scaleTransform.ScaleY = newScaleY;

              // 更新滚动条的滑块长度
              scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset * scale);
              scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset * scale);

              // 更新水平滚动条的滑块长度
              scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset * scale);
              scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset * scale);

              // 更新垂直滚动条的滑块长度
              var verticalScrollBar = scrollViewer.Template.FindName("PART_VerticalScrollBar", scrollViewer) as ScrollBar;
              if (verticalScrollBar != null)
              {
                  verticalScrollBar.Maximum = verticalScrollBar.Maximum * scale;
              }
              // 更新水平滚动条的滑块长度
              var horizontalScrollBar = scrollViewer.Template.FindName("PART_HorizontalScrollBar", scrollViewer) as ScrollBar;
              if (horizontalScrollBar != null)
              {
                  horizontalScrollBar.Maximum = horizontalScrollBar.Maximum * scale;
              }
          }


          */
        private Point lastPosition;






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


    //public class ImagePositionConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is double imageSize && parameter is double offset)
    //        {
    //            return imageSize / 2 - offset / 2;
    //        }
    //        return Binding.DoNothing;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class LineViewModel
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }
    }

    public class RectangleInfo
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush Fill { get; set; }
    }
}
