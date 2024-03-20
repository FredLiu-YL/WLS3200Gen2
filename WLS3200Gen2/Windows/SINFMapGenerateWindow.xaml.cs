using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using YuanliCore.Data;
using YuanliCore.Views.CanvasShapes;
using static WLS3200Gen2.Model.Recipe.SinfWaferMapping;

namespace WLS3200Gen2
{
    /// <summary>
    /// SINFMapGenerateWindow.xaml 的互動邏輯
    /// </summary>
    public partial class SINFMapGenerateWindow : Window, INotifyPropertyChanged
    {
        public SINFMapGenerateWindow()
        {
            InitializeComponent();

            FormIsEnable = true;
            //MappingImage = new WriteableBitmap(6000, 6000, 96, 96, atfMachine.Table_Module.Camera.PixelFormat, null);
            //MainImage = new WriteableBitmap(atfMachine.Table_Module.Camera.Width, atfMachine.Table_Module.Camera.Height, 96, 96, atfMachine.Table_Module.Camera.PixelFormat, null);
            //MainImage = new WriteableBitmap(atfMachine.Table_Module.Camera.Width, atfMachine.Table_Module.Camera.Height, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
            BinCodeDrawDataGridList.Add(new BinCodeDraw { BinType = "Test", Description = "OK", Code = "000" });
            BinCodeDrawDataGridList.Add(new BinCodeDraw { BinType = "Skip", Description = "NG", Code = "___" });
            BinCodeDrawDataGridList.Add(new BinCodeDraw { BinType = "Ink", Description = "NG", Code = "099" });

            Die_PitchX = "1";
            Die_PitchY = "1";
            RimThickness_Distance = "0";
            DrawCountOffset_X = 0;
            DrawCountOffset_Y = 0;
            DrawTotal_X = 0;
            DrawTotal_Y = 0;


            MainTitle = title_Ver;

            MappingIsMoveEnable = true;
        }

        public SinfWaferMapping Sinf;

        private readonly string title_Ver = "Yuanli_MappingGenerate_Ver1.2";

        private bool formIsEnable = true;

        private ObservableCollection<ROIShape> drawings = new ObservableCollection<ROIShape>();

        private ObservableCollection<ROIShape> mappingDrawings = new ObservableCollection<ROIShape>();

        private WriteableBitmap mappingImage;

        private WriteableBitmap mainImage;

        private bool isVirtualCanvas = true;

        private Point mappingMousePixel;

        private bool isEditBinGBEnable;

        private bool mappingIsMoveEnable;

        private bool mapping_IsTeseted, mapping_IsSkip, mapping_IsInk;

        private ROIShape tempselectShape;

        private Brush tempselectShapeOrgStrokeBrush;

        private Brush tempselectShapeOrgCrossBrush;

        private Brush tempselectShapeOrgFillBrush;

        private Brush mappingDrawColor_Tested = Brushes.Green;

        private Brush mappingDrawColor_Ink = Brushes.Yellow;

        private Brush mappingDrawColor_Skip = Brushes.Gray;

        private Brush mappingDrawColor_Stroke = Brushes.Black;

        private string mappingDrawBinCode_Tested = "000";

        private string mappingDrawBinCode_Skip = "___";

        private string mappingDrawBinCode_Ink = "099";

        private int dataGridSelectedIndex;

        private ObservableCollection<Defect> defectDataGridList = new ObservableCollection<Defect>();

        private ObservableCollection<BinCodeDraw> binCodeDrawDataGridList = new ObservableCollection<BinCodeDraw>();

        private double showSize_X;

        private double showSize_Y;

        private bool isWaferSize_8, isWaferSize_12;

        private string mainTitle, lotID, wafer_ID, wafer_Slot, recipe_Name, wafer_Size, notch_Type, notch_Direction, die_PitchX, die_PitchY, die_Origin, station_ID, device_ID, setup_ID, step_ID, inspection, rimThickness_Distance, test_Count, ink_Count;

        private int tabControlDraw_SelectedIndex, drawCountOffset_X, drawCountOffset_Y;

        private int drawTotal_X, drawTotal_Y;

        private string sort_Classification_1, sort_Classification_2, sort_Classification_3, sort_Classification_4, sort_Classification_5;

        private bool isSort_Except;

        private string sinfPath, sinfSavePath, picturePath, defectPathName;

        private Brush[] colorBasic = new Brush[] { Brushes.Green, Brushes.DarkRed, Brushes.OrangeRed, Brushes.GreenYellow, Brushes.DarkBlue, Brushes.Purple, Brushes.Orange, Brushes.Yellow, Brushes.Gray };
        // { Brushes.Green, Brushes.DarkRed

        /// <summary>
        /// 新增 Shape
        /// </summary>
        public ICommand AddShapeAction { get; set; }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearShapeAction { get; set; }

        /// <summary>
        /// 滑鼠在影像內 Pixcel 座標
        /// </summary>
        public System.Windows.Point MousePixcel { get; set; }
        /// <summary>
        /// 影像中心Pixel座標 Y
        /// </summary>
        public double ControlCenterY { get; set; }
        /// <summary>
        /// 影像中心Pixel座標 X
        /// </summary>
        public double ControlCenterX { get; set; }
        public WriteableBitmap MainImage { get => mainImage; set => SetValue(ref mainImage, value); }
        public WriteableBitmap MappingImage { get => mappingImage; set => SetValue(ref mappingImage, value); }
        public ObservableCollection<ROIShape> Drawings { get => drawings; set => SetValue(ref drawings, value); }
        public ObservableCollection<ROIShape> MappingDrawings { get => mappingDrawings; set => SetValue(ref mappingDrawings, value); }
        public bool IsVirtualCanvas { get => isVirtualCanvas; set => SetValue(ref isVirtualCanvas, value); }
        public System.Windows.Point MappingMousePixel { get => mappingMousePixel; set => SetValue(ref mappingMousePixel, value); }
        public bool MappingIsMoveEnable
        {
            get => mappingIsMoveEnable;
            set
            {
                IsEditBinGBEnable = !value;
                SetValue(ref mappingIsMoveEnable, value);
            }
        }
        public bool IsEditBinGBEnable { get => isEditBinGBEnable; set => SetValue(ref isEditBinGBEnable, value); }
        public bool Mapping_IsTeseted { get => mapping_IsTeseted; set => SetValue(ref mapping_IsTeseted, value); }
        public bool Mapping_IsSkip { get => mapping_IsSkip; set => SetValue(ref mapping_IsSkip, value); }
        public bool Mapping_IsInk { get => mapping_IsInk; set => SetValue(ref mapping_IsInk, value); }

