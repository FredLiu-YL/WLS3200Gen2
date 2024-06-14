using GalaSoft.MvvmLight.Command;
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
using YuanliCore.Motion;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// AxisZUC.xaml 的互動邏輯
    /// </summary>
    public partial class AxisZUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TableZProperty = DependencyProperty.Register(nameof(TableZ), typeof(Axis), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TableZMaxVelProperty = DependencyProperty.Register(nameof(TableZMaxVel), typeof(double), typeof(WorkholderUC),
                                                                                      new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ZHighIsCheckedProperty = DependencyProperty.Register(nameof(ZHighIsChecked), typeof(bool), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ZLowIsCheckedProperty = DependencyProperty.Register(nameof(ZLowIsChecked), typeof(bool), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty ZRelativeIsCheckedProperty = DependencyProperty.Register(nameof(ZRelativeIsChecked), typeof(bool), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TablePosZProperty = DependencyProperty.Register(nameof(TablePosZ), typeof(double), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(0.00, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private string tableDistance;

        private TableMoveType tableMoveType;

        public double TablePosZ
        {
            get => (double)GetValue(TablePosZProperty);
            set => SetValue(TablePosZProperty, value);
        }
        public string TableDistance { get => tableDistance; set => SetValue(ref tableDistance, value); }
        public bool ZHighIsChecked
        {
            get => (bool)GetValue(ZHighIsCheckedProperty);
            set => SetValue(ZHighIsCheckedProperty, value);
        }
        public bool ZLowIsChecked
        {
            get => (bool)GetValue(ZLowIsCheckedProperty);
            set => SetValue(ZLowIsCheckedProperty, value);
        }
        public bool ZRelativeIsChecked
        {
            get => (bool)GetValue(ZRelativeIsCheckedProperty);
            set => SetValue(ZRelativeIsCheckedProperty, value);
        }
        public AxisZUC()
        {
            InitializeComponent();
        }
        public Axis TableZ
        {
            get => (Axis)GetValue(TableZProperty);
            set => SetValue(TableZProperty, value);
        }
        public double TableZMaxVel
        {
            get => (double)GetValue(TableZMaxVelProperty);
            set => SetValue(TableZMaxVelProperty, value);
        }
        public ICommand TableContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                if (ZHighIsChecked == true)
                {
                    tableMoveType = TableMoveType.High;
                }
                else if (ZLowIsChecked == true)
                {
                    tableMoveType = TableMoveType.Low;
                }
                else if (ZRelativeIsChecked == true)
                {
                    tableMoveType = TableMoveType.RelaTive;
                }
                if (tableMoveType == TableMoveType.Low)
                {
                    TableZ.AxisVelocity.MaxVel = TableZMaxVel / 10;
                }
                else
                {
                    TableZ.AxisVelocity.MaxVel = TableZMaxVel;
                }
                //var dis = Math.Round(Convert.ToDouble(TableDistance));
                if (tableMoveType == TableMoveType.High || tableMoveType == TableMoveType.Low)
                {
                    switch (key)
                    {
                        case "Z-":
                            await TableZ.MoveToAsync(TableZ.PositionNEL);
                            break;
                        case "Z+":
                            await TableZ.MoveToAsync(TableZ.PositionPEL);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand TableMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                var dis = Math.Round(Convert.ToDouble(TableDistance));
                if (tableMoveType == TableMoveType.High || tableMoveType == TableMoveType.Low)
                {
                    TableZ.Stop();
                    TableZ.Stop();
                }
                else
                {
                    switch (key)
                    {
                        case "Z+":
                            await TableZ.MoveAsync(dis);
                            break;
                        case "Z-":
                            await TableZ.MoveAsync(-dis);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public enum TableMoveType
        {
            High = 0,
            Low = 1,
            RelaTive
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
