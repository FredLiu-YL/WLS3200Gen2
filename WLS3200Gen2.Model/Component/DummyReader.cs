using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YuanliCore.Model.Interface.Component;

namespace WLS3200Gen2.Model.Component
{
    public class DummyReader : IReader
    {
        public Bitmap Image { get; private set; }

        public double Score { get; private set; }

        public void Close()
        {

        }

        public void Initial()
        {

        }

        public Task<string> ReadAsync()
        {
            Score = 0;
            return (Task<string>)Task.CompletedTask;
        }

        public void SetParam(int paramID)
        {

        }
    }
}
