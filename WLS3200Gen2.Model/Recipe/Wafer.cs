using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Data
{
    public class Wafer
    {
       
        public int CassetteIndex { get; set; }

        /// <summary>
        /// Wafer 名稱
        /// </summary>
        public string WaferID { get; set; }
        /// <summary>
        /// 英寸
        /// </summary>
        public int Inch { get; set; }

        public Die[] Dies { get; set; }



    }
}