        public ICommand AddShapeMappingAction { get; set; }
        public ICommand ClearShapeMappingAction { get; set; }
        public ICommand RemoveShapeMappingAction { get; set; }
        public int DataGridSelectedIndex { get => dataGridSelectedIndex; set => SetValue(ref dataGridSelectedIndex, value); }
        public ObservableCollection<Defect> DefectDataGridList { get => defectDataGridList; set => SetValue(ref defectDataGridList, value); }
        public ObservableCollection<BinCodeDraw> BinCodeDrawDataGridList { get => binCodeDrawDataGridList; set => SetValue(ref binCodeDrawDataGridList, value); }

        /// <summary>
        /// 讀圖
        /// </summary>
        private List<BitmapSource> bitmapImageList = new List<BitmapSource>();



        public string MainTitle { get => mainTitle; set => SetValue(ref mainTitle, value); }
        public bool FormIsEnable { get => formIsEnable; set => SetValue(ref formIsEnable, value); }
        public string LotID { get => lotID; set => SetValue(ref lotID, value); }

        public string Wafer_ID { get => wafer_ID; set => SetValue(ref wafer_ID, value); }
        public string Wafer_Slot { get => wafer_Slot; set => SetValue(ref wafer_Slot, value); }
        public bool IsWaferSize_8 { get => isWaferSize_8; set => SetValue(ref isWaferSize_8, value); }
        public bool IsWaferSize_12 { get => isWaferSize_12; set => SetValue(ref isWaferSize_12, value); }

        public string Recipe_Name { get => recipe_Name; set => SetValue(ref recipe_Name, value); }
        public string Wafer_Size { get => wafer_Size; set => SetValue(ref wafer_Size, value); }
        public string Notch_Type { get => notch_Type; set => SetValue(ref notch_Type, value); }
        public string Notch_Direction { get => notch_Direction; set => SetValue(ref notch_Direction, value); }


        public string Die_PitchX { get => die_PitchX; set => SetValue(ref die_PitchX, value); }
        public string Die_PitchY { get => die_PitchY; set => SetValue(ref die_PitchY, value); }
        public string Die_Origin { get => die_Origin; set => SetValue(ref die_Origin, value); }
        public string Station_ID { get => station_ID; set => SetValue(ref station_ID, value); }
        public string Device_ID { get => device_ID; set => SetValue(ref device_ID, value); }
        public string Setup_ID { get => setup_ID; set => SetValue(ref setup_ID, value); }
        public string Step_ID { get => step_ID; set => SetValue(ref step_ID, value); }
        public string Inspection { get => inspection; set => SetValue(ref inspection, value); }
        public int TabControlDraw_SelectedIndex { get => tabControlDraw_SelectedIndex; set => SetValue(ref tabControlDraw_SelectedIndex, value); }
        public string RimThickness_Distance { get => rimThickness_Distance; set => SetValue(ref rimThickness_Distance, value); }
        public int DrawCountOffset_X { get => drawCountOffset_X; set => SetValue(ref drawCountOffset_X, value); }
        public int DrawCountOffset_Y { get => drawCountOffset_Y; set => SetValue(ref drawCountOffset_Y, value); }
        public int DrawTotal_X { get => drawTotal_X; set => SetValue(ref drawTotal_X, value); }
        public int DrawTotal_Y { get => drawTotal_Y; set => SetValue(ref drawTotal_Y, value); }
        public string Test_Count { get => test_Count; set => SetValue(ref test_Count, value); }
        public string Ink_Count { get => ink_Count; set => SetValue(ref ink_Count, value); }



        public string SinfPath { get => sinfPath; set => SetValue(ref sinfPath, value); }

        private void SINFMapGenerate_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SINFMapGenerate_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public string SinfSavePath { get => sinfSavePath; set => SetValue(ref sinfSavePath, value); }
        public string Sort_Classification_1 { get => sort_Classification_1; set => SetValue(ref sort_Classification_1, value); }
        public string Sort_Classification_2 { get => sort_Classification_2; set => SetValue(ref sort_Classification_2, value); }
        public string Sort_Classification_3 { get => sort_Classification_3; set => SetValue(ref sort_Classification_3, value); }
        public string Sort_Classification_4 { get => sort_Classification_4; set => SetValue(ref sort_Classification_4, value); }
        public string Sort_Classification_5 { get => sort_Classification_5; set => SetValue(ref sort_Classification_5, value); }
        public bool Sort_Except { get => isSort_Except; set => SetValue(ref isSort_Except, value); }
        /// <summary>
        /// 
        /// </summary>
        public double StrokeThickness { get; set; }
        public double CrossThickness { get; set; }


        public class Defect
        {
            public int ID { get; set; }
            public string Relative_X { get; set; }
            public string Relative_Y { get; set; }
            public int Index_X { get; set; }
            public int Index_Y { get; set; }
            public int Picture_X { get; set; }
            public int Picture_Y { get; set; }

            public int SubDie_X { get; set; }
            public int SubDie_Y { get; set; }

            private string classification;
            public string Classification { get; set; }// { get => classification; set => SetValue(ref classification, value); }
            public string PathName { get; set; }

            public int Org_Index_X { get; set; }
            public int Org_Index_Y { get; set; }
            public int Add_Idx { get; set; }
        }

        public string DefectPathName { get => defectPathName; set => SetValue(ref defectPathName, value); }




