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
    public class HirataRobot_RS232
    {
        private SerialPort serialPort = new SerialPort();
        private readonly object lockObj = new object();

        //LP1:880 Aligner:810 Microscope:1810 LP2:980 Macro:710
        private string[] loadPort1_Pick_Command = new string[] { " GP 880", " GP 881", " GP 882", " GP 883" };
        private string[] loadPort1_Put_Command = new string[] { " GP 680", " GP 681", " GP 682", " GP 683" };
        private double wafer_Pitch = 10;
        private string[] loadPort2_Pick_Command = new string[] { " GP 980", " GP 981", " GP 982", " GP 983" };
        private string[] loadPort2_Put_Command = new string[] { " GP 780", " GP 781", " GP 782", " GP 783" };

        private string[] aligner_Pick_Command = new string[] { " GP 810", " GP 811", " GP 812", " GP 813" };
        private string[] aligner_Put_Command = new string[] { " GP 610", " GP 611", " GP 612", " GP 613" };

        private string[] microscope_Pick_Command = new string[] { " GP 1810", " GP 1811", " GP 1812", " GP 1813" };
        private string[] microscope_Put_Command = new string[] { " GP 1610", " GP 1611", " GP 1612", " GP 1613" };

        private string[] macro_Pick_Command = new string[] { " GP 710", " GP 711", " GP 712", " GP 713" };
        private string[] macro_Put_Command = new string[] { " GP 510", " GP 511", " GP 512", " GP 513" };

        private const string RB_NO = "001";
        private const char STX = '\x2';
        private const char ETX = '\x3';
        private short RX_LRC = 3;


        private List<char> RxData;
        public HirataRobot_RS232(string comPort)
        {
            try
            {
                serialPort.PortName = comPort;
                serialPort.BaudRate = 19200;
                serialPort.DataBits = 7; //只有7,8
                serialPort.Parity = Parity.Even;
                serialPort.StopBits = StopBits.One;
                serialPort.RtsEnable = false;

                serialPort.Open();
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
        /// 取得Robot目前狀態
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public async Task<RobotStatus> GetStatus()
        {
            RobotStatus robotStatus = new RobotStatus();
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("LS", CheckMessageType.Status);
                    foreach (var item in str)
                    {
                        robotStatus = TransStatus(item);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return robotStatus;
        }
        /// <summary>
        /// 取得Robot的AddressPos address:點位
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<RobotPoint> GetAddressPos(int address)
        {
            RobotPoint robot_Point = new RobotPoint();
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage(" LD " + address, CheckMessageType.Position);
                    foreach (var item in str)
                    {
                        robot_Point = TransPoint(item);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
            return robot_Point;
        }
        /// <summary>
        /// 移動到設定的Robot點位 address:點位，zShift:Z軸位移量
        /// </summary>
        /// <param name="address"></param>
        /// <param name="zShift"></param>
        /// <returns></returns>
        public async Task MovAddress(int address, double zShift)
        {
            RobotStatus robotStatus = new RobotStatus();
            int i = 0;
            double tolerance = 0.1;
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage(" GP " + address + " ( 0 0 " + zShift + " 0)", CheckMessageType.Status);
                    foreach (var item in str)
                    {
                        robotStatus = TransStatus(item);
                    }
                    RobotPoint robotAddressPoint = new RobotPoint();
                    RobotPoint robotNowPoint = new RobotPoint();
                    robotAddressPoint = await GetAddressPos(address);
                    while (true)
                    {
                        i++;
                        await Task.Delay(50);
                        robotNowPoint = await GetNowPos();
                        if (address == 1)
                        {
                            if (Math.Abs(robotAddressPoint.X - robotNowPoint.X) <= tolerance &&
                            Math.Abs(robotAddressPoint.Y - robotNowPoint.Y) <= tolerance)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (Math.Abs(robotAddressPoint.X - robotNowPoint.X) <= tolerance &&
                            Math.Abs(robotAddressPoint.Y - robotNowPoint.Y) <= tolerance &&
                            Math.Abs(robotAddressPoint.Z - robotNowPoint.Z) <= tolerance &&
                            Math.Abs(robotAddressPoint.W - robotNowPoint.W) <= tolerance)
                            {
                                break;
                            }
                        }
                        if (i >= 200) throw new Exception("Robot Move Time Out");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("MovAddress:" + ex);
                }
            });
        }
        /// <summary>
        /// Robot真空 isOn = true 開啟 isOn = false 關閉
        /// </summary>
        /// <param name="isOn"></param>
        /// <returns></returns>
        public async Task Vacuum(bool isOn)
        {
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    int Vacuum = 2;
                    if (isOn == true)
                    {
                        Vacuum = 1;
                    }
                    else
                    {
                        Vacuum = 2;
                    }
                    str = SendGetMessage(" SOD  0 " + Vacuum, CheckMessageType.Status);
                    foreach (var item in str)
                    {
                        TransStatus(item);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Vacuum:" + ex);
                }
            });
        }
        /// <summary>
        /// 設定Robot的移動速度 xywSpeed:XYW的速度設定 zSpeed:Z軸的速度
        /// </summary>
        /// <param name="xywSpeed"></param>
        /// <param name="zSpeed"></param>
        /// <returns></returns>
        public async Task SetMovSpeed(int xywSpeed, int zSpeed)
        {
            RobotStatus robotStatus = new RobotStatus();
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage(" SP " + xywSpeed + " " + zSpeed, CheckMessageType.Status);
                    foreach (var item in str)
                    {
                        robotStatus = TransStatus(item);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("SetMovSpeed:" + ex);
                }
            });
        }
        /// <summary>
        /// 取得RobotInput狀態
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetInput(int id)
        {
            bool isOn = false;
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("LID", CheckMessageType.IO);
                    foreach (var item in str)
                    {
                        isOn = TransIO(item, id, 8);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("GetInput:" + ex);
                }
            });
            return isOn;
        }
        /// <summary>
        /// 取得RobotOutput狀態
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetOutput(int id)
        {
            bool isOn = false;
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("LOD 0", CheckMessageType.IO);
                    foreach (var item in str)
                    {
                        isOn = TransIO(item, id, 8);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("GetOutput:" + ex);
                }
            });
            return isOn;
        }
        /// <summary>
        /// 取得Robot目前各軸位置
        /// </summary>
        /// <returns></returns>
        public async Task<RobotPoint> GetNowPos()
        {
            RobotPoint robot_Point = new RobotPoint();
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage(" LR", CheckMessageType.Position);
                    foreach (var item in str)
                    {
                        robot_Point = TransPoint(item);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("GetNowPos:" + ex);
                }
            });
            return robot_Point;
        }
        public async Task EMGStop()
        {
            RobotStatus robotStatus = new RobotStatus();

            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage(" GD ", CheckMessageType.Status);
                    foreach (var item in str)
                    {
                        robotStatus = TransStatus(item);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("EMGStop:" + ex);
                }
            });
        }
        public async Task Continue()
        {
            RobotStatus robotStatus = new RobotStatus();
            await Task.Run(async () =>
            {
                try
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage(" GE ", CheckMessageType.Status);
                    foreach (var item in str)
                    {
                        robotStatus = TransStatus(item);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Continue:" + ex);
                }
            });
        }

        private enum CheckMessageType
        {
            Status = 0,
            Position = 1,
            IO = 2
        }
        private bool TransIO(string str, int id, int bitCount)
        {
            try
            {
                bool status = false;
                string binaryValue = Convert.ToString(Convert.ToInt32(str), 2);
                string paddedBinaryValue = binaryValue.PadLeft(bitCount, '0');
                char[] paddedBinaryArray = paddedBinaryValue.ToCharArray();

                if (paddedBinaryArray[(paddedBinaryArray.Length - 1) - id] == '1')
                {
                    status = true;
                }
                else
                {
                    status = false;
                }
                return status;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private RobotPoint TransPoint(string str)
        {
            try
            {
                RobotPoint robotPoint = new RobotPoint();
                string[] split_data = str.Split(' ');
                robotPoint.X = Convert.ToDouble(split_data[0]);
                robotPoint.Y = Convert.ToDouble(split_data[1]);
                robotPoint.Z = Convert.ToDouble(split_data[2]);
                robotPoint.W = Convert.ToDouble(split_data[3].Replace("L", ""));

                return robotPoint;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private RobotStatus TransStatus(string str)
        {
            try
            {
                RobotStatus robotStatus = new RobotStatus();
                char[] receive_data;
                receive_data = str.ToCharArray();
                #region LS command process
                if (receive_data[0] == 'E') // abnormal status
                {
                    // S4 S3 S2 S1
                    switch (receive_data[4])
                    {
                        case '0':
                            robotStatus.Mode = "ON-LINE mode,Key 切換到MANUAL";
                            break;
                        case '1':
                            robotStatus.Mode = "ON-LINE";
                            break;
                        case '2':
                            robotStatus.Mode = "MANUAL";
                            break;
                        case '4':
                            robotStatus.Mode = "AUTO";
                            break;
                        default:
                            break;
                    }
                    switch (receive_data[3])
                    {
                        case '0':
                            robotStatus.IsStopSignal = false;
                            robotStatus.IsEStopSignal = false;
                            break;
                        case '2':
                            robotStatus.IsStopSignal = true;
                            robotStatus.IsEStopSignal = false;
                            break;
                        case '4':
                            robotStatus.IsStopSignal = false;
                            robotStatus.IsEStopSignal = true;
                            break;
                        case '6':
                            robotStatus.IsStopSignal = true;
                            robotStatus.IsEStopSignal = true;
                            break;
                        default:
                            break;
                    }
                    switch (receive_data[2])
                    {
                        case '0':
                            robotStatus.IsCommandDoneSignal = false;
                            robotStatus.IsMovDoneSignal = false;
                            break;
                        case '2':
                            robotStatus.IsCommandDoneSignal = false;
                            robotStatus.IsMovDoneSignal = true;
                            break;
                        case '4':
                            robotStatus.IsCommandDoneSignal = true;
                            robotStatus.IsMovDoneSignal = false;
                            break;
                        case '6':
                            robotStatus.IsCommandDoneSignal = true;
                            robotStatus.IsMovDoneSignal = true;
                            break;
                        default:
                            break;
                    }
                    switch (receive_data[1])
                    {
                        case '0':
                            robotStatus.IsRunning = false;
                            break;
                        case '4':
                            robotStatus.IsRunning = true;
                            break;
                        default:
                            break;
                    }
                    //E2 E1
                    string error_code = receive_data[5].ToString() + receive_data[6].ToString();
                    //AXYZWRC
                    string error_axis_info = receive_data[7].ToString() + receive_data[8].ToString() + receive_data[9].ToString() + receive_data[10].ToString();

                    robotStatus.ErrorCode = error_code;
                    try
                    {
                        robotStatus.ErrorX = receive_data[7];
                        robotStatus.ErrorY = receive_data[8];
                        robotStatus.ErrorZ = receive_data[9];
                        robotStatus.ErrorW = receive_data[10];
                        robotStatus.ErrorR = receive_data[11];
                        robotStatus.ErrorC = receive_data[12];
                    }
                    catch (Exception)
                    {
                    }
                }
                else // normal status
                {
                    switch (receive_data[3])
                    {
                        case '0':
                            robotStatus.Mode = "ON-LINE mode,Key 切換到MANUAL";
                            break;
                        case '1':
                            robotStatus.Mode = "ON-LINE";
                            break;
                        case '2':
                            robotStatus.Mode = "MANUAL";
                            break;
                        case '4':
                            robotStatus.Mode = "AUTO";
                            break;
                    }
                    switch (receive_data[2])
                    {
                        case '0':
                            robotStatus.IsStopSignal = false;
                            robotStatus.IsEStopSignal = false;
                            break;
                        case '2':
                            robotStatus.IsStopSignal = true;
                            robotStatus.IsEStopSignal = false;
                            break;
                        case '4':
                            robotStatus.IsStopSignal = false;
                            robotStatus.IsEStopSignal = true;
                            break;
                        case '6':
                            robotStatus.IsStopSignal = true;
                            robotStatus.IsEStopSignal = true;
                            break;
                    }
                    switch (receive_data[1])
                    {
                        case '0':
                            robotStatus.IsCommandDoneSignal = false;
                            robotStatus.IsMovDoneSignal = false;
                            break;
                        case '2':
                            robotStatus.IsCommandDoneSignal = false;
                            robotStatus.IsMovDoneSignal = true;
                            break;
                        case '4':
                            robotStatus.IsCommandDoneSignal = true;
                            robotStatus.IsMovDoneSignal = false;
                            break;
                        case '6':
                            robotStatus.IsCommandDoneSignal = true;
                            robotStatus.IsMovDoneSignal = true;
                            break;
                    }
                    switch (receive_data[0])
                    {
                        case '0':
                            robotStatus.IsRunning = false;
                            break;
                        case '4':
                            robotStatus.IsRunning = true;
                            break;
                    }
                    robotStatus.ErrorCode = "";
                    robotStatus.ErrorX = 0;
                    robotStatus.ErrorY = 0;
                    robotStatus.ErrorZ = 0;
                    robotStatus.ErrorW = 0;
                    robotStatus.ErrorR = 0;
                    robotStatus.ErrorC = 0;
                }
                #endregion
                return robotStatus;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        //0 1 4
        private List<string> SendGetMessage(string message, CheckMessageType checkType)
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
                    string writestr = new string(CheckSumAdd_Analysis(message).ToArray());
                    serialPort.Write(writestr);

                    List<string> returnMessage1 = new List<string>();
                    List<string> readMessage1 = new List<string>();
                    List<string> readMessage2 = new List<string>();
                    stopwatch.Restart();
                    do
                    {
                        Thread.Sleep(delayTime);
                        readMessage1 = GetMessage();
                        if (stopwatch.ElapsedMilliseconds > timeOut2)
                        {
                            throw new Exception("Robot SendGetMessage Time Out");
                        }
                        if (checkType == CheckMessageType.Position)//要取得位置資訊的
                        {
                            if (readMessage1.Count > 0)
                            {
                                foreach (var item in readMessage1)
                                {
                                    string[] ss = item.Split(' ');
                                    if (ss.Length == 5)
                                    {
                                        returnMessage1.Clear();
                                        foreach (var item2 in ss)
                                        {
                                            returnMessage1.Add(item2);
                                        }
                                        return returnMessage1;
                                    }
                                }
                            }
                        }
                        else if (checkType == CheckMessageType.IO)//要取得IO資訊
                        {
                            if (readMessage1.Count > 0)
                            {
                                foreach (var item in readMessage1)
                                {
                                    string[] ss = item.Split(' ');
                                    if (ss.Length == 1 && ss[0].Length == 1)
                                    {
                                        returnMessage1.Clear();
                                        returnMessage1.Add(ss[0]);
                                        return returnMessage1;
                                    }
                                }
                            }
                        }
                        else//取得狀態
                        {
                            if (readMessage1.Count > 0)
                            {
                                foreach (var item in readMessage1)
                                {
                                    string[] ss = item.Split(' ');
                                    if (ss.Length == 1 && ss[0].Length >= 4)
                                    {
                                        returnMessage1.Clear();
                                        returnMessage1.Add(ss[0]);
                                        if (message.Contains("GP"))
                                        {
                                            if (ss[0] == "4401")
                                            {
                                                return returnMessage1;
                                            }
                                        }
                                        else
                                        {
                                            return returnMessage1;
                                        }
                                    }
                                }
                            }
                        }
                    } while (true);

                    //return returnMessage1;
                }
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
        private List<char> CheckSumAdd_Analysis(string str)
        {
            try
            {
                List<char> com_buf = new List<char>();
                char tx_lrc;
                for (int p = 0; p < RB_NO.Length; p++)
                {
                    com_buf.Add(RB_NO[p]);
                }
                for (int j = 0; j < str.Length; j++)
                {
                    com_buf.Add(str[j]);
                }

                string s = "";
                for (int i = 0; i < com_buf.Count; i++)
                {
                    s += com_buf[i];
                }

                tx_lrc = ReturnLRC(s);
                com_buf.Add(ETX);
                com_buf.Add(tx_lrc);
                com_buf.Insert(0, STX);
                return com_buf;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static char ReturnLRC(string RequestMessage) //輸入STX開始到ETX結束前的資料
        {
            try
            {
                int lrcAnswer = 3; //先把03H放進來
                for (int i = 0; i < RequestMessage.Length; i++)
                {
                    lrcAnswer ^= (Byte)(Encoding.UTF7.GetBytes(RequestMessage.Substring(i, 1))[0]);
                }
                return (Char)lrcAnswer;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private bool STX_flag = false, ETX_flag = false;
        public List<string> GetMessage()
        {
            try
            {

                List<string> str_return = new List<string>();

                string indata = serialPort.ReadExisting();
                char[] charArr = indata.ToCharArray();
                foreach (char rch in charArr)
                {
                    if (ETX_flag)
                    {
                        if ((char)RX_LRC == rch)//check code
                        {
                            string tmp = "";
                            foreach (char ch in RxData)
                            {
                                if (ch == STX)
                                {
                                    continue;
                                }
                                if (ch == ETX)
                                {
                                    break;
                                }
                                tmp += ch;
                            }
                            tmp = tmp.Replace(RB_NO + " ", "");
                            List<char> receive_data = new List<char>(tmp.ToArray().ToList());
                            string str = new string(receive_data.ToArray());
                            STX_flag = false;
                            ETX_flag = false;
                            str_return.Add(str);
                        }
                        else
                        {
                            RxData.Clear();
                        }
                    }
                    else if (STX_flag)
                    {
                        if (rch == ETX)
                        {
                            ETX_flag = true;
                        }
                        else
                        {
                            RxData.Add(rch);
                            RX_LRC = (short)(RX_LRC ^ rch);
                        }
                    }
                    else if (rch == STX)
                    {
                        STX_flag = true;
                        RxData = new List<char>();
                        RX_LRC = 3;
                    }
                }




                return str_return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public class RobotItems
        {
            public int Status = -1;
            public int Mode = -1;
            public int Stop_Signal = -1;
            public int A_CAL_Pos = -1;
            public int Run_Status = -1;
            public int Vacuum = -1;
            public int Have_Wafer = -1;
            public RobotPoint now_pos = new RobotPoint();
            public string error_cdoe = "";
        }

    }
}
