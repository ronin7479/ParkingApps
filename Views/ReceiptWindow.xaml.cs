using System.Windows;
using ParkingApp.Models;

namespace ParkingApp.Views
{
    public partial class ReceiptWindow : Window
    {
        private readonly Receipt _receipt;

        public ReceiptWindow(Receipt receipt)
        {
            InitializeComponent();
            _receipt = receipt;
            LoadReceiptData();
        }

        private void LoadReceiptData()
        {
            ReceiptNumberText.Text = _receipt.ReceiptNumber ?? "N/A";
            PaymentDateText.Text = _receipt.PaymentDate.ToString("dd.MM.yyyy  HH:mm");
            ParkingNameText.Text = _receipt.ParkingLotName ?? "N/A";
            SpaceNumberText.Text = _receipt.SpaceNumber.ToString();
            CarNumberText.Text = _receipt.CarNumber ?? "N/A";
            AmountText.Text = $"{_receipt.Amount:F2} грн";
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
