﻿using System;
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
    public partial class WorkholderUI : UserControl, INotifyPropertyChanged
    {
        private Axis[] axes;

        public static readonly DependencyProperty TableXProperty = DependencyProperty.Register(nameof(TableX), typeof(Axis), typeof(WorkholderUI),
                                                                                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TableYProperty = DependencyProperty.Register(nameof(TableY), typeof(Axis), typeof(WorkholderUI),
                                                                                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty AxesProperty = DependencyProperty.Register(nameof(Axes), typeof(IEnumerable<Axis>), typeof(WorkholderUI),
                                                                                       new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                                                                                           new PropertyChangedCallback((d, e) =>
                                                                                               {
                                                                                                   var dp = d as WorkholderUI;
                                                                                                   dp.InitialMotionController();
                                                                                               })));


        public WorkholderUI()
        {
            InitializeComponent();
        }
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void InitialMotionController()
        {
            try
            {
                axes = Axes.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
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


        public IEnumerable<Axis> Axes
        {
            get => (IEnumerable<Axis>)GetValue(AxesProperty);
            set => SetValue(AxesProperty, value);
        }
        public ICommand TableContinueMoveCommand => new RelayCommand<string>(async key =>
        {
            try
            {


                var dis = 0;// Convert.ToDouble();
                if (dis == 0)
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

                    }

                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        });















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