using System;
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
using YuanliCore.Motion;

namespace WLS3200Gen2.Model
{
    public partial class Machine
    {
        public void Initial()
        {

            try
            {
                WriteLog?.Invoke("Initial");

                motionController = ControlEntity();
                loadPort = LoadPortEntity();
                robot = RobotEntity();
                aligner = AlignerEntity();
                camera = CameraEntity();
                macro = MacrotEntity();
                microscope = MicroEntity();

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

                //將初始化後的元件 傳進模組內(分配io點位 以及 軸號)
                AssignComponent();

                WriteLog?.Invoke("Initial End");
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

            Feeder = new Feeder(robot, loadPort, null, macro, aligner, axes[4], machineSetting);
            MicroDetection = new MicroDetection(camera, microscope, axes, dos, dis);
            StackLight = new StackLight(dos);

        }
        public async Task<bool> BeforeHomeCheck()
        {
            try
            {
                bool isWaferInSystem = false;
                Task robotLock = Feeder.Robot.FixWafer();
                Task alignerLock = Feeder.AlignerL.FixWafer();
                Feeder.Macro.FixWafer();
                MicroDetection.TableVacuum.On();
                await Task.WhenAll(robotLock, alignerLock);
                await Task.Delay(1000); //暫停1000ms 等待真空建立完成
                if (Feeder.Robot.IsLockOK)
                {
                    isWaferInSystem = true;
                }
                else
                {
                    await Feeder.Robot.ReleaseWafer();
                }
                if (Feeder.Macro.IsLockOK)
                {
                    isWaferInSystem = true;
                }
                else
                {
                    Feeder.Macro.ReleaseWafer();
                }
                if (Feeder.AlignerL.IsLockOK)
                {
                    isWaferInSystem = true;
                }
                else
                {
                    await Feeder.AlignerL.ReleaseWafer();
                }
                if (MicroDetection.IsTableVacuum.IsSignal)
                {
                    isWaferInSystem = true;
                }
                else
                {
                    MicroDetection.TableVacuum.Off();
                }
                await Task.Delay(500); //暫停500ms 等待解真空
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
                Task feedHome = Feeder.Home();
                await Task.Delay(500); //先暫停500ms 避免判定還沒出現就過了 WaitEFEMonSafe
                await Feeder.WaitEFEMonSafe;//等待EFEM 在安全位置上 就可以先回顯微鏡
                Task microHome = MicroDetection.Home();
                //Task macroHome = macro.Home();
                await Task.WhenAll(feedHome, microHome);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private ILoadPort LoadPortEntity()
        {
            WriteLog?.Invoke("loadPort Initial");
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
                            axisConfig.Add(machineSetting.TableXConfig);
                            break;
                        case 1:
                            axisConfig.Add(machineSetting.TableYConfig);
                            break;
                        case 2:
                            axisConfig.Add(machineSetting.TableZConfig);
                            break;
                        case 3:
                            axisConfig.Add(machineSetting.TableRConfig);
                            break;
                        case 4:
                            axisConfig.Add(machineSetting.RobotAxisConfig);
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
                for (int i = 0; i <= 4; i++)
                {
                    switch (i)
                    {
                        case 0:
                            axisConfig.Add(machineSetting.TableXConfig);
                            break;
                        case 1:
                            axisConfig.Add(machineSetting.TableYConfig);
                            break;
                        case 2:
                            axisConfig.Add(machineSetting.TableZConfig);
                            break;
                        case 3:
                            axisConfig.Add(machineSetting.TableRConfig);
                            break;
                        case 4:
                            axisConfig.Add(machineSetting.RobotAxisConfig);
                            break;
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
                    //camera = new ArtificialCamera();
                    camera = new YuanliCore.CameraLib.ImageSource.ImageSourceCamera(machineSetting.CamerasSettingFileName);

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
                microscope = new BXUCB(machineSetting.MicroscopeCOM);//COM24
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
                    //LoadPortCOM machineSetting.LoadPortCOM
                    robot = new HirataRobot_RS232(machineSetting.RobotsCOM, 10, 2);

                }
            }
            return robot;
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
