using System;
using System.Globalization;
using Avalonia.Data.Converters;
using System;
using Avalonia.Data;

namespace Babana.Views;

public class MsecConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        var value2 = 0f;
        if (value is string || value is float) {
            value2 = System.Convert.ToSingle(value);
            if (value2 >= 1000) {
                return (value2 / 1000).ToString("0.00") + " s";
            }

            return value2 <= 0 ? "--" : value2.ToString("0.00") + " ms";
        }

        // converter used for the wrong type
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is string strValue) {
            if (strValue.EndsWith(" ms")) {
                strValue = strValue.Replace(" ms", "");
                return targetType == typeof(string) ? strValue : System.Convert.ToSingle(strValue);
            }

            if (strValue.EndsWith(" s")) {
                strValue = strValue.Replace(" s", "");
                return targetType == typeof(string) ? strValue : System.Convert.ToSingle(strValue) * 1000;
            }
        }

        return targetType == typeof(string) ? "-1" : -1f;
    }
}
