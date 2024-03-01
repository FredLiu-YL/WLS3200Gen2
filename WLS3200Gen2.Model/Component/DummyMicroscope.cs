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
        public int LightValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int ApertureValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task AberrationMoveCommand(double distance)
        {
            throw new NotImplementedException();
        }

        public Task AberrationMoveToCommand(double position)
        {
            throw new NotImplementedException();
        }

        public Task AF_Off()
        {
            throw new NotImplementedException();
        }

        public Task AF_OneShot()
        {
            throw new NotImplementedException();
        }

        public Task AF_Trace()
        {
            throw new NotImplementedException();
        }

        public Task ChangeAperture(int ApertureValue)
        {
            throw new NotImplementedException();
        }

        public Task ChangeCube(int idx)
        {
            throw new NotImplementedException();
        }

        public Task ChangeFilter(int wheelIdx, int idx)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLens(int idx)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLight(int LigntValue)
        {
            throw new NotImplementedException();
        }

        public Task ChangeLightSpread(int idx)
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
            throw new NotImplementedException();
        }

        public Task<double> GetZNEL()
        {
            throw new NotImplementedException();
        }

        public Task<double> GetZPEL()
        {
            throw new NotImplementedException();
        }

        public Task<double> GetZPosition()
        {
            throw new NotImplementedException();
        }

        public Task Home()
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

        public Task SetSearchRange(double FirstZPos, double Range)
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

        public Task ZMoveCommand(double distance)
        {
            throw new NotImplementedException();
        }

        public Task ZMoveToCommand(double position)
        {
            throw new NotImplementedException();
        }
    }
}
