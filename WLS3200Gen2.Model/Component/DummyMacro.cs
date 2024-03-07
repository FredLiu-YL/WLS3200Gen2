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

        public bool IsLockOK => false;

        public double Position => 0;

        public void FixWafer()
        {

        }

        public Task GoInnerRingCheckPos()
        {
            return Task.CompletedTask;
        }

        public Task GoOuterRingCheckPos()
        {
            return Task.CompletedTask;
        }

        public Task Home()
        {
            return Task.CompletedTask;
        }

        public Task HomeInnerRing()
        {
            return Task.CompletedTask;
        }

        public Task HomeOuterRing()
        {
            return Task.CompletedTask;
        }

        public void Initial()
        {

        }

        public void InnerRingPitchX_Move(bool isForward)
        {

        }

        public void InnerRingPitchX_Stop()
        {

        }

        public void InnerRingRollY_Move(bool isForward)
        {

        }

        public void InnerRingRollY_Stop()
        {

        }

        public void InnerRingYawT_Move(bool isForwards)
        {

        }

        public void InnerRingYawT_Stop()
        {

        }

        public void OuterRingRollY_Move(bool isForwards)
        {

        }

        public void OuterRingRollY_Stop()
        {

        }

        public void ReleaseWafer()
        {

        }
    }
}
