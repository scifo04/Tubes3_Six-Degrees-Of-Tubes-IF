using System;
using System.Globalization;
using System.Windows.Data;

namespace CombinedConverter
{
    public class CombinedWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double originalWidth && values[1] is double actualWidth)
            {
                // Combine the original width and the actual width
                return originalWidth + actualWidth;
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}