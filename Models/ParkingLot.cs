using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingApp.Models
{
    [Table("ParkingLots")]
    public class ParkingLot
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [MaxLength(255)]
        public string ImagePath { get; set; } = string.Empty;

        public int TotalSpaces { get; set; }
        public int OccupiedSpaces { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PricePerHour { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [NotMapped]
        public int FreeSpaces => TotalSpaces - OccupiedSpaces;

        [NotMapped]
        public string OccupancyColor
        {
            get
            {
                double pct = (double)OccupiedSpaces / TotalSpaces * 100;
                if (pct <= 30) return "#00CC44";
                if (pct <= 70) return "#FFA500";
                return "#FF4444";
            }
        }

        public ParkingLot() { CreatedDate = DateTime.Now; }
    }
}
