using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    public partial class MappingCanvas : UserControl
    {

        private bool isDragging;
        private Point lastMousePosition;
        private BitmapImage bitmapImage;
        public MappingCanvas()
        {
            InitializeComponent();

           /* 
            bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri("9.bmp");
            bitmapImage.EndInit();
            myImage.Source = bitmapImage;
           */
            // 初始化矩形的位置集合
            InitializeRectangles();

        }
        public ObservableCollection<Point> Rectangles { get; } = new ObservableCollection<Point>();
        private void InitializeRectangles()
        {
            // 添加20个等距的矩形的位置到集合中
            for (int i = 0; i < 20; i++)
            {
                Rectangles.Add(new Point(i * 50, 0)); // 在这里设置矩形的初始位置，这里的 50 是矩形之间的间距
            }

            // 设置 DataContext，将集合绑定到 XAML 中的 ItemsControl

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
            double minScale = 0.1;
            double maxScale = 10;

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

    }





}
