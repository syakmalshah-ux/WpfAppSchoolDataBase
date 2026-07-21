using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfAppSchoolDataBase.Helper
{
    public class HorizontalAlignmentToTextAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is HorizontalAlignment align)
            {
                return align switch
                {
                    HorizontalAlignment.Right => TextAlignment.Right,
                    HorizontalAlignment.Center => TextAlignment.Center,
                    _ => TextAlignment.Left
                };
            }
            return TextAlignment.Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return HorizontalAlignment.Left;
        }
    }
}
