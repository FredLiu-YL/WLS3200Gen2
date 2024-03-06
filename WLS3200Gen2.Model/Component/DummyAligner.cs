using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyAligner : IAligner
    {
        public Task AlarmReset()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Task<AlignerStatus> GetStatus()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task Run(double degree)
        {
            throw new NotImplementedException();
        }

        public Task Vaccum(bool IsOn)
        {
            throw new NotImplementedException();
        }

       
    }
}
