using System.Windows;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class AdminWindow : Window
    {
        private readonly DatabaseService _dbService;

        public AdminWindow(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            StatisticsTab_Click(null, null);
        }

        private void StatisticsTab_Click(object sender, RoutedEventArgs e)
        {
            AdminContentFrame.Navigate(new AdminStatisticsPage(_dbService));
            StatisticsTabButton.Style = (Style)FindResource("TabBtnActive");
            OccupancyTabButton.Style = (Style)FindResource("TabBtn");
        }

        private void OccupancyTab_Click(object sender, RoutedEventArgs e)
        {
            AdminContentFrame.Navigate(new AdminOccupancyPage(_dbService));
            OccupancyTabButton.Style = (Style)FindResource("TabBtnActive");
            StatisticsTabButton.Style = (Style)FindResource("TabBtn");
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
