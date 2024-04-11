using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using WLS3200Gen2.Model.Recipe;
using YuanliCore.CameraLib;
using YuanliCore.Data;
using YuanliCore.Interface;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// MappingCanvas.xaml 的互動邏輯
    /// </summary>
    public partial class MappingCanvasTest : UserControl, INotifyPropertyChanged
    {

        private const double ScaleRate = 0.1; // 縮放比率
        private ScaleTransform scaleTransform = new ScaleTransform(1, 1); // 初始縮放比例為 1

        private ObservableCollection<LineViewModel> _lines = new ObservableCollection<LineViewModel>();
        private bool isDragging, isSelectMode, isTouchMode;
        private Point lastMousePosition;
        private Point dragStartPoint;

        private List<RectangleInfo> rectangles = new List<RectangleInfo>();
        private ObservableCollection<RectangleInfo> rectangles1 = new ObservableCollection<RectangleInfo>();

        //  private ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>();
        public static readonly DependencyProperty ColProperty = DependencyProperty.Register(nameof(Col), typeof(int), typeof(MappingCanvasTest),
                                                                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty RowProperty = DependencyProperty.Register(nameof(Row), typeof(int), typeof(MappingCanvasTest),
                                                               new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty DiesProperty = DependencyProperty.Register(nameof(Dies), typeof(Die[]), typeof(MappingCanvasTest),
                                                           new FrameworkPropertyMetadata(new Die[] { }, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDiesChanged)));

        public static readonly DependencyProperty BincodeInFomationProperty = DependencyProperty.Register(nameof(BincodeInFomation), typeof(BincodeInfo[]), typeof(MappingCanvasTest),
                                                         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDiesChanged)));


        public static readonly DependencyProperty BitmapImageProperty = DependencyProperty.Register(nameof(BitmapImage), typeof(WriteableBitmap), typeof(MappingCanvasTest),
                                                              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public MappingCanvasTest()
        {
            InitializeComponent();

            // 添加滑鼠左鍵拖曳事件
            myGrid.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
            myGrid.MouseLeftButtonUp += Canvas_MouseLeftButtonUp;
            myGrid.MouseMove += Canvas_MouseMove;
            myGrid.MouseWheel += Window_PreviewMouseWheel;
            myGrid.MouseEnter += Grid_MouseEnter;
            myGrid.MouseLeave += Grid_MouseLeave;


            isSelectMode = false;
            isTouchMode = true;


            //  DrawCross();
        }
        public ObservableCollection<RectangleInfo> Rectangles1
        {
            get => rectangles1;
            set => SetValue(ref rectangles1, value);
        }

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
        public Die[] Dies
        {
            get => (Die[])GetValue(DiesProperty);
            set => SetValue(DiesProperty, value);
        }
        public BincodeInfo[] BincodeInFomation
        {
            get => (BincodeInfo[])GetValue(BincodeInFomationProperty);
            set => SetValue(BincodeInFomationProperty, value);
        }


        public WriteableBitmap BitmapImage
        {
            get => (WriteableBitmap)GetValue(BitmapImageProperty);
            set => SetValue(BitmapImageProperty, value);
        }




        public ObservableCollection<LineViewModel> Lines
        {
            get => _lines;
            set => SetValue(ref _lines, value);
        }


        #region 縮放
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
        private void ZoomFit_Click(object sender, RoutedEventArgs e)
        {
            double scaleW = scrollViewer.ActualWidth / BitmapImage.Width;
            double scaleH = scrollViewer.ActualHeight / BitmapImage.Height;
            if (scaleW <= scaleH)//取最小比例來做縮放
                viewbox.LayoutTransform = new ScaleTransform(scaleW, scaleW);
            else
                viewbox.LayoutTransform = new ScaleTransform(scaleH, scaleH);

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
        #endregion

        #region 繪圖
      


        private void DrawRectangles(int cols, int rows, Die[] dice, BincodeInfo[] bincodeInfo)
        {
            Stopwatch stopwatch = new Stopwatch();
            int width = 30;
            int height = 30;
            object lockObj = new object();

            Point startPoint = new Point(20, 20);

            Canvas canvas = new Canvas();
            canvas.Width = cols * (width + 3) + width; //數量 * 寬度+線寬 +圖像BUFF 每個方框都抓20*20寬高，計算出需要的圖像大小
            canvas.Height = rows * (height + 3) + height;//數量 * 高度+線寬
            canvas.Background = Brushes.White;

            myGrid.Width = canvas.Width;
            myGrid.Height = canvas.Height;
            rectangles.Clear();
            canvas.Children.Clear();
            var dieGroup = dice.OrderBy(d => d.IndexX).GroupBy(d => d.IndexX).ToArray();//先把X分類出來  加速後面搜尋速度
            stopwatch.Start();
            Parallel.For(0, cols, i =>
            {

                Parallel.For(0, rows, j =>
                {

                    var rect = new RectangleInfo(startPoint.X + ((width + 3) * i), startPoint.Y + ((height + 3) * j), width, height);

                    rect.Row = j;
                    rect.Col = i;
                    var die = dieGroup[i].Where(d => d.IndexY == j).FirstOrDefault();
                    //   var die = dice.Where(d => d.IndexX == i && d.IndexY == j).FirstOrDefault();

                    if (die == null || bincodeInfo == null)//沒提供資訊 方框就畫灰色
                        rect.Fill = Brushes.Gray;
                    else
                    {
                        var bincode = bincodeInfo.Where(code => code.Code == die.BinCode).FirstOrDefault();
                        if (bincode == null)//沒對應到的資訊 就畫灰色
                            rect.Fill = Brushes.Gray;
                        else
                            rect.Fill = bincode.Color;

                    }

                    lock (lockObj)
                    {
                        rectangles.Add(rect);
                    }

                });
            });

            var count2 = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();


            foreach (var rect in rectangles)
            {
                canvas.Children.Add(CreateRectangle(rect.CenterX, rect.CenterY, rect.Width, rect.Height, rect.Fill));

            }
            var count3 = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            BitmapImage = CreateBitmap(canvas);

            /*
            var cc=  BitmapImage.ToByteFrame();          
            var bip=   cc.ToBitmapSource();        
            WriteableBitmap aaa = new WriteableBitmap(bip);   
            var count6 = stopwatch.ElapsedMilliseconds;
           */

        }

        private Rectangle CreateRectangle(double centerX, double centerY, double width, double height, Brush fill)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.Fill = fill;
            Canvas.SetLeft(rectangle, centerX - width / 2);
            Canvas.SetTop(rectangle, centerY - height / 2);
            return rectangle;
        }



        private WriteableBitmap CreateBitmap(Canvas canvas)
        {

            // 测量和排列 Canvas
            canvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            canvas.Arrange(new Rect(canvas.DesiredSize));

            // 渲染 Canvas 并保存为图像
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)canvas.RenderSize.Width, (int)canvas.RenderSize.Height, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(canvas);

            return new WriteableBitmap(bitmap.FormatConvertTo(PixelFormats.Bgr32));
        }

        private void DrawRectangle(RectangleInfo rectangleinfo)
        {

            Rectangle rectangle = new Rectangle();
            rectangle.Width = rectangleinfo.Width;
            rectangle.Height = rectangleinfo.Height;
            rectangle.Fill = rectangleinfo.Fill; // 可以根據需要設置不同的顏色
            rectangle.Stroke = Brushes.Red;
            // 設置 Rectangle 的位置
            Canvas.SetLeft(rectangle, rectangleinfo.CenterX - rectangleinfo.Width / 2); // 每個 Rectangle 的水平間距為 120
            Canvas.SetTop(rectangle, rectangleinfo.CenterY - rectangleinfo.Height / 2); // 所有 Rectangle 的垂直位置相同

            // 將 Rectangle 加入到 Canvas 中
            canvas.Children.Add(rectangle);

        }
        #endregion

        #region 滑鼠事件
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isTouchMode)
            {
                dragStartPoint = e.GetPosition(scrollViewer);
                myGrid.CaptureMouse();
            }
            else if (isSelectMode)
            {

            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isTouchMode)
            {
                myGrid.ReleaseMouseCapture();
            }
            else if (isSelectMode)
            {
                Point pixel = e.GetPosition(myGrid);

                var selectRect = rectangles.Where(rect => rect.Rectangle.Contains(pixel)).FirstOrDefault();
                if (selectRect == null) return;

                DrawRectangle(selectRect);




            }
        }
        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isTouchMode)
                Mouse.OverrideCursor = Cursors.Hand; // 更改滑鼠外型為手形
            else
                Mouse.OverrideCursor = null; // 恢復滑鼠外型為預設
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null; // 恢復滑鼠外型為預設
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

        static BitmapSource LoadBitmapSourceFromFile(string filePath)
        {
            // 從圖片檔案讀取 BitmapSource
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(filePath, UriKind.RelativeOrAbsolute);
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        private void CreateRetagle_Click(object sender, RoutedEventArgs e)
        {
            DrawRectangles(Col, Row, Dies, BincodeInFomation);
        }

        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            isSelectMode = true;
            isTouchMode = false;
        }
        private void TouchBtn_Click(object sender, RoutedEventArgs e)
        {
            isSelectMode = false;
            isTouchMode = true;
        }

       


        
        private static void OnDiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MappingCanvasTest;
            dp.DrawMapping();
        }

        private void DrawMapping()
        {
            if (Dies == null) return;
            if (BitmapImage != null) return;
            int col = Dies.Max(die => die.IndexX);
            int row = Dies.Max(die => die.IndexY);
            DrawRectangles(col + 1, row + 1, Dies, BincodeInFomation); //取Inedex最大值 會是總數量-1  ，所以要+1回來

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

        public RectangleInfo(double centerX, double centerY, double Width, double Height)
        {
            this.CenterX = centerX;
            this.CenterY = centerY;
            this.Width = Width;
            this.Height = Height;
            Rectangle = new Rect(centerX - Width / 2, centerY - Height / 2, Width, Height);


        }
        public int Col { get; set; }
        public int Row { get; set; }
        public double CenterX { get; }
        public double CenterY { get; }
        public double Width { get; }
        public double Height { get; }
        public Rect Rectangle { get; }
        public Brush Fill { get; set; }

    }
}
