using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using System;
using System.Globalization;
using System.IO;

namespace eTheka.App.Converters;
public class ByteBitmapTypeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is byte[] byteArray && byteArray.Length > 0)
        {
            using (var stream = new MemoryStream(byteArray))
            {
                var bitmap = new Bitmap(stream);
                return bitmap;
            }
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}


