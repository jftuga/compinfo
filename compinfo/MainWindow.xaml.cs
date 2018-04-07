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

namespace compinfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string displayUserName { get; set; }
        public static string displayComputerName { get; set; }
        public static string displayOS { get; set; }
        public static string displayModel { get; set; }
        public static string displaySerial { get; set; }
        public static string displayUptime { get; set; }
        public static string displayCPU { get; set; }
        public static string displayMemory { get; set; }
        public static string displayIPv4 { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            CompInfoGrid.DataContext = this;
            ShowCompInfo();
        }

        public void ShowCompInfo()
        {
            var c = new Computer();

            displayUserName = c.GetUserName;
            displayComputerName = c.GetComputerName;
            displayOS = c.GetOS;
            displayModel = c.GetModel;
            displaySerial = c.GetSerial;
            displayUptime = c.GetUptime;
            displayCPU = c.GetCPU;
            displayMemory = c.GetMemory;
            displayIPv4 = c.GetIPv4;
        }

        private void Click_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
