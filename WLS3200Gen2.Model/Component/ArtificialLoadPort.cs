using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class ArtificialLoadPort : ILoadPort
    {
        private bool isMapping = true;
        private bool?[] slot;

        public ArtificialLoadPort()
        {
            slot = new bool?[] { true };

        }

        public bool IsMapping => isMapping;

        public bool?[] Slot => slot;

        public void AlarmReset()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public Task<LoadPortParam> GetParam()
        {
            throw new NotImplementedException();
        }

        public Task<LoadPortStatus> GetStatus()
        {
            throw new NotImplementedException();
        }

        public Task Home()
        {
            throw new NotImplementedException();
        }

        public void Initial()
        {
            throw new NotImplementedException();
        }

        public Task Load()
        {
            throw new NotImplementedException();
        }

        public Task SetParam(LoadPortParam loadPortParam)
        {
            throw new NotImplementedException();
        }

        Task ILoadPort.AlarmReset()
        {
            throw new NotImplementedException();
        }
    }
}
