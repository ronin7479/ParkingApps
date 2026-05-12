using System;

namespace ParkingApp.Models
{
    public class Receipt
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; }
        public int BookingId { get; set; }
        public string CarNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Amount { get; set; }
        public string ParkingLotName { get; set; }
        public int SpaceNumber { get; set; }
        public DateTime PaymentDate { get; set; }

        public Receipt()
        {
            PaymentDate = DateTime.Now;
        }
    }
}