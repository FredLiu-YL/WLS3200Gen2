using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyMacro : IMacro
    {
        public void FixWafer()
        {
            throw new NotImplementedException();
        }

        public void Flip(double pos)
        {
            throw new NotImplementedException();
        }

        public void Home()
        {
             
        }

        public void Initial()
        {
       
        }

        public void ReleaseWafer()
        {
            throw new NotImplementedException();
        }

        public void Rotate(double pos)
        {
            throw new NotImplementedException();
        }

        public void Vertical(double pos)
        {
            throw new NotImplementedException();
        }
    }
}
