using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ParkingApp.Models;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class HomePage : Page
    {
        private readonly Models.User _currentUser;
        private readonly DatabaseService _dbService;
        private readonly BookingService _bookingService;

        public HomePage(Models.User user, DatabaseService dbService)
        {
            InitializeComponent();
            _currentUser = user;
            _dbService = dbService;
            _bookingService = new BookingService(dbService, new PaymentService());

            Loaded += (s, e) => LoadParkingLots();
        }

        public void LoadParkingLots()
        {
            var parkingLots = _dbService.GetAllParkingLots();
            ParkingCountText.Text = $"· {parkingLots.Count} локацій";

            ParkingPanel.Children.Clear();

            foreach (var lot in parkingLots)
            {
                var card = CreateParkingCard(lot);
                ParkingPanel.Children.Add(card);
            }
        }

        private Border CreateParkingCard(ParkingLot lot)
        {
            var border = new Border
            {
                Width = 320,
                Margin = new Thickness(8),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(14),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(180) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Image area with gradient overlay
            var imageGrid = new Grid { Height = 180 };

            // Color banner fallback (since images may not load in all environments)
            var colorBanner = new Border
            {
                Background = GetParkingGradient(lot.Id),
                CornerRadius = new CornerRadius(14, 14, 0, 0)
            };
            imageGrid.Children.Add(colorBanner);

            // Try to load image
            try
            {
                var uri = new Uri($"pack://application:,,,/Resources/Images/{lot.ImagePath}");
                var bitmap = new System.Windows.Media.Imaging.BitmapImage(uri);
                var image = new Image
                {
                    Source = bitmap,
                    Stretch = Stretch.UniformToFill,
                    Height = 180,
                    ClipToBounds = true
                };
                image.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                imageGrid.Children.Add(image);
            }
            catch { /* Use color banner fallback */ }

            // Overlay gradient for readability
            var overlay = new Border
            {
                CornerRadius = new CornerRadius(14, 14, 0, 0),
                Background = new LinearGradientBrush(
                    new GradientStopCollection
                    {
                        new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.5),
                        new GradientStop(Color.FromArgb(160, 0, 0, 0), 1.0)
                    },
                    new Point(0, 0), new Point(0, 1))
            };
            imageGrid.Children.Add(overlay);

            // Parking number badge
            var badge = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8, 4, 8, 4),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 10, 10, 0)
            };
            badge.Child = new TextBlock
            {
                Text = $"#{lot.Id}",
                Foreground = new SolidColorBrush(Color.FromRgb(26, 26, 26)),
                FontWeight = FontWeights.Bold,
                FontSize = 12
            };
            imageGrid.Children.Add(badge);

            // Status badge
            var statusBadge = new Border
            {
                Background = GetStatusBrush(lot),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(8, 4, 8, 4),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 10, 0, 0)
            };
            statusBadge.Child = new TextBlock
            {
                Text = GetStatusText(lot),
                Foreground = Brushes.White,
                FontWeight = FontWeights.SemiBold,
                FontSize = 11
            };
            imageGrid.Children.Add(statusBadge);

            Grid.SetRow(imageGrid, 0);
            grid.Children.Add(imageGrid);

            // Name
            var nameBlock = new TextBlock
            {
                Text = lot.Name,
                FontSize = 15,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(14, 12, 14, 2),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(nameBlock, 1);
            grid.Children.Add(nameBlock);

            // Location with map link
            var locationPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(14, 0, 14, 10)
            };
            var locationText = new TextBlock
            {
                Text = "📍 " + lot.Location,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(150, 150, 150)),
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 200
            };
            locationPanel.Children.Add(locationText);

            // Map button
            var mapBtn = new Button
            {
                Content = "🗺 Карта",
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Margin = new Thickness(8, 0, 0, 0),
                Padding = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center
            };
            mapBtn.Click += (s, e) => OpenInMaps(lot);
            locationPanel.Children.Add(mapBtn);

            Grid.SetRow(locationPanel, 2);
            grid.Children.Add(locationPanel);

            // Occupancy bar
            var barContainer = new Grid { Margin = new Thickness(14, 4, 14, 4) };
            barContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            barContainer.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var barLabel = new TextBlock
            {
                FontSize = 11,
                Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170)),
                Margin = new Thickness(0, 0, 0, 4)
            };
            double percentage = (double)lot.OccupiedSpaces / lot.TotalSpaces * 100;
            barLabel.Text = $"Зайнятість: {lot.OccupiedSpaces}/{lot.TotalSpaces} місць  ({percentage:F0}%)";
            Grid.SetRow(barLabel, 0);
            barContainer.Children.Add(barLabel);

            var trackBorder = new Border
            {
                Height = 8,
                Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)),
                CornerRadius = new CornerRadius(4)
            };
            var fillBorder = new Border
            {
                Height = 8,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = Math.Max(4, (percentage / 100.0) * 252),
                Background = GetOccupancyBrush(percentage),
                CornerRadius = new CornerRadius(4)
            };
            var barGrid = new Grid();
            barGrid.Children.Add(trackBorder);
            barGrid.Children.Add(fillBorder);
            Grid.SetRow(barGrid, 1);
            barContainer.Children.Add(barGrid);

            Grid.SetRow(barContainer, 3);
            grid.Children.Add(barContainer);

            // Price & free spaces
            var infoPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(14, 8, 14, 4),
                HorizontalAlignment = HorizontalAlignment.Left
            };

            var freeBadge = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 30)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(80, 80, 40)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10, 4, 10, 4)
            };
            freeBadge.Child = new TextBlock
            {
                Text = $"🟢 {lot.FreeSpaces} вільно",
                Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            };
            infoPanel.Children.Add(freeBadge);

            var priceBadge = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(35, 35, 35)),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10, 4, 10, 4),
                Margin = new Thickness(8, 0, 0, 0)
            };
            priceBadge.Child = new TextBlock
            {
                Text = $"₴{lot.PricePerHour:F0}/год",
                Foreground = new SolidColorBrush(Color.FromRgb(200, 200, 200)),
                FontSize = 12,
                FontWeight = FontWeights.SemiBold
            };
            infoPanel.Children.Add(priceBadge);

            Grid.SetRow(infoPanel, 4);
            grid.Children.Add(infoPanel);

            // Book button
            bool canBook = lot.FreeSpaces > 0;
            var bookButton = new Button
            {
                Content = canBook ? "Забронювати" : "Немає місць",
                Height = 42,
                Margin = new Thickness(14, 10, 14, 14),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                IsEnabled = canBook,
                Cursor = canBook ? System.Windows.Input.Cursors.Hand : System.Windows.Input.Cursors.No
            };

            // Style button
            var bookTemplate = new ControlTemplate(typeof(Button));
            var bookFactory = new FrameworkElementFactory(typeof(Border));
            bookFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(10));
            bookFactory.SetValue(Border.BackgroundProperty,
                canBook
                    ? (Brush)new SolidColorBrush(Color.FromRgb(255, 215, 0))
                    : (Brush)new SolidColorBrush(Color.FromRgb(60, 60, 60)));
            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            bookFactory.AppendChild(contentFactory);
            bookTemplate.VisualTree = bookFactory;
            bookButton.Template = bookTemplate;
            bookButton.Foreground = canBook
                ? new SolidColorBrush(Color.FromRgb(26, 26, 26))
                : new SolidColorBrush(Color.FromRgb(120, 120, 120));

            bookButton.Click += (s, e) => BookButton_Click(lot);

            Grid.SetRow(bookButton, 5);
            grid.Children.Add(bookButton);

            border.Child = grid;
            return border;
        }

        private Brush GetParkingGradient(int id)
        {
            var colors = new[]
            {
                new[] { Color.FromRgb(30, 60, 30), Color.FromRgb(10, 30, 10) },
                new[] { Color.FromRgb(60, 40, 10), Color.FromRgb(30, 20, 5) },
                new[] { Color.FromRgb(20, 40, 70), Color.FromRgb(10, 20, 40) },
                new[] { Color.FromRgb(60, 20, 20), Color.FromRgb(30, 10, 10) },
                new[] { Color.FromRgb(40, 20, 60), Color.FromRgb(20, 10, 30) },
                new[] { Color.FromRgb(10, 50, 50), Color.FromRgb(5, 25, 25) },
                new[] { Color.FromRgb(60, 50, 10), Color.FromRgb(30, 25, 5) },
                new[] { Color.FromRgb(20, 60, 40), Color.FromRgb(10, 30, 20) },
                new[] { Color.FromRgb(60, 30, 50), Color.FromRgb(30, 15, 25) },
                new[] { Color.FromRgb(30, 50, 60), Color.FromRgb(15, 25, 30) },
            };
            var idx = (id - 1) % colors.Length;
            return new LinearGradientBrush(colors[idx][0], colors[idx][1], 45);
        }

        private Brush GetOccupancyBrush(double pct)
        {
            if (pct <= 30) return new SolidColorBrush(Color.FromRgb(50, 205, 50));
            if (pct <= 70) return new SolidColorBrush(Color.FromRgb(255, 165, 0));
            return new SolidColorBrush(Color.FromRgb(255, 80, 80));
        }

        private Brush GetStatusBrush(ParkingLot lot)
        {
            double pct = (double)lot.OccupiedSpaces / lot.TotalSpaces * 100;
            if (lot.FreeSpaces == 0) return new SolidColorBrush(Color.FromRgb(200, 50, 50));
            if (pct <= 30) return new SolidColorBrush(Color.FromRgb(30, 150, 30));
            return new SolidColorBrush(Color.FromRgb(200, 120, 0));
        }

        private string GetStatusText(ParkingLot lot)
        {
            if (lot.FreeSpaces == 0) return "● Зайнято";
            double pct = (double)lot.OccupiedSpaces / lot.TotalSpaces * 100;
            if (pct <= 30) return "● Вільно";
            return "● Майже повне";
        }

        private void OpenInMaps(ParkingLot lot)
        {
            try
            {
                string query = Uri.EscapeDataString($"{lot.Name}, {lot.Location}, Київ, Україна");
                string url = $"https://www.google.com/maps/search/?api=1&query={query}";
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                MessageBox.Show("Не вдалось відкрити карту. Переконайтесь, що браузер встановлено.",
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BookButton_Click(ParkingLot lot)
        {
            if (lot.FreeSpaces <= 0)
            {
                MessageBox.Show("На цьому паркінгу немає вільних місць", "Вибачте",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            PaymentWindow paymentWindow = new PaymentWindow(_currentUser, lot, _dbService, _bookingService);
            paymentWindow.ShowDialog();

            // Reload
            LoadParkingLots();
        }
    }
}
