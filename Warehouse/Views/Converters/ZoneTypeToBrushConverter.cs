using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Warehouse.Models;

namespace Warehouse.Views.Converters
{
    public class ZoneTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ZoneType z) return z switch
            {
                ZoneType.Passage => Brushes.LightGray,
                ZoneType.Storage => Brushes.LightBlue,
                ZoneType.ShippingArea => Brushes.Orange,
                ZoneType.ReceivingArea => Brushes.Green,
                _ => Brushes.Transparent
            };
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
