using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using WLS3200Gen2.Model;

namespace WLS3200Gen2
{
    public partial class MainViewModel
    {


       
        private string version;

 
        public string Version { get => version; set => SetValue(ref version, value); }



    }
}
