using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using YuanliCore.Data;
using YuanliCore.Interface;

namespace WLS3200Gen2.Model.Recipe
{
    /// <summary>
    /// 紀錄Map圖 的全部座標
    /// </summary>
    public class WaferMapping
    {
        public WaferMapping(bool isMotionXMirror, bool isMotionYMirror)
        {
            //  var a = ReadWaferFile(path, isMotionXMirror, isMotionYMirror);
            //   Dies = a.dies;
            //  WaferSize = a.waferSize;
        }
        /// <summary>
        /// Wafer 計算起點
        /// </summary>
        public System.Drawing.Point OriginPoint { get; set; }
        /// <summary>
        /// Notch角度 0  90  180  270
        /// </summary>
        public double NotchDirection { get; set; }
        /// <summary>
        /// Map 中心位置(畫出來的Map要是對稱性，偏邊的話此值對應會有問題)
        /// </summary>
        public Point MapCenterPoint { get; set; }
        /// <summary>
        /// 每個Die的資訊
        /// </summary>
        public Die[] Dies { get; set; }

        /// <summary>
        /// Wafer尺寸(um)
        /// </summary>
        public Size WaferSize { get; set; }

        /// <summary>
        /// 行總數
        /// </summary>
        public int ColumnCount { get; set; }
        /// <summary>
        /// 列總數
        /// </summary>
        public int RowCount { get; set; }
        public string Lot_ID { get; set; }
        public int Wafer_Idx { get; set; }
        public int Count_Row { get; set; }
        public int Count_Column { get; set; }
        /// <summary>
        /// DUTMS(單位大小)
        /// </summary>
        public string Unit { get; set; }//DUTMS
        public double DieSize_X { get; set; }
        public double DieSize_Y { get; set; }
        public double Start_LeftX { get; set; }
        public double Start_TopY { get; set; }

        public WaferMapping Copy()
        {
            Die[] copiedDies = new Die[this.Dies.Length];
            for (int i = 0; i < this.Dies.Length; i++)
            {
                copiedDies[i] = this.Dies[i].Copy();
            }

            return new WaferMapping(false, false)
            {
                OriginPoint = this.OriginPoint,
                NotchDirection = this.NotchDirection,
                MapCenterPoint = this.MapCenterPoint,
                WaferSize = this.WaferSize,
                ColumnCount = this.ColumnCount,
                RowCount = this.RowCount,
                Lot_ID = this.Lot_ID,
                Wafer_Idx = this.Wafer_Idx,
                Count_Row = this.Count_Row,
                Count_Column = this.Count_Column,
                Unit = this.Unit,
                DieSize_X = this.DieSize_X,
                DieSize_Y = this.DieSize_Y,
                Start_LeftX = this.Start_LeftX,
                Start_TopY = this.Start_TopY,
                Dies = copiedDies

            };
        }
    }

