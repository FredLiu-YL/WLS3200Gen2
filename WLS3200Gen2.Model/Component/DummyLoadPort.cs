using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyLoadPort : ILoadPort
    {
        private bool isMapping;
        private bool?[] slot;

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

        public LoadPortParam GetParam()
        {
            throw new NotImplementedException();
        }

        public LoadPortStatus GetStatus()
        {
            throw new NotImplementedException();
        }

        public async Task Home()
        {
           
        }

        public void Initial()
        {
     
        }

        public async Task Load()
        {
            
        }

        public Task SetParam(LoadPortParam loadPortParam)
        {
            throw new NotImplementedException();
        }

        public void SlotMapping()
        {
            try
            {


                List<bool?> slotList = new List<bool?>();

                for (int i = 0; i < 20; i++)
                    slotList.Add(true);

                slot = slotList.ToArray();
                isMapping = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        Task ILoadPort.AlarmReset()
        {
            throw new NotImplementedException();
        }

        Task<LoadPortParam> ILoadPort.GetParam()
        {
            throw new NotImplementedException();
        }

        Task<LoadPortStatus> ILoadPort.GetStatus()
        {
            throw new NotImplementedException();
        }
    }
}
