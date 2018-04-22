using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace compinfo
{
    [ValueConversion(typeof(UInt64), typeof(UInt64))]
    internal sealed class DiskSizeValueConverter : IValueConverter
    {
        // 1 mb = 1048576   1 gb = 1073741824   1 tb = 1099511627776
        private static readonly UInt64 mb_size = 1048576;
        private static readonly UInt64 gb_size = mb_size * 1024;
        private static readonly UInt64 tb_size = gb_size * 1024;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debug.Assert(targetType == typeof(string));
            Debug.Assert(parameter == null);
            Debug.Assert(culture != null);

            UInt64 rawValue = System.Convert.ToUInt64(value);
            return changeUnits(rawValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static string changeUnits(UInt64 size)
        {
            if (size >= tb_size)
            {
                return String.Format("{0:0.0#} tb", (size/tb_size));
            }
            else if (size >= gb_size)
            { 
                return String.Format("{0:0.0#} gb", (size/gb_size));
            }
            else if (size >= mb_size)
            {
                return String.Format("{0:0.0#} mb", (size / mb_size));
            }
            else
            {
                return size.ToString();
            }

        }
    }
}
