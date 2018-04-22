using System;
using System.ComponentModel;

namespace compinfo
{
    internal sealed class FixedDiskViewModel : INotifyPropertyChanged
    {
        private readonly string _deviceID;
        private readonly UInt64 _freeSpace;
        private readonly UInt64 _size;

        public FixedDiskViewModel(string deviceID, UInt64 freeSpace, UInt64 size)
        {
            this._deviceID = deviceID;
            this._freeSpace = freeSpace;
            this._size = size;
        }

        // is this the proper place to convert from int to string, or should I use an additional ValueConverter class?
        public string DeviceID
        {
            get { return this._deviceID.ToString(); }
        }

        public UInt64 FreeSpace
        {
            get { return this._freeSpace; }
        }

        public UInt64 Size
        {
            get { return this._size; }
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
