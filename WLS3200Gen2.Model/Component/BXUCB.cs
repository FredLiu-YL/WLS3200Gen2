using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class BXUCB : IMicroScope
    {
        private readonly object lockObj = new object();
        private SerialPort serialPort = new SerialPort();
        private string Microscope_Terminator;
        public BXUCB(string comPort)
        {
            try
            {
                serialPort.PortName = comPort;
                serialPort.BaudRate = 19200;
                serialPort.DataBits = 8; //只有7,8
                serialPort.Parity = Parity.Even;
                serialPort.StopBits = StopBits.Two;
                serialPort.RtsEnable = false;
                Microscope_Terminator = "\r\n";
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
                serialPort.Open();
                LogInOutBXFM(true);
                LogInOutA2M(true);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        //private double z_PositionNEL = 0;

        //private double z_PositionPEL = 0;

        //public double Z_PositionNEL
        //{
        //    get => z_PositionNEL;
        //    set
        //    {
        //        z_PositionNEL = value;
        //        SetZNEL(Convert.ToInt32(value));
        //    }
        //}
        //public double Z_PositionPEL
        //{
        //    get => z_PositionPEL;
        //    set
        //    {
        //        z_PositionPEL = value;
        //        SetZPEL(Convert.ToInt32(value));
        //    }
        //}

        public Task AberrationMoveCommand(double distance)
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    if (distance > 0)
                    {
                        str = SendGetMessage("2AMOV N," + Math.Abs(distance), "AMOV");
                    }
                    else
                    {
                        str = SendGetMessage("2AMOV F," + Math.Abs(distance), "AMOV");
                    }
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("AMOV"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2AMOV !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB AberrationMoveCommand Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task AberrationMoveToCommand(double position)
        {
            try
            {
                return Task.Run(async () =>
                {
                    double nowPos = await GetAberationPosition();
                    double distance = position - nowPos;
                    List<string> str = new List<string>();
                    if (distance > 0)
                    {
                        str = SendGetMessage("2AMOV N," + Math.Abs(distance), "AMOV");
                    }
                    else
                    {
                        str = SendGetMessage("2AMOV F," + Math.Abs(distance), "AMOV");
                    }
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("AMOV"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2AMOV !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB AberrationMoveToCommand Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task AF_Off()
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("2AF OFF", "AF");
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("AF"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2AF !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB AF_Off Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task AF_OneShot()
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("2AF SHOT", "AF");
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("AF"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2AF !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB AF_OneShot Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task AF_Trace()
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("2AF REAL", "AF");
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("AF"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2AF !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB AF_Trace Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ChangeAperture(int ApertureValue)
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("1EAS " + ApertureValue, "EAS");
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("EAS"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("1EAS !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB ChangeAperture Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ChangeCube(int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    if (idx == 0)
                    {
                        str = SendGetMessage("1CUBE 1" + idx, "CUBE");
                    }
                    else if (idx == 1)
                    {
                        str = SendGetMessage("1CUBE 2" + idx, "1CUBE");
                    }
                    else if (idx == 2)
                    {
                        str = SendGetMessage("1CUBE 3" + idx, "1CUBE");
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ChangeFilter(int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("1FW " + idx, "FW");
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("FW"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("1FW !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB ChangeFilter Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task ChangeLight(int LigntValue)
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    bool isOK = false;
                    string errorStr = "";

                    if (LigntValue <= 0)
                    {
                        str = SendGetMessage("1LMPSW OFF", "LMPSW");
                    }
                    else
                    {
                        str = SendGetMessage("1LMPSW ON", "LMPSW");
                    }
                    foreach (var item in str)
                    {
                        if (item.Contains("LMPSW"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("1LMPSW !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB ChangeLight Error:" + errorStr);
                    }

                    isOK = false;
                    errorStr = "";
                    str = SendGetMessage("1LMP " + LigntValue, "LMP");
                    foreach (var item in str)
                    {
                        if (item.Contains("LMP"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("1LMP !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB ChangeLight Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task ChangeLightSpread(int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    if (idx == 0)
                    {
                        str = SendGetMessage("1LMPSEL DIA" + idx, "LMPSEL");
                    }
                    else if (idx == 1)
                    {
                        str = SendGetMessage("1LMPSEL EPI" + idx, "LMPSEL");
                    }
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("LMPSEL"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("1LMPSEL !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB ChangeLightSpread Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ChangeLens(int idx)
        {
            try
            {
                return Task.Run(async () =>
                {
                    List<string> str = new List<string>();
                    await AF_Off();
                    str = SendGetMessage("1OB " + idx, "OB");
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("OB"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("1OB !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB ChangeLens Error:" + errorStr);
                    }
                });
            }
            catch (Exception)
            {

                throw;
            }


        }



        public Task SetSearchRange(double FirstZPos, double Range)
        {
            try
            {
                return Task.Run(async () =>
                {
                    await AF_Off();
                    List<string> str = new List<string>();
                    double setZPEL = FirstZPos + Range;
                    double z_PositionPEL = await GetZPEL();
                    double z_PositionNEL = await GetZNEL();
                    if (FirstZPos + Range >= z_PositionPEL)
                    {
                        setZPEL = z_PositionPEL;
                    }
                    double setZNEL = FirstZPos - Range;
                    if (FirstZPos - Range <= z_PositionNEL)
                    {
                        setZNEL = z_PositionNEL;
                    }
                    await SetAFPEL(Convert.ToInt32(setZPEL));//710000
                    await SetAFNEL(Convert.ToInt32(setZNEL));//350000
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task ZMoveCommand(double distance)
        {
            try
            {
                return Task.Run(() =>
               {
                   List<string> str = new List<string>();
                   if (distance > 0)
                   {
                       str = SendGetMessage("2MOV N," + Math.Abs(distance), "MOV");
                   }
                   else
                   {
                       str = SendGetMessage("2MOV F," + Math.Abs(distance), "MOV");
                   }
                   bool isOK = false;
                   string errorStr = "";
                   foreach (var item in str)
                   {
                       if (item.Contains("MOV"))
                       {
                           if (item.Contains("+"))
                           {
                               isOK = true;
                           }
                           if (item.Contains("!"))
                           {
                               errorStr = item.Replace("2MOV !,", "");
                               break;
                           }
                       }
                   }
                   if (isOK == false)
                   {
                       throw new Exception("BXUCB ZMoveCommand Error:" + errorStr);
                   }
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ZMoveToCommand(double position)
        {
            try
            {
                return Task.Run(async () =>
                {
                    double nowPos = await GetZPosition();
                    double distance = position - nowPos;
                    List<string> str = new List<string>();
                    if (distance > 0)
                    {
                        str = SendGetMessage("2MOV N," + Math.Abs(distance), "MOV");
                    }
                    else
                    {
                        str = SendGetMessage("2MOV F," + Math.Abs(distance), "MOV");
                    }
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("MOV"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2MOV !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB ZMoveToCommand Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void LogInOutBXFM(bool isLogIn)
        {
            try
            {
                List<string> str = new List<string>();
                if (isLogIn)
                {
                    str = SendGetMessage("1LOG IN", "LOG");
                }
                else
                {
                    str = SendGetMessage("1LOG OUT", "LOG");
                }
                bool isOK = false;
                string errorStr = "";
                foreach (var item in str)
                {
                    if (item.Contains("LOG"))
                    {
                        if (item.Contains("+"))
                        {
                            isOK = true;
                        }
                        if (item.Contains("!"))
                        {
                            errorStr = item.Replace("1LOG !,", "");
                            break;
                        }
                    }
                }
                if (isOK == false)
                {
                    throw new Exception("BXUCB LogInOutBXFM Error:" + errorStr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void LogInOutA2M(bool isLogIn)
        {
            try
            {
                List<string> str = new List<string>();
                if (isLogIn)
                {
                    str = SendGetMessage("2LOG IN", "LOG");
                }
                else
                {
                    str = SendGetMessage("2LOG OUT", "LOG");
                }
                bool isOK = false;
                string errorStr = "";
                foreach (var item in str)
                {
                    if (item.Contains("LOG"))
                    {
                        if (item.Contains("+"))
                        {
                            isOK = true;
                        }
                        if (item.Contains("!"))
                        {
                            errorStr = item.Replace("2LOG !,", "");
                            break;
                        }
                    }
                }
                if (isOK == false)
                {
                    throw new Exception("BXUCB LogInOutA2M Error:" + errorStr);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<double> GetZNEL()
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("2FARLMT?", "FARLMT");
                    bool isOK = false;
                    string errorStr = "";
                    double nowPos = 0;
                    foreach (var item in str)
                    {
                        if (item.Contains("2FARLMT"))
                        {
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2FARLMT !,", "");
                                break;
                            }
                            else
                            {
                                isOK = true;
                                nowPos = Convert.ToDouble(item.Replace("2FARLMT ", ""));
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB GetZNEL Error:" + errorStr);
                    }
                    return nowPos;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task SetZNEL(int position)
        {
            try
            {
                return Task.Run(() =>
               {
                   List<string> str = new List<string>();
                   str = SendGetMessage("2FARLMT " + position, "FARLMT");
                   //z_PositionNEL = position;
                   bool isOK = false;
                   string errorStr = "";
                   foreach (var item in str)
                   {
                       if (item.Contains("FARLMT"))
                       {
                           if (item.Contains("+"))
                           {
                               isOK = true;
                           }
                           if (item.Contains("!"))
                           {
                               errorStr = item.Replace("2FARLMT !,", "");
                               break;
                           }
                       }
                   }
                   if (isOK == false)
                   {
                       throw new Exception("BXUCB SetZNEL Error:" + errorStr);
                   }
               });

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task<double> GetZPEL()
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("2NEARLMT?", "NEARLMT");
                    bool isOK = false;
                    string errorStr = "";
                    double nowPos = 0;
                    foreach (var item in str)
                    {
                        if (item.Contains("2NEARLMT"))
                        {
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2NEARLMT !,", "");
                                break;
                            }
                            else
                            {
                                isOK = true;
                                nowPos = Convert.ToDouble(item.Replace("2NEARLMT ", ""));
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB GetZPEL Error:" + errorStr);
                    }
                    return nowPos;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task SetZPEL(int position)
        {
            try
            {
                return Task.Run(() =>
               {
                   List<string> str = new List<string>();
                   str = SendGetMessage("2NEARLMT " + position, "NEARLMT");
                   //z_PositionPEL = position;
                   bool isOK = false;
                   string errorStr = "";
                   foreach (var item in str)
                   {
                       if (item.Contains("NEARLMT"))
                       {
                           if (item.Contains("+"))
                           {
                               isOK = true;
                           }
                           if (item.Contains("!"))
                           {
                               errorStr = item.Replace("2NEARLMT !,", "");
                               break;
                           }
                       }
                   }
                   if (isOK == false)
                   {
                       throw new Exception("BXUCB SetZPEL Error:" + errorStr);
                   }
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task<int> GetAFNEL()
        {
            try
            {
                return Task.Run(() =>
               {
                   List<string> str = new List<string>();
                   str = SendGetMessage("2AFFLMT?", "AFFLMT");
                   int value = 0;
                   foreach (var item in str)
                   {
                       if (item.Contains("AFFLMT"))
                       {
                           value = Convert.ToInt32(item.Replace("2AFFLMT ", ""));
                       }
                   }
                   return value;
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task SetAFNEL(int position)
        {
            try
            {
                return Task.Run(() =>
               {
                   List<string> str = new List<string>();
                   str = SendGetMessage("2AFFLMT " + position, "AFFLMT");
                   bool isOK = false;
                   string errorStr = "";
                   foreach (var item in str)
                   {
                       if (item.Contains("AFFLMT"))
                       {
                           if (item.Contains("+"))
                           {
                               isOK = true;
                           }
                           if (item.Contains("!"))
                           {
                               errorStr = item.Replace("2AFFLMT !,", "");
                               break;
                           }
                       }
                   }
                   if (isOK == false)
                   {
                       throw new Exception("BXUCB SetAberationNEL Error:" + errorStr);
                   }
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 取得AFPEL值
        /// </summary>
        /// <returns></returns>
        public Task<int> GetAFPEL()
        {
            try
            {
                return Task.Run(() =>
               {
                   List<string> str = new List<string>();
                   str = SendGetMessage("2AFNLMT?", "AFNLMT");
                   int value = 0;
                   foreach (var item in str)
                   {
                       if (item.Contains("AFNLMT"))
                       {
                           value = Convert.ToInt32(item.Replace("2AFNLMT ", ""));
                       }
                   }
                   return value;
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public Task SetAFPEL(int position)
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("2AFNLMT " + position, "AFNLMT");
                    bool isOK = false;
                    string errorStr = "";
                    foreach (var item in str)
                    {
                        if (item.Contains("AFNLMT"))
                        {
                            if (item.Contains("+"))
                            {
                                isOK = true;
                            }
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2AFNLMT !,", "");
                                break;
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB SetAberationPEL Error:" + errorStr);
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task<double> GetZPosition()
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("2POS?", "POS");
                    bool isOK = false;
                    string errorStr = "";
                    double nowPos = 0;
                    foreach (var item in str)
                    {
                        if (item.Contains("2POS"))
                        {
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2POS !,", "");
                                break;
                            }
                            else
                            {
                                isOK = true;
                                nowPos = Convert.ToDouble(item.Replace("2POS ", ""));
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB GetZPosition Error:" + errorStr);
                    }
                    return nowPos;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task<double> GetAberationPosition()
        {
            try
            {
                return Task.Run(() =>
                {
                    List<string> str = new List<string>();
                    str = SendGetMessage("2APOS? ", "APOS");
                    bool isOK = false;
                    string errorStr = "";
                    double nowPos = 0;
                    foreach (var item in str)
                    {
                        if (item.Contains("2APOS"))
                        {
                            if (item.Contains("!"))
                            {
                                errorStr = item.Replace("2APOS !,", "");
                                break;
                            }
                            else
                            {
                                isOK = true;
                                nowPos = Convert.ToDouble(item.Replace("2APOS ", ""));
                            }
                        }
                    }
                    if (isOK == false)
                    {
                        throw new Exception("BXUCB GetAberationPosition Error:" + errorStr);
                    }
                    return nowPos;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public List<string> SendGetMessage(string message, string checkString)
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
                    serialPort.Write(message + Microscope_Terminator);
                    List<string> returnMessage1 = new List<string>();
                    List<string> readMessage1 = new List<string>();
                    stopwatch.Restart();
                    bool isSendOK = false;
                    do
                    {
                        Thread.Sleep(delayTime);
                        readMessage1 = GetMessage();
                        if (stopwatch.ElapsedMilliseconds > timeOut2)
                        {
                            throw new Exception("BXUCB SendGetMessage Time Out");
                        }
                        if (readMessage1.Count > 0)
                        {
                            foreach (var item in readMessage1)
                            {
                                returnMessage1.Add(item);
                                if (item.Contains(checkString))
                                {
                                    isSendOK = true;
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
                List<string> str_return = new List<string>();
                //string indata;
                //indata = serialPort.ReadExisting();
                //char[] charArr = indata.ToCharArray();
                //foreach (char rch in charArr)
                //{
                //}
                int bytesCnt = serialPort.BytesToRead;
                if (bytesCnt != 0)
                {
                    byte[] sp_Read = new byte[bytesCnt];
                    serialPort.Read(sp_Read, 0, bytesCnt);
                    string Receive_Data = "";
                    for (int i = 0; i < bytesCnt; i++)
                    {
                        Receive_Data = Receive_Data + Convert.ToChar(sp_Read[i]);
                    }
                    serialPort.DiscardInBuffer();
                    string[] receive_array = Receive_Data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    str_return.AddRange(receive_array);
                }
                return str_return;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
