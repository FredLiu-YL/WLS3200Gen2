using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class HirataAligner_RS232 : IAligner
    {
        private SerialPort serialPort = new SerialPort();
        private readonly object lockObj = new object();
        private const char SOH = (char)0x01;
        private const char CR = (char)0x0D;
        private const string CODE = "00";
        private const string ADR = "00";
        private bool start_flag = false;
        private List<char> RxData;
        private string deviceStatus;
        private string errorCode;
        private string notchStatus;
        private bool isWafer;
        private bool isOrg;
        private bool isVaccum;

        public string DeviceStatus => deviceStatus;

        public string ErrorCode => errorCode;

        public string NotchStatus => notchStatus;

        public bool IsWafer => isWafer;

        public bool IsOrg => isOrg;

        public bool IsVaccum => isVaccum;

        public HirataAligner_RS232(string comPort)
        {
            try
            {
                ///
                serialPort.PortName = comPort;
                serialPort.BaudRate = 19200;
                serialPort.DataBits = 8; //只有7,8
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;
                //serialPort.RtsEnable = false;
                //serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler_Notch_Finder);
                //serialPort.Open();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 開啟連線
        /// </summary>
        public void Open()
        {
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// 關閉連線
        /// </summary>
        public void Close()
        {
            try
            {
                serialPort.Close();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Aligner移動到原點位置
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public AlignerItems Command_MovORG()
        {
            try
            {
                try
                {
                    AlignerItems alignerItems = new AlignerItems();
                    alignerItems.IsMovOK = false;
                    alignerItems.IsDone = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("MOV:ORGN;", true);
                    foreach (var item in str3)
                    {
                        if (item.Contains("MOV"))
                        {
                            alignerItems.IsMovOK = true;
                        }
                        if (item.Contains("INF"))
                        {
                            alignerItems.IsDone = true;
                        }
                    }
                    return alignerItems;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Aligner尋找Notch後移動到設定的偏移位置
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public AlignerItems Command_MovFindNotch()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                alignerItems.IsMovOK = false;
                alignerItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:ARLD;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        alignerItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        alignerItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        alignerItems.ErrorCode = error_code;
                        alignerItems.IsError = true;
                    }
                }
                return alignerItems;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Aligner移動到IDReader的位置
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public async Task Move_IDReadPos(AlignerItems alignerItems)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        alignerItems.IsMovOK = false;
                        alignerItems.IsDone = false;
                        List<string> str3 = new List<string>();
                        str3 = SendGetMessage("MOV:OTN2;", true);
                        foreach (var item in str3)
                        {
                            if (item.Contains("MOV"))
                            {
                                alignerItems.IsMovOK = true;
                            }
                            if (item.Contains("INF"))
                            {
                                alignerItems.IsDone = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                });

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 當Aligner移動到IDReader的位置完成後，可以移動到設定Notch找完後偏移的位置
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public async Task Move_IDReadPosOKThanMoveFindNotchPos(AlignerItems alignerItems)
        {
            try
            {
                await Task.Run(async () =>
                {
                    try
                    {
                        alignerItems.IsMovOK = false;
                        alignerItems.IsDone = false;
                        List<string> str3 = new List<string>();
                        str3 = SendGetMessage("MOV:N2TS;", true);
                        foreach (var item in str3)
                        {
                            if (item.Contains("MOV"))
                            {
                                alignerItems.IsMovOK = true;
                            }
                            if (item.Contains("INF"))
                            {
                                alignerItems.IsDone = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                });

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Aligner停止
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public async Task Mov_Stop(AlignerItems alignerItems)
        {
            await Task.Run(async () =>
            {
                try
                {
                    alignerItems.IsMovOK = false;
                    alignerItems.IsDone = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("MOV:STTP;", true);
                    foreach (var item in str3)
                    {
                        if (item.Contains("MOV"))
                        {
                            alignerItems.IsMovOK = true;
                        }
                        if (item.Contains("INF"))
                        {
                            alignerItems.IsDone = true;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
        }
        /// <summary>
        /// 取得目前Aligner的狀態
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public AlignerItems Command_GetStatus()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                alignerItems.IsGetOK = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("GET:STAS;", false);
                foreach (var item in str3)
                {
                    if (item.Contains("GET:STAS"))
                    {
                        alignerItems = TransStatus(item);
                        alignerItems.IsGetOK = true;
                    }
                }
                return alignerItems;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Aligner真空開啟
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public AlignerItems Command_MovVaccumON()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                alignerItems.IsMovOK = false;
                alignerItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:ACCL;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        alignerItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        alignerItems.IsDone = true;
                    }
                }
                return alignerItems;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Aligner真空關閉
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public AlignerItems Command_MovVaccumOFF()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                alignerItems.IsMovOK = false;
                alignerItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:ACOP;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        alignerItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        alignerItems.IsDone = true;
                    }
                }
                return alignerItems;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 設定找到AlignerNotch後偏移的位置
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public AlignerItems Command_SetFindNotchPos(double degree)
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                alignerItems.IsSetOK = false;
                alignerItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("SET:OFSE" + alignerItems.AlignerPos.FindNotchPos + ";", true);
                foreach (var item in str3)
                {
                    if (item.Contains("SET"))
                    {
                        alignerItems.IsSetOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        alignerItems.IsDone = true;
                    }
                }
                return alignerItems;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 設定讀取AlignerIDReader的位置
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public async Task Set_IDReadPos(AlignerItems alignerItems)
        {
            await Task.Run(async () =>
            {
                try
                {
                    alignerItems.IsSetOK = false;
                    alignerItems.IsDone = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("SET:OFS2" + alignerItems.AlignerPos.IDReadPos + ";", true);
                    foreach (var item in str3)
                    {
                        if (item.Contains("SET"))
                        {
                            alignerItems.IsMovOK = true;
                        }
                        if (item.Contains("INF"))
                        {
                            alignerItems.IsDone = true;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            });
        }
        /// <summary>
        /// Aligner異常重置
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <returns></returns>
        public AlignerItems Command_SetReset()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                alignerItems.IsSetOK = false;
                alignerItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("SET:RSET;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("SET"))
                    {
                        alignerItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        alignerItems.IsDone = true;
                    }
                }
                return alignerItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 轉換Get_Status讀取的狀態
        /// </summary>
        /// <param name="alignerItems"></param>
        /// <param name="str"></param>
        public AlignerItems TransStatus(string str)
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                char[] receive_data;
                receive_data = str.ToCharArray();
                //string type_code = str.Substring(0, 4);
                //if (type_code != "0000")
                //{
                //    //switch (str.Substring(0, 4))
                //    //{
                //    //    case "0100":
                //    //        logger_Notch_Finder.Write_Error_Logger("Checksum error");
                //    //        break;
                //    //    case "0200":
                //    //        logger_Notch_Finder.Write_Error_Logger("Command error");
                //    //        break;
                //    //    case "0300":
                //    //        logger_Notch_Finder.Write_Error_Logger("None");
                //    //        break;
                //    //    case "0400":
                //    //        logger_Notch_Finder.Write_Error_Logger("Interlock");
                //    //        break;
                //    //    case "0500":
                //    //        logger_Notch_Finder.Write_Error_Logger("In alarming");
                //    //        break;
                //    //    case "0600":
                //    //        可能要下STOP(MOV:STTP)
                //    //        logger_Notch_Finder.Write_Error_Logger("In command processing");
                //    //        break;
                //    //    case "0700":
                //    //        logger_Notch_Finder.Write_Error_Logger("Mode error");
                //    //        break;
                //    //}
                //}
                if (receive_data.Length > 20)
                {
                    int start_idx = 13;

                    if (receive_data[start_idx].ToString() == "0")
                    {
                        alignerItems.AlignerErrorStatus = AlignerErrorType.Normal;
                    }
                    else if (receive_data[start_idx].ToString() == "1")
                    {
                        alignerItems.AlignerErrorStatus = AlignerErrorType.Error;
                    }

                    alignerItems.AlignerModeStatus = (AlignerModeType)(int.Parse(receive_data[start_idx + 1].ToString()));

                    if (receive_data[start_idx + 2] == '0' || receive_data[start_idx + 2] == '1' || receive_data[start_idx + 2] == '2')
                    {
                        alignerItems.AlignerMachineDeviceStatus = (AlignerMachineDeviceType)(int.Parse(receive_data[start_idx + 2].ToString()));
                    }
                    else if (receive_data[start_idx + 2] == '4')
                    {
                        alignerItems.AlignerMachineDeviceStatus = AlignerMachineDeviceType.Load_Ready;
                    }

                    alignerItems.AlignerRunStatus = (AlignerRunType)(int.Parse(receive_data[start_idx + 3].ToString()));

                    char[] tmp = { receive_data[start_idx + 4], receive_data[start_idx + 5] };
                    if (new string(tmp) == "00")
                    {
                        alignerItems.ErrorCode = "";
                    }
                    else
                    {
                        alignerItems.ErrorCode = new string(tmp);
                    }


                    if (receive_data[start_idx + 6] == '0' && receive_data[start_idx + 7] == '1' && receive_data[start_idx + 8] == '1')
                    {
                        alignerItems.AlignerWaferStatus = AlignerWaferType.Wafer_Without;
                    }
                    else if (receive_data[start_idx + 6] == '1' && receive_data[start_idx + 7] == '1' && receive_data[start_idx + 8] == '1')
                    {
                        alignerItems.AlignerWaferStatus = AlignerWaferType.Wafer_Have;
                    }

                    alignerItems.AlignerOriginStatus = (AlignerOriginType)(int.Parse(receive_data[start_idx + 9].ToString()));

                    alignerItems.AlignerVaccumStatus = (AlignerVaccumType)(int.Parse(receive_data[start_idx + 10].ToString()));

                    alignerItems.AlignerLiftStatus = (AlignerLiftType)(int.Parse(receive_data[start_idx + 11].ToString()));

                    //AlignerNotchDetectionStatus

                    if (receive_data[start_idx + 12] == '1' && receive_data[start_idx + 13] == '0' && receive_data[start_idx + 14] == '0')
                    {
                        alignerItems.AlignerNotchDetectionStatus = AlignerNotchDetectionType.Detected_OK;
                    }
                    else if (receive_data[start_idx + 12] == '2' && receive_data[start_idx + 13] == '0' && receive_data[start_idx + 14] == '0')
                    {
                        alignerItems.AlignerNotchDetectionStatus = AlignerNotchDetectionType.PosFinNotch_Completed;
                    }
                    else if (receive_data[start_idx + 12] == '3' && receive_data[start_idx + 13] == '0' && receive_data[start_idx + 14] == '0')
                    {
                        alignerItems.AlignerNotchDetectionStatus = AlignerNotchDetectionType.PosIDReader_Completed;
                    }
                    else if (receive_data[start_idx + 12] == '0' && receive_data[start_idx + 13] == '0' && receive_data[start_idx + 14] == '0')
                    {
                        alignerItems.AlignerNotchDetectionStatus = AlignerNotchDetectionType.Detected_Not;
                    }
                }
                return alignerItems;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// RS2322送收資料，message要送的資料，isInfAbs是否要判讀INForABS結尾字元(正常執行完成or異常執行完成)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isInfAbs"></param>
        /// <returns></returns>
        public List<string> SendGetMessage(string message, bool isInfAbs)
        {
            try
            {
                lock (lockObj)
                {
                    int delayTime = 200;
                    int timeOut1 = 10 * 1000;
                    int timeOut2 = 600 * 1000;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    string writestr = CODE + ADR + message;
                    CheckSumAdd checkSum = CheckSumAdd_Analysis(writestr);
                    writestr = SOH + writestr + checkSum.CSh + checkSum.CSl + CR;
                    serialPort.Write(writestr);

                    List<string> returnMessage1 = new List<string>();
                    List<string> readMessage1 = new List<string>();
                    List<string> readMessage2 = new List<string>();
                    stopwatch.Restart();
                    bool isSendOK = false;
                    bool isSendOK_1 = false;
                    bool isSendOK_2 = false;
                    do
                    {
                        Thread.Sleep(delayTime);
                        readMessage1 = GetMessage();
                        if (stopwatch.ElapsedMilliseconds > timeOut2)
                        {
                            throw new Exception();
                        }
                        if (readMessage1.Count > 0)
                        {
                            foreach (var item in readMessage1)
                            {
                                returnMessage1.Add(item);
                                if (isInfAbs == false)
                                {
                                    if (item.Contains(message.Substring(0, 3)))
                                    {
                                        isSendOK = true;
                                    }
                                }
                                else
                                {
                                    if (item.Contains(message.Substring(0, 3)))
                                    {
                                        isSendOK_2 = true;
                                        string code = item.Substring(0, 2);
                                        if (code != "00")
                                        {
                                            isSendOK_1 = true;
                                        }
                                    }
                                    if (item.Contains("INF") || item.Contains("ABS"))
                                    {
                                        isSendOK_1 = true;
                                    }
                                    if (isSendOK_1 && isSendOK_2)
                                    {
                                        isSendOK = true;
                                    }
                                }
                            }
                        }
                    } while (isSendOK == false);
                    return returnMessage1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<string> GetMessage()
        {
            try
            {
                string indata;
                List<string> str_return = new List<string>();
                indata = serialPort.ReadExisting();
                char[] charArr = indata.ToCharArray();
                foreach (char rch in charArr)
                {
                    if (start_flag)
                    {
                        if (rch == CR)
                        {
                            string tmp1 = "";
                            foreach (char ch in RxData)
                            {
                                if (ch == SOH)
                                {
                                    continue;
                                }
                                if (ch == CR)
                                {
                                    break;
                                }
                                tmp1 += ch;
                            }
                            List<char> receive_data;
                            receive_data = new List<char>(tmp1.ToArray().ToList());
                            string str = new string(receive_data.ToArray());
                            start_flag = false;
                            str_return.Add(str);
                        }
                        else
                        {
                            RxData.Add(rch);
                        }
                    }
                    else if (rch == SOH)
                    {
                        start_flag = true;
                        RxData = new List<char>();
                    }
                }
                return str_return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 新增傳輸的CheckSum
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public CheckSumAdd CheckSumAdd_Analysis(string arr)
        {
            try
            {
                int sum = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    sum += arr[i];
                }
                string check_sum_str = sum.ToString("x").ToUpper();
                int len = check_sum_str.Length;

                CheckSumAdd checkSum = new CheckSumAdd();
                checkSum.CSh = check_sum_str[len - 2];
                checkSum.CSl = check_sum_str[len - 1];
                return checkSum;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Initial()
        {
            try
            {
                Open();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task Home()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                await Task.Run(() =>
                {
                    alignerItems = Command_MovORG();
                    if (alignerItems.IsMovOK == true)
                    {
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Run(double degree)
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                await Task.Run(() =>
                {
                    alignerItems = Command_SetFindNotchPos(degree);
                    if (alignerItems.IsSetOK == true)
                    {
                        alignerItems = Command_MovFindNotch();
                        if (alignerItems.IsMovOK == true)
                        {

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task VaccumOn()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                await Task.Run(() =>
                {
                    alignerItems = Command_MovVaccumON();
                    if (alignerItems.IsMovOK == true)
                    {

                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task VaccumOff()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                await Task.Run(() =>
                {
                    alignerItems = Command_MovVaccumOFF();
                    if (alignerItems.IsMovOK == true)
                    {

                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AlarmReset()
        {
            try
            {
                AlignerItems alignerItems = new AlignerItems();
                await Task.Run(() =>
                {
                    alignerItems = Command_SetReset();
                    if (alignerItems.IsSetOK == true)
                    {

                    }
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<AlignerStatus> GetStatus()
        {
            try
            {
                AlignerStatus alignerStatus = new AlignerStatus();
                AlignerItems alignerItems = new AlignerItems();
                await Task.Run(() =>
                {
                    alignerItems = Command_GetStatus();
                    if (alignerItems.IsGetOK == true)
                    {
                        alignerStatus = UpdateStatus(alignerItems);
                    }
                });
                return alignerStatus;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AlignerStatus UpdateStatus(AlignerItems alignerItems)
        {
            try
            {
                AlignerStatus alignerStatus = new AlignerStatus();
                if (alignerItems.AlignerMachineDeviceStatus == AlignerMachineDeviceType.Load_Ready)
                {
                    alignerStatus.DeviceStatus = "完成";
                }
                else if (alignerItems.AlignerMachineDeviceStatus == AlignerMachineDeviceType.Supply)
                {
                    alignerStatus.DeviceStatus = "供應正常";
                }
                else if (alignerItems.AlignerMachineDeviceStatus == AlignerMachineDeviceType.Extraction)
                {
                    alignerStatus.DeviceStatus = "提取";
                }
                else
                {
                    alignerStatus.DeviceStatus = "正常";
                }

                alignerStatus.ErrorCode = alignerItems.ErrorCode;
                if (alignerItems.AlignerNotchDetectionStatus == AlignerNotchDetectionType.Detected_OK)
                {
                    alignerStatus.NotchStatus = "有偵測到Notch";
                }
                else if (alignerItems.AlignerNotchDetectionStatus == AlignerNotchDetectionType.PosFinNotch_Completed)
                {
                    alignerStatus.NotchStatus = "FinNotch座標完成";
                }
                else if (alignerItems.AlignerNotchDetectionStatus == AlignerNotchDetectionType.PosIDReader_Completed)
                {
                    alignerStatus.NotchStatus = "IDReader座標完成";
                }
                else if (alignerItems.AlignerNotchDetectionStatus == AlignerNotchDetectionType.Detected_Not)
                {
                    alignerStatus.NotchStatus = "未偵測到Notch";
                }
                else
                {
                    alignerStatus.NotchStatus = "None";
                }

                if (alignerItems.AlignerWaferStatus == AlignerWaferType.Wafer_Have)
                {
                    alignerStatus.IsWafer = true;
                }
                else
                {
                    alignerStatus.IsWafer = false;
                }

                if (alignerItems.AlignerOriginStatus == AlignerOriginType.Origin_On)
                {
                    alignerStatus.IsOrg = true;
                }
                else
                {
                    alignerStatus.IsOrg = false;
                }

                if (alignerItems.AlignerVaccumStatus == AlignerVaccumType.ON)
                {
                    alignerStatus.IsVaccum = true;
                }
                else
                {
                    alignerStatus.IsVaccum = false;
                }

                return alignerStatus;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public class CheckSumAdd
        {
            public char CSh;
            public char CSl;
            public CheckSumAdd()
            {
                CSh = ' ';
                CSl = ' ';
            }
        }
        public enum Aligner_Command
        {
            IDEL,
            Get_Stas,
            Mov_Orgn,
            Set_Vaccum_On,
            Mov_Start_Aligner,
            Mov_ID_Reader_Pos,
            Mov_To_Notch_Pos,
            Set_Vaccum_Off,
            Set_Find_Notch_Pos,
            Set_ID_Reader_Pos,
            Clear_Error,
            Mov_Stop,
        }

    }
    public class AlignerItems
    {
        /// <summary>
        /// Mov指令OK
        /// </summary>
        public bool IsMovOK;
        /// <summary>
        /// Set指令OK
        /// </summary>
        public bool IsSetOK;
        /// <summary>
        /// Get_Mapping指令OK
        /// </summary>
        public bool IsMappingOK;
        /// <summary>
        /// 現在是否Error
        /// </summary>
        public bool IsError;
        /// <summary>
        /// Get_Status指令OK
        /// </summary>
        public bool IsGetOK;
        /// <summary>
        /// Mov指令或者是Set指令執行做完了
        /// </summary>
        public bool IsDone;

        public AlignerErrorType AlignerErrorStatus;

        public AlignerModeType AlignerModeStatus;

        public AlignerMachineDeviceType AlignerMachineDeviceStatus;

        public AlignerRunType AlignerRunStatus;
        /// <summary>
        /// 異常狀態Str
        /// </summary>
        public string ErrorCode;

        public AlignerWaferType AlignerWaferStatus;

        public AlignerOriginType AlignerOriginStatus;

        public AlignerVaccumType AlignerVaccumStatus;

        public AlignerLiftType AlignerLiftStatus;

        public AlignerNotchDetectionType AlignerNotchDetectionStatus;

        public AlignerPos AlignerPos = new AlignerPos();

        public AlignerItems()
        {
            IsMovOK = false;
            IsSetOK = false;
            IsError = false;
            IsGetOK = false;
            IsDone = false;
        }
    }
    public enum AlignerErrorType
    {
        None = -1,
        Normal = 0,
        Error = 1
    }
    public enum AlignerModeType
    {
        None = -1,
        Online = 0,
        Maintenance = 1
    }
    /// <summary>
    /// 目前運作的狀態
    /// </summary>
    public enum AlignerMachineDeviceType
    {
        None = -1,
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 供應正常
        /// </summary>
        Supply = 1,
        /// <summary>
        /// 提取
        /// </summary>
        Extraction = 2,
        /// <summary>
        /// 加載完成
        /// </summary>
        Load_Ready = 3
    }
    public enum AlignerRunType
    {
        None = -1,
        Stop = 0,
        Running = 1
    }
    public enum AlignerWaferType
    {
        None = -1,
        /// <summary>
        /// 沒有片子
        /// </summary>
        Wafer_Without = 0,
        /// <summary>
        /// 有片子
        /// </summary>
        Wafer_Have = 1
    }
    public enum AlignerOriginType
    {
        None = -1,
        /// <summary>
        /// 不在原點上
        /// </summary>
        Origin_Not = 0,
        /// <summary>
        /// 在原點上
        /// </summary>
        Origin_On = 1
    }
    public enum AlignerVaccumType
    {
        None = -1,
        /// <summary>
        /// 無真空建立
        /// </summary>
        OFF = 0,
        /// <summary>
        /// 真空建立
        /// </summary>
        ON = 1
    }
    public enum AlignerLiftType
    {
        None = -1,
        Position_Up = 0,
        Position_Down = 1,
        Unknown = 2
    }
    public enum AlignerNotchDetectionType
    {
        None = -1,
        /// <summary>
        /// 有偵測到
        /// </summary>
        Detected_OK = 0,
        /// <summary>
        /// FinNotch座標完成
        /// </summary>
        PosFinNotch_Completed = 1,
        /// <summary>
        /// IDReader座標完成
        /// </summary>
        PosIDReader_Completed = 2,
        /// <summary>
        /// 未偵測到
        /// </summary>
        Detected_Not = 3,
    }
    public enum AlignerSendMessageType
    {
        None = -1,
        Mov = 0,
        Set = 1,
        GetStats = 2
    }
    public class AlignerPos
    {
        public int FindNotchPos;
        public int IDReadPos;

        public AlignerPos()
        {
            FindNotchPos = 0;
            IDReadPos = 0;
        }
    }
}
