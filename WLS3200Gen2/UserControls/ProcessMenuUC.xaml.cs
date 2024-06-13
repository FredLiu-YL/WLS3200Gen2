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
        private bool isDegree0, isDegree90, isDegree180, isDegree270;
        private bool isNoRotate, isForwardRotate, isBackwardRotate;
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
            set
            {
                if (value == Degree.Degree0)
                {
                    IsDegree0 = true;
                }
                else if (value == Degree.Degree90)
                {
                    IsDegree90 = true;
                }
                else if (value == Degree.Degree180)
                {
                    IsDegree180 = true;
                }
                else if (value == Degree.Degree270)
                {
                    IsDegree270 = true;
                }
                SetValue(DegreeUnLoadProperty, value);
            }
        }
        /// <summary>
        /// 金面不旋轉、一直正轉、一直反轉
        /// </summary>
        public TopContinueRotate TopContinueRotate
        {
            get => (TopContinueRotate)GetValue(TopContinueRotateProperty);
            set
            {
                if (value == TopContinueRotate.No)
                {
                    IsNoRotate = true;
                }
                else if (value == TopContinueRotate.Forward)
                {
                    IsForwardRotate = true;
                }
                else if (value == TopContinueRotate.Backward)
                {
                    IsBackwardRotate = true;
                }
                SetValue(TopContinueRotateProperty, value);
            }
        }
        public bool IsDegree0
        {
            get => isDegree0;
            set
            {
                if (value && isDegree0 != value)
                {
                    SetValue(ref isDegree0, value);
                    DegreeUnLoad = Degree.Degree0;
                }
                SetValue(ref isDegree0, value);
            }
        }
        public bool IsDegree90
        {
            get => isDegree90;
            set
            {
                if (value && isDegree90 != value)
                {
                    SetValue(ref isDegree90, value);
                    DegreeUnLoad = Degree.Degree90;
                }
                SetValue(ref isDegree90, value);
            }
        }
        public bool IsDegree180
        {
            get => isDegree180;
            set
            {
                if (value && isDegree180 != value)
                {
                    SetValue(ref isDegree180, value);
                    DegreeUnLoad = Degree.Degree180;
                }
                SetValue(ref isDegree180, value);
            }
        }
        public bool IsDegree270
        {
            get => isDegree270;
            set
            {
                if (value && isDegree270 != value)
                {
                    SetValue(ref isDegree270, value);
                    DegreeUnLoad = Degree.Degree90;
                }
                SetValue(ref isDegree270, value);
            }
        }
        public bool IsNoRotate
        {
            get => isNoRotate;
            set
            {
                if (value && isNoRotate != value)
                {
                    SetValue(ref isNoRotate, value);
                    TopContinueRotate = TopContinueRotate.No;
                }
                SetValue(ref isNoRotate, value);
            }
        }
        public bool IsForwardRotate
        {
            get => isForwardRotate;
            set
            {
                if (value && isForwardRotate != value)
                {
                    SetValue(ref isForwardRotate, value);
                    TopContinueRotate = TopContinueRotate.Forward;
                }
                SetValue(ref isForwardRotate, value);
            }
        }
        public bool IsBackwardRotate
        {
            get => isBackwardRotate;
            set
            {
                if (value && isBackwardRotate != value)
                {
                    SetValue(ref isBackwardRotate, value);
                    TopContinueRotate = TopContinueRotate.Backward;
                }
                SetValue(ref isBackwardRotate, value);
            }
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
