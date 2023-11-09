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
using GalaSoft.MvvmLight.CommandWpf;
using MaterialDesignThemes.Wpf;
using YuanliCore.Model.LoadPort;

namespace WLS3200Gen2.UserControls
{
    /// <summary>
    /// Informaiton.xaml 的互動邏輯
    /// </summary>
    public partial class ProductionUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty AutoSaveIsCheckedProperty = DependencyProperty.Register(nameof(AutoSaveIsChecked), typeof(bool), typeof(ProductionUC),
                                                                                    new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty CircleIsCheckedProperty = DependencyProperty.Register(nameof(CircleIsChecked), typeof(bool), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty DieInsideAllCheckedProperty = DependencyProperty.Register(nameof(DieInsideAllChecked), typeof(bool), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty AddButtonActionProperty = DependencyProperty.Register(nameof(AddButtonAction), typeof(ICommand), typeof(ProductionUC),
                                                                                     new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty CassetteUCProperty = DependencyProperty.Register(nameof(CassetteUC), typeof(ObservableCollection<CassetteUC>), typeof(ProductionUC),
                                                                                                     new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public ProductionUC()
        {
            InitializeComponent();
        }
        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AddButtonAction = new RelayCommand<int>(key => { AddButton(key); });
                CassetteUC = new ObservableCollection<CassetteUC>();
                AddButton(25);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool AutoSaveIsChecked
        {
            get => (bool)GetValue(AutoSaveIsCheckedProperty);
            set => SetValue(AutoSaveIsCheckedProperty, value);
        }
        public bool CircleIsChecked
        {
            get => (bool)GetValue(CircleIsCheckedProperty);
            set => SetValue(CircleIsCheckedProperty, value);
        }
        public bool DieInsideAllChecked
        {
            get => (bool)GetValue(DieInsideAllCheckedProperty);
            set => SetValue(DieInsideAllCheckedProperty, value);
        }
        public ICommand AddButtonAction
        {
            get => (ICommand)GetValue(AddButtonActionProperty);
            set => SetValue(AddButtonActionProperty, value);
        }
        public ObservableCollection<CassetteUC> CassetteUC
        {
            get => (ObservableCollection<CassetteUC>)GetValue(CassetteUCProperty);
            set { SetValue(CassetteUCProperty, value); }
        }
        public void AddButton(int Rows)
        {
            try
            {
                CassetteUC.Clear();
                for (int i = 1; i <= Rows; i++)
                {
                    if (i % 2 == 0)
                    {
                        CassetteUC.Add(new CassetteUC(true));
                    }
                    else
                    {
                        CassetteUC.Add(new CassetteUC(false));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
