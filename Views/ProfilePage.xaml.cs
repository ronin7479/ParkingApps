using System.Windows;
using System.Windows.Controls;
using ParkingApp.Models;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class ProfilePage : Page
    {
        private readonly User _currentUser;
        private readonly DatabaseService _dbService;

        public ProfilePage(User user, DatabaseService dbService)
        {
            InitializeComponent();
            _currentUser = user;
            _dbService = dbService;

            LoadProfile();
        }

        private void LoadProfile()
        {
            // Отримуємо свіжі дані з БД
            var freshUser = _dbService.GetUserById(_currentUser.Id) ?? _currentUser;

            // Avatar letter from login
            if (!string.IsNullOrEmpty(freshUser.Login))
                AvatarLetter.Text = freshUser.Login[0].ToString().ToUpper();

            LoginText.Text = freshUser.Login;
            LoginDisplayText.Text = freshUser.Login;
            EmailText.Text = freshUser.Email;
            BookingsCountText.Text = freshUser.SuccessfulBookings.ToString();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // Close parent MainWindow
            Window parentWindow = Window.GetWindow(this);
            parentWindow?.Close();
        }
    }
}
