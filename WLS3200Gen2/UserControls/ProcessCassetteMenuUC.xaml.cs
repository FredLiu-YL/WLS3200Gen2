﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// ProcessCassetteMenuUC.xaml 的互動邏輯
    /// </summary>
    public partial class ProcessCassetteMenuUC : UserControl, INotifyPropertyChanged
    {
     


        public ProcessCassetteMenuUC()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty ProcessStationProperty = DependencyProperty.Register(nameof(ProcessStation), typeof(ObservableCollection<ProcessStationAssign>), typeof(ProcessCassetteMenuUC),
                                                                                  new FrameworkPropertyMetadata(new ObservableCollection<ProcessStationAssign>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<ProcessStationAssign> ProcessStation
        {
            get => (ObservableCollection<ProcessStationAssign>)GetValue(ProcessStationProperty);
            set => SetValue(ProcessStationProperty, value);
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
