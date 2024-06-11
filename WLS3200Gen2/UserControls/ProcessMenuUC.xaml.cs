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
using WLS3200Gen2.Model.Recipe;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// ProcessMenuUC.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessMenuUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty IsAutoSaveProperty = DependencyProperty.Register(nameof(IsAutoSave), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsWaferCirclePhotoProperty = DependencyProperty.Register(nameof(IsWaferCirclePhoto), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsDieAllPhotoProperty = DependencyProperty.Register(nameof(IsDieAllPhoto), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsDegreeUnLoadProperty = DependencyProperty.Register(nameof(IsDegreeUnLoad), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsSecondFlipProperty = DependencyProperty.Register(nameof(IsSecondFlip), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsTestRunProperty = DependencyProperty.Register(nameof(IsTestRun), typeof(bool), typeof(ProcessMenuUC),
                                                                                         new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty SecondFlipPosProperty = DependencyProperty.Register(nameof(SecondFlipPos), typeof(int), typeof(ProcessMenuUC),
                                                                                 new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty DegreeUnLoadProperty = DependencyProperty.Register(nameof(DegreeUnLoad), typeof(Degree), typeof(ProcessMenuUC),
                                                                                 new FrameworkPropertyMetadata(Degree.Degree0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TopContinueRotateProperty = DependencyProperty.Register(nameof(TopContinueRotate), typeof(TopContinueRotate), typeof(ProcessMenuUC),
                                                                                 new FrameworkPropertyMetadata(TopContinueRotate.No, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ProcessMenuUC()
        {
            InitializeComponent();
        }
        public bool IsAutoSave
        {
            get => (bool)GetValue(IsAutoSaveProperty);
            set => SetValue(IsAutoSaveProperty, value);
        }
        public bool IsWaferCirclePhoto
        {
            get => (bool)GetValue(IsWaferCirclePhotoProperty);
            set => SetValue(IsWaferCirclePhotoProperty, value);
        }
        public bool IsDieAllPhoto
        {
            get => (bool)GetValue(IsDieAllPhotoProperty);
            set => SetValue(IsDieAllPhotoProperty, value);
        }
        public bool IsDegreeUnLoad
        {
            get => (bool)GetValue(IsDegreeUnLoadProperty);
            set => SetValue(IsDegreeUnLoadProperty, value);
        }
        public bool IsSecondFlip
        {
            get => (bool)GetValue(IsSecondFlipProperty);
            set => SetValue(IsSecondFlipProperty, value);
        }
        public bool IsTestRun
        {
            get => (bool)GetValue(IsTestRunProperty);
            set => SetValue(IsTestRunProperty, value);
        }
        /// <summary>
        /// 二次翻背位置
        /// </summary>
        public int SecondFlipPos
        {
            get => (int)GetValue(SecondFlipPosProperty);
            set => SetValue(SecondFlipPosProperty, value);
        }
        /// <summary>
        /// 出貨角度
        /// </summary>
        public Degree DegreeUnLoad
        {
            get => (Degree)GetValue(DegreeUnLoadProperty);
            set => SetValue(DegreeUnLoadProperty, value);
        }
        /// <summary>
        /// 金面不旋轉、一直正轉、一直反轉
        /// </summary>
        public TopContinueRotate TopContinueRotate
        {
            get => (TopContinueRotate)GetValue(TopContinueRotateProperty);
            set => SetValue(TopContinueRotateProperty, value);
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
}
