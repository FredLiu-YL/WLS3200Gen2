﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WLS3200Gen2.Model.Recipe
{

    public class BincodeInfo : INotifyPropertyChanged
    {
        private string code;
        private string describe;
        private Brush color;


        public string Code { get => code; set => SetValue(ref code, value); }

        public string Describe { get => describe; set => SetValue(ref describe, value); }

        public Brush Color { get => color; set => SetValue(ref color, value); }

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
}
