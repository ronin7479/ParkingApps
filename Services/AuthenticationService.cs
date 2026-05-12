using System;
using System.Security.Cryptography;
using System.Text;
using ParkingApp.Models;

namespace ParkingApp.Services
{
    public class AuthenticationService
    {
        private readonly DatabaseService _dbService;

        public AuthenticationService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public void EnsureTestUserExists()
        {
            if (!_dbService.UserExists("testuser"))
                Register("test@parking.ua", "testuser", "test123");
        }

        public bool Register(string email, string login, string password)
        {
            if (_dbService.UserExists(login)) return false;
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(login) || password.Length < 6)
                return false;
            var newUser = new User
            {
                Email = email,
                Login = login,
                PasswordHash = HashPassword(password),
                AvatarPath = "",
                SuccessfulBookings = 0
            };
            return _dbService.AddUser(newUser);
        }

        public User Login(string login, string password)
        {
            var user = _dbService.GetUserByLogin(login);
            if (user == null) return null;
            if (!VerifyPassword(password, user.PasswordHash)) return null;
            return user;
        }

        public bool AdminLogin(string login, string password)
        {
            return login == Admin.ADMIN_LOGIN && password == Admin.ADMIN_PASSWORD;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
            => HashPassword(password) == hash;
    }
}
