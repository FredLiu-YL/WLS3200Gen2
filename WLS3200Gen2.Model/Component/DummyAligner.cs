using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyAligner : IAligner
    {
        public void AlarmReset()
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
