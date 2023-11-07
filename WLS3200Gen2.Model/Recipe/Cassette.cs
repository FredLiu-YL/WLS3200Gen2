using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Data
{
    public class Cassette
    {

        public string LotID { get; set; }

        public Wafer[] Wafers { get; set; }
    
    }

}
