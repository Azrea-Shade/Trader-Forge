using Services;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Presentation.Converters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            if (value is double d) return d.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
            if (value is float f) return ((double)f).ToString("C2", CultureInfo.GetCultureInfo("en-US"));
            if (value is decimal m) return m.ToString("C2", CultureInfo.GetCultureInfo("en-US"));
            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (value ?? "").ToString()!.Trim();
            if (string.IsNullOrEmpty(s)) return null!;
            if (double.TryParse(
                  s,
                  NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
                | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint
                | NumberStyles.AllowThousands | NumberStyles.AllowCurrencySymbol,
                  CultureInfo.GetCultureInfo("en-US"),
                  out var d))
                return d;
            return null!;
        }
    }
}
