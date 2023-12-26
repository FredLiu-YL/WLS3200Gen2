using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Data
{
    public class Wafer
    {
        public Wafer(int index)
        {
            CassetteIndex = index;
        }


        /// <summary>
        /// 在卡匣內第幾格 必填!
        /// </summary>
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


        public WaferProcessStatus ProcessStatus { get; set; }
    }

    public enum WaferProcessStatus
    {
        Usable,
        InProgress,
        Complate

    }
}
