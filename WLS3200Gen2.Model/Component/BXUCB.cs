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
                serialPort.WriteTimeout = 3000;
                serialPort.ReadTimeout = 3000;
                microscope_Terminator = "\r\n";
                //serialPort.DataReceived += DataReceived;
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

        public int AberationPosition => GetAberationPosition();

        public int AFNEL { get => GetAFNEL(); set => SetAFNEL(value); }
        public int AFPEL { get => GetAFPEL(); set => SetAFPEL(value); }
        public int TimeOutRetryCount { get; set; } = 1;

        public int LensIndex { get; private set; } = -1;
        public int CubeIndex { get; private set; } = -1;
        public int Filter1Index { get; private set; } = -1;
        public int Filter2Index { get; private set; } = -1;
        public int Filter3Index { get; private set; } = -1;

        public event Action<Exception> Error;

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
                   await ChangeFilter1Async(1);
                   await ChangeFilter2Async(1);
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
                    string str = "";
                    int nowCount = 0;
                    while (true)
                    {
                        if (distance > 0)
                        {
                            str = SendGetMessage("2AMOV N," + Math.Abs(distance), 3);
                        }
                        else
                        {
                            str = SendGetMessage("2AMOV F," + Math.Abs(distance), 3);
                        }

                        if (str.Contains("AMOV +"))
                        {
                            break;
                        }
                        else if (str.Contains("AMOV !"))
                        {
                            string errorStr = str.Replace("2AMOV !,", "");
                            throw new Exception("BXUCB AberrationMoveCommand Error:" + errorStr);
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("BXUCB AberrationMoveCommand Error:Retry " + TimeOutRetryCount + " Count");
                            }
                        }
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
                    double nowPos = 0;
                    double distance = 0;
                    string str = "";
                    int nowCount = 0;

                    while (true)
                    {
                        nowPos = GetAberationPosition();
                        distance = position - nowPos;
                        if (distance > 0)
                        {
                            str = SendGetMessage("2AMOV N," + Math.Abs(distance), 3);
                        }
                        else
                        {
                            str = SendGetMessage("2AMOV F," + Math.Abs(distance), 3);
                        }

                        if (str.Contains("AMOV +"))
                        {
                            break;
                        }
                        else if (str.Contains("AMOV !"))
                        {
                            string errorStr = str.Replace("2AMOV !,", "");
                            throw new Exception("BXUCB AberrationMoveToAsync Error:" + errorStr);
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("BXUCB AberrationMoveToAsync Error:Retry " + TimeOutRetryCount + " Count");
                            }
                        }
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
                string str = "";
                int nowCount = 0;
                while (true)
                {
                    str = SendGetMessage("2AF OFF", 3);
                    if (str.Contains("AF +"))
                    {
                        break;
                    }
                    else if (str.Contains("AF !"))
                    {
                        string errorStr = str.Replace("2AF !,", "");
                        throw new Exception("BXUCB AF_Off Error:" + errorStr);
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB AF_Off Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                int nowCount = 0;
                string str = "";
                return Task.Run(() =>
                {
                    while (true)
                    {
                        str = SendGetMessage("2AF SHOT", 3);
                        if (str.Contains("AF +"))
                        {
                            break;
                        }
                        else if (str.Contains("AF !"))
                        {
                            string errorStr = str.Replace("2AF !,", "");
                            throw new Exception("BXUCB AFOneShotAsync Error:" + errorStr);
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("BXUCB AFOneShotAsync Error:Retry " + TimeOutRetryCount + " Count");
                            }
                        }
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
                int nowCount = 0;
                string str = "";
                while (true)
                {
                    str = SendGetMessage("2AF REAL", 3);
                    if (str.Contains("AF +"))
                    {
                        break;
                    }
                    else if (str.Contains("AF !"))
                    {
                        string errorStr = str.Replace("2AF !,", "");
                        throw new Exception("BXUCB AF_Trace Error:" + errorStr);
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB AF_Trace Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                        int nowCount = 0;
                        string str = "";
                        while (true)
                        {
                            str = SendGetMessage("1EAS " + ApertureValue, 3);
                            if (str.Contains("EAS +"))
                            {
                                break;
                            }
                            else if (str.Contains("EAS !"))
                            {
                                string errorStr = str.Replace("1EAS !,", "");
                                throw new Exception("BXUCB ChangeAperture Error:" + errorStr);
                            }
                            else
                            {
                                nowCount++;
                                if (nowCount > TimeOutRetryCount)
                                {
                                    throw new Exception("BXUCB ChangeAperture Error:Retry " + TimeOutRetryCount + " Count");
                                }
                            }
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
                        string str = "";
                        int nowCount = 0;
                        while (true)
                        {
                            str = SendGetMessage("1CUBE " + idx, 3);

                            if (str.Contains("CUBE +"))
                            {
                                CubeIndex = idx;
                                break;
                            }
                            else if (str.Contains("CUBE !"))
                            {
                                string errorStr = str.Replace("1CUBE !,", "");
                                throw new Exception("BXUCB ChangeCubeAsync Error:" + errorStr);
                            }
                            else
                            {
                                nowCount++;
                                if (nowCount > TimeOutRetryCount)
                                {
                                    throw new Exception("BXUCB ChangeCubeAsync Error:Retry " + TimeOutRetryCount + " Count");
                                }
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task ChangeFilter1Async(int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    ChangeFilterAsync(1, idx);
                    Filter1Index = idx;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task ChangeFilter2Async(int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    ChangeFilterAsync(1, idx);
                    Filter2Index = idx;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Task ChangeFilter3Async(int idx)
        {
            try
            {
                return Task.Run(() =>
                {
                    ChangeFilterAsync(1, idx);
                    Filter3Index = idx;
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void ChangeFilterAsync(int wheelIdx, int idx)
        {
            try
            {
                if (filterWheelIdx[wheelIdx] != idx)
                {
                    filterWheelIdx[wheelIdx] = idx;
                    int nowCount = 0;
                    string str = "";
                    while (true)
                    {
                        str = SendGetMessage("1FW" + wheelIdx + " " + idx, 3);
                        if (str.Contains("1FW" + wheelIdx + " +"))
                        {
                            break;
                        }
                        else if (str.Contains("1FW" + wheelIdx + " !"))
                        {
                            string errorStr = str.Replace("1FW !,", "");
                            throw new Exception("BXUCB ChangeFilter Error:" + errorStr);
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("BXUCB ChangeFilter Error:Retry " + TimeOutRetryCount + " Count");
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
        public Task ChangeLightAsync(int LigntValue)
        {
            try
            {
                return Task.Run(() =>
                {
                    if (lightValue != LigntValue)
                    {
                        lightValue = LigntValue;
                        string str = "";
                        int nowCount = 0;

                        //開關燈
                        while (true)
                        {
                            if (LigntValue <= 0)
                            {
                                str = SendGetMessage("1LMPSW OFF", 3);
                            }
                            else
                            {
                                str = SendGetMessage("1LMPSW ON", 3);
                            }
                            if (str.Contains("LMPSW +"))
                            {
                                break;
                            }
                            else if (str.Contains("LMPSW !"))
                            {
                                string errorStr = str.Replace("1LMPSW !,", "");
                                throw new Exception("BXUCB ChangeLight Error:" + errorStr);
                            }
                            else
                            {
                                nowCount++;
                                if (nowCount > TimeOutRetryCount)
                                {
                                    throw new Exception("BXUCB ChangeLight Error:Retry " + TimeOutRetryCount + " Count");
                                }
                            }
                        }

                        nowCount = 0;

                        //調整亮度
                        while (true)
                        {
                            str = SendGetMessage("1LMP " + LigntValue, 3);
                            if (str.Contains("LMP +"))
                            {
                                break;
                            }
                            else if (str.Contains("LMP !"))
                            {
                                string errorStr = str.Replace("1LMP !,", "");
                                throw new Exception("BXUCB ChangeLight Error:" + errorStr);
                            }
                            else
                            {
                                nowCount++;
                                if (nowCount > TimeOutRetryCount)
                                {
                                    throw new Exception("BXUCB ChangeLight Error:Retry " + TimeOutRetryCount + " Count");
                                }
                            }
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
                        string str = "";
                        int nowCount = 0;
                        while (true)
                        {
                            if (idx == 0)
                            {
                                str = SendGetMessage("1LMPSEL DIA" + idx, 3);
                            }
                            else if (idx == 1)
                            {
                                str = SendGetMessage("1LMPSEL EPI" + idx, 3);
                            }

                            if (str.Contains("LMPSEL +"))
                            {
                                break;
                            }
                            else if (str.Contains("LMPSEL !"))
                            {
                                string errorStr = str.Replace("1LMPSEL !,", "");
                                throw new Exception("BXUCB ChangeLightSpread Error:" + errorStr);
                            }
                            else
                            {
                                nowCount++;
                                if (nowCount > TimeOutRetryCount)
                                {
                                    throw new Exception("BXUCB ChangeLightSpread Error:Retry " + TimeOutRetryCount + " Count");
                                }
                            }
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
                        string str = "";
                        int nowCount = 0;

                        AFOff();
                        while (true)
                        {
                            str = SendGetMessage("1OB " + idx, 3);

                            if (str.Contains("OB +"))
                            {
                                LensIndex = idx;
                                break;
                            }
                            else if (str.Contains("OB !"))
                            {
                                string errorStr = str.Replace("1OB !,", "");
                                throw new Exception("BXUCB ChangeLens Error:" + errorStr);
                            }
                            else
                            {
                                nowCount++;
                                if (nowCount > TimeOutRetryCount)
                                {
                                    throw new Exception("BXUCB ChangeLens Error:Retry " + TimeOutRetryCount + " Count");
                                }
                            }
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
                   string str = "";
                   int nowCount = 0;
                   while (true)
                   {
                       if (distance > 0)
                       {
                           str = SendGetMessage("2MOV N," + Math.Abs(distance), 3);
                       }
                       else
                       {
                           str = SendGetMessage("2MOV F," + Math.Abs(distance), 3);
                       }

                       if (str.Contains("MOV +"))
                       {
                           break;
                       }
                       else if (str.Contains("MOV !"))
                       {
                           string errorStr = str.Replace("2MOV !,", "");
                           throw new Exception("BXUCB ZMoveCommand Error:" + errorStr);
                       }
                       else
                       {
                           nowCount++;
                           if (nowCount > TimeOutRetryCount)
                           {
                               throw new Exception("BXUCB ZMoveCommand Error:Retry " + TimeOutRetryCount + " Count");
                           }
                       }
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
                    double nowPos = 0;
                    double distance = 0;
                    string str = "";
                    int nowCount = 0;
                    while (true)
                    {
                        nowPos = this.Position;
                        distance = position - nowPos;
                        if (distance > 0)
                        {
                            str = SendGetMessage("2MOV N," + Math.Abs(distance), 3);
                        }
                        else
                        {
                            str = SendGetMessage("2MOV F," + Math.Abs(distance), 3);
                        }

                        if (str.Contains("MOV +"))
                        {
                            break;
                        }
                        else if (str.Contains("MOV !"))
                        {
                            string errorStr = str.Replace("2MOV !,", "");
                            throw new Exception("BXUCB ZMoveToCommand Error:" + errorStr);
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("BXUCB ZMoveToCommand Error:Retry " + TimeOutRetryCount + " Count");
                            }
                        }
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
                string str = "";
                int nowCount = 0;
                while (true)
                {
                    if (isLogIn)
                    {
                        str = SendGetMessage("1LOG IN", 3);
                    }
                    else
                    {
                        str = SendGetMessage("1LOG OUT", 3);
                    }

                    if (str.Contains("1LOG +"))
                    {
                        break;
                    }
                    else if (str.Contains("1LOG !"))
                    {
                        string errorStr = str.Replace("1LOG !,", "");
                        throw new Exception("BXUCB LogInOutBXFM Error:" + errorStr);
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB LogInOutBXFM Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                string str = "";
                int nowCount = 0;
                while (true)
                {
                    if (isLogIn)
                    {
                        str = SendGetMessage("2LOG IN", 3);
                    }
                    else
                    {
                        str = SendGetMessage("2LOG OUT", 3);
                    }

                    if (str.Contains("2LOG +"))
                    {
                        break;
                    }
                    else if (str.Contains("2LOG !"))
                    {
                        string errorStr = str.Replace("2LOG !,", "");
                        throw new Exception("BXUCB LogInOutA2M Error:" + errorStr);
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB LogInOutA2M Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                string str = "";
                int nowCount = 0;
                double nowPos = 0;
                while (true)
                {
                    str = SendGetMessage("2FARLMT?", 3);

                    if (str.Contains("2FARLMT !"))
                    {
                        string errorStr = str.Replace("2FARLMT !,", "");
                        throw new Exception("BXUCB GetZNEL Error:" + errorStr);
                    }
                    else if (str.Contains("2FARLMT"))
                    {
                        nowPos = Convert.ToDouble(str.Replace("2FARLMT ", ""));
                        break;
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB GetZNEL Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                string str = "";
                int nowCount = 0;
                while (true)
                {
                    str = SendGetMessage("2FARLMT " + position, 3);

                    if (str.Contains("2FARLMT +"))
                    {
                        break;
                    }
                    else if (str.Contains("2FARLMT !"))
                    {
                        string errorStr = str.Replace("2FARLMT !,", "");
                        throw new Exception("BXUCB SetZNEL Error:" + errorStr);
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB SetZNEL Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                string str = "";
                int nowCount = 0;
                double nowPos = 0;
                while (true)
                {
                    str = SendGetMessage("2NEARLMT?", 3);

                    if (str.Contains("2FARLMT !"))
                    {
                        string errorStr = str.Replace("2NEARLMT !,", "");
                        throw new Exception("BXUCB GetZPEL Error:" + errorStr);
                    }
                    else if (str.Contains("2NEARLMT"))
                    {
                        nowPos = Convert.ToDouble(str.Replace("2NEARLMT ", ""));
                        break;
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB GetZPEL Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                string str = "";
                int nowCount = 0;
                while (true)
                {
                    str = SendGetMessage("2NEARLMT " + position, 3);

                    if (str.Contains("2NEARLMT +"))
                    {
                        break;
                    }
                    else if (str.Contains("2NEARLMT !"))
                    {
                        string errorStr = str.Replace("2NEARLMT !,", "");
                        throw new Exception("BXUCB SetZPEL Error:" + errorStr);
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB SetZPEL Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                string str = "";
                int nowCount = 0;
                int nowPos = 0;
                while (true)
                {
                    str = SendGetMessage("2AFFLMT?", 3);

                    if (str.Contains("2AFFLMT !"))
                    {
                        string errorStr = str.Replace("2AFFLMT !,", "");
                        throw new Exception("BXUCB GetAFNEL Error:" + errorStr);
                    }
                    else if (str.Contains("2AFFLMT"))
                    {
                        nowPos = Convert.ToInt32(str.Replace("2AFFLMT ", ""));
                        break;
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB GetAFNEL Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
                }
                return nowPos;
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
                string str = "";
                int nowCount = 0;
                while (true)
                {
                    str = SendGetMessage("2AFFLMT " + position, 3);

                    if (str.Contains("2AFFLMT +"))
                    {
                        break;
                    }
                    else if (str.Contains("2AFFLMT !"))
                    {
                        string errorStr = str.Replace("2AFFLMT !,", "");
                        throw new Exception("BXUCB SetAFNEL Error:" + errorStr);
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB SetAFNEL Error:Retry " + TimeOutRetryCount + " Count");
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
        /// 取得AFPEL值
        /// </summary>
        /// <returns></returns>
        public int GetAFPEL()
        {
            try
            {
                string str = "";
                int nowCount = 0;
                int nowPos = 0;
                while (true)
                {
                    str = SendGetMessage("2AFNLMT?", 3);

                    if (str.Contains("2AFFLMT !"))
                    {
                        string errorStr = str.Replace("2AFFLMT !,", "");
                        throw new Exception("BXUCB GetAFNEL Error:" + errorStr);
                    }
                    else if (str.Contains("2AFNLMT "))
                    {
                        nowPos = Convert.ToInt32(str.Replace("2AFNLMT ", ""));
                        break;
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB GetAFNEL Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
                }
                return nowPos;
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
                string str = "";
                int nowCount = 0;
                while (true)
                {
                    str = SendGetMessage("2AFNLMT " + position, 3);

                    if (str.Contains("2AFNLMT +"))
                    {
                        break;
                    }
                    else if (str.Contains("2AFNLMT !"))
                    {
                        string errorStr = str.Replace("2AFNLMT !,", "");
                        throw new Exception("BXUCB SetAFPEL Error:" + errorStr);
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB SetAFPEL Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
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
                string str = "";
                int nowCount = 0;
                int nowPos = 0;
                while (true)
                {
                    str = SendGetMessage("2POS?", 3);
                    if (str.Contains("2POS !"))
                    {
                        string errorStr = str.Replace("2POS !,", "");
                        throw new Exception("BXUCB GetZPosition Error:" + errorStr);
                    }
                    else if (str.Contains("2POS "))
                    {
                        nowPos = Convert.ToInt32(str.Replace("2POS ", ""));
                        break;
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB GetZPosition Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
                }
                return nowPos;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int GetAberationPosition()
        {
            try
            {
                string str = "";
                int nowCount = 0;
                int nowPos = 0;
                while (true)
                {
                    str = SendGetMessage("2APOS? ", 3);
                    if (str.Contains("2APOS !"))
                    {
                        string errorStr = str.Replace("2APOS !,", "");
                        throw new Exception("BXUCB GetAberationPosition Error:" + errorStr);
                    }
                    else if (str.Contains("2APOS "))
                    {
                        nowPos = Convert.ToInt32(str.Replace("2APOS ", ""));
                        break;
                    }
                    else
                    {
                        nowCount++;
                        if (nowCount > TimeOutRetryCount)
                        {
                            throw new Exception("BXUCB GetAberationPosition Error:Retry " + TimeOutRetryCount + " Count");
                        }
                    }
                }
                return nowPos;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        public List<string> SendGetMessage_Old(string message, string checkString)
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
        /// <summary>
        /// 一送一收RS232，message要送出的RS232文字，timeOutCount有些動作比較久，TimeOut時間內等不到，要再等多少次數
        /// </summary>
        /// <param name="message"></param>
        /// <param name="timeOutCount"></param>
        /// <returns></returns>
        private string SendGetMessage(string message, int timeOutCount)
        {
            try
            {
                lock (lockObj)
                {
                    List<string> returnMessage1 = new List<string>();
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    int retryCount = 0;
                    string data = "";
                    serialPort.WriteLine(message);
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
        private List<string> SendGetMessage_New(string message, string checkString)
        {
            try
            {


                lock (lockObj)
                {
                    List<string> returnMessage1 = new List<string>();
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    int maxRetryMilliseconds = 5 * 1000;
                    string data = "";
                    SpinWait.SpinUntil(() =>
                    {
                        try
                        {
                            serialPort.WriteLine(message);




                            data = tcsReceived.Task.Result;//會停在這一行


                            tcsReceived.Task.Wait();
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
                        //errcode = data;
                    }
                    returnMessage1.Add("Error");
                    return returnMessage1;

                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        private List<string> SendGetWhileMessage(string message, string checkString)
        {
            try
            {


                lock (lockObj)
                {
                    List<string> returnMessage1 = new List<string>();
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    serialPort.WriteLine(message);

                    int maxRetryMilliseconds = 5 * 1000;
                    string data = "";
                    //SpinWait.SpinUntil(() =>
                    //{
                    //    try
                    //    {
                    //        data = tcsReceived.Task.Result;//會停在這一行
                    //    }
                    //    catch
                    //    {
                    //    }
                    //    return (data != "");
                    //}, maxRetryMilliseconds);

                    tcsReceived.Task.Wait();
                    data = tcsReceived.Task.Result;//會停在這一行



                    tcsReceived.Task.Wait();
                    data = tcsReceived.Task.Result;//會停在這一行




                    if (data.Contains("+"))
                    {
                        returnMessage1.Add("OK");
                        return returnMessage1;
                    }

                    if (data.Contains("!"))
                    {
                        //errcode = data;
                    }
                    returnMessage1.Add("Error");
                    return returnMessage1;

                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadExisting();
                tcsReceived.SetResult(data);
                tcsReceived = new TaskCompletionSource<string>();
            }
            catch (Exception ex)
            {

                Error.Invoke(ex);
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


    public class ErrorcodeException : Exception
    {

        public ErrorcodeException(string code, string des)
        {


        }

    }

}
