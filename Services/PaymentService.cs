using System;
using ParkingApp.Models;

namespace ParkingApp.Services
{
    public class PaymentService
    {
        // Валідація номера карти (просто базова перевірка)
        public bool ValidateCardNumber(string cardNumber)
        {
            // Видаляємо пробіли
            string cleaned = cardNumber.Replace(" ", "");

            // Перевіряємо що це 16 цифр
            if (cleaned.Length != 16)
                return false;

            // Перевіряємо що це лише цифри
            if (!System.Text.RegularExpressions.Regex.IsMatch(cleaned, "^[0-9]{16}$"))
                return false;

            return true;
        }

        // Обробка платежу (тестовий)
        public (bool success, string message) ProcessPayment(
            string cardNumber,
            decimal amount,
            string bookingReference)
        {
            // Валідація карти
            if (!ValidateCardNumber(cardNumber))
                return (false, "Недійсний номер карти");

            if (amount <= 0)
                return (false, "Сума повинна бути більше 0");

            // Тестовий платіж (реально ніщо не списується)
            // В реального додатку тут би була інтеграція з платіжною системою
            return (true, "Платіж успішно оброблений");
        }
    }
}