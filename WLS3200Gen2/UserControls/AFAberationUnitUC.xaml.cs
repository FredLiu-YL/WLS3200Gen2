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
using System.Windows.Shapes;
using YuanliCore.Model.Interface;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// AFAberationUnitUC.xaml 的互動邏輯
    /// </summary>
    public partial class AFAberationUnitUC : UserControl, INotifyPropertyChanged
    {
        private static readonly DependencyProperty MicroscopeProperty = DependencyProperty.Register(nameof(Microscope), typeof(IMicroscope), typeof(AFAberationUnitUC),
                                                                                   new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty AFAberationUIShowProperty = DependencyProperty.Register(nameof(AFAberationUIShow), typeof(AFAberationUI), typeof(AFAberationUnitUC),
                                                                                    new FrameworkPropertyMetadata(new AFAberationUI(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        private bool isAberationMove = true;
        private int distanceAberation = 0;
        public IMicroscope Microscope
        {
            get => (IMicroscope)GetValue(MicroscopeProperty);
            set => SetValue(MicroscopeProperty, value);
        }
        public AFAberationUI AFAberationUIShow
        {
            get => (AFAberationUI)GetValue(AFAberationUIShowProperty);
            set => SetValue(AFAberationUIShowProperty, value);
        }
        public AFAberationUnitUC()
        {
            InitializeComponent();
        }
        public bool IsAberationMove
        {
            get => isAberationMove;
            set
            {
                SetValue(ref isAberationMove, value);
            }
        }
        public int DistanceAberation
        {
            get => distanceAberation;
            set
            {
                SetValue(ref distanceAberation, value);
            }
        }
        public ICommand AberationMove => new RelayCommand<string>(async key =>
        {
            try
            {
                IsAberationMove = false;
                switch (key)
                {
                    case "Up":
                        await Microscope.AberrationMoveCommand(-DistanceAberation);
                        break;
                    case "Down":
                        await Microscope.AberrationMoveCommand(DistanceAberation);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsAberationMove = true;
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
    public class AFAberationUI : INotifyPropertyChanged
    {
        private int aberationValue;
        public int AberationValue
        {
            get => aberationValue;
            set => SetValue(ref aberationValue, value);
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
