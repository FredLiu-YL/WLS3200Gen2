using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyRobot : IEFEMRobot
    {
        public PauseTokenSource pauseToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CancellationTokenSource cancelToken { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsOpen => throw new NotImplementedException();

        public double MoveTolerance => throw new NotImplementedException();

        public double CassetteWaferPitch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int SpeedPercent => throw new NotImplementedException();

        public void Continue()
        {
            throw new NotImplementedException();
        }

        public void FixWafer()
        {
            throw new NotImplementedException();
        }

        public RobotPoint GetPositionCommand()
        {
            throw new NotImplementedException();
        }

        public RobotStatus GetStatus()
        {
            throw new NotImplementedException();
        }

        public void Home()
        {

        }

        public void Initial()
        {
            throw new NotImplementedException();
        }

        public bool IsHavePiece()
        {
            throw new NotImplementedException();
        }

        public bool IsLockOK()
        {
            throw new NotImplementedException();
        }

        public void MovAddress(int address, double zShift)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_GoIn(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_GoIn(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_LiftUp(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_LiftUp(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_Retract(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_Retract(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_Safety(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_Standby(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public void PickWafer_Standby(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_GoIn(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_GoIn(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_PutDown(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_PutDown(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_Retract(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_Retract(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_Safety(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_Standby(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public void PutWafer_Standby(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public void ReleaseWafer()
        {
            throw new NotImplementedException();
        }

        public void SetSpeedPercentCommand(int motionPercent)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
