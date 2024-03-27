using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YuanliCore.Data
{
    public class Wafer
    {
        /// <summary>
        /// 流程中使用 傳遞Wafer資訊
        /// </summary>
        /// <param name="cassetteIndex"></param>
        public Wafer(int cassetteIndex)
        {
            CassetteIndex = cassetteIndex;
            ProcessStatus = new ProcessStation(CassetteIndex);
        }


        /// <summary>
        /// 在卡匣內第幾格 必填!  從1開始
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


        public ProcessStation ProcessStatus { get; set; }
    }


    public class ProcessStation : INotifyPropertyChanged
    {
        public ProcessStation(int cassetteIndex)
        {
            CassetteIndex = cassetteIndex;
            MacroTop = WaferProcessStatus.NotSelect;
            MacroBack = WaferProcessStatus.NotSelect;
            WaferID = WaferProcessStatus.NotSelect;
            Micro = WaferProcessStatus.NotSelect;
        }
        private int cassetteIndex;

        private WaferProcessStatus macroTop, macroBack, waferID, micro, totally;

        /// <summary>
        /// 巨觀檢查站晶面 狀態
        /// </summary>
        public WaferProcessStatus MacroTop { get => macroTop; set => SetValue(ref macroTop, value); }
        /// <summary>
        /// 巨觀檢查站晶背後 狀態
        /// </summary>
        public WaferProcessStatus MacroBack { get => macroBack; set => SetValue(ref macroBack, value); }
        /// <summary>
        /// 讀取WAFERID站
        /// </summary>
        public WaferProcessStatus WaferID { get => waferID; set => SetValue(ref waferID, value); }
        /// <summary>
        /// 微觀檢查站
        /// </summary>
        public WaferProcessStatus Micro { get => micro; set => SetValue(ref micro, value); }

        /// <summary>
        /// 單純紀錄整片的狀態
        /// </summary>
        public WaferProcessStatus Totally { get => totally; set => SetValue(ref totally, value); }
        /// <summary>
        /// 在卡匣內第幾格 必填!  從1開始
        /// </summary>
        public int CassetteIndex { get => cassetteIndex; set => SetValue(ref cassetteIndex, value); }




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
    }

    public enum WaferProcessStatus
    {
        None,//空的
        NotSelect,//不可用
        Select,//可用
        InProgress,//流程進行中
        Reject,  // NG
        Pass,
        Complate//完成
    }
}
