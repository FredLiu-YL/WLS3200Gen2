﻿using System;
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

        public int Position => 0;

        public int NEL { get; set; }
        public int PEL { get; set; }

        public int AberationPosition => 0;

        public int AFNEL { get; set; }
        public int AFPEL { get; set; }
        public int TimeOutRetryCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int LensIndex { get; set; }

        public int CubeIndex { get; set; }
        public int Filter1Index { get; set; }
        public int Filter2Index { get; set; }
        public int Filter3Index { get; set; }

        public bool IsAutoFocusOk { get; private set; } = true;

        public bool IsAutoFocusTrace { get; private set; } = true;

        public Task AberrationMoveAsync(double distance)
        {
            throw new NotImplementedException();
        }

        public Task AberrationMoveToAsync(double position)
        {
            throw new NotImplementedException();
        }

        public void AFOff()
        {
            throw new NotImplementedException();
        }

        public Task AFOneShotAsync()
        {
            throw new NotImplementedException();
        }

        public void AFTrace()
        {
            throw new NotImplementedException();
        }

        public Task ChangeApertureAsync(int ApertureValue)
        {
            throw new NotImplementedException();
        }

        public Task ChangeCubeAsync(int idx)
        {
            throw new NotImplementedException();
        }

        public Task ChangeFilterAsync(int wheelIdx, int idx)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLensAsync(int idx)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLightAsync(int LigntValue)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLightSpreadAsync(int idx)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public Task SetAFPEL(int position)
        {
            throw new NotImplementedException();
        }

        public void SetSearchRange(double FirstZPos, double Range)
        {
            throw new NotImplementedException();
        }

        public Task SetZNEL(int position)
        {
            throw new NotImplementedException();
        }

        public Task SetZPEL(int position)
        {
            throw new NotImplementedException();
        }

        public Task MoveAsync(double distance)
        {
            throw new NotImplementedException();
        }

        public Task MoveToAsync(double position)
        {
            throw new NotImplementedException();
        }

        public Task ChangeFilter1Async(int idx)
        {
            throw new NotImplementedException();
        }

        public Task ChangeFilter2Async(int idx)
        {
            throw new NotImplementedException();
        }

        public Task ChangeFilter3Async(int idx)
        {
            throw new NotImplementedException();
        }
    }
}
