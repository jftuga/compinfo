using System;
using System.Net.NetworkInformation;
using System.Management;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Globalization;
using System.Collections.ObjectModel;

namespace compinfo 
{
    class Computer
    {
        private static string NA;

        public Computer()
        {
            NA = "N/A";
        }

        // helper methods

        private static string HKLM_GetString(string path, string key)
        {
            RegistryKey registryKey = null;
            try
            {
                registryKey = Registry.LocalMachine.OpenSubKey(path, false);
                if (registryKey == null)
                {
                    return NA;
                }
                if (registryKey.GetValueKind(key) != RegistryValueKind.String)
                {
                    return NA;
                }
                return (string)registryKey.GetValue(key);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return NA;
        }

        private static string getPropertyValueFromManObject(ManagementObject obj, string propertyName, string noResult = "")
        {
            var prop = obj.Properties.Cast<PropertyData>().Where(x => x.Name == propertyName).FirstOrDefault();
            if (prop == null)
            {
                return NA;
            }

            string propStr = prop.Value.ToString().Trim();
            return (propStr.Length > 0) ? propStr : noResult;
        }

        private static string getPropertyValueFromSearcher(ManagementObjectSearcher searcher, string propertyName, string noResult = "")
        {
            using (ManagementObjectCollection queryCollection = searcher.Get())
            {
                ManagementObject obj = queryCollection.OfType<ManagementObject>().FirstOrDefault();
                return getPropertyValueFromManObject(obj, propertyName, noResult);
            }
        }

        // external methods

        public string UserName
        {
            get
            {
                return Environment.UserName;
            }
        }

        public string ComputerName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public string OS
        {
            get
            {
                return HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            }
        }

        public string Model
        {
            get
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Manufacturer, Model from Win32_ComputerSystem"))
                {
                    string Manufacturer = getPropertyValueFromSearcher(searcher, "Manufacturer");
                    string Model = getPropertyValueFromSearcher(searcher, "Model");

                    return (String.Format("{0} {1}", Manufacturer, Model)).Trim();
                }
            }
        }

        public string Serial
        {
            get
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select SerialNumber, Caption, Description from Win32_BIOS"))
                {
                    string SerialNumber = getPropertyValueFromSearcher(searcher, "SerialNumber");
                    string Caption = getPropertyValueFromSearcher(searcher, "Caption");
                    string Description = getPropertyValueFromSearcher(searcher, "Description");

                    string Details = (Caption.Length >= Description.Length) ? Caption : Description;
                    return (Details.Length > 0) ? String.Format("{0} [{1}]", SerialNumber, Details) : SerialNumber;
                }
            }
        }

        public string CPU
        {
            get
            {
                string Name = "";
                string NumberOfCores = "";
                string NumberOfLogicalProcessors = "";
                long NumberOfCoresLong = 0;
                long NumberOfLogicalProcessorsLong = 0;

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Name, NumberOfCores, NumberOfLogicalProcessors from Win32_Processor"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        if (0 == Name.Length)
                        {
                            Name = getPropertyValueFromSearcher(searcher, "Name");
                        }

                        NumberOfCores = getPropertyValueFromSearcher(searcher, "NumberOfCores", NA);
                        NumberOfCoresLong += Convert.ToInt64(NumberOfCores);

                        NumberOfLogicalProcessors = getPropertyValueFromSearcher(searcher, "NumberOfLogicalProcessors", NA);
                        NumberOfLogicalProcessorsLong += Convert.ToInt64(NumberOfLogicalProcessors);
                    }
                }

                return String.Format("{0} [{1} cores, {2} logical processors]", Name, NumberOfCoresLong, NumberOfLogicalProcessorsLong);
            }
        }

        public string Memory
        {
            get
            {
                string Capacity = "";
                string Speed = "";
                long totalSize = 0;
                long totalSizeGB = 0;

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Capacity, Speed from Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        Capacity = getPropertyValueFromManObject(obj, "Capacity");
                        totalSize += Convert.ToInt64(Capacity);

                        try
                        {
                            if (0 == Speed.Length)
                            {
                                Speed = getPropertyValueFromManObject(obj, "Speed") + " MHz";
                            }
                        } catch
                        {
                            // VMWare VMs do not have Speed defined
                            Speed = NA;
                        }
                    }
                }

                totalSizeGB = (totalSize) / 1073741824; // 1024**3 (1 GB)
                return String.Format("{0:0.00} GB [{1}]", totalSizeGB, Speed);
            }
        }

        public string Graphics
        {
            get
            {
                List<string> adapter_list = new List<string>();

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        adapter_list.Add(getPropertyValueFromManObject(obj, "Name"));
                    }
                }

                return String.Join(", ", adapter_list);
            }
        }

        public ObservableCollection<FixedDiskViewModel> FixedDisk
        {
            get
            {
                string DeviceID = string.Empty;
                UInt64 FreeSpace = 0;
                UInt64 Size = 0;
                ObservableCollection<FixedDiskViewModel> collection = new ObservableCollection<FixedDiskViewModel>();
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT DeviceID, FreeSpace, Size FROM Win32_LogicalDisk WHERE Description='Local Fixed Disk'"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        DeviceID = getPropertyValueFromManObject(obj, "DeviceID");
                        FreeSpace = Convert.ToUInt64(getPropertyValueFromManObject(obj, "FreeSpace"));
                        Size = Convert.ToUInt64(getPropertyValueFromManObject(obj, "Size"));
                        collection.Add(new FixedDiskViewModel(DeviceID, FreeSpace, Size));
                    }
                }

                return collection;
            }
        }

        public ObservableCollection<NetworkAddressViewModel> IPv4
        {
            get
            {
                ObservableCollection<NetworkAddressViewModel> collection = new ObservableCollection<NetworkAddressViewModel>();
                foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.OperationalStatus == OperationalStatus.Down)
                    {
                        continue;
                    }
                    if (netInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                        netInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                    {
                        continue;
                    }

                    IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                    {
                        if (addr.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            continue;
                        }
                        string ipAddress = addr.Address.ToString();
                        string macAddress = netInterface.GetPhysicalAddress().ToString();
                        collection.Add(new NetworkAddressViewModel(ipAddress, macAddress));
                    }
                }
                return collection;
            }
        }

        public string Uptime
        {
            get
            {
                return NativeMethods.Uptime();
            }
        }
    }
}
