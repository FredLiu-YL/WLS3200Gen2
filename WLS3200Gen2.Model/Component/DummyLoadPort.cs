using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyLoadPort : ILoadPort
    {
        private bool?[] slot ;
        public bool?[] Slot => slot; 

        public Task AlarmReset()
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
            return Task.CompletedTask;
        }

        public void Initial()
        {

        }

        public Task Load()
        {

            slot = new bool?[] {true, true, null, null, null, null, true, true, true, true,
                                            true, true, true, true, true, true, true, true, true, true,
                                             null, null, null, null, true};
            return Task.CompletedTask;
        }

        public Task SetParam(LoadPortParam loadPortParam)
        {
            throw new NotImplementedException();
        }

        
        
    }
}
