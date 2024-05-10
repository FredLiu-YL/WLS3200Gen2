﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WLS3200Gen2.Model.Component
{
    public class DummyLampControl : ILampControl
    {
        public int LightValue { get; private set; } = 1;

        public Task ChangeLightAsync(int LightValue)
        {
            this.LightValue = LightValue;
            return Task.CompletedTask;
        }

        public void Close()
        {

        }

        public void Initial()
        {

        }
    }
}
