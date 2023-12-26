using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyAligner : IAligner
    {
        public string DeviceStatus => throw new NotImplementedException();

        public string ErrorCode => throw new NotImplementedException();

        public string NotchStatus => throw new NotImplementedException();

        public bool IsWafer => throw new NotImplementedException();

        public bool IsOrg => throw new NotImplementedException();

        public bool IsVaccum => throw new NotImplementedException();

        public void AlarmReset()
        {
            throw new NotImplementedException();
        }

        public void GetStatus()
        {
            throw new NotImplementedException();
        }

        public void Home()
        {
             
        }

        public void Initial()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public void Run(double degree)
        {
            throw new NotImplementedException();
        }

        public void VaccumOff()
        {
            throw new NotImplementedException();
        }

        public void VaccumOn()
        {
            throw new NotImplementedException();
        }
    }
}
