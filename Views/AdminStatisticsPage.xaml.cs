using System.Linq;
using System.Text;
using System.Windows.Controls;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class AdminStatisticsPage : Page
    {
        private readonly DatabaseService _dbService;

        public AdminStatisticsPage(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            TodayBookingsText.Text = _dbService.GetTodayBookingsCount().ToString();
            TodayRevenueText.Text = $"{_dbService.GetTodayRevenue():F2} грн";
            WeekRevenueText.Text = $"{_dbService.GetWeekRevenue():F2} грн";
            TotalBookingsText.Text = _dbService.GetTotalBookings().ToString();
            TotalUsersText.Text = _dbService.GetTotalUsers().ToString();

            var lots = _dbService.GetAllParkingLots();
            var sb = new StringBuilder();
            foreach (var lot in lots)
            {
                double pct = (double)lot.OccupiedSpaces / lot.TotalSpaces * 100;
                string status = lot.FreeSpaces == 0 ? "Повне" : pct <= 30 ? "Вільне" : "Зайняте";
                sb.AppendLine($"• {lot.Name} — {lot.FreeSpaces}/{lot.TotalSpaces} вільно  ({status})  |  {lot.PricePerHour:F0} грн/год");
            }
            ParkingsSummaryText.Text = sb.ToString().TrimEnd();
        }
    }
}
