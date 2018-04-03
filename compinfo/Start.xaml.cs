using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Management;
using System.Threading;
using System.Windows.Threading;
using System.Net.NetworkInformation;


namespace compinfo
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        [System.Runtime.InteropServices.DllImport("kernel32")]
        extern static UInt64 GetTickCount64();

        private static string compSerial;
        private static string compCPU;
        private static string compCalc;
        private static string compMemory;
        private static List<string> compIPV4;


        public Start()
        {
            InitializeComponent();
            ShowCompInfo();
        }

        public void ShowCompInfo()
        {
            TextBox_ComputerName.Text = Environment.MachineName;
            TextBox_User.Text = Environment.UserName;
            TextBox_OS.Text = HKLM_GetString(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName");
            TextBox_Uptime.Text = GetUptime();

            /*
            Action onGetCPUCompleted = () =>
            {
                if (false)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TextBox_CPU.Text = compCPU;
                        TextBox_Uptime.Text = compCalc;
                    });
                }
                else
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        TextBox_CPU.Text = compCPU;
                        TextBox_Uptime.Text = compCalc;
                    }), DispatcherPriority.ContextIdle);
                }
            };
            */

            var threadCPU = new Thread(() =>
              {
                  try
                  {
                      GetCPU();
                  }
                  finally
                  {
                      //onGetCPUCompleted();
                      this.Dispatcher.Invoke(() =>
                      {
                          TextBox_CPU.Text = compCPU;
                          //TextBox_Uptime.Text = compCalc;
                      });
                  }
              });
            threadCPU.Start();

            var threadSerial = new Thread(() =>
            {
                try
                {
                    GetSerial();
                }
                finally
                {
                    //onGetCPUCompleted();
                    this.Dispatcher.Invoke(() =>
                    {
                        //TextBox_CPU.Text = compCPU;
                        TextBox_Serial.Text = compSerial;
                        //TextBox_Uptime.Text = compCalc;
                    });
                }
            });
            threadSerial.Start();

            var threadMemory = new Thread(() =>
            {
                try
                {
                    GetMemory();
                }
                finally
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TextBox_Memory.Text = compMemory;
                        //TextBox_Uptime.Text = compCalc;
                    });
                }
            });
            threadMemory.Start();

            var threadIPV4 = new Thread(() =>
            {
                try
                {
                    GetIPAddresses();
                }
                finally
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        TextBox_IPV4.Text = String.Join(" ", compIPV4);
                    });
                }
            });
            threadIPV4.Start();

        } // enf of ShowCompInfo()

        public static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static void DoCalc()
        {
            int i, j, k;
            long total = 0;
            for (i = 0; i < 947483647; i++)
            {
                for (j = 0; j < 1; j++)
                {
                    for (k = 0; k < 1; k++)
                    {
                        total += 1;
                    }
                }
            }
            compCalc = String.Format("{0}", total);
        }

        public static string HKLM_GetString(string path, string key)
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

        public static string GetSerial()
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
                } catch
                {
                    man += "";
                }
                try
                {
                    mod += obj.Properties["Model"].Value.ToString().Trim();
                } catch
                {
                    mod += "";
                }
                if (tag.Length > 3)
                {
                    break;
                }
            }

            string spc = (mod.Length > 0) ? " " : "";
            compSerial = String.Format("{0} [{1}{2}{3}]", tag, man, spc, mod);
            return compSerial;
        }

        public string GetCPU()
        {
            //System.Threading.Thread.Sleep(5000);
            String cpu = "";
            using (ManagementObjectSearcher win32Proc = new ManagementObjectSearcher("select * from Win32_Processor"))
            {
                foreach (ManagementObject obj in win32Proc.Get())
                {
                    cpu = obj["Name"].ToString();
                    if (cpu.Length > 3)
                    {
                        break;
                    }
                }
            }
            compCPU = String.Format("{0} [{1} cores]", cpu, Environment.ProcessorCount);
            return cpu;
        }

        public static string GetMemory()
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
            compMemory = String.Format("{0:0.00} GB {1} {2} {3}", total_gb, left_brack, speed, right_brack);
            return compMemory;
        }

        public static List<string> GetIPAddresses()
        {
            List<string> ip_list = new List<string>();
            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                if ((netInterface.NetworkInterfaceType.ToString() != "Wireless80211" && netInterface.NetworkInterfaceType.ToString() != "Ethernet") || netInterface.OperationalStatus.ToString() == "Down")
                {
                    continue;
                }

                //Console.WriteLine("Name        : " + netInterface.Name);
                //Console.WriteLine("Description : " + netInterface.Description);
                //Console.WriteLine("Type        : " + netInterface.NetworkInterfaceType);
                //Console.WriteLine("Status      : " + netInterface.OperationalStatus);
                //Console.WriteLine("Addresses   : ");
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    String currentAddress = addr.Address.ToString();
                    if (!currentAddress.Contains(":"))
                    {
                        //Console.WriteLine(" " + currentAddress);
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
                //Console.WriteLine("");
            }
            compIPV4 = ip_list;
            return ip_list;
        }

        public static string addColons(string mac)
        {
            string newMac = "";
            int i;
            for(i=0; i<12; i+=2)
            {
                newMac += mac.Substring(i, 2) + ":";
            }

            return newMac.ToLower().Substring(0, 17);
        }

        public static string GetUptime()
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

        private void Click_Close(object sender, RoutedEventArgs e)
        {
            //System.Windows.Application.Current.Shutdown();
            Application.Current.MainWindow.Close();
        }
    } // end of class
} // end of namespace
