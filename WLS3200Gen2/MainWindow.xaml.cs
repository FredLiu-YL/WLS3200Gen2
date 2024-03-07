using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace WLS3200Gen2
{
    /// <summary>
    /// MainWindow1.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // 检查是否点击在标题栏上
            if (e.LeftButton == MouseButtonState.Pressed && e.OriginalSource is FrameworkElement sourceElement)
            {
                if (IsTitleBar(sourceElement))
                {
                    // 取消事件的进一步处理，防止拖动窗口
                    e.Handled = true;
                }
            }
        }

        // 检查元素是否是标题栏
        private bool IsTitleBar(FrameworkElement element)
        {
            while (element != null)
            {
                if (element.Name == "PART_TitleBar")
                {
                    return true;
                }
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }
            return false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OpenPalette_Click(object sender, RoutedEventArgs e)
        {
            double ss = 0;
            ss = 3;
        }
    }
}
