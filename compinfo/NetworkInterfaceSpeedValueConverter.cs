using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace compinfo
{
    [ValueConversion(typeof(long), typeof(string))]
    internal sealed class NetworkInterfaceSpeedValueConverter : IValueConverter
    {
        private const ulong BitsInMegabits = 1000000UL; 
        private const ulong BitsInGigabits = 1000000UL * 1000UL; 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(string));
            Debug.Assert(parameter == null);
            Debug.Assert(culture != null);

            ulong rawValue = System.Convert.ToUInt64(value);
            return FormatAsHumanReadableString(rawValue, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static string FormatAsHumanReadableString(ulong speed, CultureInfo culture)
        {
            if (speed >= BitsInGigabits)
            {
                return string.Format(culture, "{0} Gbps", (speed / BitsInGigabits));
            }

            return string.Format(culture, "{0} Mbps", (speed / BitsInMegabits));
        }
    }
}

