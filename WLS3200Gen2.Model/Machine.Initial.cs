using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Component;
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

                //Log?.Invoke("loadPort Initial");
                //loadPort.Initial();

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

                //AssignComponent();
            }
            catch (Exception ex)
            {

                throw ex;
            }





        
        }

        public void AssignComponent()
        {

            Axis[] axes = motionController.Axes.ToArray();
            var dis = motionController.IutputSignals.ToArray();
            var dos = motionController.OutputSignals.ToArray();

            feeder = new Feeder(robot, macro, aligner, axes[2]);
            microDetection = new MicroDetection(camera, microscope, axes, dos, dis);

        }
        public async void Home()
        {
            try
            {
                await feeder.Home();
                await microDetection.Home();

            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        private ILoadPort LoadPortEntity(bool isSimulate)
        {
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


            }
            else
            {


            }
           
            return motionController;

        }
        private ICamera CameraEntity(CameraType cameraType)
        {
            ICamera camera = null;
            if (isSimulate)
            {

                camera = new SimulateCamera("");
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


            }
            else
            {


            }
           
            return macro;

        }
        private IRobot RobotEntity(RobotType robotType)
        {
            IRobot robot = null;
            if (isSimulate)
            {


            }
            else
            {


            }
         
            return robot;
        }

    }

}
