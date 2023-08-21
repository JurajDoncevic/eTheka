using Avalonia.Data.Converters;
using Avalonia.Media;
using eTheka.Base;
using System;
using System.Globalization;

namespace eTheka.App.Converters;
public class ResultTypeColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ResultTypes resultType)
        {
            return resultType switch
            {
                ResultTypes.SUCCESS => Brushes.Green,
                ResultTypes.FAILURE => Brushes.Red,
                ResultTypes.EXCEPTION => Brushes.Red,
                _ => Brushes.Transparent,
            };
        }
        return Brushes.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
