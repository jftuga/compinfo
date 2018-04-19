using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace compinfo
{
    [ValueConversion(typeof(string), typeof(string))]
    internal sealed class MacAddressValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(string));
            Debug.Assert(parameter == null);
            Debug.Assert(culture != null);

            string rawValue = value as string;
            if (rawValue == null)
            {
                return null;
            }

            return addColons(rawValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static string addColons(string mac)
        {
            if (!string.IsNullOrEmpty(mac) && mac.Length == 12)
            {
                char[] chars = new char[17];
                chars[0] = mac[0];
                chars[1] = mac[1];
                chars[2] = ':';
                chars[3] = mac[2];
                chars[4] = mac[3];
                chars[5] = ':';
                chars[6] = mac[4];
                chars[7] = mac[5];
                chars[8] = ':';
                chars[9] = mac[6];
                chars[10] = mac[7];
                chars[11] = ':';
                chars[12] = mac[8];
                chars[13] = mac[9];
                chars[14] = ':';
                chars[15] = mac[10];
                chars[16] = mac[11];
                return new string(chars);
            }

            return mac;
        }
    }
}
