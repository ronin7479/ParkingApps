using System.Windows;
using System.Windows.Controls;
using ParkingApp.Models;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly DatabaseService _dbService;

        public MainWindow(User user, DatabaseService dbService)
        {
            InitializeComponent();
            _currentUser = user;
            _dbService = dbService;

            // Set avatar letter and username
            if (!string.IsNullOrEmpty(user.Login))
            {
                AvatarLetter.Text = user.Login[0].ToString().ToUpper();
                UserNameLabel.Text = user.Login;
            }

            MainButton_Click(null, null);
        }

        private void SetActiveButton(Button active)
        {
            var style = (Style)FindResource("NavButton");
            var activeStyle = (Style)FindResource("NavButtonActive");
            MainNavButton.Style = style;
            ProfileNavButton.Style = style;
            BookingsNavButton.Style = style;
            active.Style = activeStyle;
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new HomePage(_currentUser, _dbService));
            SetActiveButton(MainNavButton);
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ProfilePage(_currentUser, _dbService));
            SetActiveButton(ProfileNavButton);
        }

        private void BookingsButton_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new BookingsPage(_currentUser, _dbService));
            SetActiveButton(BookingsNavButton);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
