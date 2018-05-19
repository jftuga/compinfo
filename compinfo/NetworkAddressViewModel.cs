using System.ComponentModel;

namespace compinfo
{
    internal sealed class NetworkAddressViewModel : INotifyPropertyChanged
    {
        private readonly string _ipAddress;
        private readonly string _macAddress;
        private readonly long _speed;

        public NetworkAddressViewModel(string ipAddress, string macAddress, long speed)
        {
            this._ipAddress = ipAddress;
            this._macAddress = macAddress;
            this._speed = speed;
        }

        public string IpAddress
        {
            get { return this._ipAddress; }
        }

        public string MacAddress
        {
            get { return this._macAddress; }
        }

        public long Speed
        {
            get { return this._speed;  }
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