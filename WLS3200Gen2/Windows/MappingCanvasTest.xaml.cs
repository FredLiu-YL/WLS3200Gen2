using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
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
        private BitmapSource bitmapImage;

        //  private ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>();
        public static readonly DependencyProperty ColProperty = DependencyProperty.Register(nameof(Col), typeof(int), typeof(MappingCanvasTest),
                                                                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty RowProperty = DependencyProperty.Register(nameof(Row), typeof(int), typeof(MappingCanvasTest),
                                                               new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));



        public MappingCanvasTest()
        {
            InitializeComponent();


            //         bitmapImage = new BitmapImage();
            //         bitmapImage.BeginInit();
            //         bitmapImage.UriSource = new Uri("D:\\10.bmp");
            //         bitmapImage.EndInit();
            //         myImage.Source = bitmapImage;

            // 初始化矩形的位置集合



            // 添加滑鼠左鍵拖曳事件
            myGrid.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            myGrid.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            myGrid.MouseMove += Canvas_MouseMove;
            myGrid.MouseWheel += Window_PreviewMouseWheel;




            //  DrawCross();
        }
        /*public ObservableCollection<Rectangle> Rectangles
        {
            get => rectangles;
            set => SetValue(ref rectangles, value);
        }*/

        public int Col
        {
            get => (int)GetValue(ColProperty);
            set => SetValue(ColProperty, value);
        }
        public int Row
        {
            get => (int)GetValue(RowProperty);
            set => SetValue(RowProperty, value);
        }


        public BitmapSource BitmapImage
        {
            get => bitmapImage;
            set => SetValue(ref bitmapImage, value);
        }

        #region MyRegion


        private const double ScaleRate = 0.1; // 縮放比率
        private ScaleTransform scaleTransform = new ScaleTransform(1, 1); // 初始縮放比例為 1

        private ObservableCollection<LineViewModel> _lines = new ObservableCollection<LineViewModel>();
        public ObservableCollection<LineViewModel> Lines
        {
            get => _lines;
            set => SetValue(ref _lines, value);
        }
        private ObservableCollection<RectangleInfo> rectangles1 = new ObservableCollection<RectangleInfo>();
        public ObservableCollection<RectangleInfo> Rectangles1
        {
            get => rectangles1;
            set => SetValue(ref rectangles1, value);
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
        private void CreateRetagle_Click(object sender, RoutedEventArgs e)
        {
            DrawRectangles();
        }
        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (e.Delta > 0)
            {
                if (viewbox.LayoutTransform.Value.M11 > 2) return;
                // 放大因子
                double scaleDelta = 0.05;
                // 縮放
                viewbox.LayoutTransform = new ScaleTransform(viewbox.LayoutTransform.Value.M11 + scaleDelta, viewbox.LayoutTransform.Value.M22 + scaleDelta);

            }
            else
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
            Canvas canvas = new Canvas();
            canvas.Width = 10000;
            canvas.Height = 10000;
            Rectangles1.Clear();
            for (int i = 0; i < 300; i++)
            {
                for (int j = 0; j < 300; j++)
                {

                    //Rectangles1.Add(new RectangleInfo { X = 20 + (30 * i), Y = 20 + (30 * j), Width = 20, Height = 20, Fill = Brushes.DarkSeaGreen });

                    canvas.Children.Add(CreateRectangle(20 + (30 * i), 20 + (30 * j), 20, 20, Brushes.DarkSeaGreen));
                }
            }
            BitmapImage = CreateBitmap(canvas);


            //// 将渲染的图像保存为 BMP 文件
            //BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(bitmap));

            //using (FileStream stream = new FileStream("D:\\WERT.bmp", FileMode.Create))
            //{
            //    encoder.Save(stream);
            //}


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

        private Rectangle CreateRectangle(double x, double y, double width, double height, Brush fill)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.Fill = fill;
            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);
            return rectangle;
        }
        #endregion


        private BitmapSource CreateBitmap(Canvas canvas)
        {

            // 测量和排列 Canvas
            canvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            canvas.Arrange(new Rect(canvas.DesiredSize));

            // 渲染 Canvas 并保存为图像
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(canvas);
           
            return bitmap;
        }



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
