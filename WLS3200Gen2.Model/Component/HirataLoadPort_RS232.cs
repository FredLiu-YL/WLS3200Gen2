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
    public class HirataLoadPort_RS232 : ILoadPort
    {
        private bool?[] slot;

        private int waferThickness, cassettePitch, starOffset, waferPitchTolerance, waferPositionTolerance;
        private SerialPort serialPort = new SerialPort();
        private readonly object lockObj = new object();
        private const char SOH = (char)0x01;
        private const char CR = (char)0x0D;
        private const string CODE = "00";
        private const string ADR = "00";
        private bool start_flag = false;
        private List<char> RxData;
        private string dataReceiveString;
        private bool IsdataReceivecOK;

        public HirataLoadPort_RS232(string comPort)
        {
            try
            {
                //包率 維修孔:19200, auto CNA2:9600
                serialPort.PortName = comPort;
                serialPort.BaudRate = 9600;
                serialPort.DataBits = 8; //只有7,8
                serialPort.Parity = Parity.None;
                serialPort.StopBits = StopBits.One;
                serialPort.Handshake = Handshake.None;
                //serialPort.RtsEnable = false;
                //serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler_Notch_Finder);
                //serialPort.Open();
                //serialPort.DataReceived += TriggerDataReceive;

                //Task.Run(GetStatus);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool?[] Slot { get => slot; }
        public string ErrorStatus { get; private set; }
        public string DeviceStatus { get; private set; }
        public string ErrorCode { get; private set; }
        public bool IsCassettePutOK { get; private set; }
        public bool IsClamp { get; private set; }
        public bool IsSwitchDoor { get; private set; }
        public bool IsVaccum { get; private set; }
        public bool IsDoorOpen { get; private set; } = false;
        public bool IsSensorCheckDoorOpen { get; private set; }
        public bool IsDock { get; private set; }
        public int WaferThickness { get; private set; }
        public int CassettePitch { get; private set; }
        public int StarOffset { get; private set; }
        public int WaferPitchTolerance { get; private set; }
        public int WaferPositionTolerance { get; private set; }
        public int TimeOutRetryCount { get; set; } = 1;
        public Task<LoadPortStatus> GetStatus()
        {
            try
            {
                return Task.Run(() =>
                {
                    int nowCount = 0;
                    while (true)
                    {
                        LoadPortStatus loadPortStatus = new LoadPortStatus();
                        LoadPortItems loadPortItems = new LoadPortItems();
                        loadPortItems = Get_Status();
                        if (loadPortItems.IsGetOK)
                        {
                            loadPortStatus = UpdateStatus(loadPortItems);
                            return loadPortStatus;
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("Get Status Error");
                            }

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("LoadPort GetStatus:" + ex);
            }
        }
        public Task<LoadPortParam> GetParam()
        {
            try
            {
                return Task.Run(async () =>
                {
                    int nowCount = 0;
                    while (true)
                    {
                        LoadPortParam loadPortParam = new LoadPortParam();
                        LoadPortItems loadPortItems = new LoadPortItems();
                        loadPortItems = Get_FOUPParam(LoadPortFOUPType.TYPE_1);
                        if (loadPortItems.IsGetOK)
                        {
                            loadPortParam = UpdateParam(loadPortItems);
                            return loadPortParam;
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("Get Param Error");
                            }

                        }
                    }
                });

            }
            catch (Exception ex)
            {
                throw new Exception("LoadPort GetParam:" + ex);
            }
        }
        public void Initial()
        {
            try
            {
                Open();
                AlarmReset().Wait();
            }
            catch (Exception ex)
            {

                throw new Exception("LoadPort Initial:" + ex);
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
                throw new Exception("LoadPort Close:" + ex);
            }
        }
        public Task Load()
        {
            try
            {
                return Task.Run(() =>
                {
                    IsDoorOpen = false;
                    LoadPortItems loadPortItems = new LoadPortItems();
                    loadPortItems = Set_Type(LoadPortFOUPType.TYPE_1);
                    if (loadPortItems.IsSetOK == false)
                    {
                        throw new Exception("LoadPort SetType Error");
                    }

                    loadPortItems = Mov_UnLoad();
                    loadPortItems.MappingWaferStatus.Clear();
                    UpdateMapping(loadPortItems);
                    if (loadPortItems.IsMovOK == false)
                    {
                        throw new Exception("LoadPort Home Error");
                    }
                    //有門的開法
                    loadPortItems = Mov_Load();
                    if (loadPortItems.IsMovOK == false)
                    {
                        throw new Exception("LoadPort Load Error");
                    }

                    ////沒有/有門的開法
                    //loadPortItems = Mov_ClampON();
                    //if (loadPortItems.IsMovOK == false)
                    //{
                    //    throw new Exception("Load Error");
                    //}
                    //loadPortItems = Mov_DockMoveIn();
                    //if (loadPortItems.IsMovOK == false)
                    //{
                    //    throw new Exception("Load Error");
                    //}
                    //loadPortItems = Mov_VaccumON();
                    ////如果真空吸的起來，代表有門
                    //if (loadPortItems.IsMovOK)
                    //{
                    //    loadPortItems = Mov_LoadMapping_FromDock();
                    //    if (loadPortItems.IsMovOK == false)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //}
                    //else if (loadPortItems.IsError = true && loadPortItems.ErrorCode == "16")
                    //{
                    //    loadPortItems = Set_Reset();
                    //    if (loadPortItems.IsSetOK == true)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //    loadPortItems = Get_Status();
                    //    UpdateStatus(loadPortItems);
                    //    if (loadPortItems.ErrorStatus != LoadPortErrorType.Normal)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //    loadPortItems = Mov_DoorToOpenPos();
                    //    if (loadPortItems.IsMovOK == false)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //    loadPortItems = Mov_MappingElevator_StartPos();
                    //    if (loadPortItems.IsMovOK == false)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //    loadPortItems = Mov_MappingFW();
                    //    if (loadPortItems.IsMovOK == false)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //    loadPortItems = Mov_MappingElevator_EndPos();
                    //    if (loadPortItems.IsMovOK == false)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //    loadPortItems = Mov_MappingBW();
                    //    if (loadPortItems.IsMovOK == false)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //    loadPortItems = Mov_Elevator_LoadPos();
                    //    if (loadPortItems.IsMovOK == false)
                    //    {
                    //        throw new Exception("Load Error");
                    //    }
                    //}
                    //else
                    //{
                    //    throw new Exception("Load Error");
                    //}
                    loadPortItems = Get_FOUPParam(LoadPortFOUPType.TYPE_1);
                    loadPortItems = Get_Mapping(loadPortItems.FOUP_ParameterStatus.CassetteSlotCount);
                    UpdateMapping(loadPortItems);
                    IsDoorOpen = true;
                });
            }
            catch (Exception ex)
            {
                throw new Exception("LoadPort Load:" + ex);
            }
        }
        public Task Home()
        {
            try
            {
                return Task.Run(() =>
                {
                    int nowCount = 0;
                    while (true)
                    {
                        IsDoorOpen = false;
                        LoadPortItems loadPortItems = new LoadPortItems();
                        loadPortItems = Mov_UnLoad();
                        loadPortItems.MappingWaferStatus.Clear();
                        UpdateMapping(loadPortItems);
                        if (loadPortItems.IsMovOK)
                        {
                            break;
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("Home Error");
                            }

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("LoadPort Home:" + ex);
            }
        }
        public Task AlarmReset()
        {
            try
            {
                return Task.Run(() =>
               {
                   int nowCount = 0;
                   while (true)
                   {
                       LoadPortItems loadPortItems = new LoadPortItems();
                       loadPortItems = Set_Reset();
                       if (loadPortItems.IsSetOK)
                       {
                           break;
                       }
                       else
                       {
                           nowCount++;
                           if (nowCount > TimeOutRetryCount)
                           {
                               throw new Exception("Alarm SetReset Error");
                           }

                       }
                   }
                   nowCount = 0;
                   while (true)
                   {
                       LoadPortItems loadPortItems = new LoadPortItems();
                       loadPortItems = Get_Status();
                       UpdateStatus(loadPortItems);
                       if (loadPortItems.IsGetOK)
                       {
                           break;
                       }
                       else
                       {
                           nowCount++;
                           if (nowCount > TimeOutRetryCount)
                           {
                               throw new Exception("Get Status Error");
                           }

                       }
                   }


               });
            }
            catch (Exception ex)
            {
                throw new Exception("LoadPort AlarmReset:" + ex);
            }
        }
        public Task SetParam(LoadPortParam loadPortParam)
        {
            try
            {
                return Task.Run(() =>
                {
                    int nowCount = 0;
                    while (true)
                    {
                        LoadPortItems loadPortItems = new LoadPortItems();
                        LoadPortFOUP_Parameter fOUP_Parameter = new LoadPortFOUP_Parameter();
                        fOUP_Parameter.WaferThickness = loadPortParam.WaferThickness;
                        fOUP_Parameter.CassettePitch = loadPortParam.CassettePitch;
                        fOUP_Parameter.OffsetDistance = loadPortParam.StarOffset;
                        fOUP_Parameter.WaferPitchTolerance = loadPortParam.WaferPitchTolerance;
                        fOUP_Parameter.WaferPositionTolerance = loadPortParam.WaferPositionTolerance;
                        loadPortItems = Set_FOUPParam(fOUP_Parameter);
                        if (loadPortItems.IsSetOK)
                        {
                            break;
                        }
                        else
                        {
                            nowCount++;
                            if (nowCount > TimeOutRetryCount)
                            {
                                throw new Exception("Set FOUPParam Error");
                            }

                        }
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception("LoadPort SetParam:" + ex);
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
                throw ex;
            }
        }

        /// <summary>
        /// LoadPort自動夾持、檢查、開蓋、Mapping
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_Load()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:FPML;", true);
                bool isMovOK = false;
                bool isINF = false;
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        isMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        isINF = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                if (isMovOK == true && isINF == true)
                {

                    loadPortItems.IsMovOK = true;
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 取得LoadPort的Mapping資訊
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Get_Mapping(int CassetteSlotCount)
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsGetOK = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("GET:MAPR;", false);
                foreach (var item in str3)
                {
                    if (item.Contains("GET"))
                    {
                        loadPortItems.IsGetOK = true;
                        loadPortItems = TransMapping(item, CassetteSlotCount);
                    }
                }
                //await SendMessage(loadPortItems, "GET:MAPR;", LoadPortSendMessageType.GetMapr);
                return loadPortItems;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// LoadPort錯誤重置
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Set_Reset()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsSetOK = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("SET:RSET;", true);
                bool isSetOK = false;
                bool isINF = false;
                foreach (var item in str3)
                {
                    if (item.Contains("SET"))
                    {
                        isSetOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        isINF = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                if (isSetOK == true && isINF == true)
                {
                    loadPortItems.IsSetOK = true;
                }

                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///  LoadPort自動關門
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_UnLoad()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:ORGN;", true);
                bool isMovOK = false;
                bool isINF = false;
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        isMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        isINF = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                if (isMovOK == true && isINF == true)
                {

                    loadPortItems.IsMovOK = true;
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 取得LoadPort目前狀態
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Get_Status()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsGetOK = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("GET:STAS;", false);
                foreach (var item in str3)
                {
                    if (item.Contains("GET"))
                    {
                        loadPortItems.IsGetOK = true;
                        TransStatus(loadPortItems, item);
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 取得LoadPort設定的Cassette參數(板厚、板間距、板數量、板片起始位置偏移、板厚誤差、板間距誤差、板子大小)
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <param name="fOUPType"></param>
        /// <returns></returns>
        public LoadPortItems Get_FOUPParam(LoadPortFOUPType fOUPType)
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                if (LoadPortFOUPType.TYPE_1 != LoadPortFOUPType.None)
                {
                    string A = ((int)fOUPType).ToString("D2");
                    loadPortItems.IsGetOK = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("GET:MAPP" + A + ";", false);
                    foreach (var item in str3)
                    {
                        if (item.Contains("GET"))
                        {
                            loadPortItems.IsGetOK = true;
                            TransFOUP_Parameter(loadPortItems, item);
                        }
                    }
                    return loadPortItems;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 設定LoadPort的Cassette參數(板厚、板間距、板數量、板片起始位置偏移、板厚誤差、板間距誤差、板子大小)
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <param name="fOUP_Parameter"></param>
        /// <returns></returns>
        public LoadPortItems Set_FOUPParam(LoadPortFOUP_Parameter fOUP_Parameter)
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                if (fOUP_Parameter.FOUPType != LoadPortFOUPType.None)
                {
                    loadPortItems.IsSetOK = false;
                    string A = ((int)fOUP_Parameter.FOUPType).ToString("D2");
                    string B = fOUP_Parameter.WaferThickness.ToString("X4");
                    string C = fOUP_Parameter.CassettePitch.ToString("X4");
                    string D = fOUP_Parameter.CassetteSlotCount.ToString("X4");
                    string E = fOUP_Parameter.OffsetDistance.ToString("X4");
                    string F = fOUP_Parameter.WaferPitchTolerance.ToString("X4");
                    string G = fOUP_Parameter.WaferPositionTolerance.ToString("X4");
                    string H = ((int)fOUP_Parameter.SizeStatus).ToString("D2");
                    loadPortItems.IsSetOK = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("SET:MAPP" + A + B + C + D + E + F + G + H + ";", true);
                    bool isSetOK = false;
                    bool isINF = false;
                    foreach (var item in str3)
                    {
                        if (item.Contains("SET"))
                        {
                            isSetOK = true;
                        }
                        if (item.Contains("INF"))
                        {
                            isINF = true;
                        }
                        if (item.Contains("ABS"))
                        {
                            //abnormal finish 
                            //取得error code
                            //更新error狀態
                            int error_code_idx = item.IndexOf("/");
                            string error_code = item.Substring(error_code_idx + 1, 2);
                            loadPortItems.ErrorCode = error_code;
                            loadPortItems.IsError = true;
                        }
                    }
                    if (isSetOK == true && isINF == true)
                    {
                        loadPortItems.IsSetOK = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 設定LoadPort的Cassette第幾組Type
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <param name="fOUPType"></param>
        /// <returns></returns>
        public LoadPortItems Set_Type(LoadPortFOUPType fOUPType)
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                if (fOUPType != LoadPortFOUPType.None)
                {
                    loadPortItems.IsSetOK = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("SET:TYP" + ((int)fOUPType + 1) + ";", true);
                    bool isSetOK = false;
                    bool isINF = false;
                    foreach (var item in str3)
                    {
                        if (item.Contains("SET"))
                        {
                            isSetOK = true;
                        }
                        if (item.Contains("INF"))
                        {
                            isINF = true;
                        }
                        if (item.Contains("ABS"))
                        {
                            //abnormal finish 
                            //取得error code
                            //更新error狀態
                            int error_code_idx = item.IndexOf("/");
                            string error_code = item.Substring(error_code_idx + 1, 2);
                            loadPortItems.ErrorCode = error_code;
                            loadPortItems.IsError = true;
                        }
                    }
                    if (isSetOK == true && isINF == true)
                    {
                        loadPortItems.IsSetOK = true;
                    }
                    //await SendMessage(loadPortItems, "SET:TYP" + ((int)fOUPType + 1) + ";", LoadPortSendMessageType.Set);
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /////////////////////////////////////////////////////////////////////////以下沒有門的Load指令//////////////////////////////////////////////////////////////////////////
        ///Mov_ClampON -> Mov_DockMoveIn -> Mov_VaccumON -> 判斷真空是否成立 -> 不成功 -> Set_Reset -> Mov_DoorOpen -> Mov_DoorToOpenPos -> Mov_MappingElevator_StartPos -> Mov_MappingFW -> Mov_MappingElevator_EndPos -> Mov_MappingBW -> Mov_Elevator_LoadPos
        ///                                                              ->   成功 ->　Mov_LoadMapping_FromDock

        /// <summary>
        /// LoadPort夾持
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_ClampON()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:FCCL;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// LoadPort鬆開
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public async Task Mov_ClampOFF(LoadPortItems loadPortItems)
        {
            await Task.Run(async () =>
            {
                try
                {
                    loadPortItems.IsMovOK = false;
                    loadPortItems.IsDone = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("MOV:FCOP;", true);
                    foreach (var item in str3)
                    {
                        if (item.Contains("MOV"))
                        {
                            loadPortItems.IsMovOK = true;
                        }
                        if (item.Contains("INF"))
                        {
                            loadPortItems.IsDone = true;
                        }
                        if (item.Contains("ABS"))
                        {
                            //abnormal finish 
                            //取得error code
                            //更新error狀態
                            int error_code_idx = item.IndexOf("/");
                            string error_code = item.Substring(error_code_idx + 1, 2);
                            loadPortItems.ErrorCode = error_code;
                            loadPortItems.IsError = true;
                        }
                    }
                    //await SendMessage(loadPortItems, "MOV:FPML;", LoadPortSendMessageType.Mov);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }
        /// <summary>
        /// LoadPort移動Cassette平台到可開門地方
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_DockMoveIn()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:Y_FW;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// LoadPort移動Cassette平台移動到可拿Cassette位置
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public async Task Mov_DockMoveOut(LoadPortItems loadPortItems)
        {
            await Task.Run(async () =>
            {
                try
                {
                    loadPortItems.IsMovOK = false;
                    loadPortItems.IsDone = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("MOV:Y_BW;", true);
                    foreach (var item in str3)
                    {
                        if (item.Contains("MOV"))
                        {
                            loadPortItems.IsMovOK = true;
                        }
                        if (item.Contains("INF"))
                        {
                            loadPortItems.IsDone = true;
                        }
                        if (item.Contains("ABS"))
                        {
                            //abnormal finish 
                            //取得error code
                            //更新error狀態
                            int error_code_idx = item.IndexOf("/");
                            string error_code = item.Substring(error_code_idx + 1, 2);
                            loadPortItems.ErrorCode = error_code;
                            loadPortItems.IsError = true;
                        }
                    }
                    //await SendMessage(loadPortItems, "MOV:FPML;", LoadPortSendMessageType.Mov);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }
        /// <summary>
        /// LoadPort門真空開啟
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_VaccumON()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                loadPortItems.ErrorCode = "";
                loadPortItems.IsError = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:VCON;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// LoadPort開門
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public async Task Mov_DoorOpen(LoadPortItems loadPortItems)
        {
            await Task.Run(async () =>
            {
                try
                {
                    loadPortItems.IsMovOK = false;
                    loadPortItems.IsDone = false;
                    List<string> str3 = new List<string>();
                    str3 = SendGetMessage("MOV:DROP;", true);
                    foreach (var item in str3)
                    {
                        if (item.Contains("MOV"))
                        {
                            loadPortItems.IsMovOK = true;
                        }
                        if (item.Contains("INF"))
                        {
                            loadPortItems.IsDone = true;
                        }
                        if (item.Contains("ABS"))
                        {
                            //abnormal finish 
                            //取得error code
                            //更新error狀態
                            int error_code_idx = item.IndexOf("/");
                            string error_code = item.Substring(error_code_idx + 1, 2);
                            loadPortItems.ErrorCode = error_code;
                            loadPortItems.IsError = true;
                        }
                    }
                    //await SendMessage(loadPortItems, "MOV:FPML;", LoadPortSendMessageType.Mov);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            });
        }
        /// <summary>
        /// LoadPort開門完後，移動到門可下降位置
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_DoorToOpenPos()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:DRFW;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// LoadPort電梯機構，移動到Mapping開始位置
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_MappingElevator_StartPos()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:Z_ST;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Mapping偵測Sensor伸進去
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_MappingFW()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:MAFW;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// LoadPort電梯機構，移動到Mapping結束位置
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_MappingElevator_EndPos()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:Z_ED;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Mapping偵測Sensor回來
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_MappingBW()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:MABW;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// LoadPort電梯機構，移動到遠離Cassette區域位置
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_Elevator_LoadPos()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:Z_DN;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// LoadPort從Dock狀態到Load+Mapping
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <returns></returns>
        public LoadPortItems Mov_LoadMapping_FromDock()
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                loadPortItems.IsMovOK = false;
                loadPortItems.IsDone = false;
                List<string> str3 = new List<string>();
                str3 = SendGetMessage("MOV:FDML;", true);
                foreach (var item in str3)
                {
                    if (item.Contains("MOV"))
                    {
                        loadPortItems.IsMovOK = true;
                    }
                    if (item.Contains("INF"))
                    {
                        loadPortItems.IsDone = true;
                    }
                    if (item.Contains("ABS"))
                    {
                        //abnormal finish 
                        //取得error code
                        //更新error狀態
                        int error_code_idx = item.IndexOf("/");
                        string error_code = item.Substring(error_code_idx + 1, 2);
                        loadPortItems.ErrorCode = error_code;
                        loadPortItems.IsError = true;
                    }
                }
                return loadPortItems;
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
        private List<string> SendGetMessage(string message, bool isInfAbs)
        {
            try
            {
                lock (lockObj)
                {
                    int delayTime = 200;
                    int timeOut1 = 10 * 1000;
                    int timeOut2 = 300 * 1000;
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
                            throw new Exception("LoadPort Error");
                        }
                        if (readMessage1.Count > 0)
                        {
                            foreach (var item in readMessage1)
                            {
                                //每次用Add是因為有時候會有兩個字串判斷，isSendOK_1、isSendOK_2
                                //send:MOV...
                                //get:MOV...
                                //get:INF... or ABS...

                                //send:GET:STAS
                                //get:GET:STAS........

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
        private string SendGetMessage2(string message)
        {
            try
            {

                lock (lockObj)
                {
                    dataReceiveString = "";
                    IsdataReceivecOK = false;
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

                    Task.Run(() =>
                   {
                       while (!IsdataReceivecOK)
                       {
                           Thread.Sleep(100);

                       }
                   }).Wait();

                    return dataReceiveString;

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
        private CheckSumAdd CheckSumAdd_Analysis(string arr)
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
                CheckSumAdd checkSun = new CheckSumAdd();
                checkSun.CSh = check_sum_str[len - 2];
                checkSun.CSl = check_sum_str[len - 1];
                return checkSun;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private class CheckSumAdd
        {

            public char CSh;
            public char CSl;
            public CheckSumAdd()
            {
                CSh = ' ';
                CSl = ' ';
            }
        }

        private void TriggerDataReceive(object sender, SerialDataReceivedEventArgs e)
        {

            var data = serialPort.ReadExisting();

            dataReceiveString += data;

            if (dataReceiveString.Contains(SOH.ToString()) && dataReceiveString.Contains(CR.ToString()))
                IsdataReceivecOK = true;




        }

        /// <summary>
        /// 轉換Get_Status讀取的狀態
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <param name="str"></param>
        public void TransStatus(LoadPortItems loadPortItems, string str)
        {
            try
            {
                char[] receive_data;
                receive_data = str.ToCharArray();
                int start_idx = 13;

                if (receive_data[start_idx].ToString() == "0")
                {
                    loadPortItems.ErrorStatus = LoadPortErrorType.Normal;
                }
                else if (receive_data[start_idx].ToString() == "A")
                {
                    loadPortItems.ErrorStatus = LoadPortErrorType.ErrorCanRecover;
                }
                else if (receive_data[start_idx].ToString() == "E")
                {
                    loadPortItems.ErrorStatus = LoadPortErrorType.ErrorNotRecover;
                }

                loadPortItems.ModeStatus = (LoadPortModeType)int.Parse(receive_data[start_idx + 1].ToString());

                loadPortItems.MachineDeviceStatus = (LoadPortMachineDeviceType)int.Parse(receive_data[start_idx + 2].ToString());

                loadPortItems.RunStatus = (LoadPortRunType)int.Parse(receive_data[start_idx + 3].ToString());

                //status_.operating_state = (LP_status_Operating_state)int.Parse(receive_data[start_idx + 3].ToString());

                char[] tmp = { receive_data[start_idx + 4], receive_data[start_idx + 5] };
                loadPortItems.ErrorCode = new string(tmp);

                loadPortItems.CassetteStatus = (LoadPortCassetteType)int.Parse(receive_data[start_idx + 6].ToString());

                if (receive_data[start_idx + 7].ToString() == "?")
                {
                    loadPortItems.ClampStatus = LoadPortClampType.Running;
                }
                else
                {
                    loadPortItems.ClampStatus = (LoadPortClampType)int.Parse(receive_data[start_idx + 7].ToString());
                }

                if (receive_data[start_idx + 8].ToString() == "?")
                {
                    loadPortItems.DoorStatus = LoadPortDoorType.Running;
                }
                else
                {
                    loadPortItems.DoorStatus = (LoadPortDoorType)int.Parse(receive_data[start_idx + 8].ToString());
                }

                loadPortItems.VaccumStatus = (LoadPortVaccumType)int.Parse(receive_data[start_idx + 9].ToString());

                if (receive_data[start_idx + 10].ToString() == "?")
                {
                    loadPortItems.DoorPositionStatus = LoadPortDoorPositionType.Running;
                }
                else
                {
                    loadPortItems.DoorPositionStatus = (LoadPortDoorPositionType)int.Parse(receive_data[start_idx + 10].ToString());
                }
                try
                {
                    loadPortItems.DieSensorStatus = (LoadPortDieSensorType)int.Parse(receive_data[start_idx + 10].ToString());
                }
                catch (Exception)
                {
                    //throw;
                }
                if (receive_data[start_idx + 12].ToString() == "?")
                {
                    loadPortItems.ElevetorPosStatus = LoadPortElevetorPosType.Running;
                }
                else
                {
                    loadPortItems.ElevetorPosStatus = (LoadPortElevetorPosType)int.Parse(receive_data[start_idx + 12].ToString());
                }

                if (receive_data[start_idx + 13].ToString() == "?")
                {
                    loadPortItems.DockStatus = LoadPortDockType.Running;
                }
                else
                {
                    loadPortItems.DockStatus = (LoadPortDockType)int.Parse(receive_data[start_idx + 13].ToString());
                }

                //start_idx + 14 is reserve

                if (receive_data[start_idx + 15].ToString() == "?")
                {
                    loadPortItems.MappingWaitingPosStatus = LoadPortMappingWaitingPosType.Running;
                }
                else
                {
                    loadPortItems.MappingWaitingPosStatus = (LoadPortMappingWaitingPosType)int.Parse(receive_data[start_idx + 15].ToString());
                }
                //start_idx + 16 is reserve

                if (receive_data[start_idx + 17].ToString() == "?")
                {
                    loadPortItems.MappingStatus = LoadPortMappingType.Running;
                }
                else
                {
                    loadPortItems.MappingStatus = (LoadPortMappingType)int.Parse(receive_data[start_idx + 17].ToString());
                }

                loadPortItems.FOUP_NowType = (LoadPortFOUPType)int.Parse(receive_data[start_idx + 18].ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 轉換Get_Mapping讀取的狀態
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <param name="str"></param>
        public LoadPortItems TransMapping(string str, int CassetteSlotCount)
        {
            try
            {
                LoadPortItems loadPortItems = new LoadPortItems();
                int mapping_res_start_idx = str.IndexOf("/");
                int slotNum = CassetteSlotCount;
                string mapping_res = str.Substring(mapping_res_start_idx + 1, slotNum);
                loadPortItems.MappingWaferStatus.Clear();
                for (int m_idx = 1; m_idx <= slotNum; m_idx++)
                {
                    loadPortItems.MappingWaferStatus.Add((LoadPortMapping_Result)(Convert.ToInt32(mapping_res[m_idx - 1].ToString())));
                }
                return loadPortItems;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 轉換Get_FOUPParam讀取的狀態
        /// </summary>
        /// <param name="loadPortItems"></param>
        /// <param name="str"></param>
        public void TransFOUP_Parameter(LoadPortItems loadPortItems, string str)
        {
            try
            {
                char[] receive_data;
                receive_data = str.ToCharArray();
                if (loadPortItems.FOUP_ParameterStatus == null)
                {
                    loadPortItems.FOUP_ParameterStatus = new LoadPortFOUP_Parameter();
                }
                int start_idx = 13;

                string A = (receive_data[start_idx].ToString() + receive_data[start_idx + 1].ToString() + receive_data[start_idx + 2].ToString() + receive_data[start_idx + 3].ToString());
                loadPortItems.FOUP_ParameterStatus.WaferThickness = Convert.ToInt32(A, 16);

                start_idx = 17;
                string B = (receive_data[start_idx].ToString() + receive_data[start_idx + 1].ToString() + receive_data[start_idx + 2].ToString() + receive_data[start_idx + 3].ToString());
                loadPortItems.FOUP_ParameterStatus.CassettePitch = Convert.ToInt32(B, 16);

                start_idx = 21;
                string C = (receive_data[start_idx].ToString() + receive_data[start_idx + 1].ToString() + receive_data[start_idx + 2].ToString() + receive_data[start_idx + 3].ToString());
                loadPortItems.FOUP_ParameterStatus.CassetteSlotCount = Convert.ToInt32(C, 16);

                start_idx = 25;
                string D = (receive_data[start_idx].ToString() + receive_data[start_idx + 1].ToString() + receive_data[start_idx + 2].ToString() + receive_data[start_idx + 3].ToString());
                loadPortItems.FOUP_ParameterStatus.OffsetDistance = Convert.ToInt32(D, 16);

                start_idx = 29;
                string E = (receive_data[start_idx].ToString() + receive_data[start_idx + 1].ToString() + receive_data[start_idx + 2].ToString() + receive_data[start_idx + 3].ToString());
                loadPortItems.FOUP_ParameterStatus.WaferPitchTolerance = Convert.ToInt32(E, 16);

                start_idx = 33;
                string F = (receive_data[start_idx].ToString() + receive_data[start_idx + 1].ToString() + receive_data[start_idx + 2].ToString() + receive_data[start_idx + 3].ToString());
                loadPortItems.FOUP_ParameterStatus.WaferPositionTolerance = Convert.ToInt32(F, 16);

                start_idx = 37;
                string G = (receive_data[start_idx].ToString() + receive_data[start_idx + 1].ToString());
                loadPortItems.FOUP_ParameterStatus.SizeStatus = (LoadPortSizeType)Convert.ToInt32(G, 16);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }














        public void UpdateMapping(LoadPortItems loadPortItems)
        {
            try
            {
                slot = new bool?[loadPortItems.MappingWaferStatus.Count];
                int idx = slot.Length - 1;
                foreach (var item in loadPortItems.MappingWaferStatus)
                {
                    if (item == LoadPortMapping_Result.WaferExists)
                    {
                        slot[idx] = true;
                    }
                    else if (item == LoadPortMapping_Result.WaferNo)
                    {
                        slot[idx] = null;
                    }
                    else
                    {
                        slot[idx] = false;
                    }
                    idx--;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public LoadPortStatus UpdateStatus(LoadPortItems loadPortItems)
        {
            try
            {
                LoadPortStatus loadPortStatus = new LoadPortStatus();
                if (loadPortItems.ErrorStatus == LoadPortErrorType.Normal)
                {
                    loadPortStatus.ErrorStatus = "正常";
                }
                else if (loadPortItems.ErrorStatus == LoadPortErrorType.ErrorCanRecover)
                {
                    loadPortStatus.ErrorStatus = "可復原的錯誤";
                }
                else if (loadPortItems.ErrorStatus == LoadPortErrorType.ErrorNotRecover)
                {
                    loadPortStatus.ErrorStatus = "不可復原的錯誤";
                }

                if (loadPortItems.MachineDeviceStatus == LoadPortMachineDeviceType.Run)
                {
                    loadPortStatus.DeviceStatus = "運作中";
                }
                else if (loadPortItems.MachineDeviceStatus == LoadPortMachineDeviceType.Load)
                {
                    loadPortStatus.DeviceStatus = "可取片位置";
                }
                else if (loadPortItems.MachineDeviceStatus == LoadPortMachineDeviceType.HomePosition)
                {
                    loadPortStatus.DeviceStatus = "原點位置";
                }

                ErrorCode = loadPortItems.ErrorCode;

                if (loadPortItems.CassetteStatus == LoadPortCassetteType.PutOK)
                {
                    loadPortStatus.IsCassettePutOK = true;
                }
                else
                {
                    loadPortStatus.IsCassettePutOK = false;
                }

                if (loadPortItems.ClampStatus == LoadPortClampType.UnClamp)
                {
                    loadPortStatus.IsClamp = false;
                }
                else
                {
                    loadPortStatus.IsClamp = true;
                }

                if (loadPortItems.DoorStatus == LoadPortDoorType.Close)
                {
                    loadPortStatus.IsSwitchDoor = false;
                }
                else
                {
                    loadPortStatus.IsSwitchDoor = true;
                }

                if (loadPortItems.VaccumStatus == LoadPortVaccumType.Vaccum)
                {
                    loadPortStatus.IsVaccum = true;
                }
                else
                {
                    loadPortStatus.IsVaccum = false;
                }

                if (loadPortItems.DoorPositionStatus == LoadPortDoorPositionType.Close)
                {
                    loadPortStatus.IsDoorOpen = false;
                }
                else
                {
                    loadPortStatus.IsDoorOpen = true;
                }

                if (loadPortItems.DieSensorStatus == LoadPortDieSensorType.Lighting)
                {
                    loadPortStatus.IsSensorCheckDoorOpen = true;
                }
                else
                {
                    loadPortStatus.IsSensorCheckDoorOpen = false;
                }
                //isSensorCheckDoorOpen


                if (loadPortItems.DockStatus == LoadPortDockType.UnDock)
                {
                    loadPortStatus.IsDock = false;
                }
                else
                {
                    loadPortStatus.IsDock = true;
                }

                return loadPortStatus;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public LoadPortParam UpdateParam(LoadPortItems loadPortItems)
        {
            try
            {
                LoadPortParam loadPortParam = new LoadPortParam();
                loadPortParam.WaferThickness = loadPortItems.FOUP_ParameterStatus.WaferThickness;
                loadPortParam.CassettePitch = loadPortItems.FOUP_ParameterStatus.CassettePitch;
                loadPortParam.StarOffset = loadPortItems.FOUP_ParameterStatus.OffsetDistance;
                loadPortParam.WaferPitchTolerance = loadPortItems.FOUP_ParameterStatus.WaferPitchTolerance;
                loadPortParam.WaferPositionTolerance = loadPortItems.FOUP_ParameterStatus.WaferPositionTolerance;

                return loadPortParam;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
    public class LoadPortItems
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
        /// Get指令OK
        /// </summary>
        public bool IsGetOK;
        /// <summary>
        /// 現在是否Error
        /// </summary>
        public bool IsError;
        /// <summary>
        /// Mov指令或者是Set指令執行做完了
        /// </summary>
        public bool IsDone;
        /// <summary>
        /// LoadPortError狀態(正常、可復原的錯誤、不可復原的錯誤)
        /// </summary>
        public LoadPortErrorType ErrorStatus;
        /// <summary>
        /// 目前運作的型態(自動模式、校驗模式、工程模式)
        /// </summary>
        public LoadPortModeType ModeStatus;
        /// <summary>
        /// LoadPort目前運作的位置
        /// </summary>
        public LoadPortMachineDeviceType MachineDeviceStatus;
        /// <summary>
        /// LoadPort目前運作狀態
        /// </summary>
        public LoadPortRunType RunStatus;
        /// <summary>
        /// 異常狀態Str:00-FF
        /// </summary>
        public string ErrorCode;
        /// <summary>
        /// LoadPort上面目前有無Cassette、放置狀態
        /// </summary>
        public LoadPortCassetteType CassetteStatus;
        /// <summary>
        /// LoadPort是否夾持Cassette
        /// </summary>
        public LoadPortClampType ClampStatus;
        /// <summary>
        /// LoadPort轉開門機構
        /// </summary>
        public LoadPortDoorType DoorStatus;
        /// <summary>
        /// LoadPort吸附門真空狀態
        /// </summary>
        public LoadPortVaccumType VaccumStatus;
        /// <summary>
        /// LoadPort門目前的位置
        /// </summary>
        public LoadPortDoorPositionType DoorPositionStatus;
        /// <summary>
        /// Sensor偵測LoadPort門位置的狀態
        /// </summary>
        public LoadPortDieSensorType DieSensorStatus;
        /// <summary>
        /// 電梯軸位置
        /// </summary>
        public LoadPortElevetorPosType ElevetorPosStatus;
        /// <summary>
        /// LoadPort移動Cassette的平台(Dock)，目前的位置
        /// </summary>
        public LoadPortDockType DockStatus;
        /// <summary>
        /// LoadPort的Mapping機構狀態
        /// </summary>
        public LoadPortMappingWaitingPosType MappingWaitingPosStatus;
        /// <summary>
        /// LoadPort的Mapping運作狀態
        /// </summary>
        public LoadPortMappingType MappingStatus;
        /// <summary>
        /// LoadPort設定的參數組TYPE_1、TYPE_2、TYPE_3、TYPE_4、TYPE_5
        /// </summary>
        public LoadPortFOUPType FOUP_NowType;
        /// <summary>
        /// LoadPort的設定參數
        /// </summary>
        public LoadPortFOUP_Parameter FOUP_ParameterStatus = new LoadPortFOUP_Parameter();
        /// <summary>
        /// LoadPort的Mapping結果
        /// </summary>
        public List<LoadPortMapping_Result> MappingWaferStatus = new List<LoadPortMapping_Result>();

        public LoadPortItems()
        {
            IsMovOK = false;
            IsSetOK = false;
            IsGetOK = false;
            IsError = false;
            ErrorStatus = LoadPortErrorType.ErrorCanRecover;
        }
    }
    /// <summary>
    /// LoadPort的錯誤狀態
    /// </summary>
    public enum LoadPortErrorType
    {
        None = -1,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 可復原的錯誤
        /// </summary>
        ErrorCanRecover = 1,
        /// <summary>
        /// 不可復原的錯誤
        /// </summary>
        ErrorNotRecover = 2
    }
    /// <summary>
    /// 目前運作的型態(自動模式、校驗模式、工程模式)
    /// </summary>
    public enum LoadPortModeType
    {
        None = -1,
        /// <summary>
        /// 自動模式
        /// </summary>
        Auto = 0,
        /// <summary>
        /// 校驗模式
        /// </summary>
        Teaching = 1,
        /// <summary>
        /// 工程模式
        /// </summary>
        Maintenance = 2
    }
    /// <summary>
    /// LoadPort目前運作的位置
    /// </summary>
    public enum LoadPortMachineDeviceType
    {
        None = -1,
        /// <summary>
        /// 運作中
        /// </summary>
        Run = 0,
        /// <summary>
        /// 原點位置
        /// </summary>
        HomePosition = 1,
        /// <summary>
        /// CassetteLoad位置
        /// </summary>
        Load = 2
    }
    /// <summary>
    /// LoadPort目前運作狀態
    /// </summary>
    public enum LoadPortRunType
    {
        None = -1,
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 0,
        /// <summary>
        /// 運作中
        /// </summary>
        Running = 1
    }

    public enum LoadPortErrorCode
    {
        Normal = 0,
        Clamp_Time = 10

    }

    /// <summary>
    /// LoadPort上面目前有無Cassette、放置狀態
    /// </summary>
    public enum LoadPortCassetteType
    {
        None = -1,
        /// <summary>
        /// 無Cassette
        /// </summary>
        Non = 0,
        /// <summary>
        /// Cassette放置OK
        /// </summary>
        PutOK = 1,
        /// <summary>
        /// Cassette放置錯誤
        /// </summary>
        PutError = 2
    }
    /// <summary>
    /// LoadPort是否夾持Cassette
    /// </summary>
    public enum LoadPortClampType
    {
        None = -1,
        /// <summary>
        /// 沒有夾住Cassette
        /// </summary>
        UnClamp = 0,
        /// <summary>
        /// 夾住Cassette
        /// </summary>
        Clamp = 1,
        /// <summary>
        /// 運作中
        /// </summary>
        Running = 2
    }
    /// <summary>
    /// LoadPort轉開門機構
    /// </summary>
    public enum LoadPortDoorType
    {
        None = -1,
        /// <summary>
        /// LoadPort轉開門機構_轉開狀態
        /// </summary>
        Open = 0,
        /// <summary>
        /// LoadPort轉開門機構_可插入狀態(正常狀態下)
        /// </summary>
        Close = 1,
        /// <summary>
        /// 運作中
        /// </summary>
        Running = 2
    }
    /// <summary>
    /// LoadPort吸附門真空狀態
    /// </summary>
    public enum LoadPortVaccumType
    {
        None = -1,
        /// <summary>
        /// 門真空關閉
        /// </summary>
        NoVaccum = 0,
        /// <summary>
        /// 門真空開啟
        /// </summary>
        Vaccum = 1
    }
    /// <summary>
    /// LoadPort門目前的位置
    /// </summary>
    public enum LoadPortDoorPositionType
    {
        None = -1,
        /// <summary>
        /// 門在開門位置
        /// </summary>
        Open = 0,
        /// <summary>
        /// 門在關門位置
        /// </summary>
        Close = 1,
        /// <summary>
        /// 運作中
        /// </summary>
        Running = 2
    }
    /// <summary>
    /// Sensor偵測LoadPort門位置的狀態
    /// </summary>
    public enum LoadPortDieSensorType
    {
        None = -1,
        /// <summary>
        /// Sensor偵測門在裡面
        /// </summary>
        Shading = 0,
        /// <summary>
        ///Sensor偵測門在外面
        /// </summary>
        Lighting = 1
    }
    /// <summary>
    /// 電梯軸位置
    /// </summary>
    public enum LoadPortElevetorPosType
    {
        None = -1,
        /// <summary>
        /// 上升位置
        /// </summary>
        Ascending = 0,
        /// <summary>
        /// 下降位置
        /// </summary>
        Descending = 1,
        /// <summary>
        /// Mapping開始位置
        /// </summary>
        MappingStart = 2,
        /// <summary>
        /// Mapping結束位置
        /// </summary>
        MappingEnd = 3,
        /// <summary>
        /// 運轉中
        /// </summary>
        Running = 4
    }
    /// <summary>
    /// LoadPort移動Cassette的平台(Dock)，目前的位置
    /// </summary>
    public enum LoadPortDockType
    {
        None = -1,
        /// <summary>
        /// LoadPort移動Cassette平台，在可拿取Cassette箱子位置
        /// </summary>
        UnDock = 0,
        /// <summary>
        /// LoadPort移動Cassette平台，在等待片子做完位置
        /// </summary>
        Dock = 1,
        /// <summary>
        /// LoadPort移動Cassette平台運作中
        /// </summary>
        Running = 2
    }
    /// <summary>
    /// LoadPort的Mapping機構狀態
    /// </summary>
    public enum LoadPortMappingWaitingPosType
    {
        None = -1,
        /// <summary>
        /// Mapping功能等待
        /// </summary>
        Waiting = 0,
        /// <summary>
        /// 正在Mapping片子
        /// </summary>
        Measuring = 1,
        /// <summary>
        /// 動作中(移動到可以Mapping位置，或者是移出Mapping位置。)
        /// </summary>
        Running = 2
    }
    /// <summary>
    /// LoadPort的Mapping運作狀態
    /// </summary>
    public enum LoadPortMappingType
    {
        None = -1,
        /// <summary>
        /// 正在Mapping片子
        /// </summary>
        Running = 0,
        /// <summary>
        /// 正常Mapping結束
        /// </summary>
        EndOK = 1,
        /// <summary>
        /// 異常Mapping結束
        /// </summary>
        EndError = 2
    }
    /// <summary>
    /// LoadPort設定的參數組TYPE_1、TYPE_2、TYPE_3、TYPE_4、TYPE_5
    /// </summary>
    public enum LoadPortFOUPType
    {
        None = -1,
        TYPE_1 = 0,
        TYPE_2 = 1,
        TYPE_3 = 2,
        TYPE_4 = 3,
        TYPE_5 = 4
    }
    /// <summary>
    /// LoadPort的Mapping結果
    /// </summary>
    public enum LoadPortMapping_Result
    {
        None = -1,
        /// <summary>
        /// 沒有wafer
        /// </summary>
        WaferNo = 0,
        /// <summary>
        /// 有wafer
        /// </summary>
        WaferExists = 1,
        /// <summary>
        /// wafer不是正常擺放，只有單邊有偵測到
        /// </summary>
        Cross = 2,
        /// <summary>
        /// wafer厚度異常，太厚
        /// </summary>
        ThicknessError_Thick = 3,
        /// <summary>
        /// wafer厚度異常，太薄
        /// </summary>
        ThicknessError_Thin = 4,
        /// <summary>
        /// 位置異常，超出範圍
        /// </summary>
        Position_error = 5
    }
    /// <summary>
    /// LoadPort的設定參數
    /// </summary>
    public class LoadPortFOUP_Parameter
    {
        /// <summary>
        /// LoadPort目前運作的Type
        /// </summary>
        public LoadPortFOUPType FOUPType = LoadPortFOUPType.None;
        /// <summary>
        /// Wafer厚度
        /// </summary>
        public int WaferThickness = 0;
        /// <summary>
        /// Cassette間距
        /// </summary>
        public int CassettePitch = 0;
        /// <summary>
        /// Cassette片子數量
        /// </summary>
        public int CassetteSlotCount = 0;
        /// <summary>
        /// LoadPort起始點偏移
        /// </summary>
        public int OffsetDistance = 0;
        /// <summary>
        /// Wafer間距容忍值
        /// </summary>
        public int WaferPitchTolerance = 0;
        /// <summary>
        /// Wafer位置容忍值
        /// </summary>
        public int WaferPositionTolerance = 0;
        /// <summary>
        /// LoadPort的Wafer尺寸大小8、12吋
        /// </summary>
        public LoadPortSizeType SizeStatus = LoadPortSizeType.Inches_12;

        public LoadPortFOUP_Parameter()
        {
        }
    }
    /// <summary>
    /// LoadPort的Wafer尺寸大小8、12吋
    /// </summary>
    public enum LoadPortSizeType
    {
        /// <summary>
        /// 12吋晶圓
        /// </summary>
        Inches_12 = 0,
        /// <summary>
        /// 8吋晶圓
        /// </summary>
        Inches_8 = 1
    }
    public enum LoadPortSendMessageType
    {
        None = -1,
        Mov = 0,
        Set = 1,
        GetStats = 2,
        GetMapr = 3,
        GetMapp = 4
    }
}
