using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Warehouse.Models;

namespace Warehouse.Views.Converters
{
    /// <summary>
    /// Делает видимым элемент, когда CurrentMode == ConverterParameter (например, "ChangeType").
    /// </summary>
    public class TopologyModeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TopologyMode mode && parameter is string param
                && Enum.TryParse<TopologyMode>(param, out var target))
            {
                return mode == target
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