        public class BinCodeDraw
        {
            public string BinType { get; set; }
            public string Description { get; set; }
            public string Code { get; set; }
        }
        public class BinCodeDrawStartEnd
        {
            public int startDraw_X = 9999999;
            public int endDraw_X = -1;
            public int startDraw_Y = 9999999;
            public int endDraw_Y = -1;
        }
        public ICommand SINFTrans => new RelayCommand(async () =>
        {
            try
            {
                FormIsEnable = false;

                MappingImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                //"D:\\晶圓檢測_AOI結果_sinf\\example\\example.sinf"
                //"D:\\晶圓檢測_AOI結果_sinf\\SINF_sample\\SINF_sample_1.txt"
                //"D:\\晶圓檢測_AOI結果_sinf\\SINF_sample_v2\\sample_1.txt"
                string path = "D:\\晶圓檢測_AOI結果_sinf\\SINF_sample\\SINF_sample_1.txt";

                string SINF_Path = "";
                string Picture_Path = "";
                try
                {
                    string pfilePath = "D:\\SINF_ReadPath.txt";
                    string[] lines = File.ReadAllLines(pfilePath);
                    SINF_Path = lines[0];
                    //MainTitle = title_Ver + "   " + lines[0];
                    Picture_Path = lines[1];
                }
                catch (Exception)
                {
                }

                //if (SINF_Path = "")
                //{
                System.Windows.Forms.OpenFileDialog dlg_image = new System.Windows.Forms.OpenFileDialog();
                dlg_image.Filter = "TXT files (*.txt)|*.txt|SINF files (*.sinf)|*.sinf";
                dlg_image.InitialDirectory = SINF_Path;
                if (dlg_image.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SINF_Path = dlg_image.FileName;
                    MainTitle = title_Ver;// + "   " + dlg_image.FileName;
                }
                else
                {
                    SINF_Path = "";
                    MainTitle = title_Ver;
                }
                //}

                if (SINF_Path != "")
                {
                    if (Sinf == null)
                    {
                        Sinf = new SinfWaferMapping("", true, false);
                    }
                    path = SINF_Path;
                    (Sinf.Dies, Sinf.WaferSize) = Sinf.ReadWaferFile(path, true, false);
                    SinfPath = path;
                    Wafer_ID = Sinf.Lot_ID;
                    Wafer_Slot = Sinf.Wafer_Idx.ToString();
                    Die_PitchX = Sinf.DieSize_X.ToString();
                    Die_PitchY = Sinf.DieSize_Y.ToString();
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    //Wafer_Size = Sinf.Notch_Degree;
                    Die_Origin = Sinf.Start_LeftX + "," + Sinf.Start_TopY;
                    DrawTotal_X = Sinf.Count_Column;
                    DrawTotal_Y = Sinf.Count_Row;
                    MappingImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                    ShowMappingDrawings();
                }
                else
                {
                    DrawTotal_X = 0;
                    DrawTotal_Y = 0;
                    SinfPath = "";
                    DefectDataGridList.Clear();
                    Wafer_ID = "";
                    Wafer_Slot = "";
                    Die_PitchX = "1";
                    Die_PitchY = "1";
                    Wafer_Size = "";
                    Die_Origin = "";
                    //Sinf = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                FormIsEnable = true;
            }
        });


        public void ShowMappingDrawings()
        {
            try
            {
                ClearShapeMappingAction.Execute(true);



                int showScale = 100;

                int pID = 0;
                Wafer_ID = Sinf.Lot_ID;
                Die_PitchX = Sinf.DieSize_X.ToString();
                Die_PitchY = Sinf.DieSize_Y.ToString();

                int mc = Sinf.Count_Column;
                int mr = Sinf.Count_Row;

                double Mainlength_X = 3000;
                double Mainlength_Y = 3000;
                int mappingImageDrawSize = 3000;
                double offsetDraw = mappingImageDrawSize / 150;
                double scale = 100;
                scale = Math.Max(Sinf.Count_Column * Sinf.DieSize_X, Sinf.Count_Row * Sinf.DieSize_Y) / (mappingImageDrawSize - offsetDraw * 2);

                this.StrokeThickness = Math.Min(Sinf.DieSize_X / 2 / scale, Sinf.DieSize_X / 2 / scale) / 4;
                this.CrossThickness = Math.Min(Sinf.DieSize_X / 2 / scale, Sinf.DieSize_X / 2 / scale) / 4;

                showSize_X = Sinf.Count_Column * Sinf.DieSize_X / (mappingImageDrawSize - offsetDraw * 2);
                showSize_Y = Sinf.Count_Row * Sinf.DieSize_Y / (mappingImageDrawSize - offsetDraw * 2);
                //this.StrokeThickness = Math.Min(showSize_X, showSize_X) / 5;

                int greenPoint = 0;
                int inkPoint = 0;

                foreach (var item in Sinf.Dies)
                {
                    Brush drawStroke = mappingDrawColor_Stroke;
                    Brush drawFill = Brushes.Gray;
                    if (item.BinCode == mappingDrawBinCode_Skip)
                    {
                        drawFill = mappingDrawColor_Skip;
                    }
                    else if (item.BinCode == mappingDrawBinCode_Ink)
                    {
                        inkPoint++;
                        drawFill = mappingDrawColor_Ink;
                    }
                    else if (item.BinCode == mappingDrawBinCode_Tested)
                    {
                        greenPoint++;
                        drawFill = mappingDrawColor_Tested;
                    }

                    AddShapeMappingAction.Execute(new ROIRotatedRect
                    {
                        Stroke = drawStroke,
                        StrokeThickness = this.StrokeThickness,
                        Fill = drawFill,
                        X = (item.OperationPixalX / showSize_X) + offsetDraw,
                        Y = (item.OperationPixalY / showSize_Y) + offsetDraw,
                        LengthX = Sinf.DieSize_X / 2.5 / showSize_X,
                        LengthY = Sinf.DieSize_Y / 2.5 / showSize_Y,
                        IsInteractived = true,
                        IsMoveEnabled = false,
                        IsResizeEnabled = false,
                        IsRotateEnabled = false,
                        CenterCrossLength = this.CrossThickness,
                        CenterCrossBrush = drawFill,
                        ToolTip = "X:" + (item.IndexX + 1) + " Y:" + (item.IndexY + 1)// + " X:" + item.MapTransX + " Y:" + item.MapTransY
                    });
                }

                for (int i = 0; i < mc; i++)
                {
                    for (int j = 0; j < mr; j++)
                    {

                        //}
                    }
                }
                Test_Count = greenPoint.ToString();
                Ink_Count = inkPoint.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public ICommand MappingIsMoveEnableClick => new RelayCommand(async () =>
        {
        });

        public ICommand SINFSave => new RelayCommand(async () =>
        {
            try
            {
                FormIsEnable = false;
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Title = "另存為";
                saveFileDialog.Filter = "文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";
                string defaultFileName = "";
                try
                {
                    string[] liens = SinfPath.Split('\\');
                    defaultFileName = liens[liens.Length - 1];
                }
                catch (Exception)
                {
                }
                saveFileDialog.FileName = defaultFileName;

                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SinfSavePath = saveFileDialog.FileName;
                    Sinf.Lot_ID = Wafer_ID;
                    Sinf.Wafer_Idx = Convert.ToInt32(Wafer_Slot);
                    BinCodeDrawStartEnd binCodeDrawStartEnd = UpdateXYStartEndDraw();
                    Sinf.SaveAS(saveFileDialog.FileName, binCodeDrawStartEnd.startDraw_X, binCodeDrawStartEnd.endDraw_X, binCodeDrawStartEnd.startDraw_Y, binCodeDrawStartEnd.endDraw_Y);//"D:\\晶圓檢測_AOI結果_sinf\\Test_1.txt"
                    if (Recipe_Name != "" && Recipe_Name != null)
                    {
                        await SaveINI(saveFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                FormIsEnable = true;
            }
        });

        public async Task SaveINI(string pathFile)
        {
            await Task.Run(async () =>
            {
                try
                {
                    string[] pathSplit = pathFile.Split('\\');
                    List<string> lines = new List<string>();
                    lines.Add("[lot]");
                    lines.Add("output_folder=" + Sinf.Lot_ID);
                    lines.Add("recipe_name=" + Recipe_Name);
                    lines.Add("wafer_qty=1");
                    lines.Add("userid=");
                    lines.Add("expired_date=");
                    lines.Add("");
                    lines.Add("[wafer_id]");
                    string Wafer_Slot_Stirng = Convert.ToInt32(Wafer_Slot).ToString("D2");
                    string Wafer_ID = pathSplit[pathSplit.Length - 1].Split('.')[0];
                    lines.Add(Wafer_Slot_Stirng + "=" + Wafer_ID);

                    //Regex.Replace(xlsxFilePath, ".csv", ".xlsx", RegexOptions.IgnoreCase);
                    string newPathFile = pathSplit[0];
                    for (int i = 1; i < pathSplit.Length - 1; i++)
                    {
                        newPathFile += "\\" + pathSplit[i];
                    }
                    newPathFile += "\\" + "lot_info.ini";

                    File.WriteAllLines(newPathFile, lines);
                }
                catch (Exception)
                {

                    throw;
                }
            });
        }


        public enum MappingDrawType
        {
            None = 0,
            /// <summary>
            /// 要測試的
            /// </summary>
            Tested = 1,
            /// <summary>
            /// 空白區
            /// </summary>
            Skip = 2,
            /// <summary>
            /// 不測試的
            /// </summary>
            Ink = 3
        }
        public ICommand MappingPreviewMouseUpCommand => new RelayCommand(() =>
        {
            try
            {
                bool isEdit = false;
                MappingDrawType mappingDrawType = MappingDrawType.None;

                if (Sinf != null && Sinf.Dies_result.Length > 0)
                {
                    if (MappingIsMoveEnable == false)
                    {
                        isEdit = true;
                        //tempselectShape = null;
                        if (Mapping_IsTeseted == true)
                        {
                            mappingDrawType = MappingDrawType.Tested;
                        }
                        else if (Mapping_IsInk == true)
                        {
                            mappingDrawType = MappingDrawType.Ink;
                        }
                        else if (Mapping_IsSkip == true)
                        {
                            mappingDrawType = MappingDrawType.Skip;
                        }
                    }
                }


                ROIShape selectShape = MappingDrawings.Select(shape =>
                {
                    var rectBegin = shape.LeftTop;
                    var rectEnd = shape.RightBottom;
                    var rect = new Rect(rectBegin, rectEnd);
                    if (rect.Contains(MappingMousePixel) && !(shape is ROICircle))
                        return shape;
                    else
                        return null;
                }).Where(s => s != null).FirstOrDefault();

                //selectShape = null;
                int select_indexX = 0;
                int select_indexY = 0;
                if (selectShape != null)
                {
                    if (tempselectShape != null)
                    {
                        tempselectShape.StrokeThickness = this.StrokeThickness;
                        tempselectShape.CenterCrossBrush = tempselectShapeOrgCrossBrush;
                        tempselectShape.Stroke = tempselectShapeOrgStrokeBrush;

                    }
                    try
                    {
                        tempselectShapeOrgCrossBrush = selectShape.CenterCrossBrush;
                        tempselectShapeOrgFillBrush = selectShape.Fill;
                        tempselectShapeOrgStrokeBrush = selectShape.Stroke;
                        string tip = selectShape.ToolTip.ToString();

                        string[] line = tip.Split(' ');
                        int idxX = Convert.ToInt32(line[0].Split(':')[1]) - 1;
                        int idxY = Convert.ToInt32(line[1].Split(':')[1]) - 1;


                        if (mappingDrawType != MappingDrawType.None)
                        {
                            //m_WaferMapping.DieDrawInfo[m_WaferMapping.DrawColumnCount * idxY + idxX].BinCode = mappingDrawBinCode_Tested;
                            string nowBincode = Sinf.Dies.FirstOrDefault(die => die.IndexX == idxX && die.IndexY == idxY).BinCode;
                            if (mappingDrawType == MappingDrawType.Tested)
                            {
                                selectShape.CenterCrossBrush = System.Windows.Media.Brushes.Red;
                                selectShape.Fill = mappingDrawColor_Tested;
                                selectShape.Stroke = System.Windows.Media.Brushes.Red;
                                tempselectShapeOrgCrossBrush = mappingDrawColor_Tested;
                                if (nowBincode != mappingDrawBinCode_Tested)
                                {
                                    int test = Convert.ToInt32(Test_Count);
                                    int ink = Convert.ToInt32(Ink_Count);
                                    Test_Count = (test + 1).ToString();
                                    if (nowBincode == mappingDrawBinCode_Ink)
                                    {
                                        Ink_Count = (ink - 1).ToString();
                                    }
                                }
                                Sinf.Dies.FirstOrDefault(die => die.IndexX == idxX && die.IndexY == idxY).BinCode = mappingDrawBinCode_Tested;
                            }
                            else if (mappingDrawType == MappingDrawType.Ink)
                            {
                                selectShape.CenterCrossBrush = System.Windows.Media.Brushes.Red;
                                selectShape.Fill = mappingDrawColor_Ink;
                                selectShape.Stroke = System.Windows.Media.Brushes.Red;
                                tempselectShapeOrgCrossBrush = mappingDrawColor_Ink;
                                //m_WaferMapping.DieDrawInfo[m_WaferMapping.DrawColumnCount * idxY + idxX].BinCode = mappingDrawBinCode_Ink;
                                if (nowBincode != mappingDrawBinCode_Ink)
                                {
                                    int test = Convert.ToInt32(Test_Count);
                                    int ink = Convert.ToInt32(Ink_Count);
                                    Ink_Count = (ink + 1).ToString();
                                    if (nowBincode == mappingDrawBinCode_Tested)
                                    {
                                        Test_Count = (test - 1).ToString();
                                    }
                                }
                                Sinf.Dies.FirstOrDefault(die => die.IndexX == idxX && die.IndexY == idxY).BinCode = mappingDrawBinCode_Ink;
                            }
                            else if (mappingDrawType == MappingDrawType.Skip)
                            {
                                selectShape.CenterCrossBrush = System.Windows.Media.Brushes.Red;
                                selectShape.Fill = mappingDrawColor_Skip;
                                selectShape.Stroke = System.Windows.Media.Brushes.Red;
                                tempselectShapeOrgCrossBrush = mappingDrawColor_Skip;
                                //m_WaferMapping.DieDrawInfo[m_WaferMapping.DrawColumnCount * idxY + idxX].BinCode = mappingDrawBinCode_Skip;
                                if (nowBincode == mappingDrawBinCode_Tested)
                                {
                                    int test = Convert.ToInt32(Test_Count);
                                    int ink = Convert.ToInt32(Ink_Count);
                                    Test_Count = (test - 1).ToString();
                                }
                                Sinf.Dies.FirstOrDefault(die => die.IndexX == idxX && die.IndexY == idxY).BinCode = mappingDrawBinCode_Skip;
                            }
                        }
                        else
                        {
                            selectShape.CenterCrossBrush = System.Windows.Media.Brushes.Red;
                            selectShape.Stroke = System.Windows.Media.Brushes.Red;
                        }
                        tempselectShape = selectShape;
                        tempselectShape.StrokeThickness = this.StrokeThickness;
                        //m_WaferMapping.DieDrawInfo
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        });



        public ICommand SINFSort => new RelayCommand(async () =>
        {
            try
            {
                DefectDataGridList.Clear();
                int pID = 0;
                for (int i = 0; i < Sinf.Count_Column; i++)
                {
                    for (int j = 0; j < Sinf.Count_Row; j++)
                    {
                        if (Sinf.Dies_result[i, j].TransDieData != "__" && Sinf.Dies_result[i, j].TransDieData != "___")
                        {
                            if (Sort_Except == false)
                            {
                                if (Sinf.Dies_result[i, j].TransDieData != Sort_Classification_1 && Sinf.Dies_result[i, j].TransDieData != Sort_Classification_2 &&
                                Sinf.Dies_result[i, j].TransDieData != Sort_Classification_3 && Sinf.Dies_result[i, j].TransDieData != Sort_Classification_4 &&
                                Sinf.Dies_result[i, j].TransDieData != Sort_Classification_5)
                                {
                                    pID += 1;
                                    Defect defect_result = new Defect
                                    {
                                        ID = pID,
                                        Relative_X = Sinf.Dies_result[i, j].PositionX.ToString(),
                                        Relative_Y = Sinf.Dies_result[i, j].PositionY.ToString(),
                                        Index_X = Sinf.Dies_result[i, j].IndexX + 1,
                                        Index_Y = Sinf.Dies_result[i, j].IndexY + 1,
                                        Classification = Sinf.Dies_result[i, j].TransDieData
                                    };
                                    DefectDataGridList.Add(defect_result);
                                }
                            }
                            else
                            {
                                if (Sinf.Dies_result[i, j].TransDieData == Sort_Classification_1 || Sinf.Dies_result[i, j].TransDieData == Sort_Classification_2 ||
                                Sinf.Dies_result[i, j].TransDieData == Sort_Classification_3 || Sinf.Dies_result[i, j].TransDieData == Sort_Classification_4 ||
                                Sinf.Dies_result[i, j].TransDieData == Sort_Classification_5)
                                {
                                    pID += 1;
                                    Defect defect_result = new Defect
                                    {
                                        ID = pID,
                                        Relative_X = Sinf.Dies_result[i, j].PositionX.ToString(),
                                        Relative_Y = Sinf.Dies_result[i, j].PositionY.ToString(),
                                        Index_X = Sinf.Dies_result[i, j].IndexX + 1,
                                        Index_Y = Sinf.Dies_result[i, j].IndexY + 1,
                                        Classification = Sinf.Dies_result[i, j].TransDieData
                                    };
                                    DefectDataGridList.Add(defect_result);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand BINSelect => new RelayCommand(async () =>
        {
            try
            {
                FormIsEnable = false;
                System.Windows.Forms.OpenFileDialog dlg_image = new System.Windows.Forms.OpenFileDialog();
                dlg_image.InitialDirectory = "D://晶圓檢測_AOI結果_sinf";
                dlg_image.Filter = "TXT files (*.txt)|*.txt";
                string path;
                path = "";
                if (dlg_image.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = dlg_image.FileName;
                    if (Sinf == null)
                    {
                        Sinf = new SinfWaferMapping("", true, false);
                    }
                    Sinf.ReadBinCode(path);
                    BinCodeDrawDataGridList.Clear();
                    int count = 0;
                    foreach (var bincodeLine in Sinf.BinCode_result)
                    {

                        if (count == 0)
                        {
                            BinCodeDrawDataGridList.Add(new BinCodeDraw { BinType = "Test", Description = bincodeLine.Describe, Code = bincodeLine.Code });
                            mappingDrawBinCode_Tested = bincodeLine.Code;
                        }
                        else if (count == 1)
                        {
                            BinCodeDrawDataGridList.Add(new BinCodeDraw { BinType = "Skip", Description = bincodeLine.Describe, Code = bincodeLine.Code });
                            mappingDrawBinCode_Skip = bincodeLine.Code;
                        }
                        else if (count == 2)
                        {
                            BinCodeDrawDataGridList.Add(new BinCodeDraw { BinType = "Ink", Description = bincodeLine.Describe, Code = bincodeLine.Code });
                            mappingDrawBinCode_Ink = bincodeLine.Code;
                        }


                        count++;
                    }
                    MappingImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                    ShowMappingDrawings();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            finally
            {
                FormIsEnable = true;
            }
        });

        public ICommand BtnLoadPicture => new RelayCommand(async () =>
        {
            try
            {
                var bitmapImage = new BitmapImage();
                string path = "C://Users//zhengye_lin//Desktop//7788output.bmp"; // "C://Users//zhengye_lin//Desktop//333output.bmp"
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(path);//333output.bmp
                bitmapImage.EndInit();

                ClearShapeMappingAction.Execute(true);
                MappingImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                if (!(bitmapImage is null))
                {
                    MappingImage = new WriteableBitmap(bitmapImage);
                }
                else
                {
                    if (!(MappingImage is null))
                    {
                        MappingImage.Lock();
                        // 清除 WriteableBitmap 的像素数据
                        int stride = MappingImage.BackBufferStride;
                        int bytesPerPixel = (MappingImage.Format.BitsPerPixel + 7) / 8;
                        byte[] pixels = new byte[stride * MappingImage.PixelHeight];
                        MappingImage.WritePixels(new Int32Rect(0, 0, MappingImage.PixelWidth, MappingImage.PixelHeight), pixels, stride, 0);

                        MappingImage.Unlock();


                        MappingImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand BtnTest => new RelayCommand(async () =>
        {
            try
            {
                MainTitle = title_Ver + "   " + "666666666666";
                string sss = "123";
                if (IsWaferSize_8 == true)
                {
                    sss = "777";
                }
                else if (IsWaferSize_12 == true)
                {
                    sss = "777";
                }
                //this.StrokeThickness = 10;
                //ClearShapeMappingAction.Execute(true);
                //MappingImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);

                //AddShapeMappingAction.Execute(new ROICircle
                //{
                //    Stroke = Brushes.Yellow,
                //    StrokeThickness = this.StrokeThickness,
                //    Fill = Brushes.Transparent,//Brushes.Blue,
                //    X = 1000,
                //    Y = 1000,
                //    Radius = 1000,
                //    //LengthX = showSize_X / 3,//(dieSize.Width / 3) / showScale,
                //    //LengthY = showSize_Y / 3,//(dieSize.Height / 2) / showScale,
                //    IsInteractived = true,
                //    IsMoveEnabled = false,
                //    IsResizeEnabled = false,
                //    IsRotateEnabled = false,
                //    CenterCrossLength = 0,
                //    ToolTip = "Circle"
                //});
                AddShapeMappingAction.Execute(new ROIRotatedRect
                {
                    Stroke = Brushes.Green,
                    StrokeThickness = this.StrokeThickness,
                    Fill = Brushes.Blue,
                    X = 1000,
                    Y = 1000,
                    LengthX = 50,//(dieSize.Width / 3) / showScale,
                    LengthY = 50,//(dieSize.Height / 2) / showScale,
                    IsInteractived = true,
                    IsMoveEnabled = false,
                    IsResizeEnabled = false,
                    IsRotateEnabled = false,
                    CenterCrossLength = 0,
                    ToolTip = "Rectangle"
                });
                //AddShapeMappingAction.Execute(new ROIRotatedRect
                //{
                //    Stroke = Brushes.Green,
                //    StrokeThickness = this.StrokeThickness,
                //    Fill = Brushes.Blue,
                //    X = 2000,
                //    Y = 2000,
                //    LengthX = 50,//(dieSize.Width / 3) / showScale,
                //    LengthY = 50,//(dieSize.Height / 2) / showScale,
                //    IsInteractived = true,
                //    IsMoveEnabled = false,
                //    IsResizeEnabled = false,
                //    IsRotateEnabled = false,
                //    CenterCrossLength = 0,
                //    ToolTip = "Rectangle"
                //});


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public MappingDrawType IsCircleTouchingRectangle(Point circleCenter, double circleRadius, Point rectangleTopLeft, Point rectangleBottomRight, double rimThickness)
        {
            try
            {


                double h = circleCenter.X;
                double k = circleCenter.Y;
                double a = rectangleTopLeft.X;
                double b = rectangleTopLeft.Y;
                double c = rectangleBottomRight.X;
                double d = rectangleBottomRight.Y;

                // 找到矩形內最近的點
                double closestX = Math.Max(a, Math.Min(h, c));
                double closestY = Math.Max(b, Math.Min(k, d));

                // 計算圓心到最近點的距離
                double distance_closest = Math.Sqrt(Math.Pow(h - closestX, 2) + Math.Pow(k - closestY, 2));

                // 找到矩形內最遠的點
                double farthestX = Math.Abs(h - (a + c) / 2) > Math.Abs(h - a) ? c : a;
                double farthestY = Math.Abs(k - (b + d) / 2) > Math.Abs(k - b) ? d : b;

                // 計算圓心到最遠點的距離
                double distance_farthest = Math.Sqrt(Math.Pow(h - farthestX, 2) + Math.Pow(k - farthestY, 2));

                if (distance_farthest > (circleRadius - rimThickness) && (circleRadius + rimThickness) >= distance_closest)
                {
                    return MappingDrawType.Ink;
                }
                else if (distance_farthest > circleRadius || distance_closest > circleRadius)
                {
                    return MappingDrawType.Skip;
                }
                else
                {
                    return MappingDrawType.Tested;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public MappingDrawType IsCircleTouchingRectangle_TotalType(Point circleCenter, double circleRadius, Point rectangleTopLeft, Point rectangleBottomRight)
        {
            try
            {
                double h = circleCenter.X;
                double k = circleCenter.Y;
                double a = rectangleTopLeft.X;
                double b = rectangleTopLeft.Y;
                double c = rectangleBottomRight.X;
                double d = rectangleBottomRight.Y;

                // 找到矩形內最近的點
                double closestX = Math.Max(a, Math.Min(h, c));
                double closestY = Math.Max(b, Math.Min(k, d));

                // 計算圓心到最近點的距離
                double distance_closest = Math.Sqrt(Math.Pow(h - closestX, 2) + Math.Pow(k - closestY, 2));

                // 找到矩形內最遠的點
                double farthestX = Math.Abs(h - (a + c) / 2) > Math.Abs(h - a) ? c : a;
                double farthestY = Math.Abs(k - (b + d) / 2) > Math.Abs(k - b) ? d : b;

                // 計算圓心到最遠點的距離
                double distance_farthest = Math.Sqrt(Math.Pow(h - farthestX, 2) + Math.Pow(k - farthestY, 2));

                if (distance_farthest > circleRadius)
                {
                    return MappingDrawType.Skip;
                }
                else
                {
                    return MappingDrawType.Tested;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public BinCodeDrawStartEnd UpdateXYStartEndDraw()
        {
            try
            {
                BinCodeDrawStartEnd binCodeDrawStartEnd = new BinCodeDrawStartEnd();

                binCodeDrawStartEnd.startDraw_X = 9999999;
                binCodeDrawStartEnd.endDraw_X = -1;
                binCodeDrawStartEnd.startDraw_Y = 9999999;
                binCodeDrawStartEnd.endDraw_Y = -1;

                int CountX = Sinf.Dies_result.GetLength(0);
                int CountY = Sinf.Dies_result.GetLength(1);
                for (int i = 0; i < CountX; i++)
                {
                    for (int j = 0; j < CountY; j++)
                    {
                        if (Sinf.Dies_result[i, j].DieData == mappingDrawBinCode_Ink || Sinf.Dies_result[i, j].DieData == mappingDrawBinCode_Tested)
                        {
                            if (binCodeDrawStartEnd.startDraw_X > i)
                            {
                                binCodeDrawStartEnd.startDraw_X = i;
                            }
                            if (binCodeDrawStartEnd.endDraw_X < i)
                            {
                                binCodeDrawStartEnd.endDraw_X = i;
                            }

                            if (binCodeDrawStartEnd.startDraw_Y > j)
                            {
                                binCodeDrawStartEnd.startDraw_Y = j;
                            }
                            if (binCodeDrawStartEnd.endDraw_Y < j)
                            {
                                binCodeDrawStartEnd.endDraw_Y = j;
                            }
                        }
                    }
                }


                return binCodeDrawStartEnd;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }






        //private WaferMapping m_WaferMapping = new WaferMapping();
        public ICommand BtnDraw => new RelayCommand(async () =>
        {
            try
            {
                FormIsEnable = false;


                double orgScale = 100;
                double scale = 100;
                int mappingImageDrawSize = 3000;
                this.StrokeThickness = 5;

                await Task.Delay(100);
                ClearShapeMappingAction.Execute(true);
                tempselectShape = null;
                MappingImage = new WriteableBitmap(mappingImageDrawSize, mappingImageDrawSize, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);

                int dieSize_X = Convert.ToInt32(Die_PitchX);//3000
                int dieSize_Y = Convert.ToInt32(Die_PitchY);//3000
                int waferSize = 300000;
                if (IsWaferSize_8 == true)
                {
                    waferSize = 200000;
                }
                else if (IsWaferSize_12 == true)
                {
                    waferSize = 300000;
                }


                int xMod = waferSize % dieSize_X;
                int yMod = waferSize % dieSize_Y;

                int wafer_CountX = 0;
                int wafer_CountY = 0;



                if (xMod == 0)
                {
                    wafer_CountX = waferSize / dieSize_X - 2;
                }
                else
                {
                    wafer_CountX = waferSize / dieSize_X - 1;
                }

                if (yMod == 0)
                {
                    wafer_CountY = waferSize / dieSize_Y - 2;
                }
                else
                {
                    wafer_CountY = waferSize / dieSize_Y - 1;
                }

                int draw_CountX = wafer_CountX + 8 + Convert.ToInt32(DrawCountOffset_X);
                int draw_CountY = wafer_CountY + 8 + Convert.ToInt32(DrawCountOffset_Y);

                //m_WaferMapping.DieSizeX = dieSize_X;
                //m_WaferMapping.DieSizeY = dieSize_Y;
                //m_WaferMapping.DrawColumnCount = draw_CountX;
                //m_WaferMapping.DrawRowCount = draw_CountY;
                //m_WaferMapping.DieDrawInfo.Clear();

                Sinf = new SinfWaferMapping("", true, false);
                Sinf.Lot_ID = Wafer_ID;
                Sinf.Unit = "MM";
                Sinf.DieSize_X = dieSize_X;
                Sinf.DieSize_Y = dieSize_Y;
                Sinf.Count_Column = draw_CountX;
                Sinf.Count_Row = draw_CountY;
                Sinf.Dies_result = new DieDraw[draw_CountX, draw_CountY];


                //畫圖的參數
                scale = (Math.Max((draw_CountX * dieSize_X), (draw_CountY * dieSize_Y))) / mappingImageDrawSize;
                int greenPoint = 0, inkPoint = 0;
                this.StrokeThickness = Math.Min(dieSize_X / 2 / scale, dieSize_X / 2 / scale) / 4;
                this.CrossThickness = Math.Min(dieSize_X / 2 / scale, dieSize_X / 2 / scale) / 4;

                Point circleCenter = new Point((dieSize_X * draw_CountX / 2), (dieSize_Y * draw_CountY / 2));
                double circleRadius = waferSize / 2;


                if (TabControlDraw_SelectedIndex == 0)
                {
                    circleRadius = waferSize / 2;
                }
                else
                {
                    double xLength = DrawTotal_X * dieSize_X;
                    double yLength = DrawTotal_Y * dieSize_Y;

                    circleRadius = Math.Max(xLength, yLength) / 2;
                }
                List<bool> isDrawGreen = new List<bool>();
                for (int y = 0; y < draw_CountY; y++)
                {
                    for (int x = 0; x < draw_CountX; x++)
                    {
                        Point rectangleTopLeft = new Point(dieSize_X * x, dieSize_Y * y);
                        Point rectangleBottomRight = new Point(dieSize_X * (x + 1), dieSize_Y * (y + 1));
                        MappingDrawType waferMappingDrawType;
                        if (TabControlDraw_SelectedIndex == 0)
                        {
                            waferMappingDrawType = IsCircleTouchingRectangle(circleCenter, circleRadius, rectangleTopLeft, rectangleBottomRight, Convert.ToDouble(RimThickness_Distance));
                        }
                        else
                        {
                            waferMappingDrawType = IsCircleTouchingRectangle_TotalType(circleCenter, circleRadius, rectangleTopLeft, rectangleBottomRight);
                        }
                        Sinf.Dies_result[x, y] = new DieDraw();
                        Sinf.Dies_result[x, y].IndexX = x;
                        Sinf.Dies_result[x, y].IndexY = y;
                        Sinf.Dies_result[x, y].DieSizeX = dieSize_X;
                        Sinf.Dies_result[x, y].DieSizeY = dieSize_Y;
                        Sinf.Dies_result[x, y].PositionX = dieSize_X * (x + 0.5);
                        Sinf.Dies_result[x, y].PositionY = dieSize_Y * (y + 0.5);
                        Sinf.Dies_result[x, y].MapTransX = dieSize_X * (x + 0.5);
                        Sinf.Dies_result[x, y].MapTransY = dieSize_Y * (y + 0.5);

                        if (waferMappingDrawType == MappingDrawType.Skip)
                        {
                            Sinf.Dies_result[x, y].DieData = mappingDrawBinCode_Skip;
                            Sinf.Dies_result[x, y].TransDieData = mappingDrawBinCode_Skip;
                            //m_WaferMapping.DieDrawInfo.Add(new DieDrawInfo { BinCode = mappingDrawBinCode_Skip, IdxX = x, IdxY = y, DrawPosCenter = new Point(dieSize_X * (x + 0.5), dieSize_Y * (y + 0.5)) });
                        }
                        else if (waferMappingDrawType == MappingDrawType.Ink)
                        {
                            Sinf.Dies_result[x, y].DieData = mappingDrawBinCode_Ink;
                            Sinf.Dies_result[x, y].TransDieData = mappingDrawBinCode_Ink;
                            //m_WaferMapping.DieDrawInfo.Add(new DieDrawInfo { BinCode = mappingDrawBinCode_Ink, IdxX = x, IdxY = y, DrawPosCenter = new Point(dieSize_X * (x + 0.5), dieSize_Y * (y + 0.5)) });
                        }
                        else
                        {
                            Sinf.Dies_result[x, y].DieData = mappingDrawBinCode_Tested;
                            Sinf.Dies_result[x, y].TransDieData = mappingDrawBinCode_Tested;
                            //m_WaferMapping.DieDrawInfo.Add(new DieDrawInfo { BinCode = mappingDrawBinCode_Tested, IdxX = x, IdxY = y, DrawPosCenter = new Point(dieSize_X * (x + 0.5), dieSize_Y * (y + 0.5)) });
                        }
                    }
                }



                Sinf.Dies = new Die[Sinf.Dies_result.GetLength(0) * Sinf.Dies_result.GetLength(1)];
                Sinf.ColumnCount = Sinf.Dies_result.GetLength(0);
                Sinf.RowCount = Sinf.Dies_result.GetLength(1);
                int idx = 0;
                for (int i = 0; i < Sinf.Dies_result.GetLength(0); i++)
                {
                    for (int j = 0; j < Sinf.Dies_result.GetLength(1); j++)
                    {
                        double dieSizeX = 0;
                        double dieSizeY = 0;
                        dieSizeX = Sinf.Dies_result[i, j].DieSizeX;
                        dieSizeY = Sinf.Dies_result[i, j].DieSizeY;
                        Sinf.Dies[idx] = new Die
                        {
                            IndexX = Sinf.Dies_result[i, j].IndexX,
                            IndexY = Sinf.Dies_result[i, j].IndexY,
                            OperationPixalX = Sinf.Dies_result[i, j].PositionX,
                            OperationPixalY = Sinf.Dies_result[i, j].PositionY,
                            MapTransX = Sinf.Dies_result[i, j].PositionX,
                            MapTransY = Sinf.Dies_result[i, j].PositionY,
                            BinCode = Sinf.Dies_result[i, j].DieData,
                            DieSize = new Size(dieSizeX, dieSizeY)
                        };
                        idx++;
                    }
                }
                MappingImage = new WriteableBitmap(3000, 3000, 96, 96, System.Windows.Media.PixelFormats.Gray8, null);
                ShowMappingDrawings();

                //foreach (var item in m_WaferMapping.DieDrawInfo)
                //{
                //    Brush drawStroke = mappingDrawColor_Stroke;
                //    Brush drawFill = Brushes.Gray;
                //    if (item.BinCode == mappingDrawBinCode_Skip)
                //    {
                //        drawFill = mappingDrawColor_Skip;
                //    }
                //    else if (item.BinCode == mappingDrawBinCode_Ink)
                //    {
                //        drawFill = mappingDrawColor_Ink;
                //    }
                //    else if (item.BinCode == mappingDrawBinCode_Tested)
                //    {
                //        drawFill = mappingDrawColor_Tested;
                //    }

                //    AddShapeMappingAction.Execute(new ROIRotatedRect
                //    {
                //        Stroke = drawStroke,
                //        StrokeThickness = this.StrokeThickness,
                //        Fill = drawFill,
                //        X = item.DrawPosCenter.X / scale,
                //        Y = item.DrawPosCenter.Y / scale,
                //        LengthX = m_WaferMapping.DieSizeX / 2.5 / scale,//(dieSize.Width / 3) / showScale,
                //        LengthY = m_WaferMapping.DieSizeY / 2.5 / scale,//(dieSize.Height / 2) / showScale,
                //        IsInteractived = true,
                //        IsMoveEnabled = false,
                //        IsResizeEnabled = false,
                //        IsRotateEnabled = false,
                //        CenterCrossLength = this.CrossThickness,
                //        CenterCrossBrush = drawFill,
                //        ToolTip = "X=" + item.IdxX + " Y=" + item.IdxY + " posX=" + item.DrawPosCenter.X / scale + " posY=" + item.DrawPosCenter.Y / scale
                //    });
                //}






                //AddShapeMappingAction.Execute(new ROICircle
                //{
                //    Stroke = Brushes.Yellow,
                //    StrokeThickness = this.StrokeThickness,
                //    Fill = Brushes.Transparent,//Brushes.Blue,
                //    X = dieSize_X * draw_CountX / 2 / scale,
                //    Y = dieSize_Y * draw_CountY / 2 / scale,
                //    Radius = waferSize / 2 / scale,
                //    //LengthX = showSize_X / 3,//(dieSize.Width / 3) / showScale,
                //    //LengthY = showSize_Y / 3,//(dieSize.Height / 2) / showScale,
                //    IsInteractived = true,
                //    IsMoveEnabled = false,
                //    IsResizeEnabled = false,
                //    IsRotateEnabled = false,
                //    CenterCrossLength = 0,
                //    ToolTip = "Circle"
                //});


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                FormIsEnable = true;
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
