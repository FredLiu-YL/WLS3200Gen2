using System;
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
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// CassetteUC.xaml 的互動邏輯
    /// </summary>
    /// 
    public partial class CassetteUnitUC : UserControl, INotifyPropertyChanged
    {
         
        public CassetteUnitUC()
        {
            InitializeComponent();

        }

        public static readonly DependencyProperty IsMacroTopProperty = DependencyProperty.Register(nameof(IsMacroTop), typeof(bool), typeof(CassetteUnitUC),
                                                                                 new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsMacroBackProperty = DependencyProperty.Register(nameof(IsMacroBack), typeof(bool), typeof(CassetteUnitUC),
                                                                                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty IsMicroProperty = DependencyProperty.Register(nameof(IsMicro), typeof(bool), typeof(CassetteUnitUC),
                                                                             new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsMacroTop
        {
            get => (bool)GetValue(IsMacroTopProperty);
            set => SetValue(IsMacroTopProperty, value);
        }
        public bool IsMacroBack
        {
            get => (bool)GetValue(IsMacroBackProperty);
            set => SetValue(IsMacroBackProperty, value);
        }
        public bool IsMicro
        {
            get => (bool)GetValue(IsMicroProperty);
            set => SetValue(IsMicroProperty, value);
        }


        public ICommand Top_Command => new RelayCommand(async () =>
        {
            try
            {
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand Back_Command => new RelayCommand(async () =>
        {
            try
            {
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        });
        public ICommand Micro_Command => new RelayCommand(async () =>
        {
            try
            {
                
            }
            catch (Exception ex)
            {

                throw ex;
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
    public class WorkItem : INotifyPropertyChanged
    {
        private bool isTop;

        private bool isBack;

        private bool isMicro;

        private Brush backGroundTop;

        private Brush backGroundBack;

        private Brush backGroundMicro;

        public bool IsTop { get => isTop; set => SetValue(ref isTop, value); }
        public bool IsBack { get => isBack; set => SetValue(ref isBack, value); }
        public bool IsMicro { get => isMicro; set => SetValue(ref isMicro, value); }

        public Brush BackGroundTop { get => backGroundTop; set => SetValue(ref backGroundTop, value); }
        public Brush BackGroundBack { get => backGroundBack; set => SetValue(ref backGroundBack, value); }
        public Brush BackGroundMicro { get => backGroundMicro; set => SetValue(ref backGroundMicro, value); }


        //變更顏色-

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
