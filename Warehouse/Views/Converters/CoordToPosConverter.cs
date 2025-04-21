using System;
using System.Globalization;
using System.Windows.Data;

namespace Warehouse.Views.Converters
{
    /// <summary>
    /// Преобразует координату (int) в положение на Canvas.
    /// </summary>
    public class CoordToPosConverter : IValueConverter
    {
        private const double Size = 50.0;
        private const double Pad = 5.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is int c ? c * (Size + Pad) : 0.0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
