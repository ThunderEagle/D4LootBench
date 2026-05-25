using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace D4Loot.App.Converters;

[ValueConversion(typeof(bool), typeof(Brush))]
public sealed class BoolToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is true) return Brushes.Red;
        return Application.Current.Resources[SystemColors.ControlTextBrushKey] as Brush
               ?? SystemColors.ControlTextBrush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
