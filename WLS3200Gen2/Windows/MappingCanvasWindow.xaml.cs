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
using System.Windows.Shapes;
using WLS3200Gen2.Model.Recipe;

namespace WLS3200Gen2
{
    /// <summary>
    /// BincodeSettingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MappingCanvasWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<BincodeInfo> bincodeList = new ObservableCollection<BincodeInfo>();
        private int selectList, column, row;
        public MappingCanvasWindow(int column, int row)
        {
            InitializeComponent();
            this.Column = column;
            this.Row = row;
        }



        public ObservableCollection<BincodeInfo> BincodeList { get => bincodeList; set => SetValue(ref bincodeList, value); }
        public int SelectList { get => selectList; set => SetValue(ref selectList, value); }


        public int Column { get => column; set => SetValue(ref column, value); }
        public int Row { get => row; set => SetValue(ref row, value); }

        public ICommand OpenPaletteCommand => new RelayCommand(() =>
        {
            try
            {


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        });


        private void OpenPalette_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            //如果陣列數量沒有自動產生 需要防呆自動產生
           /* var temp = SelectList;
            if (BincodeList.Count == SelectList)
            {
                BincodeList.Add(new BincodeInfo());
                SelectList = temp;
            }*/

            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {



                Color selectedColor = Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);

                BincodeList[SelectList].Color = new SolidColorBrush(selectedColor);

                // 使用選擇的顏色
                // MessageBox.Show($"選擇的顏色是: {selectedColor.ToString()}");

                if (BincodeList.Count == SelectList + 1)//點到最後一列  就自動增加一列
                {
                    BincodeInfo bincode2 = new BincodeInfo
                    {                     
                        Color = Brushes.GreenYellow
                    };
                    BincodeList.Add(bincode2);
                }
                    
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
