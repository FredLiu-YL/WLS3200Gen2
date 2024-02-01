using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class ArtificialLoadPort : ILoadPort
    {
        private bool?[] slot;
        public ArtificialLoadPort()
        {
            slot = new bool?[] { true };

        }
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

        public void Home()
        {
            throw new NotImplementedException();
        }

        public void Initial()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void SetParam(LoadPortParam loadPortParam)
        {
            throw new NotImplementedException();
        }

        LoadPortParam ILoadPort.GetParam()
        {
            throw new NotImplementedException();
        }

        LoadPortStatus ILoadPort.GetStatus()
        {
            throw new NotImplementedException();
        }
    }
}
