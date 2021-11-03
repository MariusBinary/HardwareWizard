using System;
using System.Globalization;
using System.Windows.Data;

namespace HardwareWizard.Converters
{
    public class DimentionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
              object parameter, CultureInfo culture)
        {
            return (double)value / double.Parse(parameter as string);
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
