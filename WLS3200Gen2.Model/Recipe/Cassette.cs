using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Data
{
    public class Cassette
    {
        /// <summary>
        /// 
        /// </summary>
        public string LotID { get; set; }
        /// <summary>
        /// Wafer資訊以Cassette格數計算 
        /// 例:其他空的 
        /// </summary>
        public Wafer[] Wafers { get; set; }
    

    }

}
