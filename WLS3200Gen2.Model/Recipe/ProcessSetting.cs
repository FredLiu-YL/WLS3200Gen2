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
    public class ProcessSetting
    {

        public InchType Inch { get; set; }

        /// <summary>
        /// 使用 1號 Loadport
        /// </summary>
        public bool IsLoadport1 { get; set; }
        /// <summary>
        /// 使用 2號 Loadport
        /// </summary>
        public bool IsLoadport2 { get; set; }
        public bool AutoSave { get; set; }

        public bool RemoteDefectPoint { get; set; }

        /// <summary>
        /// 晶圓檢查站點
        /// </summary>
        public ProcessStation[] ProcessStation { get; set; }

        /// <summary>
        ///  讀取WAFER ID
        /// </summary>
        public bool IsReadWaferID { get; set; }



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
