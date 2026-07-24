using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfAppSchoolDataBase.Converters
{
    // Inverse boolean for IsEnabled / Visibility toggling
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return !b;
            return Binding.DoNothing;
        }
    }

    // Handles WPF TextBox string to DateOnly conversions safely
    public class DateOnlyToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateOnly date)
            {
                return date.ToString("yyyy-MM-dd");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && DateOnly.TryParse(str, out DateOnly parsedDate))
            {
                return parsedDate;
            }
            return DateOnly.FromDateTime(DateTime.Now);
        }
    }
}