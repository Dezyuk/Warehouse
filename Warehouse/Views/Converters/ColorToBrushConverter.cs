using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Warehouse.Views.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var colorStr = value as string;
            if (string.IsNullOrEmpty(colorStr))
                return Brushes.Transparent;

            try
            {
                var brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorStr));
                return brush ?? Brushes.Transparent;
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
