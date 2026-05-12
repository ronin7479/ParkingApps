using System;

namespace ParkingApp.Utils
{
    public class DateTimeHelper
    {
        // Конвертуємо тривалість в годинах до красивого формату
        public static string FormatDuration(double hours)
        {
            if (hours < 1)
            {
                int minutes = (int)(hours * 60);
                return $"{minutes} хв";
            }
            else if (hours < 24)
            {
                return $"{hours:F1} год";
            }
            else
            {
                int days = (int)(hours / 24);
                int remainingHours = (int)(hours % 24);
                return $"{days}д {remainingHours}г";
            }
        }

        // Форматуємо дату в українському форматі
        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        // Форматуємо час
        public static string FormatTime(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm");
        }

        // Форматуємо дату і час разом
        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy HH:mm");
        }

        // Отримуємо назву дня тижня
        public static string GetDayName(DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => "Понеділок",
                DayOfWeek.Tuesday => "Вівторок",
                DayOfWeek.Wednesday => "Середа",
                DayOfWeek.Thursday => "Четвер",
                DayOfWeek.Friday => "П'ятниця",
                DayOfWeek.Saturday => "Субота",
                DayOfWeek.Sunday => "Неділя",
                _ => ""
            };
        }
    }
}