    public class SinfWaferMapping : WaferMapping
    {
        /// <summary>
        /// path路徑 isMotionXMirror是否要映射X isMotionYMirror是否要映射Y
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isMotionXMirror"></param>
        /// <param name="isMotionYMirror"></param>
        public SinfWaferMapping(bool isMotionXMirror, bool isMotionYMirror) : base(isMotionXMirror, isMotionYMirror)
        {

        }
        public SinfWaferMapping Copy()
        {
            Die[] copiedDies = new Die[this.Dies.Length];
            for (int i = 0; i < this.Dies.Length; i++)
            {
                copiedDies[i] = this.Dies[i].Copy();
            }

            return new SinfWaferMapping(false, false)
            {
                OriginPoint = this.OriginPoint,
                NotchDirection = this.NotchDirection,
                MapCenterPoint = this.MapCenterPoint,
                WaferSize = this.WaferSize,
                ColumnCount = this.ColumnCount,
                RowCount = this.RowCount,
                Lot_ID = this.Lot_ID,
                Wafer_Idx = this.Wafer_Idx,
                Count_Row = this.Count_Row,
                Count_Column = this.Count_Column,
                Unit = this.Unit,
                DieSize_X = this.DieSize_X,
                DieSize_Y = this.DieSize_Y,
                Start_LeftX = this.Start_LeftX,
                Start_TopY = this.Start_TopY,
                Dies = copiedDies

            };
        }
        /// <summary>
        /// path路徑 isMotionXMirror是否要映射X isMotionYMirror是否要映射Y
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isMotionXMirror"></param>
        /// <param name="isMotionYMirror"></param>
        /// <returns></returns>
        public (Die[] dies, Size waferSize) ReadWaferFile(string path, bool isMotionXMirror, bool isMotionYMirror)
        {
            try
            {
                Mode = SinfMode.Sinf_Sample;
                Device_Name = "";
                Lot_ID = "";
                Wafer_Idx = 1;
                Notch_Degree = 0;
                Count_Row = 0;

                Count_Column = 0;
                Bin_Codes = "000";
                Ref_PointX = 0;
                Ref_PointY = 0;
                Unit = "MM";
                DieSize_X = 1;
                DieSize_Y = 1;
                Die[] die = new Die[1];
                die[0] = new Die();
                die[0].IndexX = 0;
                die[0].IndexY = 0;
                die[0].PosX = 0;
                die[0].PosY = 0;
                die[0].BinCode = "";
                die[0].DieSize = new Size(0, 0);
                Size waferSize = new Size();
                if (System.IO.File.Exists(path))
                {
                    dies_result = new DieDraw[0, 0];
                    ReadSinf(path);
                    dies_result = RotateMap(posDirection, dies_result, isMotionXMirror, isMotionYMirror);
                    if (posDirection == Direction.Right_Button || posDirection == Direction.Left_Top)
                    {
                        MapCenterPoint = new Point(mapCenter.X, mapCenter.Y);
                    }
                    else
                    {
                        MapCenterPoint = new Point(mapCenter.Y, mapCenter.X);
                    }
                    die = new Die[dies_result.GetLength(0) * dies_result.GetLength(1)];
                    int idx = 0;
                    ColumnCount = dies_result.GetLength(0);
                    RowCount = dies_result.GetLength(1);
                    for (int i = 0; i < dies_result.GetLength(0); i++)
                    {
                        for (int j = 0; j < dies_result.GetLength(1); j++)
                        {
                            die[idx] = new Die
                            {
                                IndexX = dies_result[i, j].TransIndexX,
                                IndexY = dies_result[i, j].TransIndexY,
                                OperationPixalX = dies_result[i, j].TransPositionX,
                                OperationPixalY = dies_result[i, j].TransPositionY,
                                MapTransX = dies_result[i, j].MapTransX,
                                MapTransY = dies_result[i, j].MapTransY,
                                BinCode = dies_result[i, j].TransDieData,
                                DieSize = new Size(dies_result[i, j].DieSizeX, dies_result[i, j].DieSizeY)
                            };
                            idx++;
                        }
                    }
                }
                return (die, waferSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SaveWaferFile(string path, bool isMotionXMirror, bool isMotionYMirror)
        {
            try
            {
                List<string> lines = new List<string>();
                lines.Add("DEVICE:" + Device_Name);
                lines.Add("LOT:" + Lot_ID);
                lines.Add("WAFER:" + Wafer_Idx);
                lines.Add("FNLOC:" + Notch_Degree);
                lines.Add("ROWCT:" + Count_Row);
                lines.Add("COLCT:" + Count_Column);
                lines.Add("BCEQU:" + Bin_Codes);
                lines.Add("REFPX:" + Ref_PointX);
                lines.Add("REFPY:" + Ref_PointY);
                lines.Add("DUTMS:" + Unit);
                int scale = 1;
                if (Unit.ToUpper() == "MM")
                {
                    scale = 1000;
                }
                lines.Add("XDIES:" + DieSize_X / scale);
                lines.Add("YDIES:" + DieSize_Y / scale);

                if (Mode == SinfMode.Sinf_Dir || Mode == SinfMode.Sinf_Dir_Total)
                {
                    lines.Add("LEFT_X:" + Start_LeftX);
                    lines.Add("TOP_Y:" + Start_TopY);
                    lines.Add("X_INC_DIR:" + Direction_X);
                    lines.Add("Y_INC_DIR:" + Direction_Y);
                    if (Mode == SinfMode.Sinf_Dir_Total)
                    {
                        lines.Add("TOTAL_DIE:" + Count_Column * Count_Row);
                    }
                }

                if (isMotionYMirror)
                {
                    if (isMotionXMirror)
                    {
                        for (int j = Count_Row; j >= 0; j--)
                        {
                            int startX = Count_Column;
                            string rowString = Dies.FirstOrDefault(die => die.IndexX == startX && die.IndexY == j).BinCode;
                            for (int i = Count_Column - 1; i >= 0; i--)
                            {
                                var date = Dies.FirstOrDefault(die => die.IndexX == i && die.IndexY == j).BinCode;
                                rowString += " " + date;
                            }
                            lines.Add("RowData:" + rowString);
                        }
                    }
                    else
                    {
                        for (int j = Count_Row; j >= 0; j--)
                        {
                            int startX = 0;
                            string rowString = Dies.FirstOrDefault(die => die.IndexX == startX && die.IndexY == j).BinCode;
                            for (int i = 1; i < Count_Column; i++)
                            {
                                var date = Dies.FirstOrDefault(die => die.IndexX == i && die.IndexY == j).BinCode;
                                rowString += " " + date;
                            }
                            lines.Add("RowData:" + rowString);
                        }
                    }
                }
                else
                {
                    if (isMotionXMirror)
                    {
                        for (int j = 0; j < Count_Row; j++)
                        {
                            int startX = Count_Column;
                            string rowString = Dies.FirstOrDefault(die => die.IndexX == startX && die.IndexY == j).BinCode;
                            for (int i = Count_Column - 1; i >= 0; i--)
                            {
                                var date = Dies.FirstOrDefault(die => die.IndexX == i && die.IndexY == j).BinCode;
                                rowString += " " + date;
                            }
                            lines.Add("RowData:" + rowString);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < Count_Row; j++)
                        {
                            int startX = 0;
                            string rowString = Dies.FirstOrDefault(die => die.IndexX == startX && die.IndexY == j).BinCode;
                            for (int i = 1; i < Count_Column; i++)
                            {
                                var date = Dies.FirstOrDefault(die => die.IndexX == i && die.IndexY == j).BinCode;
                                rowString += " " + date;
                            }
                            lines.Add("RowData:" + rowString);
                        }
                    }
                }
                System.IO.File.WriteAllLines(path, lines);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private SinfMode Mode { get; set; }
        /// <summary>
        /// FNLOC(Wafer的大小)
        /// </summary>
        private string Wafer_Size { get; set; }
        private string Device_Name { get; set; }
        private double Notch_Degree { get; set; }
        /// <summary>
        /// BCEQU
        /// </summary>
        private string Bin_Codes { get; set; }
        private double Ref_PointX { get; set; }
        private double Ref_PointY { get; set; }
        private string Direction_X { get; set; }
        private string Direction_Y { get; set; }
        private double Total_Die { get; set; }
        public List<BincodeInfo> BinCode_result { get; set; } = new List<BincodeInfo>();
        private DieDraw[,] dies_result { get; set; }

        private Direction posDirection;

        private Point mapCenter;
        private void ReadSinf(string pfilePath)
        {
            try
            {
                Wafer_Idx = 0;
                Start_LeftX = 1;
                Start_TopY = 1;
                Direction_X = "";
                Direction_Y = "";
                Mode = SinfMode.Error;
                posDirection = Direction.Right_Button;
                string[] lines = System.IO.File.ReadAllLines(pfilePath);
                string lineRowData_1 = lines[12].Substring(0, 7);
                string lineRowData_2 = lines[16].Substring(0, 7);
                string lineRowData_3 = lines[17].Substring(0, 7);
                DieSize_X = 1;
                DieSize_Y = 1;
                if (lineRowData_1 == "RowData")
                {
                    Mode = SinfMode.Sinf_Sample;
                }
                else if (lineRowData_2 == "RowData")
                {
                    Mode = SinfMode.Sinf_Dir;
                }
                else if (lineRowData_3 == "RowData")
                {
                    Mode = SinfMode.Sinf_Dir_Total;
                }
                else
                {
                    Mode = SinfMode.Error;
                }

                Device_Name = lines[0].Split(':')[1];
                Lot_ID = lines[1].Split(':')[1];
                if ((lines[2].Split(':')[1]) != "")
                {
                    Wafer_Idx = Convert.ToInt32(lines[2].Split(':')[1]);
                }
                Notch_Degree = Convert.ToDouble(lines[3].Split(':')[1]);
                if ((lines[4].Split(':')[1]) != "")
                {
                    Count_Row = Convert.ToInt32(lines[4].Split(':')[1]);
                }
                if ((lines[5].Split(':')[1]) != "")
                {
                    Count_Column = Convert.ToInt32(lines[5].Split(':')[1]);
                }
                Bin_Codes = lines[6].Split(':')[1];
                if ((lines[7].Split(':')[1]) != "")
                {
                    Ref_PointX = Convert.ToDouble((lines[7].Split(':')[1]));
                }
                if ((lines[8].Split(':')[1]) != "")
                {
                    Ref_PointY = Convert.ToDouble(lines[8].Split(':')[1]);
                }
                Unit = lines[9].Split(':')[1];
                if ((lines[10].Split(':')[1]) != "")
                {
                    if (Unit.ToUpper() == "MM")
                    {
                        DieSize_X = Convert.ToDouble(lines[10].Split(':')[1]) * 1000;
                    }
                    else
                    {
                        DieSize_X = Convert.ToDouble(lines[10].Split(':')[1]);
                    }

                }
                if ((lines[11].Split(':')[1]) != "")
                {
                    if (Unit.ToUpper() == "MM")
                    {
                        DieSize_Y = Convert.ToDouble(lines[11].Split(':')[1]) * 1000;
                    }
                    else
                    {
                        DieSize_Y = Convert.ToDouble(lines[11].Split(':')[1]);
                    }

                }

                if (Mode == SinfMode.Sinf_Dir || Mode == SinfMode.Sinf_Dir_Total)
                {
                    Start_LeftX = Convert.ToDouble(lines[12].Split(':')[1]);
                    Start_TopY = Convert.ToDouble(lines[13].Split(':')[1]);
                    Direction_X = lines[14].Split(':')[1];
                    Direction_Y = lines[15].Split(':')[1];

                    if (Direction_X == "R")
                    {
                        if (Direction_Y == "B")
                        {
                            posDirection = Direction.Right_Button;
                        }
                        else
                        {
                            posDirection = Direction.Right_Top;
                        }
                    }
                    else
                    {
                        if (Direction_Y == "B")
                        {
                            posDirection = Direction.Left_Button;
                        }
                        else
                        {
                            posDirection = Direction.Left_Top;
                        }
                    }

                    if (Mode == SinfMode.Sinf_Dir_Total)
                    {
                        Total_Die = Convert.ToInt32(lines[16].Split(':')[1]);
                    }
                }
                int startIdx = 0;
                if (Mode == SinfMode.Sinf_Sample)
                {
                    startIdx = 12;
                }
                else if (Mode == SinfMode.Sinf_Dir)
                {
                    startIdx = 16;
                }
                else if (Mode == SinfMode.Sinf_Dir_Total)
                {
                    startIdx = 17;
                }
                else
                {

                }

                int total_X = Count_Column;

                int total_Y = Count_Row;

                dies_result = new DieDraw[total_X, total_Y];

                string[] mapline;
                for (int i = startIdx; i <= lines.Length - 1; i++)
                {
                    mapline = lines[i].Replace("RowData:", "").Split(' ');
                    for (int j = 0; j <= mapline.Length - 1; j++)
                    {
                        if (j == 60)
                        {
                            int cc = 0;
                        }
                        if (!(j >= Count_Column || (i - startIdx) >= Count_Row))
                        {
                            dies_result[j, i - startIdx] = new DieDraw();
                            dies_result[j, i - startIdx].DieData = mapline[j];
                            dies_result[j, i - startIdx].IndexX = j;
                            dies_result[j, i - startIdx].IndexY = i - startIdx;

                            dies_result[j, i - startIdx].DieSizeX = DieSize_X;
                            dies_result[j, i - startIdx].DieSizeY = DieSize_Y;
                            dies_result[j, i - startIdx].PositionX = j * DieSize_X;//DieSize_X / 2 +
                            dies_result[j, i - startIdx].PositionY = (i - startIdx) * DieSize_Y;//DieSize_Y / 2 + 
                        }
                    }
                }

                double centerX = (Count_Column * DieSize_X) / 2;
                double centerY = ((lines.Length - startIdx) * DieSize_Y) / 2;
                mapCenter = new Point(centerX, centerY);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void NewDieDraw(DieDraw[,] newDieDraw)
        {
            try
            {
                dies_result = newDieDraw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private DieDraw[,] RotateMap(Direction direction, DieDraw[,] die_Draw, bool isMotionXMirror, bool isMotionYMirror)
        {
            try
            {
                int total_X = die_Draw.GetLength(0);
                int total_Y = die_Draw.GetLength(1);
                double dieSizeX = 0;
                double dieSizeY = 0;
                if (direction == Direction.Right_Button || direction == Direction.Left_Top)
                {
                    dieSizeX = die_Draw[0, 0].DieSizeX;
                    dieSizeY = die_Draw[0, 0].DieSizeY;
                }
                else
                {
                    dieSizeX = die_Draw[0, 0].DieSizeY;
                    dieSizeY = die_Draw[0, 0].DieSizeX;
                }

                int newX;
                int newY;
                int mapTransIndexX;
                int mapTransIndexY;
                int newX_idx = total_X - 1;
                int newX_idY = total_Y - 1;
                for (int i = 0; i <= total_X - 1; i++)
                {
                    if (direction == Direction.Right_Button || direction == Direction.Right_Top)
                    {
                        newX = i;
                        mapTransIndexX = i;
                        if (isMotionXMirror)
                        {
                            mapTransIndexX = newX_idx;
                        }
                    }
                    else
                    {
                        newX = newX_idx;
                        mapTransIndexX = newX_idx;
                        if (isMotionXMirror)
                        {
                            mapTransIndexX = i;

                        }
                    }
                    for (int j = 0; j <= total_Y - 1; j++)
                    {
                        newY = j;
                        if (direction == Direction.Right_Button || direction == Direction.Left_Button)
                        {
                            newY = j;
                            mapTransIndexY = j;
                            if (isMotionYMirror)
                            {
                                mapTransIndexY = newX_idY;
                            }
                        }
                        else
                        {
                            newY = newX_idY;
                            mapTransIndexY = newX_idY;
                            if (isMotionYMirror)
                            {
                                mapTransIndexY = j;
                            }
                        }

                        die_Draw[i, j].TransIndexX = newX;
                        die_Draw[i, j].TransIndexY = newY;
                        die_Draw[i, j].TransPositionX = dieSizeX / 2 + newX * dieSizeX;
                        die_Draw[i, j].TransPositionY = dieSizeY / 2 + newY * dieSizeY;
                        die_Draw[i, j].MapTransX = dieSizeX / 2 + mapTransIndexX * dieSizeX;
                        die_Draw[i, j].MapTransY = dieSizeY / 2 + mapTransIndexY * dieSizeY;
                        die_Draw[i, j].TransDieData = die_Draw[newX, newY].DieData;

                        newX_idY--;
                    }
                    newX_idx--;
                    newX_idY = total_Y - 1;
                }




                return die_Draw;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void MappingGenerateSaveAS(string pathFile, int star_X, int end_X, int star_Y, int end_Y)
        {
            try
            {
                DieDraw[] flattenedArray = dies_result.Cast<DieDraw>().ToArray();

                foreach (var item in Dies)
                {
                    flattenedArray.FirstOrDefault(die => die.TransIndexX == item.IndexX && die.TransIndexY == item.IndexY).DieData = item.BinCode;
                }
                int cols = dies_result.GetLength(0);
                int rows = dies_result.GetLength(1);

                List<string> lines = new List<string>();
                lines.Add("DEVICE:" + Device_Name);
                lines.Add("LOT:" + Lot_ID);
                lines.Add("WAFER:" + Wafer_Idx);
                lines.Add("FNLOC:" + Notch_Degree);
                lines.Add("ROWCT:" + (end_Y - star_Y + 1));
                lines.Add("COLCT:" + (end_X - star_X + 1));
                lines.Add("BCEQU:" + Bin_Codes);
                lines.Add("REFPX:" + Ref_PointX);
                lines.Add("REFPY:" + Ref_PointY);
                lines.Add("DUTMS:" + Unit);
                int scale = 1;
                if (Unit.ToUpper() == "MM")
                {
                    scale = 1000;
                }
                lines.Add("XDIES:" + DieSize_X / scale);
                lines.Add("YDIES:" + DieSize_Y / scale);

                if (Mode == SinfMode.Sinf_Dir || Mode == SinfMode.Sinf_Dir_Total)
                {
                    lines.Add("LEFT_X:" + Start_LeftX);
                    lines.Add("TOP_Y:" + Start_TopY);
                    lines.Add("X_INC_DIR:" + Direction_X);
                    lines.Add("Y_INC_DIR:" + Direction_Y);
                    if (Mode == SinfMode.Sinf_Dir_Total)
                    {
                        lines.Add("TOTAL_DIE:" + Total_Die);
                    }
                }
                for (int j = star_Y; j <= end_Y; j++)
                {
                    int indexX = dies_result[star_X, j].IndexX;
                    int indexY = dies_result[star_X, j].IndexY;

                    string rowString = flattenedArray.FirstOrDefault(die => die.IndexX == indexX && die.IndexY == indexY).DieData;

                    for (int i = star_X + 1; i <= end_X; i++)
                    {
                        indexX = dies_result[i, j].IndexX;
                        indexY = dies_result[i, j].IndexY;
                        rowString += " " + flattenedArray.FirstOrDefault(die => die.IndexX == indexX && die.IndexY == indexY).DieData;
                    }
                    lines.Add("RowData:" + rowString);
                }
                System.IO.File.WriteAllLines(pathFile, lines);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ReadBinCode(string pfilePath)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(pfilePath);
                int count = 0;
                BinCode_result.Clear();
                foreach (var line in lines)
                {
                    string[] splitLine = line.Split(' ');
                    Brush nowBrush = Brushes.Black;
                    //if (colorBasic.Length > count)
                    //{
                    //    nowBrush = colorBasic[count];
                    //}
                    BinCode_result.Add(new BincodeInfo { Code = splitLine[0], Describe = splitLine[1], Color = nowBrush });
                    count++;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public enum SinfMode
        {
            Sinf_Sample,
            Sinf_Dir,
            Sinf_Dir_Total,
            Error
        }
        public enum Direction
        {
            /// <summary>
            /// OrgPos->LeftTop  Degree 0
            /// </summary>
            Right_Button,
            /// <summary>
            /// OrgPos->RightTop  Degree 270
            /// </summary>
            Left_Button, //
            /// <summary>
            /// OrgPos->LeftButton Degree 90
            /// </summary>
            Right_Top,
            /// <summary>
            /// OrgPos->RightButton  Degree 180
            /// </summary>
            Left_Top
        }
        public class DieDraw
        {
            public double PositionX;

            public double PositionY;

            public int IndexX;

            public int IndexY;

            public double TransPositionX;

            public double TransPositionY;

            public int TransIndexX;

            public int TransIndexY;

            public double DieSizeX;

            public double DieSizeY;

            public string DieData;

            public string TransDieData;

            public double MapTransX;

            public double MapTransY;

            public List<PictureInfo> DieDataList = new List<PictureInfo>();

            public List<PictureInfo> TransDieDataList = new List<PictureInfo>();
        }
        public class PictureInfo
        {
            public int Column { get; set; }
            public int Row { get; set; }
            public string Data { get; set; }
        }

        public static SinfWaferMapping Load(string filename)
        {

            throw new NotImplementedException();
        }
    }


    public class KLAWaferMapping : WaferMapping
    {
        public KLAWaferMapping(string path, bool isMotionXMirror, bool isMotionYMirror) : base(isMotionXMirror, isMotionYMirror)
        {
        }
        public (Die[] dies, Size waferSize) ReadWaferFile(string path, bool isMotionXMirror, bool isMotionYMirror)
        {
            throw new NotImplementedException();
        }

        public void SaveWaferFile(string path)
        {
            throw new NotImplementedException();
        }
    }


    public enum Direction
    {
        MirrorX,
        MirrorY,
        MirrorXY,

    }

}
