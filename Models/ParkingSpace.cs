using System;

namespace ParkingApp.Models
{
    public class ParkingSpace
    {
        public int Id { get; set; }
        public int ParkingLotId { get; set; }
        public int SpaceNumber { get; set; }      // Номер місця (1, 2, 3...)
        public bool IsOccupied { get; set; }      // Зайнято чи ні
        public DateTime CreatedDate { get; set; }

        public ParkingSpace()
        {
            CreatedDate = DateTime.Now;
            IsOccupied = false;
        }
    }
}