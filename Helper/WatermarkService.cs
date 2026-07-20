using System.Windows;

namespace WpfAppSchoolDataBase.Helper
{
    internal class WatermarkService
    {
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkService), new PropertyMetadata(default(string)));

        public static void SetWatermark(DependencyObject element, string value) => element.SetValue(WatermarkProperty, value);
        public static string GetWatermark(DependencyObject element) => (string)element.GetValue(WatermarkProperty);

    }
}
