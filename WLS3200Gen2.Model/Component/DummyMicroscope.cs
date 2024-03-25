using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Model.Interface;

namespace WLS3200Gen2.Model.Component
{
    public class DummyMicroscope : IMicroscope
    {
        public int LightValue { get; set; }
        public int ApertureValue { get; set; }

        public double Position { get; private set; }

        public int NEL { get; set; }
        public int PEL { get; set; }

        public double AberationPosition { get; private set; }

        public int AFNEL { get; set; }
        public int AFPEL { get; set; }
        public int TimeOutRetryCount { get; set; } = 1;

        public int LensIndex { get; set; }

        public int CubeIndex { get; set; }
        public int Filter1Index { get; set; }
        public int Filter2Index { get; set; }
        public int Filter3Index { get; set; }

        public bool IsAutoFocusOk { get; private set; } = true;

        public bool IsAutoFocusTrace { get; private set; } = true;

        public Task AberrationMoveAsync(double distance)
        {
            return Task.CompletedTask;
        }

        public Task AberrationMoveToAsync(double position)
        {
            AberationPosition = position;
            return Task.CompletedTask;
        }

        public void AFOff()
        {
            IsAutoFocusOk = false;
            IsAutoFocusTrace = false;
        }

        public Task AFOneShotAsync()
        {
            IsAutoFocusOk = true;
            IsAutoFocusTrace = false;
            return Task.CompletedTask;
        }

        public void AFTrace()
        {
            IsAutoFocusOk = true;
            IsAutoFocusTrace = true;
        }

        public Task ChangeApertureAsync(int ApertureValue)
        {
            this.ApertureValue = ApertureValue;
            return Task.CompletedTask;
        }

        public Task ChangeCubeAsync(int idx)
        {
            CubeIndex = idx;
            return Task.CompletedTask;
        }

        public Task ChangeFilterAsync(int wheelIdx, int idx)
        {
            return Task.CompletedTask;
        }

        public Task ChangeLensAsync(int idx)
        {
            LensIndex = idx;
            return Task.CompletedTask;
        }

        public Task ChangeLightAsync(int LigntValue)
        {
            LightValue = LigntValue;
            return Task.CompletedTask;
        }

        public Task ChangeLightSpreadAsync(int idx)
        {
            return Task.CompletedTask;
        }

        public Task<double> GetAberationPosition()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAFNEL()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAFPEL()
        {
            return Task.FromResult<int>(0);
        }

        public Task<double> GetZNEL()
        {
            return Task.FromResult<double>(0);
        }

        public Task<double> GetZPEL()
        {
            return Task.FromResult<double>(0);
        }

        public Task<double> GetZPosition()
        {
            return Task.FromResult<double>(0);
        }

        public Task HomeAsync()
        {
            return Task.CompletedTask;
        }

        public void Initial()
        {

        }


        public Task SetAFNEL(int position)
        {
            return Task.CompletedTask;
        }

        public Task SetAFPEL(int position)
        {
            return Task.CompletedTask;
        }

        public void SetSearchRange(double FirstZPos, double Range)
        {
        }

        public Task SetZNEL(int position)
        {
            return Task.CompletedTask;
        }

        public Task SetZPEL(int position)
        {
            return Task.CompletedTask;
        }

        public Task MoveAsync(double distance)
        {
            return Task.CompletedTask;
        }

        public Task MoveToAsync(double position)
        {
            Position = position;
            return Task.CompletedTask;
        }

        public Task ChangeFilter1Async(int idx)
        {
            Filter1Index = idx;
            return Task.CompletedTask;
        }

        public Task ChangeFilter2Async(int idx)
        {
            Filter2Index = idx;
            return Task.CompletedTask;
        }

        public Task ChangeFilter3Async(int idx)
        {
            Filter3Index = idx;
            return Task.CompletedTask;
        }
    }
}
