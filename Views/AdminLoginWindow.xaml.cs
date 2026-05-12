using System.Windows;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class AdminLoginWindow : Window
    {
        private readonly DatabaseService _dbService;
        private readonly AuthenticationService _authService;

        public AdminLoginWindow(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            _authService = new AuthenticationService(_dbService);
        }

        private void AdminLoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = AdminLoginTextBox.Text;
            string password = AdminPasswordBox.Password;

            if (_authService.AdminLogin(login, password))
            {
                AdminWindow adminWindow = new AdminWindow(_dbService);
                adminWindow.Show();
                this.Close();
            }
            else
            {
                ErrorMessage.Text = "❌ Неправильні дані адміністратора";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
