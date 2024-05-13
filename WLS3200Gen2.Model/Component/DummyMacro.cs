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

        public double InnerPitchXPosition => 0;
        public double InnerRollYPosition => 0;
        public double InnerYawTPosition => 0;
        public double OuterRollYPosition => 0;

        public double InnerRingPitchXPositionPEL { get; set; } = 0;
        public double InnerRingPitchXPositionNEL { get; set; } = 0;
        public double InnerRingYawTPositionPEL { get; set; } = 0;
        public double InnerRingYawTPositionNEL { get; set; } = 0;
        public double InnerRingRollYPositionPEL { get; set; } = 0;
        public double InnerRingRollYPositionNEL { get; set; } = 0;
        public double OuterRingRollYPositionPEL { get; set; } = 0;
        public double OuterRingRollYPositionNEL { get; set; } = 0;

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

        public Task InnerRingPitchXMoveToAsync(double pos)
        {
            return Task.CompletedTask;
        }

        public void InnerRingPitchX_Move(bool isForward)
        {

        }

        public void InnerRingPitchX_Stop()
        {

        }

        public Task InnerRingRollYMoveToAsync(double pos)
        {
            return Task.CompletedTask;
        }

        public void InnerRingRollY_Move(bool isForward)
        {

        }

        public void InnerRingRollY_Stop()
        {

        }

        public Task InnerRingYawTMoveToAsync(double pos)
        {
            return Task.CompletedTask;
        }

        public void InnerRingYawT_Move(bool isForwards)
        {

        }

        public void InnerRingYawT_Stop()
        {

        }

        public Task OuterRingRollYMoveToAsync(double pos)
        {
            return Task.CompletedTask;
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
