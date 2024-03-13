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
        public Task AlarmReset()
        {
            return Task.CompletedTask;
        }

        public void Close()
        {

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

        public Task<bool> IsLockOK()
        {
            return Task.Run(() => false);
        }

        public Task Run(double degree)
        {
            return Task.CompletedTask;
        }

        public Task Vaccum(bool IsOn)
        {
            return Task.CompletedTask;
        }


    }
}
