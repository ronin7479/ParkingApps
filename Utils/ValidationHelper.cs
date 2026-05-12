using System;
using System.Text.RegularExpressions;

namespace ParkingApp.Utils
{
    public class ValidationHelper
    {
        // Email: має бути формат xxx@xxx.xx
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                // Додаткова перевірка через regex — має бути крапка в домені
                return addr.Address == email &&
                       Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$");
            }
            catch { return false; }
        }

        // Логін: 3-20 символів, латиниця + цифри + _
        public static bool IsValidLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login)) return false;
            if (login.Length < 3 || login.Length > 20) return false;
            return Regex.IsMatch(login, @"^[a-zA-Z0-9_]+$");
        }

        // Пароль: мінімум 6 символів
        public static bool IsValidPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && password.Length >= 6;
        }

        // Номер авто БЕЗ пробілів: КА2606НА (кирилиця або латиниця)
        public static bool IsValidCarNumber(string carNumber)
        {
            if (string.IsNullOrWhiteSpace(carNumber)) return false;
            // Формат: 2 літери + 4 цифри + 2 літери, без пробілів
            return Regex.IsMatch(carNumber,
                @"^[A-ZА-ЯІЇЄ]{2}\d{4}[A-ZА-ЯІЇЄ]{2}$");
        }

        // Номер картки: рівно 16 цифр (пробіли ігноруються)
        public static bool IsValidCardNumber(string cardNumber)
        {
            string cleaned = cardNumber?.Replace(" ", "") ?? "";
            return Regex.IsMatch(cleaned, @"^\d{16}$");
        }

        // Час у форматі HH:mm
        public static bool IsValidTime(string time)
        {
            return TimeSpan.TryParse(time, out _);
        }

        // Час закінчення має бути пізніше початку
        public static bool IsValidTimeRange(TimeSpan startTime, TimeSpan endTime)
        {
            return endTime > startTime;
        }
    }
}