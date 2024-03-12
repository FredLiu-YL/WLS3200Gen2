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
    public partial class MappingCanvas : UserControl, INotifyPropertyChanged
    {

        private bool isDragging;
        private Point lastMousePosition;
        private BitmapImage bitmapImage;

        private ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>();
        public MappingCanvas()
        {
            InitializeComponent();

           
            /*bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("D:\\9.bmp");
            bitmapImage.EndInit();
            myImage.Source = bitmapImage;*/
           
            // 初始化矩形的位置集合
       
          //  CreateRectangles();
        }
        public ObservableCollection<Rectangle> Rectangles
        {
            get => rectangles;
            set => SetValue(ref rectangles, value);
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                /*   double newX = scrollViewer.HorizontalOffset - offset.X;
                   double newY = scrollViewer.VerticalOffset - offset.Y;
                   scrollViewer.ScrollToHorizontalOffset(newX);
                   scrollViewer.ScrollToVerticalOffset(newY);*/

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



        private Point lastPosition;

   

        private void CreateRectangles()
        {
            for (int i = 0; i < 100; i++)
            {
                var t = new Rectangle();
                t.Width = 40;
              //  t.ActualWidth = 40;
                t.Height = 40;
             //   t.ActualHeight = 40;
                t.Fill = Brushes.Red;

                Rectangles.Add(t);
            }
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            lastPosition = e.GetPosition(this);
            ((Rectangle)sender).CaptureMouse();
        }

        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var currentPosition = e.GetPosition(this);
                var offset = currentPosition - lastPosition;
                Canvas.SetLeft((Rectangle)sender, Canvas.GetLeft((Rectangle)sender) + offset.X);
                Canvas.SetTop((Rectangle)sender, Canvas.GetTop((Rectangle)sender) + offset.Y);
                lastPosition = currentPosition;
            }
        }

        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            ((Rectangle)sender).ReleaseMouseCapture();
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


}
