using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Runtime.CompilerServices;
using YuanliCore.Motion;
using GalaSoft.MvvmLight.Command;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// WorkholderUI.xaml 的互動邏輯
    /// </summary>
    public partial class WorkholderUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TableXProperty = DependencyProperty.Register(nameof(TableX), typeof(Axis), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TableYProperty = DependencyProperty.Register(nameof(TableY), typeof(Axis), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty TableXMaxVelProperty = DependencyProperty.Register(nameof(TableXMaxVel), typeof(double), typeof(WorkholderUC),
                                                                                      new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TableYMaxVelProperty = DependencyProperty.Register(nameof(TableYMaxVel), typeof(double), typeof(WorkholderUC),
                                                                                      new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty HighIsCheckedProperty = DependencyProperty.Register(nameof(HighIsChecked), typeof(bool), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty LowIsCheckedProperty = DependencyProperty.Register(nameof(LowIsChecked), typeof(bool), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty RelativeIsCheckedProperty = DependencyProperty.Register(nameof(RelativeIsChecked), typeof(bool), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TablePosXProperty = DependencyProperty.Register(nameof(TablePosX), typeof(double), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(0.00, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TablePosYProperty = DependencyProperty.Register(nameof(TablePosY), typeof(double), typeof(WorkholderUC),
                                                                                       new FrameworkPropertyMetadata(0.00, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private string tableDistance;

        private TableMoveType tableMoveType;

        public double TablePosX
        {
            get => (double)GetValue(TablePosXProperty);
            set => SetValue(TablePosXProperty, value);
        }
        public double TablePosY
        {
            get => (double)GetValue(TablePosYProperty);
            set => SetValue(TablePosYProperty, value);
        }
        public string TableDistance { get => tableDistance; set => SetValue(ref tableDistance, value); }
        public bool HighIsChecked
        {
            get => (bool)GetValue(HighIsCheckedProperty);
            set => SetValue(HighIsCheckedProperty, value);
        }
        public bool LowIsChecked
        {
            get => (bool)GetValue(LowIsCheckedProperty);
            set => SetValue(LowIsCheckedProperty, value);
        }
        public bool RelativeIsChecked
        {
            get => (bool)GetValue(RelativeIsCheckedProperty);
            set => SetValue(RelativeIsCheckedProperty, value);
        }
        public WorkholderUC()
        {
            InitializeComponent();
            HighIsChecked = true;
            tableMoveType = TableMoveType.High;
            TableDistance = "100";
        }
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }
        public Axis TableX
        {
            get => (Axis)GetValue(TableXProperty);
            set => SetValue(TableXProperty, value);
        }
        public Axis TableY
        {
            get => (Axis)GetValue(TableYProperty);
            set => SetValue(TableYProperty, value);
        }
        public double TableXMaxVel
        {
            get => (double)GetValue(TableXMaxVelProperty);
            set => SetValue(TableXMaxVelProperty, value);
        }
        public double TableYMaxVel
        {
            get => (double)GetValue(TableYMaxVelProperty);
            set => SetValue(TableYMaxVelProperty, value);
        }
        public ICommand TableContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                if (HighIsChecked == true)
                {
                    tableMoveType = TableMoveType.High;
                }
                else if (LowIsChecked == true)
                {
                    tableMoveType = TableMoveType.Low;
                }
                else if (RelativeIsChecked == true)
                {
                    tableMoveType = TableMoveType.RelaTive;
                }
                if (tableMoveType == TableMoveType.Low)
                {

                    TableX.AxisVelocity.MaxVel = TableXMaxVel / 10;
                    TableY.AxisVelocity.MaxVel = TableYMaxVel / 10;
                }
                else
                {
                    TableX.AxisVelocity.MaxVel = TableXMaxVel;
                    TableY.AxisVelocity.MaxVel = TableYMaxVel;
                }
                //var dis = Math.Round(Convert.ToDouble(TableDistance));
                if (tableMoveType == TableMoveType.High || tableMoveType == TableMoveType.Low)
                {
                    switch (key)
                    {
                        case "X-":
                            await TableX.MoveToAsync(TableX.PositionNEL);
                            break;
                        case "X+":
                            await TableX.MoveToAsync(TableX.PositionPEL);
                            break;
                        case "Y+":
                            await TableY.MoveToAsync(TableY.PositionPEL);
                            break;
                        case "Y-":
                            await TableY.MoveToAsync(TableY.PositionNEL);
                            break;
                        case "X+Y+":
                            await Task.WhenAll(TableX.MoveToAsync(TableX.PositionPEL), TableY.MoveToAsync(TableY.PositionPEL));
                            break;
                        case "X+Y-":
                            await Task.WhenAll(TableX.MoveToAsync(TableX.PositionPEL), TableY.MoveToAsync(TableY.PositionNEL));
                            break;
                        case "X-Y+":
                            await Task.WhenAll(TableX.MoveToAsync(TableX.PositionNEL), TableY.MoveToAsync(TableY.PositionPEL));
                            break;
                        case "X-Y-":
                            await Task.WhenAll(TableX.MoveToAsync(TableX.PositionNEL), TableY.MoveToAsync(TableY.PositionNEL));
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
                    TableX.Stop();
                    TableY.Stop();
                }
                else
                {
                    switch (key)
                    {
                        case "X+":
                            await TableX.MoveAsync(dis);
                            break;
                        case "X-":
                            await TableX.MoveAsync(-dis);
                            break;
                        case "Y+":
                            await TableY.MoveAsync(dis);
                            break;
                        case "Y-":
                            await TableY.MoveAsync(-dis);
                            break;


                        case "X+Y+":
                            await Task.WhenAll(TableX.MoveAsync(dis), TableY.MoveAsync(dis));
                            break;
                        case "X+Y-":
                            await Task.WhenAll(TableX.MoveAsync(dis), TableY.MoveAsync(-dis));
                            break;
                        case "X-Y+":
                            await Task.WhenAll(TableX.MoveAsync(-dis), TableY.MoveAsync(dis));
                            break;
                        case "X-Y-":
                            await Task.WhenAll(TableX.MoveAsync(-dis), TableY.MoveAsync(-dis));
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
