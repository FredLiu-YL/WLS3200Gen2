using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class StrongLampRS232 : ILampControl
    {
        private SerialPort serialPort = new SerialPort();
        private readonly object lockObj = new object();
        private const string header = ":";
        private const string add = "01";

        public StrongLampRS232(string comPort)
        {
            try
            {
                serialPort.PortName = comPort;
                serialPort.BaudRate = 19200;
                serialPort.DataBits = 8; //只有7,8
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.RtsEnable = false;
                serialPort.NewLine = "\r\n";
                serialPort.WriteTimeout = 3000;
                serialPort.ReadTimeout = 3000;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int LightValue => ReadLampValue();
        /// <summary>
        /// LigntValue:0~255
        /// </summary>
        /// <param name="LigntValue"></param>
        /// <returns></returns>
        public Task ChangeLightAsync(int LigntValue)
        {
            return Task.Run(() =>
            {
                string hex = "00";
                hex = LigntValue.ToString("X2");
                int valCoarse = Convert.ToInt32(hex[0].ToString(), 16);
                int valFine = Convert.ToInt32(hex[1].ToString(), 16);
                SetLampValue(valCoarse, valFine);
            });
        }
        public void Initial()
        {
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
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

        private void SetLampValue(int lightValueCoarse, int lightValueCoarseFine)
        {
            try
            {
                // 處裡超過上限數值
                if (lightValueCoarse > 15)
                {
                    lightValueCoarse = 15;
                }
                if (lightValueCoarseFine > 15)
                {
                    lightValueCoarseFine = 15;
                }
                // Register Value
                string reg_val_str = "00" + IntConvertToStringSend(lightValueCoarse) + IntConvertToStringSend(lightValueCoarseFine);
                // function(2 char) + Register Address(4 char) + Register Value(4 char)
                string message = "06" + "0001" + reg_val_str;
                string str = "";
                str = SendGetMessage(message, 3);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int ReadLampValue()
        {
            try
            {
                string message = "0300010001";
                string str = "";
                str = SendGetMessage(message, 3);
                if (str.Contains(":01030200"))
                {
                    // 收到的data: Header(0) + Additional address(1~2) + Function(3~4) + Byte Count of Data(5~6)
                    // + Data(7~10) + Check(11~12)
                    int lightValueCoarse = 0;
                    int lightValueFine = 0;
                    lightValueCoarse = CharConvertToIntSend(str[9]);
                    lightValueFine = CharConvertToIntSend(str[10]);
                    int LigntValue2 = lightValueCoarse * 16 + lightValueFine;
                    string hex2 = LigntValue2.ToString("X2");
                    return LigntValue2;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        private string SendGetMessage(string message, int timeOutCount)
        {
            try
            {
                lock (lockObj)
                {
                    string LRC_str = CalLRC(message);
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    int retryCount = 0;
                    // 加入header, additional address ,主要data,結尾符號
                    serialPort.WriteLine(header + add + message + LRC_str);
                    string data = "";
                    while (true)//有些動作比較久，TimeOut時間內等不到，要再等
                    {
                        retryCount++;
                        try
                        {
                            data = serialPort.ReadLine();
                            return data;
                        }
                        catch (Exception ex)
                        {
                            //TimeOut
                            if (retryCount >= timeOutCount)
                            {
                                throw ex;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 計算LRC檢查碼
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string CalLRC(string str)
        {
            //兩個bit一組,共6組
            char add_char = (char)(Convert.ToInt32(add[0].ToString()) * 16 + Convert.ToInt32(add[1].ToString()));
            char func = (char)(Convert.ToInt32(str[0].ToString()) * 16 + Convert.ToInt32(str[1].ToString()));
            char reg_addr1 = (char)(Convert.ToInt32(str[2].ToString()) * 16 + Convert.ToInt32(str[3].ToString()));
            char reg_addr2 = (char)(Convert.ToInt32(str[4].ToString()) * 16 + Convert.ToInt32(str[5].ToString()));
            char reg_val1 = (char)(Convert.ToInt32(str[6].ToString()) * 16 + Convert.ToInt32(str[7].ToString()));
            char reg_val2 = (char)(CharConvertToIntSend(str[8]) * 16 + CharConvertToIntSend(str[9]));

            string cal_LRC_str = add_char.ToString() + func.ToString() + reg_addr1.ToString() +
                                reg_addr2.ToString() + reg_val1.ToString() + reg_val2.ToString();

            char LRC_val = (char)0xFF;
            char tmp = (char)0x00;
            for (int i = 0; i < cal_LRC_str.Length; i++)
            {
                tmp += cal_LRC_str[i];
            }
            tmp -= (char)0x01;
            if (tmp >= 256)
            {
                tmp = Convert.ToChar((int)tmp - 256);
            }
            LRC_val -= tmp;

            //解析LRC檢查碼
            int LRC1 = (int)LRC_val / 16;
            int LRC0 = (int)LRC_val % 16;
            string LRC_str = IntConvertToStringSend(LRC1) + IntConvertToStringSend(LRC0);
            return LRC_str;
        }
        private int CharConvertToIntSend(char val)
        {
            int ascii_start_A = 65;
            int char_to_int = val;
            if (val >= ascii_start_A)
            {
                return val - ascii_start_A + 10;
            }
            return Convert.ToInt32(val.ToString());
        }
        private string IntConvertToStringSend(int val)
        {
            int ascii_start_A = 65;
            if (val >= 10)
            {
                return Convert.ToChar(val - 10 + ascii_start_A).ToString();
            }
            else
            {
                return val.ToString();
            }

        }

    }
}
