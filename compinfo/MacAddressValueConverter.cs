using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace compinfo
{
    [ValueConversion(typeof(string), typeof(string))]
    class MacAddressValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string rawValue = value as string;
            if (rawValue == null)
            {
                return null;
            }

            return addColons(rawValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static string addColons(string mac)
        {
            return (12 == mac.Length) ? (String.Format("{0}:{1}:{2}:{3}:{4}:{5}", mac.Substring(0, 2), mac.Substring(2, 2), mac.Substring(4, 2), mac.Substring(6, 2), mac.Substring(8, 2), mac.Substring(10, 2))).ToLower() : mac.ToLower();
            
        }
    }
}
