using Nito.AsyncEx;
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
    public class HirataRobot_RS232 : IEFEMRobot
    {
        private SerialPort serialPort = new SerialPort();
        private readonly object lockObj = new object();

        private int[] loadPort1_Pick_Command = new int[] { 880, 881, 882, 883 };
        private int[] loadPort1_Put_Command = new int[] { 680, 681, 682, 683 };

        private int[] loadPort2_Pick_Command = new int[] { 980, 981, 982, 983 };
        private int[] loadPort2_Put_Command = new int[] { 780, 781, 782, 783 };

        private int[] aligner_Pick_Command = new int[] { 810, 811, 812, 813 };
        private int[] aligner_Put_Command = new int[] { 610, 611, 612, 613 };

        private int[] microscope_Pick_Command = new int[] { 1810, 1811, 1812, 1813 };
        private int[] microscope_Put_Command = new int[] { 1610, 1611, 1612, 1613 };

        private int[] macro_Pick_Command = new int[] { 710, 711, 712, 713 };
        private int[] macro_Put_Command = new int[] { 510, 511, 512, 513 };

        private const string RB_NO = "001";
        private const char STX = '\x2';
        private const char ETX = '\x3';
        private short RX_LRC = 3;
        private bool STX_flag = false, ETX_flag = false;

        private List<char> RxData;
        public HirataRobot_RS232(string comPort, int speedPercent, double tolerance)
        {
            try
            {
                serialPort.PortName = comPort;
                serialPort.BaudRate = 19200;
                serialPort.DataBits = 7; //只有7,8
                serialPort.Parity = Parity.Even;
                serialPort.StopBits = StopBits.One;
                serialPort.RtsEnable = false;

                this.MoveTolerance = tolerance;
                this.SpeedPercent = speedPercent;
                //serialPort.Open();
                //IsOpen = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public PauseTokenSource pauseToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CancellationTokenSource cancelToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsOpen { get; private set; }
        public double MoveTolerance { get; private set; } = 0;
        public double CassetteWaferPitch { get; set; } = 10;
        public int SpeedPercent { get; private set; } = 10;
        public bool IsLockOK => GetInput(0);
        public int TimeOutRetryCount { get; set; } = 1;
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

        /// <summary>
        /// 取得Robot目前狀態
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public Task<RobotStatus> GetStatus()
        {
            try
            {
                return Task.Run(() =>
               {
                   RobotStatus robotStatus = new RobotStatus();
                   List<string> str = new List<string>();
                   str = SendGetMessage("LS", CheckMessageType.Status);
                   foreach (var item in str)
                   {
                       robotStatus = TransStatus(item);
                   }
                   return robotStatus;
               });
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 取得Robot的AddressPos address:點位
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public RobotPoint GetAddressPos(int address)
        {
            RobotPoint robot_Point = new RobotPoint();
            try
            {
                List<string> str = new List<string>();
                str = SendGetMessage("LD " + address, CheckMessageType.Position);
                foreach (var item in str)
                {
                    robot_Point = TransPoint(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return robot_Point;
        }
        /// <summary>
        /// 移動到設定的Robot點位 address:點位，zShift:Z軸位移量
        /// </summary>
        /// <param name="address"></param>
        /// <param name="zShift"></param>
        /// <returns></returns>
        public Task MovAddress(int address, double zShift)
        {
            try
            {
                return Task.Run(async () =>
                {
                    RobotStatus robotStatus = new RobotStatus();
                    int i = 0;
                    double tolerance = MoveTolerance;
                    List<string> str = new List<string>();
                    str = SendGetMessage(" GP " + address + " ( 0 0 " + zShift + " 0)", CheckMessageType.Status);
                    foreach (var item in str)
                    {
                        robotStatus = TransStatus(item);
                    }
                    RobotPoint robotAddressPoint = new RobotPoint();
                    RobotPoint robotNowPoint = new RobotPoint();
                    robotAddressPoint = GetAddressPos(address);
                    while (true)
                    {
                        i++;
                        await Task.Delay(50);
                        robotNowPoint = GetNowPos();
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
                });
            }
            catch (Exception ex)
            {
                throw new Exception("MovAddress:" + ex);
            }

        }
        public Task PickWafer_Safety(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                task = MovAddress(1, 0);
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWaferFromCassette_Safety:" + ex);
            }
        }

        public Task PickWafer_Standby(ArmStation armStation, int layer)
        {
            try
            {
                if (layer <= 1) { layer = 1; }
                double slot_diff_Z = (double)(layer - 1) * CassetteWaferPitch;
                Task task = Task.CompletedTask;
                if (armStation == ArmStation.Cassette1)
                {
                    task = MovAddress(loadPort1_Pick_Command[0], slot_diff_Z);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    task = MovAddress(loadPort2_Pick_Command[0], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_Standby:" + ex);
            }
        }

        public Task PickWafer_GoIn(ArmStation armStation, int layer)
        {
            try
            {
                Task task = Task.CompletedTask;
                if (layer <= 1) { layer = 1; }
                double slot_diff_Z = (double)(layer - 1) * CassetteWaferPitch;
                if (armStation == ArmStation.Cassette1)
                {
                    task = MovAddress(loadPort1_Pick_Command[1], slot_diff_Z);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    task = MovAddress(loadPort2_Pick_Command[1], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_GoIn:" + ex);
            }
        }

        public Task PickWafer_LiftUp(ArmStation armStation, int layer)
        {
            try
            {
                Task task = Task.CompletedTask;
                if (layer <= 1) { layer = 1; }
                double slot_diff_Z = (double)(layer - 1) * CassetteWaferPitch;
                if (armStation == ArmStation.Cassette1)
                {
                    task = MovAddress(loadPort1_Pick_Command[2], slot_diff_Z);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    task = MovAddress(loadPort2_Pick_Command[2], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_LiftUp:" + ex);
            }
        }

        public Task PickWafer_Retract(ArmStation armStation, int layer)
        {
            try
            {
                Task task = Task.CompletedTask;
                if (layer <= 1) { layer = 1; }
                double slot_diff_Z = (double)(layer - 1) * CassetteWaferPitch;
                if (armStation == ArmStation.Cassette1)
                {
                    task = MovAddress(loadPort1_Pick_Command[3], slot_diff_Z);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    task = MovAddress(loadPort2_Pick_Command[3], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_Retract:" + ex);
            }
        }

        public Task PickWafer_Standby(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                double slot_diff_Z = 0;
                if (armStation == ArmStation.Align)
                {
                    task = MovAddress(aligner_Pick_Command[0], slot_diff_Z);
                }
                else if (armStation == ArmStation.Macro)
                {
                    task = MovAddress(macro_Pick_Command[0], slot_diff_Z);
                }
                else if (armStation == ArmStation.Micro)
                {
                    task = MovAddress(microscope_Pick_Command[0], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_Retract:" + ex);
            }
        }

        public Task PickWafer_GoIn(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                double slot_diff_Z = 0;
                if (armStation == ArmStation.Align)
                {
                    task = MovAddress(aligner_Pick_Command[1], slot_diff_Z);
                }
                else if (armStation == ArmStation.Macro)
                {
                    task = MovAddress(macro_Pick_Command[1], slot_diff_Z);
                }
                else if (armStation == ArmStation.Micro)
                {
                    task = MovAddress(microscope_Pick_Command[1], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_GoIn:" + ex);
            }
        }

        public Task PickWafer_LiftUp(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                double slot_diff_Z = 0;
                if (armStation == ArmStation.Align)
                {
                    task = MovAddress(aligner_Pick_Command[2], slot_diff_Z);
                }
                else if (armStation == ArmStation.Macro)
                {
                    task = MovAddress(macro_Pick_Command[2], slot_diff_Z);
                }
                else if (armStation == ArmStation.Micro)
                {
                    task = MovAddress(microscope_Pick_Command[2], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_LiftUp:" + ex);
            }
        }

        public Task PickWafer_Retract(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                double slot_diff_Z = 0;
                if (armStation == ArmStation.Align)
                {
                    task = MovAddress(aligner_Pick_Command[3], slot_diff_Z);
                }
                else if (armStation == ArmStation.Macro)
                {
                    task = MovAddress(macro_Pick_Command[3], slot_diff_Z);
                }
                else if (armStation == ArmStation.Micro)
                {
                    task = MovAddress(microscope_Pick_Command[3], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_Retract:" + ex);
            }
        }

        public Task PutWafer_Safety(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;

                task = MovAddress(1, 0);

                task = MovAddress(0, 0);
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PutWafer_Safety:" + ex);
            }
        }

        public Task PutWafer_Standby(ArmStation armStation, int layer)
        {
            try
            {
                Task task = Task.CompletedTask;
                if (layer <= 1) { layer = 1; }
                double slot_diff_Z = (double)(layer - 1) * CassetteWaferPitch;
                if (armStation == ArmStation.Cassette1)
                {
                    task = MovAddress(loadPort1_Put_Command[0], slot_diff_Z);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    task = MovAddress(loadPort2_Put_Command[0], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_GoIn:" + ex);
            }
        }

        public Task PutWafer_GoIn(ArmStation armStation, int layer)
        {
            try
            {
                Task task = Task.CompletedTask;
                if (layer <= 1) { layer = 1; }
                double slot_diff_Z = (double)(layer - 1) * CassetteWaferPitch;
                if (armStation == ArmStation.Cassette1)
                {
                    task = MovAddress(loadPort1_Put_Command[1], slot_diff_Z);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    task = MovAddress(loadPort2_Put_Command[1], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_GoIn:" + ex);
            }
        }

        public Task PutWafer_PutDown(ArmStation armStation, int layer)
        {
            try
            {
                Task task = Task.CompletedTask;
                if (layer <= 1) { layer = 1; }
                double slot_diff_Z = (double)(layer - 1) * CassetteWaferPitch;
                if (armStation == ArmStation.Cassette1)
                {
                    task = MovAddress(loadPort1_Put_Command[2], slot_diff_Z);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    task = MovAddress(loadPort2_Put_Command[2], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_GoIn:" + ex);
            }
        }

        public Task PutWafer_Retract(ArmStation armStation, int layer)
        {
            try
            {
                Task task = Task.CompletedTask;
                if (layer <= 1) { layer = 1; }
                double slot_diff_Z = (double)(layer - 1) * CassetteWaferPitch;
                if (armStation == ArmStation.Cassette1)
                {
                    task = MovAddress(loadPort1_Put_Command[3], slot_diff_Z);
                }
                else if (armStation == ArmStation.Cassette2)
                {
                    task = MovAddress(loadPort2_Put_Command[3], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWafer_GoIn:" + ex);
            }
        }

        public Task PutWafer_Standby(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                double slot_diff_Z = 0;
                if (armStation == ArmStation.Align)
                {
                    task = MovAddress(aligner_Put_Command[0], slot_diff_Z);
                }
                else if (armStation == ArmStation.Macro)
                {
                    task = MovAddress(macro_Put_Command[0], slot_diff_Z);
                }
                else if (armStation == ArmStation.Micro)
                {
                    task = MovAddress(microscope_Put_Command[0], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PutWafer_Standby:" + ex);
            }
        }

        public Task PutWafer_GoIn(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                double slot_diff_Z = 0;
                if (armStation == ArmStation.Align)
                {
                    task = MovAddress(aligner_Put_Command[1], slot_diff_Z);
                }
                else if (armStation == ArmStation.Macro)
                {
                    task = MovAddress(macro_Put_Command[1], slot_diff_Z);
                }
                else if (armStation == ArmStation.Micro)
                {
                    task = MovAddress(microscope_Put_Command[1], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PutWafer_GoIn:" + ex);
            }
        }

        public Task PutWafer_PutDown(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                double slot_diff_Z = 0;
                if (armStation == ArmStation.Align)
                {
                    task = MovAddress(aligner_Put_Command[2], slot_diff_Z);
                }
                else if (armStation == ArmStation.Macro)
                {
                    task = MovAddress(macro_Put_Command[2], slot_diff_Z);
                }
                else if (armStation == ArmStation.Micro)
                {
                    task = MovAddress(microscope_Put_Command[2], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PutWafer_PutDown:" + ex);
            }
        }

        public Task PutWafer_Retract(ArmStation armStation)
        {
            try
            {
                Task task = Task.CompletedTask;
                double slot_diff_Z = 0;
                if (armStation == ArmStation.Align)
                {
                    task = MovAddress(aligner_Put_Command[3], slot_diff_Z);
                }
                else if (armStation == ArmStation.Macro)
                {
                    task = MovAddress(macro_Put_Command[3], slot_diff_Z);
                }
                else if (armStation == ArmStation.Micro)
                {
                    task = MovAddress(microscope_Put_Command[3], slot_diff_Z);
                }
                return task;
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PutWafer_Retract:" + ex);
            }
        }
        public Task Home()
        {
            try
            {
                return Task.Run(async () =>
                {
                    int motionPercent = this.SpeedPercent;
                    if (motionPercent <= 0) { motionPercent = 0; }
                    if (motionPercent >= 100) { motionPercent = 100; }
                    this.SpeedPercent = motionPercent;
                    SetMovSpeed(motionPercent, motionPercent);

                    await MovAddress(1, 0);

                    await MovAddress(0, 0);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Robot PickWaferFromCassette_Safety:" + ex);
            }
        }

        public Task Stop()
        {
            try
            {
                return Task.Run(() =>
               {
                   RobotStatus robotStatus = new RobotStatus();
                   List<string> str = new List<string>();
                   str = SendGetMessage(" GD ", CheckMessageType.Status);
                   foreach (var item in str)
                   {
                       robotStatus = TransStatus(item);
                   }
               });
            }
            catch (Exception ex)
            {
                throw new Exception("Robot Stop:" + ex);
            }
        }
        public Task Continue()
        {
            try
            {
                return Task.Run(() =>
               {
                   RobotStatus robotStatus = new RobotStatus();
                   List<string> str = new List<string>();
                   str = SendGetMessage(" GE ", CheckMessageType.Status);
                   foreach (var item in str)
                   {
                       robotStatus = TransStatus(item);
                   }
               });

            }
            catch (Exception ex)
            {
                throw new Exception("Robot Continue:" + ex);
            }
        }
        public Task<RobotPoint> GetPositionCommand()
        {
            try
            {
                return Task.Run(() =>
               {
                   RobotPoint robot_Point = new RobotPoint();
                   robot_Point = GetNowPos();
                   return robot_Point;
               });
            }
            catch (Exception ex)
            {

                throw new Exception("Robot GetPositionCommand:" + ex);
            }
        }
        public Task SetSpeedPercentCommand(int motionPercent)
        {
            try
            {
                return Task.Run(async () =>
                {
                    if (motionPercent <= 0) { motionPercent = 0; }
                    if (motionPercent >= 100) { motionPercent = 100; }
                    this.SpeedPercent = motionPercent;
                    SetMovSpeed(motionPercent, motionPercent);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Robot LockWafer:" + ex);
            }
        }

        public Task FixWafer()
        {
            try
            {
                return Task.Run(() =>
                {
                    RobotPoint robot_Point = new RobotPoint();
                    Vacuum(true);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Robot LockWafer:" + ex);
            }
        }
        public Task ReleaseWafer()
        {
            try
            {
                return Task.Run(async () =>
                {
                    RobotPoint robot_Point = new RobotPoint();
                    Vacuum(false);
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Robot LockWafer:" + ex);
            }
        }

        //private bool IsLockOK()
        //{
        //    try
        //    {
        //        bool isLockOK = false;
        //        ///確認一下 不然就只是GetOutput(0) 有開啟而已
        //        isLockOK = GetInput(0);
        //        return isLockOK;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Robot LockWafer:" + ex);
        //    }
        //}

        public Task<bool> IsHavePiece()
        {
            try
            {
                return Task.Run(() =>
               {
                   bool isHaveWafer = false;
                   isHaveWafer = GetInput(0);
                   return isHaveWafer;
               });
            }
            catch (Exception ex)
            {
                throw new Exception("Robot LockWafer:" + ex);
            }
        }
        /// <summary>
        /// 開啟連線
        /// </summary>
        private void Open()
        {
            try
            {
                serialPort.Open();
                IsOpen = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 關閉連線
        /// </summary>
        private void Close()
        {
            try
            {
                serialPort.Close();
                IsOpen = false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Robot真空 isOn = true 開啟 isOn = false 關閉
        /// </summary>
        /// <param name="isOn"></param>
        /// <returns></returns>
        private void Vacuum(bool isOn)
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
        }
        /// <summary>
        /// 設定Robot的移動速度 xywSpeed:XYW的速度設定 zSpeed:Z軸的速度
        /// </summary>
        /// <param name="xywSpeed"></param>
        /// <param name="zSpeed"></param>
        /// <returns></returns>
        private void SetMovSpeed(int xywSpeed, int zSpeed)
        {
            try
            {
                RobotStatus robotStatus = new RobotStatus();
                List<string> str = new List<string>();
                str = SendGetMessage("SP " + xywSpeed + " " + zSpeed, CheckMessageType.Status);
                foreach (var item in str)
                {
                    robotStatus = TransStatus(item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("SetMovSpeed:" + ex);
            }
        }
        /// <summary>
        /// 取得RobotInput狀態(目前只有點位0 是否有片子)
        /// </summary>
        /// <returns></returns>
        private bool GetInput(int id)
        {
            bool isOn = false;
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
            return isOn;
        }
        /// <summary>
        /// 取得RobotOutput狀態(目前只有點位0 真空開啟的Outupt有沒有做動)
        /// </summary>
        /// <returns></returns>
        private bool GetOutput(int id)
        {
            bool isOn = false;
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
            return isOn;
        }
        /// <summary>
        /// 取得Robot目前各軸位置
        /// </summary>
        /// <returns></returns>
        private RobotPoint GetNowPos()
        {
            try
            {
                RobotPoint robot_Point = new RobotPoint();
                List<string> str = new List<string>();
                str = SendGetMessage("LR", CheckMessageType.Position);
                foreach (var item in str)
                {
                    robot_Point = TransPoint(item);
                }
                return robot_Point;
            }
            catch (Exception ex)
            {
                throw new Exception("GetNowPos:" + ex);
            }
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

                if (paddedBinaryArray[(paddedBinaryArray.Length - 1) - id] == '0')
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
                    int nowCount = 0;
                    while (true)
                    {
                        try
                        {
                            int delayTime = 50;
                            int timeOut1 = 60 * 1000;
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
                            while (true)
                            {

                                Thread.Sleep(delayTime);
                                readMessage1 = GetMessage();
                                if (stopwatch.ElapsedMilliseconds > timeOut1)
                                {
                                    throw new Exception("Robot SendGetMessage Time Out");
                                }
                                if (checkType == CheckMessageType.Position && readMessage1.Count > 0)//要取得位置資訊的
                                {
                                    foreach (var item in readMessage1)
                                    {
                                        string[] ss = item.Split(' ');
                                        if (ss.Length >= 5)
                                        {
                                            returnMessage1.Clear();
                                            returnMessage1.Add(item);
                                            //foreach (var item2 in ss)
                                            //{
                                            //    returnMessage1.Add(item2);
                                            //}
                                            return returnMessage1;
                                        }
                                    }
                                }
                                else if (checkType == CheckMessageType.IO && readMessage1.Count > 0)//要取得IO資訊
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
                                else if (readMessage1.Count > 0)//取得狀態
                                {
                                    foreach (var item in readMessage1)
                                    {
                                        string[] ss = item.Split(' ');
                                        if (ss.Length == 1 && ss[0].Length >= 4)
                                        {
                                            returnMessage1.Clear();
                                            returnMessage1.Add(ss[0]);
                                            if (message.Contains("GP") && ss[0] == "4401")
                                            {
                                                return returnMessage1;
                                            }
                                            else
                                            {
                                                return returnMessage1;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw ex;
                            }
                        }
                    }
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
        private char ReturnLRC(string RequestMessage) //輸入STX開始到ETX結束前的資料
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



        private List<string> GetMessage()
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
