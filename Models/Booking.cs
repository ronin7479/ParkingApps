using System;

namespace ParkingApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ParkingLotId { get; set; }
        public int ParkingSpaceId { get; set; }
        public string CarNumber { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalPrice { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;  // Нова властивість

        public double DurationHours => (EndTime - StartTime).TotalHours;

        // Чи бронь зараз активна (час не вийшов)
        public bool IsCurrentlyActive => IsActive && EndTime > DateTime.Now;

        // Статус для відображення
        public string StatusText
        {
            get
            {
                if (!IsActive) return "Завершено";
                if (StartTime > DateTime.Now) return "Заплановано";
                if (EndTime > DateTime.Now) return "Активне";
                return "Завершено";
            }
        }

        public Booking()
        {
            CreatedDate = DateTime.Now;
            IsActive = true;
        }
    }
}
