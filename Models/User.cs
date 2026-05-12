using System;

namespace ParkingApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;  
        public string AvatarPath { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int SuccessfulBookings { get; set; } = 0;  
        public User()
        {
            CreatedDate = DateTime.Now;
            SuccessfulBookings = 0;
        }
    }
}