using System;
using System.Windows;
using System.Windows.Threading;
using ParkingApp.Services;

namespace ParkingApp
{
    public partial class App : Application
    {
        private DispatcherTimer? _bookingTimer;
        private DatabaseService? _dbService;
        private BookingService? _bookingService;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Ініціалізуємо сервіси
            _dbService = new DatabaseService();
            _bookingService = new BookingService(_dbService, new PaymentService());

            // Запускаємо таймер - перевіряємо кожну хвилину
            _bookingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _bookingTimer.Tick += (s, args) =>
            {
                _bookingService.ReleaseExpiredBookings();
            };
            _bookingTimer.Start();

            // Перевіряємо одразу при старті
            _bookingService.ReleaseExpiredBookings();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _bookingTimer?.Stop();
            base.OnExit(e);
        }
    }
}
