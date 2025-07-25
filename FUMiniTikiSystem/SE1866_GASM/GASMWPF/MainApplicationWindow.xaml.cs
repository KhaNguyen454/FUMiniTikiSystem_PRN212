using System.Windows;

namespace GASMWPF // Đảm bảo namespace là GASMWPF
{
    /// <summary>
    /// Interaction logic for MainApplicationWindow.xaml
    /// </summary>
    public partial class MainApplicationWindow : Window
    {
        public MainApplicationWindow()
        {
            InitializeComponent();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
           
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}