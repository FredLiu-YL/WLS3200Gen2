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
        public PauseTokenSource pauseToken { get; set; }
        public CancellationTokenSource cancelToken { get; set; }

        public bool IsOpen => throw new NotImplementedException();

        public double MoveTolerance => throw new NotImplementedException();

        public double CassetteWaferPitch { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int SpeedPercent => throw new NotImplementedException();

        public bool IsLockOK => false;

        public int TimeOutRetryCount { get; set; } = 1;

        public Task Continue()
        {
            return Task.CompletedTask;
        }

        public Task FixWafer()
        {
            return Task.CompletedTask;
        }

        public Task<RobotPoint> GetPositionCommand()
        {
            throw new NotImplementedException();
        }

        public Task<RobotStatus> GetStatus()
        {
            return Task.Run(() => new RobotStatus());
        }

        public Task Home()
        {
            Task task = Task.CompletedTask;
            return task;
        }

        public void Initial()
        {

        }

        public Task<bool> IsHavePiece()
        {
            return Task.Run(() => false);
        }


        public Task MovAddress(int address, double zShift)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_GoIn(ArmStation armStation, int layer)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_GoIn(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_LiftUp(ArmStation armStation, int layer)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_LiftUp(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_Retract(ArmStation armStation, int layer)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_Retract(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_Safety(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_Standby(ArmStation armStation, int layer)
        {
            return Task.CompletedTask;
        }

        public Task PickWafer_Standby(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_GoIn(ArmStation armStation, int layer)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_GoIn(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_PutDown(ArmStation armStation, int layer)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_PutDown(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_Retract(ArmStation armStation, int layer)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_Retract(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_Safety(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_Standby(ArmStation armStation, int layer)
        {
            return Task.CompletedTask;
        }

        public Task PutWafer_Standby(ArmStation armStation)
        {
            return Task.CompletedTask;
        }

        public Task ReleaseWafer()
        {
            return Task.CompletedTask;
        }

        public Task SetSpeedPercentCommand(int motionPercent)
        {
            return Task.CompletedTask;
        }

        public Task Stop()
        {
            return Task.CompletedTask;
        }


    }
}
