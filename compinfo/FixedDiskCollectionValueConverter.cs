using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace compinfo
{
    [ValueConversion(typeof(ObservableCollection<FixedDiskViewModel>), typeof(string))]
    internal sealed class FixedDiskValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<FixedDiskViewModel> collection = value as ObservableCollection<FixedDiskViewModel>;
            if (collection == null)
            {
                return null;
            }

            DiskSizeValueConverter diskSizeValueConverter = new DiskSizeValueConverter();

            StringBuilder result = new StringBuilder();
            for (int x = 0; x < collection.Count; x++)
            {
                if (x > 0) { result.Append(", "); }
                result.AppendFormat(
                    culture,
                    "{0} [{1} of {2}]",
                    collection[x].DeviceID,
                    diskSizeValueConverter.Convert(collection[x].FreeSpace, typeof(string), null, culture),
                    diskSizeValueConverter.Convert(collection[x].Size, typeof(string), null, culture));
            }

            return result.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}