using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfAppSchoolDataBase.Helper
{
    // High-performance bidirectional pipeline converting DateOnly fields for WPF DatePickers
    public class DateOnlyToDateTimeConverter : IValueConverter
    {
        // Converts DateOnly (Database) to DateTime (UI View)
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
            {
                return dateOnly.ToDateTime(TimeOnly.MinValue);
            }
            return null;
        }

        // Converts DateTime (UI View) back to DateOnly (Database)
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return DateOnly.FromDateTime(dateTime);
            }
            return null;
        }
    }
}
