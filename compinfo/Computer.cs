﻿using System;
using System.Net.NetworkInformation;
using System.Management;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;

namespace compinfo 
{
    class Computer
    {
        [System.Runtime.InteropServices.DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        private static string NA;

        public Computer()
        {
            NA = "N/A";
        }

        // helper methods

        private static string HKLM_GetString(string path, string key)
        {
            try
            {
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(path))
                {
                    return (rk == null) ? "" : (string)rk.GetValue(key);
                }
            }
            catch
            {
                // do nothing
            }

            return "";
        }

        private static string addColons(string mac)
        {
            return (String.Format("{0}:{1}:{2}:{3}:{4}:{5}", mac.Substring(0, 2), mac.Substring(2, 2), mac.Substring(4, 2), mac.Substring(6, 2), mac.Substring(8, 2), mac.Substring(10, 2))).ToLower();
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
            ManagementObjectCollection queryCollection = searcher.Get();
            ManagementObject obj = queryCollection.OfType<ManagementObject>().FirstOrDefault();

            return getPropertyValueFromManObject(obj, propertyName, noResult);
        }

        // external methods

        public string GetUserName
        {
            get
            {
                return Environment.UserName;
            }
        }

        public string GetComputerName
        {
            get
            {
                return Environment.MachineName;
            }
        }

        public string GetOS
        {
            get
            {
                return HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            }
        }

        public string GetModel
        {
            get
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Manufacturer, Model from Win32_ComputerSystem");
                string Manufacturer = getPropertyValueFromSearcher(searcher, "Manufacturer");
                string Model = getPropertyValueFromSearcher(searcher, "Model");

                return (String.Format("{0} {1}", Manufacturer, Model)).Trim();
            }
        }

        public string GetSerial
        {
            get
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select SerialNumber, Caption, Description from Win32_BIOS");
                string SerialNumber = getPropertyValueFromSearcher(searcher, "SerialNumber");
                string Caption = getPropertyValueFromSearcher(searcher, "Caption");
                string Description = getPropertyValueFromSearcher(searcher, "Description");

                string Details = (Caption.Length >= Description.Length) ? Caption : Description;
                return (Details.Length > 0) ? String.Format("{0} [{1}]", SerialNumber, Details) : SerialNumber;
            }
        }

        public string GetCPU
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

        public string GetMemory
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

        public string GetGraphics
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

        public string GetIPv4
        {
            get
            {
                List<string> ip_list = new List<string>();
                String currentAddress = "";
                foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if ((netInterface.NetworkInterfaceType.ToString() != "Wireless80211" && netInterface.NetworkInterfaceType.ToString() != "Ethernet") || netInterface.OperationalStatus.ToString() == "Down")
                    {
                        continue;
                    }

                    IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                    {
                        currentAddress = addr.Address.ToString();
                        if (!currentAddress.Contains(":"))
                        {
                            if (currentAddress.StartsWith("169.254."))
                            {
                                continue;
                            }

                            if (!ip_list.Contains(currentAddress))
                            {
                                string macAddress = netInterface.GetPhysicalAddress().ToString();
                                ip_list.Add(String.Format("{0} [{1}]", currentAddress, addColons(macAddress)));
                            }
                        }
                    }
                }
                return String.Join(", ", ip_list);
            }
        }

        public string GetUptime
        {
            get
            {
                return NativeMethods.GetUptime();
            }
        }
    }
}
