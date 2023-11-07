using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class ArtificialLoadPort : ILoadPort
    {
        private bool isMapping = true;
        private bool?[] slot;

        public ArtificialLoadPort()
        {
            slot = new bool?[] { true };

        }

        public bool IsMapping => isMapping;

        public bool?[] Slot => slot;



        public void Home()
        {

        }

        public void Initial()
        {

        }

        public void SlotMapping()
        {

        }
    }
}
