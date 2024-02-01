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

        public void Close()
        {
            throw new NotImplementedException();
        }

        public AlignerStatus GetStatus()
        {
            throw new NotImplementedException();
        }

        public void Home()
        {
        }

        public void Initial()
        {
        }

        public void Run(double degree)
        {
            throw new NotImplementedException();
        }

        public void Vaccum(bool IsOn)
        {
            throw new NotImplementedException();
        }
    }
}
