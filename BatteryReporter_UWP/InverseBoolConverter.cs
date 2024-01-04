using System;
using Windows.UI.Xaml.Data;

namespace BatteryReporter_UWP
{
    internal class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var _value = value as bool?;
            return !_value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var _value = value as bool?;
            return !_value;
        }
    }
}