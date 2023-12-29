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
using YuanliCore.Motion;

namespace WLS3200Gen2.Model
{
    public partial class Machine
    {
        public event Action<string> Log;

        public async void Initial()
        {

            try
            {
                loadPort = LoadPortEntity(isSimulate);

                robot = RobotEntity(machineSetting.RobotsType);
                aligner = AlignerEntity(isSimulate);
                motionController = ControlEntity(isSimulate);
                camera = CameraEntity(machineSetting.CamerasType);
                macro = MacrotEntity(isSimulate);



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
            DigitalInput[] dis = motionController.IutputSignals.ToArray();
            DigitalOutput[] dos = motionController.OutputSignals.ToArray();

            Feeder = new Feeder(robot, loadPort, macro, aligner, axes[3]);
            MicroDetection = new MicroDetection(camera, microscope, axes, dos, dis);

        }



        public async void Home()
        {
            try
            {
                Task feedHome = Feeder.Home();
                await Task.Delay(500); //先暫停500ms 避免判定還沒出現就過了 WaitEFEMonSafe
                await Feeder.WaitEFEMonSafe;//等待EFEM 在安全位置上 就可以先回顯微鏡
                Task microHome = MicroDetection.Home();
                await Task.WhenAll(feedHome, microHome);
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
                loadPort = new ArtificialLoadPort();

            }


            return loadPort;
        }

        private IMotionController ControlEntity(bool isSimulate)
        {
            IMotionController motionController = null;
            if (isSimulate)
            {
                List<AxisConfig> axisConfig = new List<AxisConfig>();

                for (int i = 0; i < 4; i++)
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
                            AxisConfig axisRInfo = new AxisConfig();
                            axisRInfo.AxisName = "AxisR";
                            axisRInfo.AxisID = 2;
                            axisConfig.Add(axisRInfo);

                            break;
                        case 3:
                            AxisConfig axisRobotInfo = new AxisConfig();
                            axisRobotInfo.AxisName = "RobotAxis";
                            axisRobotInfo.AxisID = 3;
                            axisConfig.Add(axisRobotInfo);
                            break;
                    }
                }

                var doNames = new string[] { "do1", "do2", "do3", "di1", "di2", "di3", "di1", "di2", "di3" };
                var diNames = new string[] { "di1", "di2", "di3", "di1", "di2", "di3", "di1", "di2", "di3" };

                motionController = new SimulateMotionControllor(axisConfig, doNames, diNames);
            }
            else
            {
                List<AxisConfig> axisConfig = new List<AxisConfig>();
                var doNames = new string[] { "do1", "do2", "do3", "di1", "di2", "di3", "di1", "di2", "di3" };
                var diNames = new string[] { "di1", "di2", "di3", "di1", "di2", "di3", "di1", "di2", "di3" };

                motionController = new APS168Controller(axisConfig, doNames, diNames);
            }

            return motionController;

        }
        private ICamera CameraEntity(CameraType cameraType)
        {
            ICamera camera = null;
            if (isSimulate)
            {

                camera = new SimulateCamera("d:\\fff123.bmp");
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


            }

            return macro;

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


            }

            return robot;
        }

        public DigitalInput[] GetInputs()
        {
            DigitalInput[] dis = motionController.IutputSignals.ToArray();
            return dis;
        }
        public DigitalOutput[] GetOutputs()
        {
            DigitalOutput[] dos = motionController.OutputSignals.ToArray();
            return dos;
        }


    }

}
