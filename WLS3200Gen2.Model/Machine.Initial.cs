﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Component;
using WLS3200Gen2.Model.Component.Adlink;
using WLS3200Gen2.Model.Module;
using WLS3200Gen2.Module;
using YuanliCore.CameraLib;
using YuanliCore.Interface;
using YuanliCore.Model.Interface;
using YuanliCore.Model.Interface.Component;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model
{
    public partial class Machine
    {
        /// <summary>
        /// Macro是否有片
        /// </summary>
        private bool isMacroHaveWafer = false;
        public void Initial()
        {

            try
            {
                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Initial");

                motionController = ControlEntity();
                loadPort = LoadPortEntity();
                robot = RobotEntity();
                aligner = AlignerEntity();
                camera = CameraEntity();
                macro = MacrotEntity();
                microscope = MicroEntity();
                lampControl = LampControlEntity();
                reader = ReaderEntity();

                motionController.InitializeCommand();

                loadPort.Initial();
                //Log?.Invoke("Robot Initial");
                robot.Initial();
                //Log?.Invoke("Aligner Initial");
                aligner.Initial();
                //Log?.Invoke("Camera Initial");
                //camera.Initial();

                //Log?.Invoke("Macro Initial");
                macro.Initial();

                microscope.Initial();

                for (int i = 0; i < lampControl.Length; i++)
                {
                    lampControl[i].Initial();
                }

                reader.Initial();
                //將初始化後的元件 傳進模組內(分配io點位 以及 軸號)
                AssignComponent();

                WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "Initial End");
            }
            catch (Exception ex)
            {

                throw ex;
            }




        }
        public void Disopse()
        {
            try
            {
                if (MicroDetection != null)
                {
                    try
                    {
                        if (MicroDetection.Camera != null)
                        {
                            MicroDetection.Camera.Stop();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    try
                    {
                        if (MicroDetection.Microscope != null)
                        {
                            MicroDetection.Microscope.ChangeLightAsync(0).Wait();
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                    try
                    {
                        if (StackLight != null)
                        {
                            StackLight.Dispose();
                            StackLight.ReflashTask.Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void AssignComponent()
        {

            Axis[] axes = motionController.Axes.ToArray();
            DigitalInput[] dis = motionController.InputSignals.ToArray();
            DigitalOutput[] dos = motionController.OutputSignals.ToArray();

            if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
            {
                Feeder = new Feeder(robot, loadPort, null, macro, aligner, lampControl[0], lampControl[1], reader, null, machineSetting);
            }
            else
            {
                Feeder = new Feeder(robot, loadPort, null, macro, aligner, lampControl[0], lampControl[1], reader, axes[4], machineSetting);
            }
            MicroDetection = new MicroDetection(camera, microscope, axes, dos, dis, machineSetting.CamerasPixelTable);
            StackLight = new StackLight(dos);

        }
        public async Task<bool> HaveWaferCheck()
        {
            try
            {
                bool isWaferInSystem = false;

                await Feeder.Robot.FixWafer();
                await Task.Delay(1000); //等待真空建立完成
                if (Feeder.Robot.IsLockOK)
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "HaveWaferCheck Robot Have Wafer!!");
                    isWaferInSystem = true;
                }
                else
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "HaveWaferCheck Robot No Wafer.");
                    await Feeder.Robot.ReleaseWafer();
                    await Task.Delay(100); //等待解真空
                }

                Feeder.Macro.FixWafer();
                await Task.Delay(1000); //等待真空建立完成
                if (Feeder.Macro.IsLockOK)
                {
                    isMacroHaveWafer = true;
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "HaveWaferCheck Macro Have Wafer!!");
                    isWaferInSystem = true;
                }
                else
                {
                    isMacroHaveWafer = false;
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "HaveWaferCheck Macro No Wafer.");
                    Feeder.Macro.ReleaseWafer();
                    await Task.Delay(100); //等待解真空
                }

                await Feeder.AlignerL.FixWafer();
                await Task.Delay(100); //等待真空建立完成
                if (Feeder.AlignerL.IsLockOK)
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "HaveWaferCheck Micro Have Wafer!!");
                    isWaferInSystem = true;
                }
                else
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "HaveWaferCheck Aligner No Wafer.");
                    await Feeder.AlignerL.ReleaseWafer();
                    await Task.Delay(100); //等待解真空
                }

                MicroDetection.TableVacuum.On();
                await Task.Delay(1000); //等待真空建立完成
                if (MicroDetection.IsTableVacuum.IsSignal)
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "HaveWaferCheck Micro Have Wafer!!");
                    isWaferInSystem = true;
                }
                else
                {
                    WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "HaveWaferCheck Micro No Wafer.");
                    MicroDetection.TableVacuum.Off();
                    await Task.Delay(100); //等待解真空
                }





                return isWaferInSystem;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task Home()
        {
            try
            {
                //顯示是否要復歸
                Task feedHome = Feeder.Home(isMacroHaveWafer);
                await Task.Delay(500); //先暫停500ms 避免判定還沒出現就過了 WaitEFEMonSafe
                await Feeder.WaitEFEMonSafe;//等待EFEM 在安全位置上 就可以先回顯微鏡
                Task microHome = MicroDetection.Home();
                await Task.WhenAll(feedHome, microHome);
                await MicroscopeDefaultParam();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private async Task MicroscopeDefaultParam()
        {
            try
            {
                bool isDF = false;
                int paramIdx = MicroDetection.Microscope.LensIndex;
                if (paramIdx <= 1) paramIdx = 1;
                if (MicroDetection.Microscope.CubeIndex == 2)
                {
                    isDF = true;
                }
                int intensity = 0;
                int apeture = 0;
                int aFParamTable = 0;

                Recipe.MicroscopeLens[] microscopeLensDefault = machineSetting.MicroscopeLensDefault.ToArray();
                if (isDF)
                {
                    intensity = microscopeLensDefault[paramIdx].DFIntensity;
                    apeture = microscopeLensDefault[paramIdx].DFApeture;
                    aFParamTable = microscopeLensDefault[paramIdx].DFAftbl;
                }
                else
                {
                    intensity = microscopeLensDefault[paramIdx].BFIntensity;
                    apeture = microscopeLensDefault[paramIdx].BFApeture;
                    aFParamTable = microscopeLensDefault[paramIdx].BFAftbl;
                }
                await MicroDetection.Microscope.ChangeLightAsync(intensity);
                await MicroDetection.Microscope.ChangeApertureAsync(apeture);
                await MicroDetection.Microscope.ChangeAFParameterTable(aFParamTable);
            }
            catch (Exception ex)
            {
            }

        }


        private ILoadPort LoadPortEntity()
        {
            WriteLog?.Invoke(YuanliCore.Logger.LogType.PROCESS, "loadPort Initial");
            ILoadPort loadPort = null;

            if (isSimulate)
            {
                loadPort = new DummyLoadPort();

            }
            else
            {
                //只有一支LOAD PORT時
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    //loadPort = new ArtificialLoadPort();
                    loadPort = new HirataLoadPort_RS232(machineSetting.LoadPort1COM);//COM2
                }
                else
                {
                    loadPort = new HirataLoadPort_RS232(machineSetting.LoadPort1COM);//COM2
                }
            }


            return loadPort;
        }

        private IMotionController ControlEntity()
        {
            IMotionController motionController = null;
            if (isSimulate)
            {
                List<AxisConfig> axisConfig = new List<AxisConfig>();

                for (int i = 0; i <= 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            axisConfig.Add(machineSetting.TableXConfig.Copy());
                            break;
                        case 1:
                            axisConfig.Add(machineSetting.TableYConfig.Copy());
                            break;
                        case 2:
                            axisConfig.Add(machineSetting.TableZConfig.Copy());
                            break;
                        case 3:
                            axisConfig.Add(machineSetting.TableRConfig.Copy());
                            break;
                        case 4:
                            axisConfig.Add(machineSetting.RobotAxisConfig.Copy());
                            break;
                    }
                    //switch (i)
                    //{
                    //    case 0:
                    //        AxisConfig axisXConfig = new AxisConfig();
                    //        axisXConfig.AxisName = "AxisX";
                    //        axisXConfig.AxisID = 0;
                    //        axisConfig.Add(axisXConfig);
                    //        break;
                    //    case 1:
                    //        AxisConfig axisYConfig = new AxisConfig();
                    //        axisYConfig.AxisName = "AxisY";
                    //        axisYConfig.AxisID = 1;
                    //        axisConfig.Add(axisYConfig);
                    //        break;
                    //    case 2:
                    //        AxisConfig axisZInfo = new AxisConfig();
                    //        axisZInfo.AxisName = "AxisZ";
                    //        axisZInfo.AxisID = 2;
                    //        axisConfig.Add(axisZInfo);
                    //        break;
                    //    case 3:
                    //        AxisConfig axisRInfo = new AxisConfig();
                    //        axisRInfo.AxisName = "AxisR";
                    //        axisRInfo.AxisID = 3;
                    //        axisConfig.Add(axisRInfo);

                    //        break;
                    //    case 4:
                    //        AxisConfig axisRobotInfo = new AxisConfig();
                    //        axisRobotInfo.AxisName = "RobotAxis";
                    //        axisRobotInfo.AxisID = 4;
                    //        axisConfig.Add(axisRobotInfo);
                    //        break;
                    //}
                }

                //var doNames = new string[] { "do1", "do2", "do3", "di1", "di2", "di3", "di1", "di2", "di3" };
                //var diNames = new string[] { "di1", "di2", "di3", "di1", "di2", "di3", "di1", "di2", "di3" };
                var doNames = new string[64];
                var diNames = new string[32];

                motionController = new SimulateMotionControllor(axisConfig, doNames, diNames);
                motionController.InitializeCommand();
            }
            else
            {
                List<AxisConfig> axisConfig = new List<AxisConfig>();
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                axisConfig.Add(machineSetting.TableXConfig.Copy());
                                break;
                            case 1:
                                axisConfig.Add(machineSetting.TableYConfig.Copy());
                                break;
                            case 2:
                                axisConfig.Add(machineSetting.TableZConfig.Copy());
                                break;
                            case 3:
                                axisConfig.Add(machineSetting.TableRConfig.Copy());
                                break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i <= 4; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                axisConfig.Add(machineSetting.TableXConfig.Copy());
                                break;
                            case 1:
                                axisConfig.Add(machineSetting.TableYConfig.Copy());
                                break;
                            case 2:
                                axisConfig.Add(machineSetting.TableZConfig.Copy());
                                break;
                            case 3:
                                axisConfig.Add(machineSetting.TableRConfig.Copy());
                                break;
                            case 4:
                                axisConfig.Add(machineSetting.RobotAxisConfig.Copy());
                                break;
                        }
                    }
                }

                //var doNames = new string[] { "do1", "do2", "do3", "di1", "di2", "di3", "di1", "di2", "di3" };
                //var diNames = new string[] { "di1", "di2", "di3", "di1", "di2", "di3", "di1", "di2", "di3" };
                var doNames = new string[64];
                var diNames = new string[32];

                motionController = new Adlink7856(axisConfig, doNames, diNames, machineSetting.MotionSettingFileName);//"C:\\WLS3200-System\\Motion.xml"
            }

            return motionController;

        }
        private ICamera CameraEntity()
        {
            try
            {
                ICamera camera = null;
                if (isSimulate)
                {
                    if (!File.Exists("9.bmp")) throw new Exception("模擬情境下需要放一張圖片到執行檔資料夾 取名9.bmp");
                    camera = new SimulateCamera("9.bmp");
                }
                else
                {
                    if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                    {
                        machineSetting.CamerasPixelTable = new System.Windows.Point(-1.095, 1.095);
                        if (machineSetting.CamerasSettingFileName == null)
                        {
                            machineSetting.CamerasSettingFileName = "";
                        }
                        camera = new YuanliCore.CameraLib.IDS.UeyeCamera();
                    }
                    else
                    {
                        machineSetting.CamerasPixelTable = new System.Windows.Point(-0.75, 0.75);
                        if (machineSetting.CamerasSettingFileName == null)
                        {
                            machineSetting.CamerasSettingFileName = "";
                        }
                        camera = new YuanliCore.CameraLib.ImageSource.ImageSourceCamera(machineSetting.CamerasSettingFileName);
                    }
                    //camera = new ArtificialCamera();
                }

                return camera;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private IAligner AlignerEntity()
        {
            IAligner aligner = null;
            if (isSimulate)
            {
                aligner = new DummyAligner();
            }
            else
            {
                aligner = new HirataAligner_RS232(machineSetting.AlignerCOM);//COM32
            }

            return aligner;
        }

        private IMacro MacrotEntity()
        {
            IMacro macro = null;
            if (isSimulate)
            {
                macro = new DummyMacro();
            }
            else
            {
                macro = new HannDeng_Macro(motionController.OutputSignals.ToArray(), motionController.InputSignals.ToArray());
                //machineSetting.InnerRingPitchXPositionPEL = 850;
                //machineSetting.InnerRingPitchXPositionNEL = -850;
                //machineSetting.InnerRingRollYPositionPEL = 850;
                //machineSetting.InnerRingRollYPositionNEL = -850;
                //machineSetting.InnerRingYawTPositionPEL = 4000;
                //machineSetting.InnerRingYawTPositionNEL = -4000;
                //machineSetting.OuterRingRollYPositionPEL = 3200;
                //machineSetting.OuterRingRollYPositionNEL = -3200;
                macro.InnerRingPitchXPositionPEL = machineSetting.InnerRingPitchXPositionPEL;
                macro.InnerRingPitchXPositionNEL = machineSetting.InnerRingPitchXPositionNEL;
                macro.InnerRingRollYPositionPEL = machineSetting.InnerRingRollYPositionPEL;
                macro.InnerRingRollYPositionNEL = machineSetting.InnerRingRollYPositionNEL;
                macro.InnerRingYawTPositionPEL = machineSetting.InnerRingYawTPositionPEL;
                macro.InnerRingYawTPositionNEL = machineSetting.InnerRingYawTPositionNEL;
                macro.OuterRingRollYPositionPEL = machineSetting.OuterRingRollYPositionPEL;
                macro.OuterRingRollYPositionNEL = machineSetting.OuterRingRollYPositionNEL;
            }

            return macro;

        }
        private IMicroscope MicroEntity()
        {
            IMicroscope microscope = null;
            if (isSimulate)
            {
                microscope = new DummyMicroscope();
            }
            else
            {
                microscope = new BXUCB(machineSetting.MicroscopeCOM, machineSetting.IsHaveDIC);//COM24
            }
            return microscope;
        }
        private IEFEMRobot RobotEntity()
        {
            IEFEMRobot robot = null;
            if (isSimulate)
            {
                robot = new DummyRobot();

            }
            else
            {
                if (machineSetting.RobotsType == RobotType.Hirata)
                {
                    if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                    {
                        robot = new HirataRobot_RS232(machineSetting.RobotsCOM, 50, 2, false);
                    }
                    else
                    {
                        robot = new HirataRobot_RS232(machineSetting.RobotsCOM, 10, 2, true);
                    }
                }
            }
            return robot;
        }
        private ILampControl[] LampControlEntity()
        {
            ILampControl[] lampControl = new ILampControl[1];
            if (isSimulate)
            {
                lampControl = new ILampControl[2];
                lampControl[0] = new DummyLampControl();
                lampControl[1] = new DummyLampControl();
            }
            else
            {
                if (machineSetting.LoadPortCount == LoadPortQuantity.Single)
                {
                    lampControl = new ILampControl[2];
                    lampControl[0] = new DummyLampControl();
                    lampControl[1] = new DummyLampControl();
                }
                else
                {
                    lampControl = new ILampControl[2];
                    machineSetting.StrongLamp1COM = "COM25";
                    machineSetting.StrongLamp2COM = "COM26";
                    lampControl[0] = new StrongLampRS232(machineSetting.StrongLamp1COM);//COM25
                    lampControl[1] = new StrongLampRS232(machineSetting.StrongLamp2COM);//COM26
                }
            }
            return lampControl;
        }
        private IReader ReaderEntity()
        {
            IReader reader = null;
            if (isSimulate)
            {
                reader = new DummyReader();
            }
            else
            {
                reader = new CognexWaferID();
            }
            return reader;
        }

        public DigitalInput[] GetInputs()
        {
            DigitalInput[] dis = motionController.InputSignals.ToArray();
            return dis;
        }
        public DigitalOutput[] GetOutputs()
        {
            DigitalOutput[] dos = motionController.OutputSignals.ToArray();
            return dos;
        }


    }

}
