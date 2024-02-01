using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyMacro : IMacro
    {
        public bool IsCanMoveAllHome => throw new NotImplementedException();

        public bool IsInnerCanMoveStartPos => throw new NotImplementedException();

        public bool IsInnerUsing => throw new NotImplementedException();

        public bool IsOuterCanMoveStartPos => throw new NotImplementedException();

        public bool IsOuterUsing => throw new NotImplementedException();

        public void FixWafer()
        {
            throw new NotImplementedException();
        }

        public void GoInnerRingCheckPos()
        {
            throw new NotImplementedException();
        }

        public void GoOuterRingCheckPos()
        {
            throw new NotImplementedException();
        }

        public void HomeAllRing()
        {
            throw new NotImplementedException();
        }

        public void HomeInnerRing()
        {
            throw new NotImplementedException();
        }

        public void HomeOuterRing()
        {
            throw new NotImplementedException();
        }

        public void Initial()
        {
            throw new NotImplementedException();
        }

        public void InnerRingPitchX_Move(bool isForward)
        {
            throw new NotImplementedException();
        }

        public void InnerRingPitchX_Stop()
        {
            throw new NotImplementedException();
        }

        public void InnerRingRollY_Move(bool isForward)
        {
            throw new NotImplementedException();
        }

        public void InnerRingRollY_Stop()
        {
            throw new NotImplementedException();
        }

        public void InnerRingYawT_Move(bool isForwards)
        {
            throw new NotImplementedException();
        }

        public void InnerRingYawT_Stop()
        {
            throw new NotImplementedException();
        }

        public void OuterRingRollY_Move(bool isForwards)
        {
            throw new NotImplementedException();
        }

        public void OuterRingRollY_Stop()
        {
            throw new NotImplementedException();
        }

        public void ReleaseWafer()
        {
            throw new NotImplementedException();
        }
    }
}
