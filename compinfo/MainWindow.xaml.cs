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
        public string displayUserName => c.GetUserName;
        public string displayComputerName => c.GetComputerName;
        public string displayOS => c.GetOS;
        public string displayModel => c.GetModel;
        public string displaySerial => c.GetSerial;
        public string displayUptime => c.GetUptime;
        public string displayCPU => c.GetCPU;
        public string displayMemory => c.GetMemory;
        public string displayIPv4 => c.GetIPv4;

        private Computer c = new Computer();

        public MainWindow()
        {
            InitializeComponent();
            CompInfoGrid.DataContext = this;
        }

        private void Click_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
    }
}
