using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Babana.Views;

public class RoundingConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        var value2 = 0f;
        if (value is string || value is float) {
            value2 = System.Convert.ToSingle(value);
            return value2 <= 0 ? "--" : value2.ToString("0.00");
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is string strValue) {
            return targetType == typeof(string) ? strValue : System.Convert.ToSingle(strValue);
        }

        return targetType == typeof(string) ? "-1" : -1f;
    }
}