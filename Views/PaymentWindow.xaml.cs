using System;
using System.Text.RegularExpressions;
using System.Windows;
using ParkingApp.Models;
using ParkingApp.Services;

namespace ParkingApp.Views
{
    public partial class PaymentWindow : Window
    {
        private readonly User _currentUser;
        private readonly ParkingLot _parkingLot;
        private readonly DatabaseService _dbService;
        private readonly BookingService _bookingService;
        private readonly PaymentService _paymentService;

        // UA car number: 2 uppercase letters + 4 digits + 2 uppercase letters (e.g. КА2606НА)
        private static readonly Regex CarNumberRegex = new Regex(@"^[А-ЯІЇЄ]{2}\d{4}[А-ЯІЇЄ]{2}$");
        private bool _updatingCard = false;
        private bool _updatingCar = false;

        public PaymentWindow(User user, ParkingLot lot, DatabaseService dbService, BookingService bookingService)
        {
            InitializeComponent();
            _currentUser = user;
            _parkingLot = lot;
            _dbService = dbService;
            _bookingService = bookingService;
            _paymentService = new PaymentService();

            InitializeWindow();
        }

        private void InitializeWindow()
        {
            ParkingNameText.Text = _parkingLot.Name;
            ParkingPriceText.Text = $"Ціна: {_parkingLot.PricePerHour:F0} грн/год  ·  Вільних місць: {_parkingLot.FreeSpaces}";

            var spaces = _dbService.GetParkingSpacesByLotId(_parkingLot.Id);
            foreach (var space in spaces)
            {
                if (!space.IsOccupied)
                    SpaceNumberComboBox.Items.Add(space.SpaceNumber);
            }
            if (SpaceNumberComboBox.Items.Count > 0)
                SpaceNumberComboBox.SelectedIndex = 0;

            BookingDatePicker.SelectedDate = DateTime.Today;
            StartTimeTextBox.Text = "09:00";
            EndTimeTextBox.Text = "11:00";
            CalculatePrice();
        }

        private void CalculatePrice()
        {
            try
            {
                if (!TimeSpan.TryParse(StartTimeTextBox.Text, out var startTime) ||
                    !TimeSpan.TryParse(EndTimeTextBox.Text, out var endTime))
                    return;
                double hours = (endTime - startTime).TotalHours;
                if (hours > 0)
                    PriceText.Text = $"{((decimal)hours * _parkingLot.PricePerHour):F2} грн";
                else
                    PriceText.Text = "0 грн";
            }
            catch { }
        }

        private void Time_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CalculatePrice();
        }

        private void CarNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_updatingCar) return;
            _updatingCar = true;

            var tb = CarNumberTextBox;
            string raw = tb.Text.ToUpper();
            // Remove non-alphanumeric
            raw = Regex.Replace(raw, @"[^А-ЯІЇЄA-Z0-9]", "");
            if (raw != tb.Text)
            {
                int caret = tb.CaretIndex;
                tb.Text = raw;
                tb.CaretIndex = Math.Min(caret, raw.Length);
            }

            // Show hint color
            bool valid = CarNumberRegex.IsMatch(raw) || raw.Length < 8;
            CarNumberTextBox.BorderBrush = raw.Length == 8 && !CarNumberRegex.IsMatch(raw)
                ? System.Windows.Media.Brushes.OrangeRed
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(68, 68, 68));

            _updatingCar = false;
        }

        private void CardNumber_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (_updatingCard) return;
            _updatingCard = true;

            var tb = CardNumberTextBox;
            // Remove non-digits and spaces
            string digits = Regex.Replace(tb.Text, @"\D", "");
            if (digits.Length > 16) digits = digits.Substring(0, 16);

            // Format as XXXX XXXX XXXX XXXX
            string formatted = "";
            for (int i = 0; i < digits.Length; i++)
            {
                if (i > 0 && i % 4 == 0) formatted += " ";
                formatted += digits[i];
            }

            if (formatted != tb.Text)
            {
                int caret = tb.CaretIndex;
                tb.Text = formatted;
                tb.CaretIndex = Math.Min(caret + 1, formatted.Length);
            }

            _updatingCard = false;
        }

        private void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Text = "";

            string carNumber = CarNumberTextBox.Text.Trim().ToUpper();
            if (string.IsNullOrWhiteSpace(carNumber))
            {
                ErrorMessage.Text = "❌ Введіть номер автомобіля";
                return;
            }
            if (!CarNumberRegex.IsMatch(carNumber))
            {
                ErrorMessage.Text = "❌ Неправильний формат номера авто. Приклад: КА2606НА";
                return;
            }

            if (SpaceNumberComboBox.SelectedIndex < 0)
            {
                ErrorMessage.Text = "❌ Виберіть місце паркування";
                return;
            }

            if (!TimeSpan.TryParse(StartTimeTextBox.Text, out var startTime) ||
                !TimeSpan.TryParse(EndTimeTextBox.Text, out var endTime))
            {
                ErrorMessage.Text = "❌ Неправильний формат часу. Приклад: 09:00";
                return;
            }

            if (endTime <= startTime)
            {
                ErrorMessage.Text = "❌ Час закінчення має бути пізніше початку";
                return;
            }

            // Card: digits only, must be 16
            string cardDigits = Regex.Replace(CardNumberTextBox.Text, @"\D", "");
            if (cardDigits.Length != 16)
            {
                ErrorMessage.Text = "❌ Номер картки має містити 16 цифр";
                return;
            }

            var bookingDate = BookingDatePicker.SelectedDate ?? DateTime.Today;
            var startDateTime = bookingDate.Date + startTime;
            var endDateTime = bookingDate.Date + endTime;

            int spaceNumber = (int)SpaceNumberComboBox.SelectedItem;

            var (success, booking, message) = _bookingService.CreateBooking(
                _currentUser.Id, _parkingLot.Id, spaceNumber,
                carNumber, startDateTime, endDateTime);

            if (success)
            {
                var receipt = _dbService.GetReceiptByNumber(booking.ReceiptNumber);
                if (receipt != null)
                {
                    ReceiptWindow receiptWindow = new ReceiptWindow(receipt);
                    receiptWindow.ShowDialog();
                    this.Close();
                }
            }
            else
            {
                ErrorMessage.Text = $"❌ {message}";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
