using GalaSoft.MvvmLight.Command;
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
using WLS3200Gen2;
using YuanliCore.Data;

namespace YuanliCore.Model.UserControls
{
    /// <summary>
    /// CassetteStateUC.xaml 的互動邏輯
    /// </summary>
    public partial class CassetteStateUC : UserControl, INotifyPropertyChanged
    {
        private int EarlierSelectIndex = -1;

        public CassetteStateUC()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty LoadPortWafersProperty = DependencyProperty.Register(nameof(LoadPortWafers), typeof(ObservableCollection<WaferUIData>), typeof(CassetteStateUC),
                                                                                     new FrameworkPropertyMetadata(new ObservableCollection<WaferUIData>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty SelectIndexProperty = DependencyProperty.Register(nameof(SelectIndex), typeof(int), typeof(CassetteStateUC),
                                                                                     new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public ObservableCollection<WaferUIData> LoadPortWafers
        {
            get => (ObservableCollection<WaferUIData>)GetValue(LoadPortWafersProperty);
            set => SetValue(LoadPortWafersProperty, value);
        }
        public int SelectIndex
        {
            get => (int)GetValue(SelectIndexProperty);
            set => SetValue(SelectIndexProperty, value);
        }

        public ICommand SelectedCommand => new RelayCommand(() =>
        {
            try
            {
                foreach (var item in LoadPortWafers)
                {
                    if (item.WaferStates != WaferProcessStatus.None)
                        item.WaferStates = WaferProcessStatus.NotSelect;
                }

                LoadPortWafers[SelectIndex].WaferStates = WaferProcessStatus.Select;

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

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



        private void ListBox_Selected(object sender, RoutedEventArgs e)
        {
            foreach (var item in LoadPortWafers)
            {
                if (item.WaferStates != WaferProcessStatus.None)
                    item.WaferStates = WaferProcessStatus.NotSelect;
            }

            LoadPortWafers[SelectIndex].WaferStates = WaferProcessStatus.Select;

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in LoadPortWafers)
            {
                if (item.WaferStates != WaferProcessStatus.None)
                    item.WaferStates = WaferProcessStatus.NotSelect;
            }
            if (SelectIndex != -1 && (LoadPortWafers.Count - 1) >= SelectIndex && LoadPortWafers[SelectIndex].WaferStates != WaferProcessStatus.None)
            {
                LoadPortWafers[SelectIndex].WaferStates = WaferProcessStatus.Select;
                EarlierSelectIndex = SelectIndex;
            }
            else
            {
                SelectIndex = EarlierSelectIndex;
            }


        }
    }
}
