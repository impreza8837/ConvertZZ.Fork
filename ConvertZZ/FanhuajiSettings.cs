﻿using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

using ConvertZZ.Models;

using Fanhuaji_API.Enum;
using Fanhuaji_API.Models;

namespace ConvertZZ {
    public class FanhuajiSettings : Config, INotifyPropertyChanged {
        [ColumnName(Visible: false)]
        public int Converter_T_to_S_Index {
            get => (int)Converter_T_to_S;
            set => Converter_T_to_S = (Enum_Converter)value;
        }

        [ColumnName(Visible: false)]
        public int Converter_S_to_T_Index {
            get => (int)Converter_S_to_T;
            set => Converter_S_to_T = (Enum_Converter)value;
        }

        [ColumnName(Visible: false)]
        public Enum_Converter Converter_T_to_S { get; set; } = Enum_Converter.Simplified;

        [ColumnName(Visible: false)]
        public Enum_Converter Converter_S_to_T { get; set; } = Enum_Converter.Taiwan;

        [ColumnName(Visible: false)]
        public int TranslateTabsToSpaces_Index {
            get => TranslateTabsToSpaces + 1;
            set => TranslateTabsToSpaces = value - 1;
        }

        [ColumnName(Visible: false)]
        public int JpStyleConversionStrategy_Index {
            get => (int)JpStyleConversionStrategy;
            set => JpStyleConversionStrategy = (Enum_jpConversionStrategy)value;
        }

        [ColumnName(Visible: false)]
        public int JpTextConversionStrategy_Index {
            get => (int)JpTextConversionStrategy;
            set => JpTextConversionStrategy = (Enum_jpConversionStrategy)value;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify() {
            PropertyInfo[] properties = typeof(FanhuajiSettings).GetProperties();
            foreach (PropertyInfo propertyInfo in properties) {
                OnPropertyChanged(propertyInfo.Name);
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
