using GalaSoft.MvvmLight.Command;
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
    public partial class MappingCanvas : UserControl, INotifyPropertyChanged
    {
        private int width = 30, height = 30;//定義繪圖方框的寬高 (PIXEL)
        private int Cuttingline = 3;// 方框中間的間隙( Die之間的切割道寬度)(PIXEL)
        private Point startPoint = new Point(20, 20); //定義繪圖的起點位置(主要是讓方框圖像留邊)

        //    private const double ScaleRate = 0.1; // 縮放比率
        //    private ScaleTransform scaleTransform = new ScaleTransform(1, 1); // 初始縮放比例為 1

        private ObservableCollection<LineViewModel> _lines = new ObservableCollection<LineViewModel>();
        private bool isDragging, isSelectMode, isTouchMode, isAdd, isDel;
        //     private Point lastMousePosition;
        private Point dragStartPoint;


        private List<RectangleInfo> rectangles = new List<RectangleInfo>();
        private ObservableCollection<RectangleInfo> selectRectangles = new ObservableCollection<RectangleInfo>();
        //    private ObservableCollection<Die> selectDies = new ObservableCollection<Die>();

        //  private ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>();
        public static readonly DependencyProperty ColProperty = DependencyProperty.Register(nameof(Col), typeof(int), typeof(MappingCanvas),
                                                                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty RowProperty = DependencyProperty.Register(nameof(Row), typeof(int), typeof(MappingCanvas),
                                                               new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty DiesProperty = DependencyProperty.Register(nameof(Dies), typeof(Die[]), typeof(MappingCanvas),
                                                           new FrameworkPropertyMetadata(new Die[] { }, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnDiesChanged)));

        public static readonly DependencyProperty BincodeInFomationProperty = DependencyProperty.Register(nameof(BincodeInFomation), typeof(BincodeInfo[]), typeof(MappingCanvas),
                                                         new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public static readonly DependencyProperty MapImageProperty = DependencyProperty.Register(nameof(MapImage), typeof(WriteableBitmap), typeof(MappingCanvas),
                                                              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty SelectDiesProperty = DependencyProperty.Register(nameof(SelectDies), typeof(ObservableCollection<Die>), typeof(MappingCanvas),
                                                       new FrameworkPropertyMetadata(new ObservableCollection<Die>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public static readonly DependencyProperty MappingImageOperateProperty = DependencyProperty.Register(nameof(MappingImageOperate), typeof(Action<MappingOperate>), typeof(MappingCanvas),
                                                              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //col,row,背景色，線條
        public static readonly DependencyProperty DrawingRectangleProperty = DependencyProperty.Register(nameof(DrawingRectangle), typeof(Action<int, int, Brush, Brush>), typeof(MappingCanvas),
                                                              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public MappingCanvas()
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

        private void Mapping_Loaded(object sender, RoutedEventArgs e)
        {
            MappingImageOperate = MapOperate;
            DrawingRectangle = DrawMapRectangle;
        }
        public ObservableCollection<Die> SelectDies
        {
            get => (ObservableCollection<Die>)GetValue(SelectDiesProperty);
            set => SetValue(SelectDiesProperty, value);
        }


        public ObservableCollection<RectangleInfo> SelectRectangles
        {
            get => selectRectangles;
            set => SetValue(ref selectRectangles, value);
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


        public WriteableBitmap MapImage
        {
            get => (WriteableBitmap)GetValue(MapImageProperty);
            set => SetValue(MapImageProperty, value);
        }


        public Action<MappingOperate> MappingImageOperate
        {
            get => (Action<MappingOperate>)GetValue(MappingImageOperateProperty);
            set => SetValue(MappingImageOperateProperty, value);
        }
        /// <summary>
        /// col,row,背景色，線條
        /// </summary>
        public Action<int, int, Brush, Brush> DrawingRectangle
        {
            get => (Action<int, int, Brush, Brush>)GetValue(DrawingRectangleProperty);
            set => SetValue(DrawingRectangleProperty, value);
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

        private void ZoomFit()
        {
            double scaleW = scrollViewer.ActualWidth / MapImage.Width;
            double scaleH = scrollViewer.ActualHeight / MapImage.Height;
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



        private (List<RectangleInfo> rectangles, Canvas canvas) DrawCanvasRectangles(Die[] dice, BincodeInfo[] bincodeInfo)
        {
            List<RectangleInfo> rects = new List<RectangleInfo>();
            Stopwatch stopwatch = new Stopwatch();

            object lockObj = new object();

            int col = dice.Max(die => die.IndexX);
            int row = dice.Max(die => die.IndexY);
            int cols = col + 1;//因從0開始算 ，最大值會是總數量-1  ，要加回來
            int rows = row + 1;



            Canvas imageCanvas = new Canvas();
            imageCanvas.Width = cols * (width + Cuttingline) + width; //數量 * 寬度+線寬 +圖像BUFF 每個方框都抓20*20寬高，計算出需要的圖像大小
            imageCanvas.Height = rows * (height + Cuttingline) + height;//數量 * 高度+線寬
            imageCanvas.Background = Brushes.White;

            myGrid.Width = imageCanvas.Width;
            myGrid.Height = imageCanvas.Height;
            rects.Clear();
            imageCanvas.Children.Clear();
            var dieGroup = dice.OrderBy(d => d.IndexX).GroupBy(d => d.IndexX).ToArray();//先把X分類出來  加速後面搜尋速度
            stopwatch.Start();
            Parallel.For(0, cols, i =>
            {

                Parallel.For(0, rows, j =>
                {

                    var rect = new RectangleInfo(startPoint.X + ((width + Cuttingline) * i), startPoint.Y + ((height + Cuttingline) * j), width, height);

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
                        rects.Add(rect);
                    }

                });
            });

            var count2 = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();


            /*
            var cc=  BitmapImage.ToByteFrame();          
            var bip=   cc.ToBitmapSource();        
            WriteableBitmap aaa = new WriteableBitmap(bip);   
            var count6 = stopwatch.ElapsedMilliseconds;
           */
            return (rects, imageCanvas);
        }
        private WriteableBitmap CreateMappingImage(IEnumerable<RectangleInfo> rects, Canvas imageCanvas)
        {
            Stopwatch stopwatch = new Stopwatch();
            foreach (var rect in rects)
            {
                imageCanvas.Children.Add(CreateRectangle(rect.CenterX, rect.CenterY, rect.Width, rect.Height, rect.Fill));

            }
            var count3 = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            var bitmapImage = CreateBitmap(imageCanvas);
            var count4 = stopwatch.ElapsedMilliseconds;
            return bitmapImage;
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

            return new WriteableBitmap(bitmap.FormatConvertTo(PixelFormats.Bgr24));
        }

        private void DrawRectangle(RectangleInfo rectangleinfo, Brush fill, Brush stroke)
        {



            Rectangle rectangle = new Rectangle();
            rectangle.Width = rectangleinfo.Width;
            rectangle.Height = rectangleinfo.Height;
            rectangle.Fill = fill; // 可以根據需要設置不同的顏色
            rectangle.Stroke = stroke;
            // 設置 Rectangle 的位置
            Canvas.SetLeft(rectangle, rectangleinfo.CenterX - rectangleinfo.Width / 2); // 每個 Rectangle 的水平間距為 120
            Canvas.SetTop(rectangle, rectangleinfo.CenterY - rectangleinfo.Height / 2); // 所有 Rectangle 的垂直位置相同

            // 將 Rectangle 加入到 Canvas 中
            canvas.Children.Add(rectangle);

        }
        #endregion

        #region 滑鼠事件

        private Point selectStartPoint;
        private bool isSelectRange;
        private Rectangle selectRange;
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isTouchMode)
            {
                dragStartPoint = e.GetPosition(scrollViewer);
                myGrid.CaptureMouse();
            }
            else if (isSelectMode)
            {
                //畫出選取紅框
                isSelectRange = true;
                selectStartPoint = e.GetPosition(myGrid);
                selectRange = new Rectangle
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    Width = 0,
                    Height = 0
                };
                Canvas.SetLeft(selectRange, selectStartPoint.X);
                Canvas.SetTop(selectRange, selectStartPoint.Y);
                canvas.Children.Add(selectRange);
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
                isSelectRange = false;
                canvas.Children.RemoveAt(canvas.Children.Count - 1);//框選完清掉選取框框

                var selectRects = SelectRectInRange(selectRange, rectangles);


                if (isAdd)
                {
                    foreach (var item in selectRects)
                    {
                        SelectRectangles.Add(item);
                        DrawRectangle(item, item.Fill, Brushes.Red);
                    }

                }
                else if (isDel)
                {

                    foreach (var item in selectRects)
                    {
                        int index = SelectRectangles.IndexOf(item);
                        if (index >= 0)
                        {
                            canvas.Children.RemoveAt(index);
                            SelectRectangles.RemoveAt(index);
                        }


                    }
                }

                //這樣數量一多會很慢 需要再改
                SelectDies.Clear();
                foreach (var item in SelectRectangles)
                {

                    var die = Dies.Where(d => d.IndexX == item.Col && d.IndexY == item.Row).FirstOrDefault();
                    SelectDies.Add(die);


                }


                // Point pixel = e.GetPosition(myGrid);
                //  DrawMapRectangle(pixel);

            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {

            Point currentPoint = e.GetPosition(canvas);

            if (isSelectMode && isSelectRange && selectRange != null)//如果選取模式下
            {
                //變更畫出的選取紅框
                double width = currentPoint.X - selectStartPoint.X;
                double height = currentPoint.Y - selectStartPoint.Y;
                if (width <= 1)
                    selectRange.Width = 1;
                else
                    selectRange.Width = width;

                if (height <= 1)
                    selectRange.Height = 1;
                else
                    selectRange.Height = height;


               
            }
            else if (myGrid.IsMouseCaptured) //拖曳移動功能
            {
              //  if (!myGrid.IsMouseCaptured) return;
                Point dragEndPoint = e.GetPosition(scrollViewer);
          
                double offsetX = dragEndPoint.X - dragStartPoint.X;
                double offsetY = dragEndPoint.Y - dragStartPoint.Y;

                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - offsetX);
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offsetY);

                dragStartPoint = dragEndPoint;

                
            }
            //計算當下滑鼠指向哪一顆
            var dieIndwx = rectangles.Where(r => r.Rectangle.Contains(currentPoint)).FirstOrDefault();
            if (dieIndwx != null)
            {
                Col = dieIndwx.Col;
                Row = dieIndwx.Row;
            }
        }

        private void DrawMapRectangle(Point pixel)
        {

            var selectRect = rectangles.Where(rect => rect.Rectangle.Contains(pixel)).FirstOrDefault();
            if (selectRect == null) return;


            DrawRectangle(selectRect, selectRect.Fill, Brushes.Red);
            //  DrawMapRectangle(selectRect.Col, selectRect.Row, Brushes.DarkBlue, Brushes.MintCream);
        }

        //col,row,背景色，線條
        private void DrawMapRectangle(int indexX, int indexY, Brush fill, Brush stroke)
        {

            var selectRect = rectangles.Where(rect => rect.Col == indexX && rect.Row == indexY).FirstOrDefault();
            if (selectRect == null) return;


            DrawRectangle(selectRect, fill, stroke);
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

        #endregion


        public ICommand BtnOperateCommand => new RelayCommand<string>(par =>
        {
            try
            {
                switch (par)
                {
                    case "selectAdd":
                        isSelectMode = true;
                        isTouchMode = false;
                        isAdd = true;
                        isDel = false;
                        break;
                    case "selectDel":
                        isSelectMode = true;
                        isTouchMode = false;
                        isAdd = false;
                        isDel = true;
                        break;

                    case "touch":
                        isSelectMode = false;
                        isTouchMode = true;
                        break;

                    case "createRetagle":
                        MapOperate(MappingOperate.Create);
                        break;
                    case "fit":
                        ZoomFit();
                        break;

                    case "clear":
                        canvas.Children.Clear();
                        break;


                    default:
                        break;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });
        private void CreateRetagle_Click(object sender, RoutedEventArgs e)
        {

            MapOperate(MappingOperate.Create);
        }



        private IEnumerable<RectangleInfo> SelectRectInRange(Rectangle selectRange, IEnumerable<RectangleInfo> rectInfos)
        {
            var rect = new Rect(Canvas.GetLeft(selectRange), Canvas.GetTop(selectRange), selectRange.Width, selectRange.Height);
            IEnumerable<RectangleInfo> selectRects = new List<RectangleInfo>();
            if (selectRange.Width <= width || selectRange.Height <= Height)
            {
                selectRects = rectInfos.Where(r => r.Rectangle.Contains(rect.TopLeft) || r.Rectangle.Contains(rect.BottomLeft)
                                   || r.Rectangle.Contains(rect.BottomRight) || r.Rectangle.Contains(rect.TopRight));

            }
            else
            {
                selectRects = rectInfos.Where(r => rect.Contains(r.Rectangle.TopLeft) || rect.Contains(r.Rectangle.BottomLeft)
                                   || rect.Contains(r.Rectangle.BottomRight) || rect.Contains(r.Rectangle.TopRight));


            }
            return selectRects;
        }

        private void MapOperate(MappingOperate operate)
        {

            switch (operate)
            {
                case MappingOperate.Create:
                    var rects = DrawCanvasRectangles(Dies, BincodeInFomation);
                    rectangles = rects.rectangles;
                    MapImage = CreateMappingImage(rects.rectangles, rects.canvas);
                    ZoomFit();
                    break;
                case MappingOperate.Fit:
                    ZoomFit();
                    break;
                case MappingOperate.Clear:
                    canvas.Children.Clear();
                    break;
                default:
                    break;
            }



        }

        private static void OnDiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as MappingCanvas;
            dp.DrawMapping();
        }

        private void DrawMapping()
        {
            if (Dies == null) return;
            if (MapImage != null) return;


            var rects = DrawCanvasRectangles(Dies, BincodeInFomation); //取Inedex最大值 會是總數量-1  ，所以要+1回來
            rectangles = rects.rectangles;
            //    BitmapImage = CreateBitmapImage(rects);//由外部控制畫圖 ，所以不自動畫圖了
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




    public class ImagePositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double imageSize && parameter is double offset)
            {
                return imageSize / 2 - offset / 2;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
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

    public enum MappingOperate
    {
        Create,
        Fit,
        Clear,
    }
}
