using System;
using System.Globalization;
using System.Windows.Data;

namespace CombinedConverter
{
    public class CombinedHeightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double originalHeight && values[1] is double actualHeight)
            {
                // Combine the original height and the actual height
                return originalHeight + actualHeight;
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}