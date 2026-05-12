using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ParkingApp.Models;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class BookingsPage : Page
    {
        private readonly User _currentUser;
        private readonly DatabaseService _dbService;

        public BookingsPage(User user, DatabaseService dbService)
        {
            InitializeComponent();
            _currentUser = user;
            _dbService = dbService;
            LoadBookings();
        }

        private void LoadBookings()
        {
            BookingsPanel.Children.Clear();
            var bookings = _dbService.GetUserBookings(_currentUser.Id);

            if (bookings.Count == 0)
            {
                var emptyBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                    CornerRadius = new CornerRadius(14),
                    Padding = new Thickness(30, 40, 30, 40),
                    Margin = new Thickness(0, 20, 0, 0)
                };
                emptyBorder.Child = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Children =
                    {
                        new TextBlock { Text = "📋", FontSize = 40, TextAlignment = TextAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center },
                        new TextBlock { Text = "У вас ще немає бронювань", FontSize = 16, Foreground = new SolidColorBrush(Color.FromRgb(150,150,150)), TextAlignment = TextAlignment.Center, Margin = new Thickness(0,12,0,0) }
                    }
                };
                BookingsPanel.Children.Add(emptyBorder);
                return;
            }

            foreach (var booking in bookings)
                BookingsPanel.Children.Add(CreateBookingCard(booking));
        }

        private Border CreateBookingCard(Booking booking)
        {
            var border = new Border
            {
                Margin = new Thickness(0, 0, 0, 12),
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(55, 55, 55)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(20, 16, 20, 16),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var infoStack = new StackPanel();

            var carText = new TextBlock
            {
                Text = $"🚗 {booking.CarNumber}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 6)
            };
            infoStack.Children.Add(carText);

            var dateText = new TextBlock
            {
                Text = $"📅 {booking.StartTime:dd.MM.yyyy}  ·  {booking.StartTime:HH:mm} – {booking.EndTime:HH:mm}",
                FontSize = 13,
                Foreground = new SolidColorBrush(Color.FromRgb(170, 170, 170)),
                Margin = new Thickness(0, 0, 0, 4)
            };
            infoStack.Children.Add(dateText);

            if (!string.IsNullOrEmpty(booking.ReceiptNumber))
            {
                infoStack.Children.Add(new TextBlock
                {
                    Text = $"🧾 #{booking.ReceiptNumber}",
                    FontSize = 11,
                    Foreground = new SolidColorBrush(Color.FromRgb(120, 120, 120))
                });
            }

            Grid.SetColumn(infoStack, 0);
            grid.Children.Add(infoStack);

            var priceBadge = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(40, 40, 20)),
                BorderBrush = new SolidColorBrush(Color.FromRgb(80, 70, 20)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(10),
                Padding = new Thickness(14, 8, 14, 8),
                VerticalAlignment = VerticalAlignment.Center
            };
            priceBadge.Child = new TextBlock
            {
                Text = $"₴{booking.TotalPrice:F2}",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 215, 0))
            };
            Grid.SetColumn(priceBadge, 1);
            grid.Children.Add(priceBadge);

            border.Child = grid;
            border.MouseDown += (s, e) => ShowReceiptWindow(booking);
            return border;
        }

        private void ShowReceiptWindow(Booking booking)
        {
            var receipt = _dbService.GetReceiptByNumber(booking.ReceiptNumber);
            if (receipt != null)
            {
                var rw = new ReceiptWindow(receipt);
                rw.ShowDialog();
            }
            else
            {
                MessageBox.Show(
                    $"Квитанція #{booking.ReceiptNumber}\nАвто: {booking.CarNumber}\nСума: {booking.TotalPrice:F2} грн",
                    "Деталі бронювання",
                    MessageBoxButton.OK);
            }
        }
    }
}
