using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyRobot : IEFEMRobot
    {
        public void Armcatch(ArmStation armPosition)
        {
            throw new NotImplementedException();
        }

        public void ArmLiftup()
        {
            throw new NotImplementedException();
        }

        public void ArmPutdown()
        {
            throw new NotImplementedException();
        }

        public void ArmToRetract(ArmStation armPosition)
        {
            throw new NotImplementedException();
        }

        public void ArmToStandby()
        {
            throw new NotImplementedException();
        }

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

        public void PutBackWafer(ArmStation armPosition)
        {
            throw new NotImplementedException();
        }

        public void PutBackWaferCassette(int layer)
        {
            throw new NotImplementedException();
        }

        public void TakeWafer(ArmStation armPosition)
        {
            throw new NotImplementedException();
        }

        public void TakeWaferCassette(int layer)
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
