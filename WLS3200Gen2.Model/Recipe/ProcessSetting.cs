﻿using System;
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
        public bool RemoteDefectPoint { get; set; }
        /// <summary>
        /// 晶圓檢查站點 //陣列0  是第25片 放在cassette最上面
        /// </summary>
        public ProcessStation[] ProcessStation { get; set; }
        /// <summary>
        ///  讀取WAFER ID
        /// </summary>
        public bool IsReadWaferID { get; set; }

        public ProcessSetting Copy()
        {
            // 复制 ProcessStation 数组
            ProcessStation[] copiedProcessStation = new ProcessStation[this.ProcessStation.Length];
            for (int i = 0; i < this.ProcessStation.Length; i++)
            {
                // 对于每个 ProcessStation，创建一个新的实例并复制属性
                copiedProcessStation[i] = new ProcessStation(this.ProcessStation[i].CassetteIndex)
                {
                    // 假设 ProcessStation 有一些属性，你需要将它们逐个复制到新实例中
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
                IsReadWaferID = this.IsReadWaferID
            };
        }

    }


    /* public class ProcessStationAssign : INotifyPropertyChanged
     {

         private bool isMacroBack;
         private bool isMacroTop;
         private bool isMicro;

         /// <summary>
         /// 晶背檢查
         /// </summary>
         public bool IsMacroBack { get => isMacroBack; set => SetValue(ref isMacroBack, value); }
         /// <summary>
         /// 晶面檢查
         /// </summary>
         public bool IsMacroTop { get => isMacroTop; set => SetValue(ref isMacroTop, value); }
         /// <summary>
         /// 顯微鏡檢查
         /// </summary>
         public bool IsMicro { get => isMicro; set => SetValue(ref isMicro, value); }


         public event PropertyChangedEventHandler PropertyChanged;
         protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
         {
             if (EqualityComparer<T>.Default.Equals(field, value)) return;
             T oldValue = field;
             field = value;
             OnPropertyChanged(propertyName, oldValue, value);
         }

         protected virtual void OnPropertyChanged<T>(string name, T oldValue, T newValue)
         {
             // oldValue 和 newValue 目前沒有用到，代爾後需要再實作。
             PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
         }


     }*/
}
