using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Interface;
using YuanliCore.Motion;

namespace WLS3200Gen2.Model.Component.Adlink
{
    public class APS168Controller : IMotionController
    {
        public APS168Controller(IEnumerable<AxisConfig> axisInfos, IEnumerable<string> doNames, IEnumerable<string> diNames)
        {

        }

        public bool IsOpen => throw new NotImplementedException();

        public Axis[] Axes => throw new NotImplementedException();

        public DigitalInput[] IutputSignals => throw new NotImplementedException();

        public IEnumerable<DigitalOutput> OutputSignals => throw new NotImplementedException();

        public AxisDirection GetAxisDirectionCommand(int id)
        {
            throw new NotImplementedException();
        }

        public void GetLimitCommand(int id, out double limitN, out double limitP)
        {
            throw new NotImplementedException();
        }

        public double GetPositionCommand(int id)
        {
            throw new NotImplementedException();
        }

        public AxisSensor GetSensorCommand(int id)
        {
            throw new NotImplementedException();
        }

        public VelocityParams GetSpeedCommand(int id)
        {
            throw new NotImplementedException();
        }

        public void HomeCommand(int id)
        {
            throw new NotImplementedException();
        }

        public void InitializeCommand()
        {
            throw new NotImplementedException();
        }

        public void MoveCommand(int id, double distance)
        {
            throw new NotImplementedException();
        }

        public void MoveToCommand(int id, double position)
        {
            throw new NotImplementedException();
        }

        public Axis[] SetAxesParam(IEnumerable<AxisConfig> axisConfig)
        {
            throw new NotImplementedException();
        }

        public void SetAxisDirectionCommand(int id, AxisDirection direction)
        {
            throw new NotImplementedException();
        }

        public DigitalInput[] SetInputs(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }

        public void SetLimitCommand(int id, double minPos, double maxPos)
        {
            throw new NotImplementedException();
        }

        public DigitalOutput[] SetOutputs(IEnumerable<string> names)
        {
            throw new NotImplementedException();
        }

        public void SetSpeedCommand(int id, VelocityParams motionVelocity)
        {
            throw new NotImplementedException();
        }

        public void StopCommand(int id)
        {
            throw new NotImplementedException();
        }
    }
}
