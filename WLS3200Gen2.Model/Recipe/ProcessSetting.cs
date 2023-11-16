using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Module;

namespace WLS3200Gen2.Model.Recipe
{
    public class ProcessSetting
    {

        public InchType Inch { get; set; }
        public bool AutoSave { get; set; }

        public bool RemoteDefectPoint { get; set; }

    }
}
