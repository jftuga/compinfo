using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace compinfo
{
    [ValueConversion(typeof(ObservableCollection<NetworkAddressViewModel>), typeof(string))]
    internal sealed class NetworkCollectionValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<NetworkAddressViewModel> collection = value as ObservableCollection<NetworkAddressViewModel>;
            if (collection == null)
            {
                return null;
            }

            MacAddressValueConverter macAddressValueConverter = new MacAddressValueConverter();

            StringBuilder result = new StringBuilder();
            for (int x = 0; x < collection.Count; x++)
            {
                if (x > 0) { result.Append(", "); }
                result.AppendFormat(
                    culture,
                    "{0} [{1}]",
                    collection[x].IpAddress,
                    macAddressValueConverter.Convert(collection[x].MacAddress, typeof(string), null, culture)); 
            }

            return result.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
