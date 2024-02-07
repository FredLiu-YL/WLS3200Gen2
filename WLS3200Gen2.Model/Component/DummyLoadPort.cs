using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyLoadPort : ILoadPort
    {
        public bool?[] Slot => throw new NotImplementedException();

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


        public void Initial()
        {

        }

        public Task Load()
        {
            throw new NotImplementedException();
        }

        public Task SetParam(LoadPortParam loadPortParam)
        {
            throw new NotImplementedException();
        }

        public void SlotMapping()
        {
            //try
            //{


            //    List<bool?> slotList = new List<bool?>();

            //    for (int i = 0; i < 20; i++)
            //        slotList.Add(true);

            //    slot = slotList.ToArray();
            //    isMapping = true;
            //}
            //catch (Exception ex)
            //{

            //    throw ex;
            //}
        }

        Task ILoadPort.AlarmReset()
        {
            throw new NotImplementedException();
        }

        Task ILoadPort.Home()
        {
            Task task = Task.CompletedTask;
            return task;
        }
    }
}
