using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Component.Adlink
{
    public class Adlink7856 : IMotionController
    {
        /// <summary>
        /// 軸卡參數
        /// </summary>
        private Axis[] axes;
        /// <summary>
        /// APS168Liberay回授OK數值
        /// </summary>
        private const int eRR_NoError = 0;
        /// <summary>
        /// 軸卡號
        /// </summary>
        private int cardID = 0;
        /// <summary>
        /// First Axis number in Motion Net bus.
        /// </summary>
        private int start_Axis_ID = 0;
        /// <summary>
        /// IO的Bus
        /// </summary>
        private int Bus_HSL = 0;
        /// <summary>
        /// 軸卡的Bus
        /// </summary>
        private int Bus_MNET = 1; //軸卡的Bus
        /// <summary>
        /// 各自軸卡張數
        /// </summary>
        private int[] totalAxis;
        /// <summary>
        /// 各自軸卡起始軸編號
        /// </summary>
        private int[] startAxis;
        /// <summary>
        /// 各自軸實際動作編號
        /// </summary>
        private int[] axisRealID;

        /// <summary>
        /// 各自軸卡的Input點位
        /// </summary>
        private int[] totalInput;
        /// <summary>
        /// Input點位對應的ID
        /// </summary>
        private int[] inputRealModID;
        /// <summary>
        /// 各自軸卡的Output點位
        /// </summary>
        private int[] totalOutput;
        /// <summary>
        /// Output點位對應的ID
        /// </summary>
        private int[] outputRealModID;

        private List<VelocityParams> axesMovVel = new List<VelocityParams>();

        private List<double> axeslimitN = new List<double>();

        private List<double> axeslimitP = new List<double>();

        public Adlink7856(IEnumerable<AxisConfig> axisInfos, IEnumerable<string> doNames, IEnumerable<string> diNames)
        {
            try
            {
                List<double> axesPos = new List<double>();
                axesMovVel = new List<VelocityParams>();
                axeslimitN = new List<double>();
                axeslimitP = new List<double>();
                totalAxis = new int[2];
                startAxis = new int[2];
                axisRealID = new int[5];
                totalAxis[0] = 4;
                totalAxis[1] = 4;
                startAxis[0] = 1500;
                startAxis[1] = 1504;
                axisRealID[0] = 1500;
                axisRealID[1] = 1501;
                axisRealID[2] = 1502;
                axisRealID[3] = 1503;
                axisRealID[4] = 1504;

                totalInput = new int[2];
                totalInput[0] = 16;
                totalInput[1] = 16;
                inputRealModID = new int[2];
                inputRealModID[0] = 1;
                inputRealModID[1] = 3;

                totalOutput = new int[3];
                totalOutput[0] = 16;
                totalOutput[1] = 16;
                totalOutput[2] = 32;
                outputRealModID = new int[3];
                outputRealModID[0] = 1;
                outputRealModID[1] = 3;
                outputRealModID[2] = 5;

                axes = axisInfos.Select((info, i) =>
                {
                    axesPos.Add(0);
                    return new Axis(this, info.AxisID)
                    {
                        AxisName = info.AxisName
                    };
                }).ToArray();

                var axisInfosArray = axisInfos.ToArray();
                //有多少軸就創建多少顆驅動器參數
                for (int i = 0; i < axes.Length; i++)
                {
                    axesMovVel.Add(axisInfosArray[i].MoveVel);
                    axeslimitN.Add(axisInfosArray[i].LimitNEL);
                    axeslimitP.Add(axisInfosArray[i].LimitPEL);

                    axes[i].HomeVelocity = axisInfosArray[i].HomeVel;
                    axes[i].HomeMode = axisInfosArray[i].HomeMode;
                    axes[i].HomeDirection = axisInfosArray[i].HomeDirection;
                }

                OutputSignals = doNames.Select((n, i) => new DigitalOutput(i, this));
                InputSignals = diNames.Select(n => new DigitalInput(n)).ToArray();
            }
            catch (Exception)
            {

                throw;
            }

        }
        public bool IsOpen { get; private set; }

        public Axis[] Axes => axes;

        public DigitalInput[] InputSignals { get; private set; }

        public IEnumerable<DigitalOutput> OutputSignals { get; set; }
        public void InitializeCommand()
        {
            try
            {
                IsOpen = InitialCard();
                foreach (var item in OutputSignals)
                {
                    item.IsSwitchOn = false;
                }
                if (IsOpen == true)
                {
                    for (int i = 0; i < Axes.Length; i++)
                    {
                        if (ServoOn(i) == false)
                        {
                            throw new Exception("ServoOn Axis Error!");
                        }
                        else
                        {
                            Axes[i].IsOpen = true;
                        }
                    }
                    Task.Run(ReflashInput);
                }
                else
                {
                    throw new Exception("Initial Card Error!");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DigitalOutput[] SetOutputNames(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }
        public void SetOutputCommand(int id, bool isOn)
        {
            try
            {
                int setCardNo = -1;
                int cardNo = 0;
                int lowCount = 0;
                for (int i = 0; i < totalOutput.Length; i++)
                {
                    if (i == 0)
                    {
                        if (0 <= id && id < totalOutput[0])
                        {
                            setCardNo = cardNo;
                            break;
                        }
                        lowCount = totalOutput[0];
                    }
                    else
                    {
                        if (lowCount <= id && id < lowCount + totalOutput[i])
                        {
                            setCardNo = cardNo;
                            break;
                        }
                        lowCount += totalOutput[i];
                    }
                    cardNo++;
                }
                if (setCardNo >= 0)
                {
                    //int realID = OutputSignals[id]
                    APS168SetOutput(setCardNo, id, isOn);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public DigitalInput[] SetInputNames(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }


        public Axis[] SetAxesParam(IEnumerable<AxisConfig> axisConfig)
        {
            throw new NotImplementedException();
        }
        public void SetServoCommand(int id, bool isOn)
        {
            try
            {
                if (isOn == true)
                {
                    Axes[id].IsOpen = true;
                    ServoOn(id);
                }
                else
                {
                    Axes[id].IsOpen = false;
                    ServoOff(id);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void MoveCommand(int id, double distance)
        {
            try
            {
                ChkAxisStop(id, 180);
                //int pos = (int)(Axes[id].Position + distance * Axes[id].Ratio);
                int axisID = axisRealID[Axes[id].AxisID];
                int ret = APS168Lib.APS_relative_move(axisID, (int)(distance * Axes[id].Ratio), (int)(Axes[id].AxisVelocity.MaxVel * Axes[id].Ratio));
                if (ret == 0)
                {
                    ChkAxisStop(id, 180);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void MoveToCommand(int id, double position)
        {
            try
            {
                ChkAxisStop(id, 180);
                int axisID = axisRealID[Axes[id].AxisID];
                int ret = APS168Lib.APS_absolute_move(axisID, (int)(position * Axes[id].Ratio), (int)(Axes[id].AxisVelocity.MaxVel * Axes[id].Ratio));
                if (ret == 0)
                {
                    ChkAxisStop(id, 180);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void StopCommand(int id)
        {
            try
            {
                int ret;
                double enr = 0;
                double decTime = Axes[id].AxisVelocity.DecelerationTime;
                double maxVelocity = Axes[id].AxisVelocity.MaxVel * Axes[id].Ratio;
                int dec = (int)(maxVelocity / decTime);
                int axisID = axisRealID[Axes[id].AxisID];
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_STP_DEC, dec);
                ret = APS168Lib.APS_stop_move(axisID);
                if (ret == eRR_NoError)
                {

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public AxisDirection GetAxisDirectionCommand(int id)
        {
            throw new NotImplementedException();
        }
        public void GetLimitCommand(int id, out double limitN, out double limitP)
        {
            try
            {
                limitP = axeslimitP[id];
                limitN = axeslimitN[id];
            }
            catch (Exception)
            {

                throw;
            }
        }
        public double GetPositionCommand(int id)
        {
            try
            {
                int ret;
                double enr = 0;
                int axisID = axisRealID[Axes[id].AxisID];
                ret = APS168Lib.APS_get_position_f(axisID, ref enr);
                if (ret == eRR_NoError)
                {

                }
                return enr / Axes[id].Ratio;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public AxisSensor GetSensorCommand(int id)
        {
            try
            {
                AxisSensor axisSensor = new AxisSensor();
                AxisInfo axisInfo = new AxisInfo();
                int axisID = axisRealID[Axes[id].AxisID];
                axisInfo = ChkAxisStatus(id);
                if (axisInfo.IO.OriginSwitch == enumMotionFlag.eHigh)
                {
                    axisSensor = AxisSensor.ORG;
                }
                else if (axisInfo.IO.PositiveLimitSwitch == enumMotionFlag.eHigh)
                {
                    axisSensor = AxisSensor.PEL;
                }
                else if (axisInfo.IO.NegativeLimitSwitch == enumMotionFlag.eHigh)
                {
                    axisSensor = AxisSensor.NEL;
                }
                else
                {
                    axisSensor = AxisSensor.NONE;
                }
                return axisSensor;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public VelocityParams GetSpeedCommand(int id)
        {
            return axesMovVel[id];
        }

        public void HomeCommand(int id)
        {
            try
            {
                int ret = 0;
                int homeMode = 0;
                int homeDir = 0;
                int homeEz = 0;
                bool homeOk = false;
                int axisID = axisRealID[Axes[id].AxisID];
                if (Axes[id].HomeMode == HomeModes.ORG)
                {
                    homeMode = 9;
                }
                else if (Axes[id].HomeMode == HomeModes.EL)
                {
                    homeMode = 26;
                }
                else if (Axes[id].HomeMode == HomeModes.ORGAndIndex)
                {
                    homeMode = 4;
                    homeEz = 0;
                }
                else if (Axes[id].HomeMode == HomeModes.ELAndIndex)
                {
                    homeMode = 27;
                    homeEz = 0;
                }

                if (Axes[id].HomeDirection == HomeDirection.Forward)
                {
                    homeDir = 0;
                }
                else if (Axes[id].HomeDirection == HomeDirection.Backward)
                {
                    homeDir = 1;
                }
                double vm = Axes[id].HomeVelocity.MaxVel * Axes[id].Ratio;
                int dec = (int)(vm / Axes[id].HomeVelocity.DecelerationTime);
                int acc = (int)(vm / Axes[id].HomeVelocity.AccelerationTime);

                //ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_HOME_MODE, homeMode); // Set home mode 0: home mode 1 (ORG)  1: home mode 2 (EL) 2: home mode 3 (EZ)
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_HOME_DIR, homeDir); // Set home direction 0: positive direction 1: negative(direction)
                ret = APS168Lib.APS_set_axis_param_f(axisID, (int)APS_Define.PRA_HOME_CURVE, 0); // Set acceleration paten (T-curve) [ 0.0 ~ 1.0 ] 0:T(curve)  1:S(curve)
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_HOME_ACC, acc); // Set homing acceleration rate
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_HOME_VM, (int)vm); // Set homing maximum velocity.
                //ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_HOME_VO, (int)(vm / 2)); // Set homing slow velocity
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_HOME_EZA, homeEz); // Set homing 0: Not enable 1: Enable()
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_HOME_POS, 0); // Homing完成後，位置命令設定
                //ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_HOME_SHIFT, 100); // Home 位置和定位訊號的偏移距離

                ret = APS168Lib.APS_home_move(axisID);// 'Start homing
                if (ret == eRR_NoError)
                {
                    ChkAxisStop(id, 300);
                    System.Threading.Thread.Sleep(100);
                    //ret = SetHomeOffset(id, 0);
                    if (ret == eRR_NoError)
                    {
                        ret = APS168Lib.APS_home_move(axisID);// 'Start homing    
                        if (ret == eRR_NoError)
                        {
                            ChkAxisStop(id, 300);
                            System.Threading.Thread.Sleep(100);
                            //ret = SetHomeOffset(id, 0);
                            if (ret == eRR_NoError)
                            {
                                homeOk = true;
                            }
                        }

                    }
                }
                if (homeOk == false)
                {
                    throw new Exception("Home Library SettingError!");
                }
            }
            catch (Exception ex)
            {

                throw new Exception("HomeError:" + Axes[id].AxisName + "  ，" + ex);
            }

        }
        public void SetAxisDirectionCommand(int id, AxisDirection direction)
        {
            throw new NotImplementedException();
        }
        public void SetLimitCommand(int id, double minPos, double maxPos)
        {
            try
            {
                axeslimitP[id] = maxPos;
                axeslimitN[id] = minPos;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void SetSpeedCommand(int id, VelocityParams motionVelocity)
        {
            try
            {
                //Axes[id].AxisVelocity = motionVelocity;
                axesMovVel[id] = motionVelocity;
                int ret;
                double enr = 0;
                double maxVelocity = Axes[id].AxisVelocity.MaxVel * Axes[id].Ratio;
                int dec = (int)(maxVelocity / Axes[id].AxisVelocity.DecelerationTime);
                int acc = (int)(maxVelocity / Axes[id].AxisVelocity.AccelerationTime);
                int axisID = axisRealID[Axes[id].AxisID];
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_ACC, dec);
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_DEC, dec);
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_VS, 0);
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_VE, 0);
                ret = APS168Lib.APS_set_axis_param(axisID, (int)APS_Define.PRA_VM, (int)maxVelocity);
                if (ret == eRR_NoError)
                {

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }






        private bool InitialCard()
        {
            try
            {
                int DPAC_ID_Bits = 0;
                bool bool_APS_Status = false;

                if (APS168Lib.APS_initial(ref DPAC_ID_Bits, 0) == 0)
                {
                    /////////////////////////IO卡設置/////////////////////////
                    Function_Result(APS168Lib.APS_set_field_bus_param(cardID, Bus_HSL, (int)APS_Define.PRF_COMMUNICATION_TYPE, 1));     // Full Duplex
                    if (!FunctionFail)
                    {

                    }
                    if (!FunctionFail)
                    {
                        Function_Result(APS168Lib.APS_set_field_bus_param(cardID, Bus_HSL, (int)APS_Define.PRF_TRANSFER_RATE, 2));     // 6Mps(3M/6M/12M) Motion Net Parameter
                                                                                                                                       // 0/1/2/3 - 2.5M/5M/10M/20M [10M]
                                                                                                                                       //Set PRF_TRANSFER_RATE: 3 (12M)
                    }
                    if (!FunctionFail)
                    {
                        Function_Result(APS168Lib.APS_set_field_bus_param(cardID, Bus_HSL, (int)APS_Define.PRF_HUB_NUMBER, 0));
                    }
                    if (!FunctionFail)
                    {
                        Function_Result(APS168Lib.APS_set_field_bus_param(cardID, Bus_HSL, (int)APS_Define.PRF_INITIAL_TYPE, 0));        // Reset DO to Zero
                    }
                    if (!FunctionFail)
                    {
                        Function_Result(APS168Lib.APS_set_field_bus_param(cardID, Bus_HSL, (int)APS_Define.PRF_CHKERRCNT_LAYER, 7));     // Error Count

                    }
                    if (!FunctionFail)
                    {
                        Function_Result(APS168Lib.APS_start_field_bus(cardID, Bus_HSL, 0));
                    }


                    /////////////////////////軸卡設置/////////////////////////
                    if (!FunctionFail)
                    {
                        Function_Result(APS168Lib.APS_set_field_bus_param(cardID, Bus_MNET, (int)APS_Define.PRF_TRANSFER_RATE, 2));     // 6Mps(3M/6M/12M) Motion Net Parameter
                                                                                                                                        // 0/1/2/3 - 2.5M/5M/10M/20M [10M]
                                                                                                                                        //Set PRF_TRANSFER_RATE: 3 (12M)
                    }
                    if (!FunctionFail)
                    {
                        Function_Result(APS168Lib.APS_start_field_bus(cardID, Bus_MNET, startAxis[0]));
                    }
                    if (!FunctionFail)
                    {
                        int totalAxisNo = 0;
                        int firstAxisNo = 0;
                        for (int i = 0; i < totalAxis.Length; i++)
                        {
                            if (!FunctionFail)
                            {
                                Function_Result(APS168Lib.APS_get_field_bus_slave_first_axisno(cardID, Bus_MNET, i, ref firstAxisNo, ref totalAxisNo));
                                if (FunctionFail || totalAxisNo != totalAxis[i] || firstAxisNo != startAxis[i])
                                {
                                    //(">>MNET_BUS(i) First-Total讀取失敗! " + IntToStr(FirstAxisNo) + "," + IntToStr(TotalAxisNo))
                                }
                                else
                                {
                                    if (i == 0)
                                    {
                                        start_Axis_ID = firstAxisNo;
                                    }
                                    //(">>MNET_BUS(i) First-Total讀取成功. " + IntToStr(FirstAxisNo) + ",(" + IntToStr(TotalAxisNo))
                                    int axisno = 0, boardid = 0, portid = 0, moduleid = 0, ret1 = 0;
                                    Function_Result(APS168Lib.APS_get_axis_info(firstAxisNo, ref boardid, ref axisno, ref portid, ref moduleid));
                                    if (moduleid != i)
                                    {
                                        FunctionFail = false;
                                        //("ModuleID=0失敗!")
                                    }
                                }
                            }
                        }
                        Function_Result(APS168Lib.APS_load_param_from_file("C:\\WLS3200-System\\Motion.xml"));//"C://" + "Motion.xml"
                        if (!FunctionFail)
                        {
                            bool_APS_Status = true;
                        }
                    }
                }
                if (bool_APS_Status == false)
                {
                    APS168Lib.APS_close();
                }
                return bool_APS_Status;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private bool FunctionFail;
        public void Function_Result(Int32 Ret)
        {
            if (Ret != 0)
            {
                FunctionFail = true;
            }
            else
            {
                FunctionFail = false;
            }

        }
        /// <summary>
        /// 設定IO Output modNo第幾張卡，output每個點的狀態
        /// </summary>
        /// <param name="modNo"></param>
        /// <param name="output"></param>
        private void APS168SetOutput(int setModID, int id, bool isOn)
        {
            try
            {
                // IO1: 1, 16in/16out
                // IO2: 3, 16in/16out
                // IO3: 5, 32out

                // IO1: 1 cbBit11 紅燈
                // IO1: 1 cbBit15 蜂鳴器

                int DOData = 0;
                int realModID = outputRealModID[setModID];
                int outputCount = totalOutput[setModID];
                int lowCount = 0;
                for (int i = 0; i < setModID; i++)
                {
                    lowCount += totalOutput[i];
                }
                APS168Lib.APS_get_field_bus_d_output(cardID, Bus_HSL, realModID, ref DOData);
                string binaryValue = Convert.ToString(DOData, 2);
                string paddedBinaryValue = binaryValue.PadLeft(outputCount, '0');
                char[] paddedBinaryArray = paddedBinaryValue.ToCharArray();
                if (isOn == true)
                {
                    paddedBinaryArray[(outputCount - 1) - (id - lowCount)] = '1';
                }
                else
                {
                    paddedBinaryArray[(outputCount - 1) - (id - lowCount)] = '0';
                }

                int value = 0;
                int bit = 0x1;
                //int On_Idx = 11;//紅燈
                for (int i = 0; i < outputCount; i++)
                {
                    // 如果 bitcheck 被選取，則設置對應位元
                    if (paddedBinaryArray[(outputCount - 1) - i] == '1')
                    {
                        value |= bit;
                    }
                    // 將位元左移一位
                    bit = bit << 1;
                }
                Function_Result(APS168Lib.APS_set_field_bus_d_output(cardID, Bus_HSL, realModID, value));
                if (FunctionFail == true)
                {

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 取得IO Input modNo第幾張卡，inputcount有幾個input點位
        /// </summary>
        /// <param name="modNo"></param>
        /// <param name="inputcount"></param>
        /// <returns></returns>
        private int[] APS168GetInput(int setModID)
        {
            try
            {
                int DI_Value = 0;
                int realModID = inputRealModID[setModID];
                int inputcount = totalInput[setModID];
                APS168Lib.APS_get_field_bus_d_input(cardID, Bus_HSL, realModID, ref DI_Value);
                string ss = Convert.ToString(DI_Value, 2).PadLeft(inputcount, '0');//  DI_Value.ToString("x");
                int[] inputArray = ss.Select(c => int.Parse(c.ToString())).ToArray();

                return inputArray;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 檢查軸的所有Sensor、運作狀態
        /// </summary>
        /// <param name="AxisID"></param>
        /// <returns></returns>
        private AxisInfo ChkAxisStatus(int id)
        {
            try
            {
                // Motion IO status bit number define.
                const short MIO_ALM = 0; // Servo alarm.
                const short MIO_PEL = 1; // Positive end limit.
                const short MIO_MEL = 2; // Negative end limit.
                const short MIO_ORG = 3; // ORG =Home
                const short MIO_EMG = 4; // Emergency stop
                const short MIO_EZ = 5; // EZ.
                const short MIO_INP = 6; // In position.
                const short MIO_SVON = 7; // Servo on signal.
                const short MIO_RDY = 8; // Ready.
                const short MIO_WARN = 9; // Warning.
                const short MIO_ZSP = 10; // Zero speed.
                const short MIO_SPEL = 11; // Soft positive end limit.
                const short MIO_SMEL = 12; // Soft negative end limit.
                const short MIO_TLC = 13; // Torque is limited by torque limit value.
                const short MIO_ABSL = 14; // Absolute position lost.
                const short MIO_STA = 15; // External start signal.
                const short MIO_PSD = 16; // Positive slow down signal
                const short MIO_MSD = 17; // Negative slow down signal

                // Motion status bit number define.
                const short MTS_CSTP = 0; // Command stop signal.
                const short MTS_VM = 1; // At maximum velocity.
                const short MTS_ACC = 2; // In acceleration.
                const short MTS_DEC = 3; // In deceleration.
                const short MTS_DIR = 4; // LastMoving direction.
                const short MTS_NSTP = 5; // Normal stop(Motion done.
                const short MTS_HMV = 6; // In home operation.
                const short MTS_SMV = 7; // Single axis move relative, absolute, velocity move.
                const short MTS_LIP = 8; // Linear interpolation.
                const short MTS_CIP = 9; // Circular interpolation.
                const short MTS_VS = 10; // At start velocity.
                const short MTS_PMV = 11; // Point table move.
                const short MTS_PDW = 12; // Point table dwell move.
                const short MTS_PPS = 13; // Point table pause state.
                const short MTS_SLV = 14; // Slave axis move.
                const short MTS_JOG = 15; // Jog move.
                const short MTS_ASTP = 16; // Abnormal stop.
                const short MTS_SVONS = 17; // Servo off stopped.
                const short MTS_EMGS = 18; // EMG / SEMG stopped.
                const short MTS_ALMS = 19; // Alarm stop.
                const short MTS_WANS = 20; // Warning stopped.
                const short MTS_PELS = 21; // PEL stopped.
                const short MTS_MELS = 22; // MEL stopped.
                const short MTS_ECES = 23; // Error counter check level reaches and stopped.
                const short MTS_SPELS = 24; // Soft PEL stopped.
                const short MTS_SMELS = 25; // Soft MEL stopped.
                const short MTS_STPOA = 26; // Stop by others axes.
                const short MTS_GDCES = 27; // Gantry deviation error level reaches and stopped.
                const short MTS_GTM = 28; // Gantry mode turn on.
                const short MTS_PAPB = 29; // Pulsar mode turn on.

                // Following definition for PCI-8254/8
                const short MTS_MDN = 5;         // // Motion done. 0: In motion, 1: Motion done ( It could be abnormal stop)
                const short MTS_WAIT = 10;        // // Axis is in waiting state. ( Wait move trigger )
                const short MTS_PTB = 11;       // // Axis is in point buffer moving. ( When this bit on, MDN and ASTP will be cleared )
                const short MTS_BLD = 17;        // // Axis (Axes) in blending moving
                const short MTS_PRED = 18;        // // Pre-distance event, 1: event arrived. The event will be clear when axis start moving 
                const short MTS_POSTD = 19;        // // Post-distance event. 1: event arrived. The event will be clear when axis start moving
                const short MTS_GER = 28;       // // 1: In geared ( This axis as slave axis and it follow a master specified in axis parameter. )

                // Motion IO status bit value define.
                const short MIO_ALM_V = 0x1; // Servo alarm.
                const short MIO_PEL_V = 0x2; // Positive end limit.
                const short MIO_MEL_V = 0x4; // Negative end limit.
                const short MIO_ORG_V = 0x8; // ORG =Home.
                const short MIO_EMG_V = 0x10; // Emergency stop.
                const short MIO_EZ_V = 0x20; // EZ.
                const short MIO_INP_V = 0x40; // In position.
                const short MIO_SVON_V = 0x80; // Servo on signal.
                const short MIO_RDY_V = 0x100; // Ready.
                const short MIO_WARN_V = 0x200; // Warning.
                const short MIO_ZSP_V = 0x400; // Zero speed.
                const short MIO_SPEL_V = 0x800; // Soft positive end limit.
                const short MIO_SMEL_V = 0x1000; // Soft negative end limit.
                const short MIO_TLC_V = 0x2000; // Torque is limited by torque limit value.
                const short MIO_ABSL_V = 0x4000; // Absolute position lost.
                const int MIO_STA_V = 0x8000; // External start signal.
                const int MIO_PSD_V = 0x10000; // Positive slow down signal.
                const int MIO_MSD_V = 0x20000; // Negative slow down signal.

                // Motion status bit value define.
                const short MTS_CSTP_V = 0x1; // Command stop signal.
                const short MTS_VM_V = 0x2; // At maximum velocity.
                const short MTS_ACC_V = 0x4; // In acceleration.
                const short MTS_DEC_V = 0x8; // In deceleration.
                const short MTS_DIR_V = 0x10; // LastMoving direction.
                const short MTS_NSTP_V = 0x20; // Normal stop Motion done.
                const short MTS_HMV_V = 0x40; // In home operation.
                const short MTS_SMV_V = 0x80; // Single axis move( relative, absolute, velocity move.
                const short MTS_LIP_V = 0x100; // Linear interpolation.
                const short MTS_CIP_V = 0x200; // Circular interpolation.
                const short MTS_VS_V = 0x400; // At start velocity.
                const short MTS_PMV_V = 0x800; // Point table move.
                const short MTS_PDW_V = 0x1000; // Point table dwell move.
                const short MTS_PPS_V = 0x2000; // Point table pause state.
                const short MTS_SLV_V = 0x4000; // Slave axis move.
                const int MTS_JOG_V = 0x8000; // Jog move.
                const int MTS_ASTP_V = 0x10000; // Abnormal stop.
                const int MTS_SVONS_V = 0x20000; // Servo off stopped.
                const int MTS_EMGS_V = 0x40000; // EMG / SEMG stopped.
                const int MTS_ALMS_V = 0x80000; // Alarm stop.
                const int MTS_WANS_V = 0x100000; // Warning stopped.
                const int MTS_PELS_V = 0x200000; // PEL stopped.
                const int MTS_MELS_V = 0x400000; // MEL stopped.
                const int MTS_ECES_V = 0x800000; // Error counter check level reaches and stopped.
                const int MTS_SPELS_V = 0x1000000; // Soft PEL stopped.
                const int MTS_SMELS_V = 0x2000000; // Soft MEL stopped.
                const int MTS_STPOA_V = 0x4000000; // Stop by others axes.
                const int MTS_GDCES_V = 0x8000000; // Gantry deviation error level reaches and stopped.
                const int MTS_GTM_V = 0x10000000; // Gantry mode turn on.
                const int MTS_PAPB_V = 0x20000000; // Pulsar mode turn on.

                int MotionStatus;
                int IOState;

                AxisInfo axisInfo = new AxisInfo(); //= m_Axis[AxisID];
                int axisID = axisRealID[Axes[id].AxisID];
                axisInfo.AxisID = axisID;
                axisInfo.ErrMessage = "";
                IOState = APS168Lib.APS_motion_io_status(axisInfo.AxisID);

                axisInfo.IO.RdyInput = (enumMotionFlag)((MIO_RDY_V & IOState) == MIO_RDY_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.AlarmSignal = (enumMotionFlag)((MIO_ALM_V & IOState) == MIO_ALM_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.PositiveLimitSwitch = (enumMotionFlag)((MIO_PEL_V & IOState) == MIO_PEL_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.NegativeLimitSwitch = (enumMotionFlag)((MIO_MEL_V & IOState) == MIO_MEL_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.OriginSwitch = (enumMotionFlag)((MIO_ORG_V & IOState) == MIO_ORG_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.DIROutput = enumMotionFlag.eLow;
                axisInfo.IO.EMGStatus = (enumMotionFlag)((MIO_EMG_V & IOState) == MIO_EMG_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.PCSSignalInput = enumMotionFlag.eLow;
                axisInfo.IO.ERCOutput = enumMotionFlag.eLow;
                axisInfo.IO.IndexSignal = (enumMotionFlag)((MIO_EZ_V & IOState) == MIO_EZ_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.ClearSignal = enumMotionFlag.eLow;
                axisInfo.IO.LatchSignalInput = enumMotionFlag.eLow;
                axisInfo.IO.InPositionSignalInput = (enumMotionFlag)((MIO_INP_V & IOState) == MIO_INP_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.ServoOnOutput = (enumMotionFlag)((MIO_SVON_V & IOState) == MIO_SVON_V ? enumMotionFlag.eLow : enumMotionFlag.eHigh);
                axisInfo.IO.PositiveSlowDownPoint = (enumMotionFlag)((MIO_PSD_V & IOState) == MIO_PSD_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.NegativeSlowDownPoint = (enumMotionFlag)((MIO_MSD_V & IOState) == MIO_MSD_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                axisInfo.IO.InterruptStatus = enumMotionFlag.eLow;
                //axisInfo.IO.CSTPInput  = (enumMotionFlag)((MIO_MSD_V & IOState) == MIO_MSD_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);


                if (axisInfo.IO.AlarmSignal == enumMotionFlag.eHigh)
                {
                    axisInfo.ErrMessage = "Driver Alarm";
                }
                MotionStatus = APS168Lib.APS_motion_status(axisInfo.AxisID);

                axisInfo.Status.CSTPFinished = (enumMotionFlag)((MTS_CSTP_V & MotionStatus) == MTS_CSTP_V ? enumMotionFlag.eHigh : enumMotionFlag.eLow);
                if (axisInfo.Status.MovementIsFinished == enumMotionFlag.eLow)
                {
                    if ((MotionStatus & MTS_NSTP_V) == MTS_NSTP_V)
                    {
                        axisInfo.Status.MovementIsFinished = enumMotionFlag.eHigh;
                    }
                    else if ((MotionStatus & MTS_ASTP_V) == MTS_ASTP_V)
                    {
                        axisInfo.Status.MovementIsFinished = enumMotionFlag.eHigh;
                        int rtnCode = 0;
                        APS168Lib.APS_get_stop_code(axisInfo.AxisID, ref rtnCode);
                        switch (rtnCode)
                        {
                            case 0:
                                // Normal Stop
                                break;
                            case 1:
                                axisInfo.ErrMessage = $"{axisID} Emergency stop signal enabled, axis card stopped driving!";
                                break;
                            case 2:
                                axisInfo.ErrMessage = $"{axisID} Driver abnormal signal enabled, axis card stopped driving!";
                                break;
                            case 3:
                                axisInfo.ErrMessage = $"{axisID} 馬達尚未激磁, axis card stopped driving!";
                                break;
                            case 4:
                                if (!axisInfo.IsHomming)
                                {
                                    axisInfo.ErrMessage = $"{axisID} Hardware positive limit signal enabled, axis card stopped driving!";
                                }
                                break;
                            case 5:
                                if (!axisInfo.IsHomming)
                                {
                                    axisInfo.ErrMessage = $"{axisID} Hardware negative limit signal enabled, axis card stopped driving!";
                                }
                                break;
                            case 6:
                                axisInfo.ErrMessage = $"{axisID} Software positive limit signal enabled, axis card stopped driving!";
                                break;
                            case 7:
                                axisInfo.ErrMessage = $"{axisID} Software negative limit signal enabled, axis card stopped driving!";
                                break;
                            case 8:
                                axisInfo.ErrMessage = $"{axisID} EMG stop by user!";
                                break;
                            case 9:
                                axisInfo.ErrMessage = $"{axisID} Stop by user!";
                                break;
                            case 10:
                                axisInfo.ErrMessage = $"{axisID} Stop by E-Gear gantry protect level 1 condition is met!";
                                break;
                            case 11:
                                axisInfo.ErrMessage = $"{axisID} Stop by E-Gear gantry protect level 2 condition is met!";
                                break;
                            case 12:
                                axisInfo.ErrMessage = $"{axisID} Stop because gear slave axis!";
                                break;
                            case 13:
                                axisInfo.ErrMessage = $"{axisID} 追隨誤差過大, axis card stopped driving!";
                                break;
                            case 14:
                                axisInfo.ErrMessage = $"{axisID} 數位訊號啟用, axis card stopped driving!";
                                break;
                            default:
                                break;
                        }
                    }
                    else if ((MotionStatus & MTS_SVONS_V) == MTS_SVONS_V)
                    {
                        axisInfo.Status.MovementIsFinished = enumMotionFlag.eHigh;
                        axisInfo.ErrMessage = "Servo off stopped";
                    }
                }
                return axisInfo;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// 檢查軸是否停止下來
        /// </summary>
        /// <param name="id"></param>
        private void ChkAxisStop(int id, int timeOutSecond)
        {
            try
            {
                int i = 0;
                AxisInfo axisInfo = new AxisInfo();
                axisInfo = ChkAxisStatus(id);
                int sleepTime = 50;
                int timeOutCnt = (timeOutSecond * 1000) / sleepTime;

                while (axisInfo.Status.MovementIsFinished != enumMotionFlag.eHigh || axisInfo.Status.CSTPFinished != enumMotionFlag.eHigh)
                {
                    i++;
                    System.Threading.Thread.Sleep(sleepTime);
                    //Task.Delay(50);//確認一下會不會停//System.Threading.Thread.Sleep(50);
                    string message = axisInfo.ErrMessage;
                    string alarmSignal = "off";
                    string movementIsFinished = "off";
                    if (axisInfo.IO.AlarmSignal == enumMotionFlag.eHigh) { alarmSignal = "on"; }
                    if (axisInfo.Status.MovementIsFinished == enumMotionFlag.eHigh) { movementIsFinished = "on"; }
                    if (i >= timeOutCnt)
                    {
                        throw new Exception($"ID{ id}  {Axes[id].AxisName}  Time out ,ErrMessage:{message} AlarmSignal:{alarmSignal} MovementIsFinished:{movementIsFinished}");
                    }
                    if (axisInfo.ErrMessage != "" || axisInfo.IO.AlarmSignal != enumMotionFlag.eLow)
                    {
                        throw new Exception($"ID{ id}  {Axes[id].AxisName}  Time out ,ErrMessage:{message} AlarmSignal:{alarmSignal} MovementIsFinished:{movementIsFinished}");
                    }
                    axisInfo = ChkAxisStatus(id);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private int SetHomeOffset(int id, double offset)
        {
            try
            {
                int ret = 0;
                int axisID = axisRealID[Axes[id].AxisID];
                ret = APS168Lib.APS_set_command_f(axisID, offset * Axes[id].Ratio);
                ret = APS168Lib.APS_set_position_f(axisID, offset * Axes[id].Ratio);
                ret = APS168Lib.APS_set_command_f(axisID, offset * Axes[id].Ratio);
                ret = APS168Lib.APS_set_position_f(axisID, offset * Axes[id].Ratio);
                ret = APS168Lib.APS_set_command_f(axisID, offset * Axes[id].Ratio);
                ret = APS168Lib.APS_set_position_f(axisID, offset * Axes[id].Ratio);
                ret = APS168Lib.APS_set_command_f(axisID, offset * Axes[id].Ratio);
                ret = APS168Lib.APS_set_position_f(axisID, offset * Axes[id].Ratio);
                return ret;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private bool ServoOn(int id)
        {
            try
            {
                int ret = 0;
                int servoOn = 1;
                int axisID = axisRealID[Axes[id].AxisID];
                ret = APS168Lib.APS_set_servo_on(axisID, servoOn);
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private bool ServoOff(int id)
        {
            try
            {
                int ret = 0;
                int servoOff = 0;
                int axisID = axisRealID[Axes[id].AxisID];
                ret = APS168Lib.APS_set_servo_on(axisID, servoOff);
                if (ret == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task ReflashInput()
        {
            try
            {
                while (true)
                {
                    int inputIdx = 0;
                    int[][] getInput = new int[inputRealModID.Length][];
                    for (int i = 0; i < inputRealModID.Length; i++)
                    {
                        getInput[i] = APS168GetInput(i);

                        int[] getInput2 = APS168GetInput(i);

                        for (int j = 0; j < getInput2.Length; j++)
                        {
                            if (getInput2[(getInput2.Length - 1) - j] == 0)
                            {
                                InputSignals[inputIdx].IsSignal = false;
                            }
                            else
                            {
                                InputSignals[inputIdx].IsSignal = true;
                            }
                            //if (getInput[i][j] == 0)
                            //{
                            //    InputSignals[inputIdx].IsSignal = false;
                            //}
                            //else
                            //{
                            //    InputSignals[inputIdx].IsSignal = true;
                            //}
                            inputIdx++;
                        }
                    }
                    await Task.Delay(50);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }


        private class AxisInfo
        {
            public int AxisID { get; set; }
            public MotionIOInfo IO { get; set; }
            public MotionStatusInfo Status { get; set; }
            public string ErrMessage { get; set; }
            public bool IsHomming { get; set; }

            public AxisInfo()
            {
                IO = new MotionIOInfo();
                Status = new MotionStatusInfo();
            }
        }

        private class MotionIOInfo
        {
            public enumMotionFlag RdyInput { get; set; }
            public enumMotionFlag AlarmSignal { get; set; }
            public enumMotionFlag PositiveLimitSwitch { get; set; }
            public enumMotionFlag NegativeLimitSwitch { get; set; }
            public enumMotionFlag OriginSwitch { get; set; }
            public enumMotionFlag DIROutput { get; set; }
            public enumMotionFlag EMGStatus { get; set; }
            public enumMotionFlag PCSSignalInput { get; set; }
            public enumMotionFlag ERCOutput { get; set; }
            public enumMotionFlag IndexSignal { get; set; }
            public enumMotionFlag ClearSignal { get; set; }
            public enumMotionFlag LatchSignalInput { get; set; }
            public enumMotionFlag InPositionSignalInput { get; set; }
            public enumMotionFlag ServoOnOutput { get; set; }
            public enumMotionFlag PositiveSlowDownPoint { get; set; }
            public enumMotionFlag NegativeSlowDownPoint { get; set; }
            public enumMotionFlag InterruptStatus { get; set; }
            public enumMotionFlag CSTPInput { get; set; }
        }

        private class MotionStatusInfo
        {
            public enumMotionFlag HomeRdy { get; set; }
            public enumMotionFlag MovementIsFinished { get; set; }
            public enumMotionFlag CSTPFinished { get; set; }
        }
        private enum enumMotionFlag
        {
            eLow = 0,
            eNotReady = 0,
            eNotSent = 0,
            eHigh = 1,
            eReady = 1,
            eSent = 1,
            eLimitP = 3,
            eLimitN = 4
        }

    }
}
