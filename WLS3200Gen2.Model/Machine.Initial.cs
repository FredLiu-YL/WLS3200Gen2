﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Component;
using WLS3200Gen2.Model.Component.Adlink;
using WLS3200Gen2.Model.Module;
using YuanliCore.CameraLib;
using YuanliCore.Interface;
using YuanliCore.Model.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model
{
    public partial class Machine
    {
        public event Action<string> Log;

        public void Initial()
        {

            try
            {
                motionController = ControlEntity(isSimulate);
                loadPort = LoadPortEntity(isSimulate);
                robot = RobotEntity(machineSetting.RobotsType);
                aligner = AlignerEntity(isSimulate);
                camera = CameraEntity(machineSetting.CamerasType);
                macro = MacrotEntity(isSimulate);
                microscope = MicroEntity(isSimulate);




                //Log?.Invoke("Controller Initial");
                //motionController.Initial();

                //Log?.Invoke("Aligner Initial");
                //aligner.Initial();

                //Log?.Invoke("Robot Initial");
                //robot.Initial();

                //Log?.Invoke("Camera Initial");
                //camera.Initial();

                //Log?.Invoke("Macro Initial");
                //macro.Initial();

                //將初始化後的元件 傳進模組內(分配io點位 以及 軸號)
                AssignComponent();
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

            Feeder = new Feeder(robot, loadPort, null, macro, aligner, axes[4]);
            MicroDetection = new MicroDetection(camera, microscope, axes, dos, dis);

        }



        public async Task Home()
        {
            try
            {
                Task feedHome = Feeder.Home();
                await Task.Delay(500); //先暫停500ms 避免判定還沒出現就過了 WaitEFEMonSafe
                await Feeder.WaitEFEMonSafe;//等待EFEM 在安全位置上 就可以先回顯微鏡
                Task microHome = MicroDetection.Home();
                Task macroHome = macro.Home();
                await Task.WhenAll(feedHome, microHome, macroHome);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private ILoadPort LoadPortEntity(bool isSimulate)
        {
            Log?.Invoke("loadPort Initial");
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
                    loadPort = new ArtificialLoadPort();
                    //loadPort = new HirataLoadPort_RS232("COM2");
                    //loadPort.Initial();
                }
                else
                {

                }
            }


            return loadPort;
        }

        private IMotionController ControlEntity(bool isSimulate)
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
                            AxisConfig axisXConfig = new AxisConfig();
                            axisXConfig.AxisName = "AxisX";
                            axisXConfig.AxisID = 0;
                            axisConfig.Add(axisXConfig);
                            break;
                        case 1:
                            AxisConfig axisYConfig = new AxisConfig();
                            axisYConfig.AxisName = "AxisY";
                            axisYConfig.AxisID = 1;
                            axisConfig.Add(axisYConfig);
                            break;
                        case 2:
                            AxisConfig axisZInfo = new AxisConfig();
                            axisZInfo.AxisName = "AxisZ";
                            axisZInfo.AxisID = 2;
                            axisConfig.Add(axisZInfo);
                            break;
                        case 3:
                            AxisConfig axisRInfo = new AxisConfig();
                            axisRInfo.AxisName = "AxisR";
                            axisRInfo.AxisID = 3;
                            axisConfig.Add(axisRInfo);

                            break;
                        case 4:
                            AxisConfig axisRobotInfo = new AxisConfig();
                            axisRobotInfo.AxisName = "RobotAxis";
                            axisRobotInfo.AxisID = 4;
                            axisConfig.Add(axisRobotInfo);
                            break;
                    }
                }

                var doNames = new string[] { "do1", "do2", "do3", "di1", "di2", "di3", "di1", "di2", "di3" };
                var diNames = new string[] { "di1", "di2", "di3", "di1", "di2", "di3", "di1", "di2", "di3" };

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
                            AxisConfig axisXConfig = new AxisConfig();
                            axisXConfig.AxisName = "AxisX";
                            axisXConfig.AxisID = 1500;
                            axisXConfig.Ratio = 10;
                            axisXConfig.MoveVel = new VelocityParams(100000, 0.5);
                            axisXConfig.HomeVel = new VelocityParams(10000, 0.8);
                            axisXConfig.HomeMode = HomeModes.ORGAndIndex;
                            axisConfig.Add(axisXConfig);
                            break;
                        case 1:
                            AxisConfig axisYConfig = new AxisConfig();
                            axisYConfig.AxisName = "AxisY";
                            axisYConfig.AxisID = 1501;
                            axisYConfig.MoveVel = new VelocityParams(1000000, 0.5);
                            axisYConfig.HomeVel = new VelocityParams(100000, 0.5);
                            axisYConfig.HomeMode = HomeModes.ORGAndIndex;
                            axisConfig.Add(axisYConfig);
                            break;
                        case 2:
                            AxisConfig axisZInfo = new AxisConfig();
                            axisZInfo.AxisName = "AxisZ";
                            axisZInfo.AxisID = 1502;
                            axisZInfo.MoveVel = new VelocityParams(50000, 0.2);
                            axisZInfo.HomeVel = new VelocityParams(50000, 0.5);
                            axisZInfo.HomeMode = HomeModes.EL;
                            axisZInfo.HomeDirection = HomeDirection.Backward;
                            axisConfig.Add(axisZInfo);
                            break;
                        case 3:
                            AxisConfig axisRInfo = new AxisConfig();
                            axisRInfo.AxisName = "AxisR";
                            axisRInfo.AxisID = 1503;
                            axisRInfo.MoveVel = new VelocityParams(45000, 0.2);
                            axisRInfo.HomeVel = new VelocityParams(4500, 0.2);
                            axisRInfo.HomeMode = HomeModes.ORG;
                            axisConfig.Add(axisRInfo);

                            break;
                        case 4:
                            AxisConfig axisRobotInfo = new AxisConfig();
                            axisRobotInfo.AxisName = "RobotAxis";
                            axisRobotInfo.AxisID = 1504;
                            axisRobotInfo.MoveVel = new VelocityParams(3000000, 0.2);
                            axisRobotInfo.HomeVel = new VelocityParams(300000, 0.2);
                            axisRobotInfo.HomeMode = HomeModes.ORGAndIndex;
                            axisConfig.Add(axisRobotInfo);
                            break;
                    }
                }
                var doNames = new string[] { "do1", "do2", "do3", "di1", "di2", "di3", "di1", "di2", "di3" };
                var diNames = new string[] { "di1", "di2", "di3", "di1", "di2", "di3", "di1", "di2", "di3" };



                motionController = new Adlink7856(axisConfig, doNames, diNames);
            }

            return motionController;

        }
        private ICamera CameraEntity(CameraType cameraType)
        {
            ICamera camera = null;
            if (isSimulate)
            {

                camera = new SimulateCamera("9.bmp");

            }
            else
            {

                camera = new ArtificialCamera();
            }

            return camera;
        }
        private IAligner AlignerEntity(bool isSimulate)
        {
            IAligner aligner = null;
            if (isSimulate)
            {
                aligner = new DummyAligner();
            }
            else
            {
                aligner = new HirataAligner_RS232("COM32");
                aligner.Initial();
            }

            return aligner;
        }

        private IMacro MacrotEntity(bool isSimulate)
        {
            IMacro macro = null;
            if (isSimulate)
            {

                macro = new DummyMacro();
            }
            else
            {
                macro = new HannDeng_Macro(motionController.OutputSignals.ToArray(), motionController.InputSignals.ToArray());
                macro.Initial();
            }

            return macro;

        }
        private IMicroscope MicroEntity(bool isSimulate)
        {
            IMicroscope microscope = null;
            if (isSimulate)
            {


            }
            else
            {
                microscope = new BXUCB("COM24");
                microscope.Initial();
            }
            return microscope;
        }
        private IEFEMRobot RobotEntity(RobotType robotType)
        {
            IEFEMRobot robot = null;
            if (isSimulate)
            {
                robot = new DummyRobot();

            }
            else
            {
                if (robotType == RobotType.Hirata)
                {
                    //LoadPortCOM machineSetting.LoadPortCOM
                    robot = new HirataRobot_RS232("COM5", 10, 2);
                    robot.Initial();
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
