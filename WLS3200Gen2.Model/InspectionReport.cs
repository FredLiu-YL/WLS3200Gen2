using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Recipe;
using YuanliCore.Data;

namespace WLS3200Gen2.Model
{
    public class InspectionReport
    {
        public Wafer WaferResult { get; set; }
        public WaferMapping WaferMapping { get; set; }
    }
}
