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
        private bool[] slot;

        public bool IsMapping => isMapping;

        public bool[] Slot => slot;

        public void Home()
        {

        }

        public void Initial()
        {

        }

        public void SlotMapping()
        {
            try
            {


                List<bool> slotList = new List<bool>();

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
    }
}
