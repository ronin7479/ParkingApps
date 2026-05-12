using ParkingApp.Models;
using ParkingApp.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ParkingApp.Views
{
    public partial class AdminOccupancyPage : Page
    {
        private readonly DatabaseService _dbService;

        public AdminOccupancyPage(DatabaseService dbService)
        {
            InitializeComponent();
            _dbService = dbService;
            LoadOccupancy();
        }

        private void LoadOccupancy()
        {
            OccupancyPanel.Children.Clear();
            var parkingLots = _dbService.GetAllParkingLots();

            if (parkingLots == null || parkingLots.Count == 0)
            {
                OccupancyPanel.Children.Add(new TextBlock
                {
                    Text = "Немає паркінгів",
                    FontSize = 16,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(20)
                });
                return;
            }

            foreach (var lot in parkingLots)
                OccupancyPanel.Children.Add(CreateOccupancyCard(lot));
        }

        private Border CreateOccupancyCard(ParkingLot lot)
        {
            double percentage = (double)lot.OccupiedSpaces / lot.TotalSpaces * 100;

            var border = new Border
            {
                Width = 260,
                Margin = new Thickness(8),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(55, 55, 55)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(0, 0, 0, 16)
            };

            var stack = new StackPanel();

            // Color banner
            var banner = new Border
            {
                Height = 8,
                Background = GetOccupancyBrush(percentage),
                CornerRadius = new CornerRadius(14, 14, 0, 0)
            };
            stack.Children.Add(banner);

            var content = new StackPanel { Margin = new Thickness(16, 12, 16, 0) };

            var nameBlock = new TextBlock
            {
                Text = lot.Name,
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 6)
            };
            content.Children.Add(nameBlock);

            var locationBlock = new TextBlock
            {
                Text = "📍 " + lot.Location,
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(140, 140, 140)),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 12)
            };
            content.Children.Add(locationBlock);

            // Track bar
            var track = new Border
            {
                Height = 10,
                Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                CornerRadius = new CornerRadius(5),
                Margin = new Thickness(0, 0, 0, 6)
            };
            var fill = new Border
            {
                Height = 10,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = Math.Max(4, (percentage / 100.0) * 228),
                Background = GetOccupancyBrush(percentage),
                CornerRadius = new CornerRadius(5)
            };
            var barGrid = new Grid();
            barGrid.Children.Add(track);
            barGrid.Children.Add(fill);
            content.Children.Add(barGrid);

            var infoBlock = new TextBlock
            {
                Text = $"Зайнято: {lot.OccupiedSpaces}/{lot.TotalSpaces}  ({percentage:F0}%)",
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(180, 180, 180)),
                Margin = new Thickness(0, 4, 0, 8)
            };
            content.Children.Add(infoBlock);

            // Free spaces badge
            var freeBadge = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 25)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(80, 70, 20)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10, 4, 10, 4),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            freeBadge.Child = new TextBlock
            {
                Text = $"🟢 {lot.FreeSpaces} вільно",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            };
            content.Children.Add(freeBadge);

            stack.Children.Add(content);
            border.Child = stack;
            return border;
        }

        private Brush GetOccupancyBrush(double pct)
        {
            if (pct <= 30) return new SolidColorBrush(Color.FromRgb(50, 200, 50));
            if (pct <= 70) return new SolidColorBrush(Color.FromRgb(255, 165, 0));
            return new SolidColorBrush(Color.FromRgb(255, 80, 80));
        }
    }
}
