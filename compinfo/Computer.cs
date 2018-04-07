using System;
using System.Net.NetworkInformation;
using System.Management;
using System.Collections.Generic;
using Microsoft.Win32;

namespace compinfo 
{
    class Computer
    {
        [System.Runtime.InteropServices.DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        // helper methods

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private static string HKLM_GetString(string path, string key)
        {
            try
            {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(path);
                if (rk == null) return "";
                return (string)rk.GetValue(key);
            }
            catch
            {
                // do nothing
            }

            return "";
        }

        private static string addColons(string mac)
        {
            string newMac = "";
            int i;
            for (i = 0; i < 12; i += 2)
            {
                newMac += mac.Substring(i, 2) + ":";
            }

            return newMac.ToLower().Substring(0, 17);
        }

        
        //
        // external methods
        //

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

        public string GetUptime
        {
            get
            {
                TimeSpan t1;
                t1 = TimeSpan.FromMilliseconds(GetTickCount64());
                TimeSpan t2;
                t2 = new TimeSpan(t1.Ticks - (t1.Ticks % 600000000));
                string uptime = t2.ToString();
                if (uptime.EndsWith(":00"))
                {
                    uptime = Truncate(uptime, uptime.Length - 3);
                }
                return uptime;
            }
        }

        public string GetSerial
        {
            get
            {
                SelectQuery selectQuery = new SelectQuery("Win32_Bios");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);
                string tag = "";
                string man = "";
                string mod = "";
                foreach (ManagementObject obj in searcher.Get())
                {
                    tag = obj.Properties["Serialnumber"].Value.ToString().Trim();
                    try
                    {
                        man += obj.Properties["Manufacturer"].Value.ToString().Trim();
                    }
                    catch
                    {
                        man += "";
                    }
                    try
                    {
                        mod += obj.Properties["Model"].Value.ToString().Trim();
                    }
                    catch
                    {
                        mod += "";
                    }
                    if (tag.Length > 3)
                    {
                        break;
                    }
                }

                string spc = (mod.Length > 0) ? " " : "";
                return String.Format("{0} [{1}{2}{3}]", tag, man, spc, mod);
            }
        }

        public string GetCPU
        {
            get
            {
                String cpu = "";
                String physical = "";
                String logical = "";
                using (ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select Name, NumberOfCores,NumberOfLogicalProcessors from Win32_Processor where DeviceID='CPU0'"))
                {
                    foreach (ManagementObject obj in win32Proc.Get())
                    {
                        cpu = obj["Name"].ToString();
                        physical = obj["NumberofCores"].ToString();
                        logical = obj["NumberOfLogicalProcessors"].ToString();
                        break;
                    }
                }
                return String.Format("{0} [{1} cores, {2} logical processors]", cpu, physical, logical);
            }
        }

        public string GetMemory
        {
            get
            {
                String capacity = "";
                String speed = "";
                String left_brack = "[";
                String right_brack = "]";
                long total_sz = 0;
                double total_gb = 0;
                using (ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select Capacity,Speed from Win32_PhysicalMemory"))
                {
                    foreach (ManagementObject obj in win32Proc.Get())
                    {
                        capacity = obj["Capacity"].ToString();
                        long sz = Convert.ToInt64(capacity);
                        total_sz += sz;

                        // VMware VMs do not have Speed defined
                        try
                        {
                            if (0 == speed.Length)
                            {
                                speed = obj["Speed"].ToString();
                                speed += "Mhz";
                            }
                        }
                        catch (System.NullReferenceException)
                        {
                            left_brack = "[";
                            right_brack = "]";
                            speed = "";
                        }
                    }
                    total_gb = (total_sz) / 1073741824; //1024**3
                }
                return String.Format("{0:0.00} GB {1} {2} {3}", total_gb, left_brack, speed, right_brack);
            }
        }

        public string GetIPv4
        {
            get
            {
                List<string> ip_list = new List<string>();
                foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if ((netInterface.NetworkInterfaceType.ToString() != "Wireless80211" && netInterface.NetworkInterfaceType.ToString() != "Ethernet") || netInterface.OperationalStatus.ToString() == "Down")
                    {
                        continue;
                    }

                    IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                    foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                    {
                        String currentAddress = addr.Address.ToString();
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
                                //ip_list.Add(currentAddress);
                            }
                        }
                    }
                }
                return String.Join(" ", ip_list);
            }
        }
    }
}
