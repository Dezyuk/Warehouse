using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Warehouse.Models;

namespace Warehouse.Views.Converters
{
    public class ProductToBrushConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2
                || values[0] is not int productId
                || values[1] is not System.Collections.IEnumerable cellsEnum)
            {
                return Brushes.Transparent;
            }

            var cells = cellsEnum.Cast<Cell>();
            var cell = cells.FirstOrDefault(c => c.ProductId == productId);
            if (cell?.FillColor != null)
            {
                try
                {
                    return (SolidColorBrush)(new BrushConverter().ConvertFrom(cell.FillColor)!);
                }
                catch { }
            }

            return Brushes.Transparent;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
