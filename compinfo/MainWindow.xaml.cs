using System.Windows;

namespace compinfo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CompInfoGrid.DataContext = new Computer();
        }

        private void Click_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
