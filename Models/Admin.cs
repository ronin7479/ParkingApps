namespace ParkingApp.Models
{
    public class Admin
    {
        public string Login { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public static readonly string ADMIN_LOGIN = "AdminParking";
        public static readonly string ADMIN_PASSWORD = "2026KRparking!";
    }
}