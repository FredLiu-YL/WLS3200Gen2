using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyLoadPort : ILoadPort
    {
        private bool?[] slot;
        public bool?[] Slot => slot;

        public int TimeOutRetryCount { get; set; } = 1;

        public bool IsDoorOpen { get; private set; } = false;

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
            return Task.Run(() => new LoadPortParam());

        }

        public Task<LoadPortStatus> GetStatus()
        {
            return Task.Run(() => new LoadPortStatus());
        }

        public Task Home()
        {
            IsDoorOpen = false;
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
            IsDoorOpen = true;
            return Task.CompletedTask;
        }

        public Task SetParam(LoadPortParam loadPortParam)
        {
            throw new NotImplementedException();
        }



    }
}
