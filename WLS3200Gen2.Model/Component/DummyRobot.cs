using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyRobot : IRobot
    {
        public void Home()
        {
             
        }

        public void Initial()
        {
           
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void MoveToAliner()
        {
            throw new NotImplementedException();
        }

        public void MoveToMacro()
        {
            throw new NotImplementedException();
        }

        public void UnLoad()
        {
            throw new NotImplementedException();
        }

        public void VacuumOff()
        {
            throw new NotImplementedException();
        }

        public void VacuumOn()
        {
            throw new NotImplementedException();
        }
    }
}
