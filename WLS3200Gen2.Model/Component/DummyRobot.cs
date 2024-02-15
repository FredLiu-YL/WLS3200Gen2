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
        public PauseTokenSource pauseToken { get; set ; }
        public CancellationTokenSource cancelToken { get ; set; }

        public bool IsOpen => throw new NotImplementedException();

        public double MoveTolerance => throw new NotImplementedException();

        public double CassetteWaferPitch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int SpeedPercent => throw new NotImplementedException();

        public Task Continue()
        {
            throw new NotImplementedException();
        }

        public Task FixWafer()
        {
            throw new NotImplementedException();
        }

        public Task<RobotPoint> GetPositionCommand()
        {
            throw new NotImplementedException();
        }

        public Task<RobotStatus> GetStatus()
        {
            throw new NotImplementedException();
        }

        public Task Home()
        {
            Task task = Task.CompletedTask;
            return task;
        }

        public Task Initial()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsHavePiece()
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsLockOK()
        {
            throw new NotImplementedException();
        }

        public Task MovAddress(int address, double zShift)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_GoIn(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_GoIn(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_LiftUp(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_LiftUp(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_Retract(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_Retract(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_Safety(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_Standby(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public Task PickWafer_Standby(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_GoIn(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_GoIn(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_PutDown(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_PutDown(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_Retract(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_Retract(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_Safety(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_Standby(ArmStation armStation, int layer)
        {
            throw new NotImplementedException();
        }

        public Task PutWafer_Standby(ArmStation armStation)
        {
            throw new NotImplementedException();
        }

        public Task ReleaseWafer()
        {
            throw new NotImplementedException();
        }

        public Task SetSpeedPercentCommand(int motionPercent)
        {
            throw new NotImplementedException();
        }

        public Task Stop()
        {
            throw new NotImplementedException();
        }
    }
}
