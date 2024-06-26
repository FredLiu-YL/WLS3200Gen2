﻿using System;
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
using GalaSoft.MvvmLight.Command;
using YuanliCore.CameraLib;
using YuanliCore.Model.Interface.Component;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// WaferIDUnitUC.xaml 的互動邏輯
    /// </summary>
    public partial class ReaderUnitUC : UserControl, INotifyPropertyChanged
    {
        private static readonly DependencyProperty ReaderProperty = DependencyProperty.Register(nameof(Reader), typeof(IReader), typeof(ReaderUnitUC),
                                                                                    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private string paramID, result;
        private BitmapSource resultImage;
        public ReaderUnitUC()
        {
            InitializeComponent();
        }
        public IReader Reader
        {
            get => (IReader)GetValue(ReaderProperty);
            set => SetValue(ReaderProperty, value);
        }
        public string ParamID
        {
            get => paramID;
            set => SetValue(ref paramID, value);
        }
        public string Result
        {
            get => result;
            set => SetValue(ref result, value);
        }
        public BitmapSource ResultImage
        {
            get => resultImage;
            set => SetValue(ref resultImage, value);
        }

        public ICommand SetParam => new RelayCommand<string>(async key =>
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        });
        public ICommand GetResult => new RelayCommand<string>(async key =>
        {
            try
            {
                await Reader.ReadAsync();
                ResultImage = Reader.Image.ToBitmapSource();
            }
            catch (Exception)
            {

                throw;
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
