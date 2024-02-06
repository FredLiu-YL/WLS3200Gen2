using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Module;
using YuanliCore.Interface;

namespace WLS3200Gen2.Model.Recipe
{
    public class ProcessSetting
    {

        public InchType Inch { get; set; }
        public bool AutoSave { get; set; }

        public bool RemoteDefectPoint { get; set; }
        /// <summary>
        /// 晶面檢查
        /// </summary>
        public bool IsMacroTop { get; set; }
        /// <summary>
        /// 晶背檢查
        /// </summary>
        public bool IsMacroBack { get; set; }
        /// <summary>
        /// 顯微鏡檢查
        /// </summary>
        public bool IsMicro { get; set; }

        /// <summary>
        ///  讀取WAFER ID
        /// </summary>
        public bool IsReadWaferID { get; set; }
    }
}
