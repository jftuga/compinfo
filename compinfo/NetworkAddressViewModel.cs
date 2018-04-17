using System.ComponentModel;

namespace compinfo
{
    internal sealed class NetworkAddressViewModel : INotifyPropertyChanged
    {
        private readonly string _ipAddress;
        private readonly string _macAddress;

        public NetworkAddressViewModel(string ipAddress, string macAddress)
        {
            this._ipAddress = ipAddress;
            this._macAddress = macAddress;
        }

        public string IpAddress
        {
            get { return this._ipAddress; }
        }

        public string MacAddress
        {
            get { return this._macAddress; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}