using System;

namespace ParkingApp.Utils
{
    public class ReceiptNumberGenerator
    {
        private static Random _random = new Random();
        private static int _counter = 0;

        // Генеруємо унікальний номер квитанції
        // Формат: РРРМДД + 4 цифри (рік + місяць + день + порядковий номер)
        public static string GenerateReceiptNumber()
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("D2");
            string day = DateTime.Now.Day.ToString("D2");

            _counter++;
            string number = _counter.ToString("D4");

            return year + month + day + number;
        }

        // Пример: 20262704228 означає 2026 рік, 27 число, 4 місяць (квітень), квитанція №228
    }
}