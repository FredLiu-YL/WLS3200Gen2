using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class LampRS232 : ILampControl
    {
        public int LightValue => throw new NotImplementedException();
        public Task ChangeLightAsync(int LigntValue)
        {
            throw new NotImplementedException();
        }
        public void Initial()
        {
            throw new NotImplementedException();
        }
        public void Close()
        {
            throw new NotImplementedException();
        }}
}
