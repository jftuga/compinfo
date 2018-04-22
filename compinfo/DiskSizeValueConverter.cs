using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace compinfo
{
    [ValueConversion(typeof(ulong), typeof(string))] // The incoming type is UInt64, the return type is String.
    internal sealed class DiskSizeValueConverter : IValueConverter
    {
        private const ulong BytesInMegabyte = 1048576UL; // 1 MB = 1048576 bytes
        private const ulong BytesInGigabyte = 1048576UL * 1024UL; // 1 GB = 1073741824 bytes
        private const ulong BytesInTerabyte = 1048576UL * 1024UL * 1024UL; // 1 TB = 1099511627776 bytes

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Perform sanity checks.  This class assumes the incoming 
            Debug.Assert(targetType == typeof(string));
            Debug.Assert(parameter == null);
            Debug.Assert(culture != null);

            // Perform conversion.
            ulong rawValue = System.Convert.ToUInt64(value);
            return FormatAsHumanReadableString(rawValue, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static string FormatAsHumanReadableString(ulong byteCount, CultureInfo culture)
        {
            if (byteCount >= BytesInTerabyte)
            {
                return string.Format(culture, "{0:0.0#} TB", (byteCount / BytesInTerabyte));
            }

            if (byteCount >= BytesInGigabyte)
            {
                return string.Format(culture, "{0:0.0#} GB", (byteCount / BytesInGigabyte));
            }

            if (byteCount >= BytesInMegabyte)
            {
                return string.Format(culture, "{0:0.0#} MB", (byteCount / BytesInMegabyte));
            }

            return string.Format(culture, "{0} bytes", byteCount);
        }
    }
}
