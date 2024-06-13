using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WLS3200Gen2.Model.Module;
using YuanliCore.Data;
using YuanliCore.Interface;

namespace WLS3200Gen2.Model.Recipe
{
    public class ProcessSetting : AbstractRecipe
    {
        /// <summary>
        /// Wafer尺寸
        /// </summary>
        public InchType Inch { get; set; } = InchType.Inch12;
        /// <summary>
        /// 使用 1號 Loadport
        /// </summary>
        public bool IsLoadport1 { get; set; }
        /// <summary>
        /// 使用 2號 Loadport
        /// </summary>
        public bool IsLoadport2 { get; set; }
        /// <summary>
        /// 是否跑自動模式
        /// </summary>
        public bool IsAutoSave { get; set; }
        /// <summary>
        /// 是否要自動對焦
        /// </summary>
        public bool IsAutoFocus { get; set; } = true;
        /// <summary>
        /// 是否要測試模式
        /// </summary>
        public bool IsTestRun { get; set; } = false;
        /// <summary>
        /// 是否要角度退片
        /// </summary>
        public bool IsDegreeUnLoad { get; set; } = false;
        /// <summary>
        /// 是否要二次翻背
        /// </summary>
        public bool IsSecondFlip { get; set; } = false;
        /// <summary>
        /// 
        /// </summary>
        public bool RemoteDefectPoint { get; set; }
        /// <summary>
        /// 晶圓檢查站點 //陣列0  是第25片 放在cassette最上面
        /// </summary>
        public ProcessStation[] ProcessStation { get; set; }
        /// <summary>
        /// 二次翻背位置
        /// </summary>
        public int SecondFlipPos { get; set; }
        /// <summary>
        /// 出貨角度
        /// </summary>
        public Degree DegreeUnLoad { get; set; }
        /// <summary>
        /// 金面不旋轉、一直正轉、一直反轉
        /// </summary>
        public TopContinueRotate TopContinueRotate { get; set; }

        public ProcessSetting Copy()
        {
            ProcessStation[] copiedProcessStation = new ProcessStation[this.ProcessStation.Length];
            for (int i = 0; i < this.ProcessStation.Length; i++)
            {
                copiedProcessStation[i] = new ProcessStation(this.ProcessStation[i].CassetteIndex)
                {
                    MacroTop = this.ProcessStation[i].MacroTop,
                    MacroBack = this.ProcessStation[i].MacroBack,
                    WaferID = this.ProcessStation[i].WaferID,
                    Micro = this.ProcessStation[i].Micro,
                    Totally = this.ProcessStation[i].Totally,
                    CassetteIndex = this.ProcessStation[i].CassetteIndex,
                };
            }
            return new ProcessSetting
            {
                Inch = this.Inch,
                IsLoadport1 = this.IsLoadport1,
                IsLoadport2 = this.IsLoadport2,
                IsAutoSave = this.IsAutoSave,
                IsAutoFocus = this.IsAutoFocus,
                IsTestRun = this.IsTestRun,
                RemoteDefectPoint = this.RemoteDefectPoint,
                ProcessStation = copiedProcessStation,
            };
        }

    }
    public enum Degree
    {
        Degree0,
        Degree90,
        Degree180,
        Degree270,
    }
    public enum TopContinueRotate
    {
        No,
        Forward,
        Backward,
    }

}
