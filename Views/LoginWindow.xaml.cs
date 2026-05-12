using System.Windows;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class LoginWindow : Window
    {
        private readonly DatabaseService _dbService;
        private readonly AuthenticationService _authService;

        public LoginWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
            _authService = new AuthenticationService(_dbService);

            // Ensure test user exists
            _authService.EnsureTestUserExists();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage.Text = "Будь ласка, заповніть усі поля";
                return;
            }

            var user = _authService.Login(login, password);

            if (user != null)
            {
                MainWindow mainWindow = new MainWindow(user, _dbService);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                ErrorMessage.Text = "Неправильний логін або пароль";
            }
        }

        private void AutoLoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = "testuser";
            PasswordBox.Password = "test123";
            LoginButton_Click(sender, e);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow(_dbService, _authService);
            registerWindow.Show();
            this.Close();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            AdminLoginWindow adminWindow = new AdminLoginWindow(_dbService);
            adminWindow.Owner = this;
            adminWindow.ShowDialog();
        }

        private void LoginTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
