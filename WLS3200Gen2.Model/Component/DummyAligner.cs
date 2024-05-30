using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyAligner : IAligner
    {
        public int TimeOutRetryCount { get; set; } = 1;

        public bool IsLockOK { get; } = false;

        public Task AlarmReset()
        {
            return Task.CompletedTask;
        }

        public void Close()
        {

        }

        public Task FixWafer()
        {
            return Task.CompletedTask;
        }

        public Task<AlignerStatus> GetStatus()
        {
            return Task.Run(() => new AlignerStatus());
        }

        public Task Home()
        {
            return Task.CompletedTask;
        }

        public void Initial()
        {
        }

        public Task<bool> IsHavePiece()
        {
            throw new NotImplementedException();
        }


        public Task ReleaseWafer()
        {
            return Task.CompletedTask;
        }

        public Task Run(double degree)
        {
            return Task.CompletedTask;
        }

        public Task Vaccum(bool IsOn)
        {
            return Task.CompletedTask;
        }

        public Task WaferIDRun1(double degree1, double degree2)
        {
            return Task.CompletedTask;
        }

        public Task WaferIDRun2()
        {
            return Task.CompletedTask;
        }
    }
}
