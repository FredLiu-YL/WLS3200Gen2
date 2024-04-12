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
using WLS3200Gen2.UserControls;
using YuanliCore.Data;

namespace WLS3200Gen2
{
    /// <summary>
    /// BincodeSettingWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MappingCanvasWindow : Window, INotifyPropertyChanged
    {
        private BincodeInfo[] bincodeList  ;
        private int selectList, column, row;
        private Die[] dieArray;
        private WriteableBitmap mappingTable;
        private Action<MappingOperate> create;
        private Action<int, int, Brush, Brush> setRectangle;
        public MappingCanvasWindow(int column, int row)
        {
            InitializeComponent();
            this.Column = column;
            this.Row = row;

            CreateDummyBincode(Column, Row);
          
        }


        public WriteableBitmap MappingTable { get => mappingTable; set => SetValue(ref mappingTable, value); }
        public BincodeInfo[] BincodeList { get => bincodeList; set => SetValue(ref bincodeList, value); }
        public Die[] DieArray { get => dieArray; set => SetValue(ref dieArray, value); }
        public int SelectList { get => selectList; set => SetValue(ref selectList, value); }
      
        public Action<MappingOperate> MappingOp { get => create; set => SetValue(ref create, value); }
        public Action<int, int, Brush, Brush> SetRectangle { get => setRectangle; set => SetValue(ref setRectangle, value); }
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


        private void CreateDummyBincode(int col, int row)
        {
            List<BincodeInfo> bincodeInfos = new List<BincodeInfo>();
            List<Die> dies = new List<Die>();
            object lockObj = new object();
            int width = col; // 矩形寬度
            int height = row; // 矩形高度
            int centerX = col/2; // 中心點 X 座標
            int centerY = row/2; // 中心點 Y 座標
            int radius = col/2; // 圓形半徑
            int radiusSquared = radius * radius; // 半徑的平方


            Parallel.For(0, col, x =>
            {

                Parallel.For(0, row, y =>
                {

                    // 計算矩形中心點到圓心的距離的平方
                    int dx = x - centerX;
                    int dy = y - centerY;
                    int distanceSquared = dx * dx + dy * dy;

                    // 如果距離的平方小於等於半徑的平方，則在圓形內
                    if (distanceSquared <= radiusSquared)
                    {
                        Die die = new Die();
                        die.IndexX = x;
                        die.IndexY = y;
                        if (x == 7)
                            die.BinCode = "A1";
                        else
                            die.BinCode = "A0";

                        lock (lockObj)
                        {
                            dies.Add(die);
                        }
                    }

                });

            });

            
            var info1 = new BincodeInfo();
            info1.Code = "A0";
            info1.Color = Brushes.Green;
            bincodeInfos.Add(info1);

            var info2 = new BincodeInfo();
            info2.Code = "A1";
            info2.Color = Brushes.Red;
            bincodeInfos.Add(info2);

            BincodeList= bincodeInfos.ToArray();
            DieArray = dies.ToArray();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            MappingOp?.Invoke( MappingOperate.Create);
        }
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            MappingOp?.Invoke(MappingOperate.Clear);
        }
        private void BtnFit_Click(object sender, RoutedEventArgs e)
        {
            MappingOp?.Invoke(MappingOperate.Fit);
        }
        private void BtnSetRect_Click(object sender, RoutedEventArgs e)
        {
            SetRectangle?.Invoke(Column,Row,Brushes.Coral,Brushes.OrangeRed);
        }

        
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
