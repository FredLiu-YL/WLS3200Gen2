using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YuanliCore.Model.Interface;

namespace WLS3200Gen2.Model.Component
{
    public class BXUCB : IMicroscope
    {
        private readonly object lockObj = new object();
        private SerialPort serialPort = new SerialPort();
        private TaskCompletionSource<string> tcsReceived;
        private string microscope_Terminator;
        private int apertureValue = -1;
        private double cubeIdx = -1;
        private int[] filterWheelIdx = new int[] { -1, -1, -1 };
        private int lens = -1;
        private int lightValue = -1;
        private int lightSpreadIdx = -1;
        private string errcode = "";
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
                serialPort.NewLine = "\r\n";
                microscope_Terminator = "\r\n";
                serialPort.DataReceived += DataReceived;
                tcsReceived = new TaskCompletionSource<string>();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int LightValue { get => lightValue; }
        public int ApertureValue { get => apertureValue; }

        public int Position => Convert.ToInt32(GetZPosition());

        public int NEL { get => Convert.ToInt32(GetZNEL()); set => SetZNEL(value); }
        public int PEL { get => Convert.ToInt32(GetZPEL()); set => SetZPEL(value); }

        public double AberationPosition => GetAberationPosition();

        public int AFNEL { get => GetAFNEL(); set => SetAFNEL(value); }
        public int AFPEL { get => GetAFPEL(); set => SetAFPEL(value); }

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
        public Task HomeAsync()
        {
            try
            {
                return Task.Run(async () =>
               {
                   await MoveToAsync(1);
                   await ChangeLensAsync(1);
                   await ChangeApertureAsync(0);
                   await ChangeLightAsync(0);
                   await ChangeFilterAsync(1, 1);
                   await ChangeFilterAsync(2, 1);
               });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        //public double ApertureValue
        //{
        //    get => apertureValue;
        //    set
        //    {
        //        if (apertureValue != value)
        //        {
        //            await ChangeAperture(value);
        //        }
        //        apertureValue = value;
        //    }
        //}


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

        public Task AberrationMoveAsync(double distance)
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

        public Task AberrationMoveToAsync(double position)
        {
            try
            {
                return Task.Run(async () =>
                {
                    double nowPos = GetAberationPosition();
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

        public void AFOff()
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task AFOneShotAsync()
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

        public void AFTrace()
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ChangeApertureAsync(int ApertureValue)
        {
            try
            {
                return Task.Run(() =>
                {
                    if (apertureValue != ApertureValue)
                    {
                        apertureValue = ApertureValue;
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
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ChangeCubeAsync(int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    if (cubeIdx != idx)
                    {
                        cubeIdx = idx;
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
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ChangeFilterAsync(int wheelIdx, int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    if (filterWheelIdx[wheelIdx] != idx)
                    {
                        filterWheelIdx[wheelIdx] = idx;
                        List<string> str = new List<string>();
                        str = SendGetMessage("1FW" + wheelIdx + " " + idx, "FW");
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
                                    errorStr = item.Replace("1FW" + wheelIdx + " !,", "");
                                    break;
                                }
                            }
                        }
                        if (isOK == false)
                        {
                            throw new Exception("BXUCB ChangeFilter Error:" + errorStr);
                        }
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task ChangeLightAsync(int LigntValue)
        {
            try
            {
                return Task.Run(() =>
                {
                    if (lightValue != LigntValue)
                    {
                        lightValue = LigntValue;
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
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task ChangeLightSpreadAsync(int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    if (lightSpreadIdx != idx)
                    {
                        lightSpreadIdx = idx;
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
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Task ChangeLensAsync(int idx)
        {
            try
            {
                return Task.Run(async () =>
                {
                    if (lens != idx)
                    {
                        lens = idx;
                        List<string> str = new List<string>();
                        AFOff();
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
                    }
                });
            }
            catch (Exception)
            {

                throw;
            }


        }



        public void SetSearchRange(double FirstZPos, double Range)
        {
            try
            {
                AFOff();
                List<string> str = new List<string>();
                double setZPEL = FirstZPos + Range;
                double z_PositionPEL = GetZPEL();
                double z_PositionNEL = GetZNEL();
                if (FirstZPos + Range >= z_PositionPEL)
                {
                    setZPEL = z_PositionPEL;
                }
                double setZNEL = FirstZPos - Range;
                if (FirstZPos - Range <= z_PositionNEL)
                {
                    setZNEL = z_PositionNEL;
                }
                SetAFPEL(Convert.ToInt32(setZPEL));//710000
                SetAFNEL(Convert.ToInt32(setZNEL));//350000
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Task MoveAsync(double distance)
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

        public Task MoveToAsync(double position)
        {
            try
            {
                return Task.Run(async () =>
                {
                    double nowPos = this.Position;
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
        private double GetZNEL()
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void SetZNEL(int position)
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public double GetZPEL()
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void SetZPEL(int position)
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int GetAFNEL()
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void SetAFNEL(int position)
        {
            try
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
        public int GetAFPEL()
        {
            try
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
        public void SetAFPEL(int position)
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private double GetZPosition()
        {
            try
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
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public double GetAberationPosition()
        {
            try
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
                    int delayTime = 50;
                    int timeOut1 = 10 * 1000;
                    int timeOut2 = 600 * 1000;
                    Stopwatch stopwatch = new Stopwatch();
                    List<string> returnMessage1 = new List<string>();
                    List<string> readMessage1 = new List<string>();

                    stopwatch.Start();
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    serialPort.Write(message + microscope_Terminator);
                    Thread.Sleep(delayTime);
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
        private List<string> SendGetMessage_New(string message, string checkString)
        {
            lock (lockObj)
            {
                List<string> returnMessage1 = new List<string>();
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();
                serialPort.WriteLine(message);

                int maxRetryMilliseconds = 5 * 1000;
                string data = "";
                SpinWait.SpinUntil(() =>
                {
                    try
                    {
                        data = tcsReceived.Task.Result;//會停在這一行
                    }
                    catch
                    {
                    }
                    return (data != "");
                }, maxRetryMilliseconds);

                if (data.Contains("+"))
                {
                    returnMessage1.Add("OK");
                    return returnMessage1;
                }

                if (data.Contains("!"))
                {
                    errcode = data;
                }
                returnMessage1.Add("Error");
                return returnMessage1;
            }
        }
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadExisting();

                tcsReceived.TrySetResult(data);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private Task TimeoutWhile(int ms)
        {
            return Task.Run(() =>
             {
                 SpinWait.SpinUntil(() =>
                 {
                     string data = "";
                     try
                     {
                         data = tcsReceived.Task.Result;//會停在這一行
                     }
                     catch
                     {
                     }
                     return (data != "");
                 }, ms);
             });
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


    public class BXFMA : IMicroscope2
    {
        private readonly object lockObj = new object();
        private SerialPort serialPort = new SerialPort();

        public BXFMA(string comPort)
        {
            try
            {
                serialPort.PortName = comPort;
                serialPort.BaudRate = 19200;
                serialPort.DataBits = 8; //只有7,8
                serialPort.Parity = Parity.Even;
                serialPort.StopBits = StopBits.Two;
                serialPort.RtsEnable = false;
                serialPort.NewLine = "\r\n";
                //    Microscope_Terminator = "\r\n";

                serialPort.DataReceived += DataReceived;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int LightValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ApertureValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double Position => throw new NotImplementedException();

        public double PositionPEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PositionNEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double AberationPosition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double AutoFocusPEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double AutoFocusNEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int LensNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public void Initial()
        {
            try
            {
                serialPort.Open();
                LogIn();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void Home()
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Open()
        {
            throw new NotImplementedException();
        }



        private void SendMessage(string message)
        {
            lock (lockObj)
            {
                serialPort.Write(message);



            }



        }

        private void LogIn()
        {
            SendMessage("1LOG IN");
            SendMessage("2LOG IN");
        }
        private void LogOut()
        {
            SendMessage("1LOG OUT");
            SendMessage("2LOG OUT");
        }
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            string data = serialPort.ReadExisting();

        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void MoveFocusPosition()
        {
            throw new NotImplementedException();
        }
    }

}
