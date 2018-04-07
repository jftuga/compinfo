using System.Windows;
using System.Windows.Controls;

namespace compinfo
{
    /// <summary>
    /// Interaction logic for Start.xaml
    /// </summary>
    public partial class Start : Page
    {
        public static string displayUserName { get; set; }
        public static string displayComputerName { get; set; }
        public static string displayOS { get; set; }
        public static string displaySerial { get; set; }
        public static string displayUptime { get; set; }
        public static string displayCPU { get; set; }
        public static string displayMemory { get; set; }
        public static string displayIPv4 { get; set; }

        public Start()
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
            displaySerial = c.GetSerial;
            displayUptime = c.GetUptime;
            displayCPU = c.GetCPU; // this is slow
            displayMemory = c.GetMemory;
            displayIPv4 = c.GetIPv4;
        } 
        
        private void Click_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    } // end of class
} // end of namespace
