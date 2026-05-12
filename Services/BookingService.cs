using System;
using System.Collections.Generic;
using ParkingApp.Models;

namespace ParkingApp.Services
{
    public class BookingService
    {
        private readonly DatabaseService _dbService;
        private readonly PaymentService _paymentService;

        public BookingService(DatabaseService dbService, PaymentService paymentService)
        {
            _dbService = dbService;
            _paymentService = paymentService;
        }

        public (bool success, Booking? booking, string message) CreateBooking(
            int userId,
            int parkingLotId,
            int spaceNumber,
            string carNumber,
            DateTime startTime,
            DateTime endTime)
        {
            try
            {
                var parkingLot = _dbService.GetParkingLotById(parkingLotId);
                if (parkingLot == null)
                    return (false, null, "Паркінг не знайдений");

                var spaces = _dbService.GetParkingSpacesByLotId(parkingLotId);
                var space = spaces.Find(s => s.SpaceNumber == spaceNumber);

                if (space == null)
                    return (false, null, "Місце паркування не знайдене");

                if (space.IsOccupied)
                    return (false, null, "Місце вже зайнято");

                if (endTime <= startTime)
                    return (false, null, "Неправильний час");

                double durationHours = (endTime - startTime).TotalHours;
                decimal totalPrice = (decimal)durationHours * parkingLot.PricePerHour;

                var booking = new Booking
                {
                    UserId = userId,
                    ParkingLotId = parkingLotId,
                    ParkingSpaceId = space.Id,
                    CarNumber = carNumber,
                    BookingDate = DateTime.Now,
                    StartTime = startTime,
                    EndTime = endTime,
                    TotalPrice = totalPrice,
                    ReceiptNumber = GenerateReceiptNumber(),
                    IsActive = true  // Нова властивість - бронь активна
                };

                _dbService.AddBooking(booking);

                space.IsOccupied = true;
                _dbService.UpdateParkingSpace(space);

                parkingLot.OccupiedSpaces++;
                _dbService.UpdateParkingLot(parkingLot);

                CreateReceipt(booking, parkingLot);

                // ВИПРАВЛЕННЯ: зберігаємо SuccessfulBookings в БД
                var user = _dbService.GetUserById(userId);
                if (user != null)
                {
                    user.SuccessfulBookings++;
                    _dbService.UpdateUser(user);
                }

                return (true, booking, "Бронювання успішно створене");
            }
            catch (Exception ex)
            {
                return (false, null, $"Помилка: {ex.Message}");
            }
        }

        // Перевіряємо та звільняємо прострочені бронювання
        public void ReleaseExpiredBookings()
        {
            try
            {
                var allBookings = _dbService.GetAllBookings();
                var now = DateTime.Now;

                foreach (var booking in allBookings)
                {
                    // Якщо час закінчення пройшов і бронь ще активна
                    if (booking.IsActive && booking.EndTime <= now)
                    {
                        // Звільняємо місце паркування
                        var space = _dbService.GetParkingSpace(booking.ParkingSpaceId);
                        if (space != null && space.IsOccupied)
                        {
                            space.IsOccupied = false;
                            _dbService.UpdateParkingSpace(space);

                            // Зменшуємо кількість зайнятих місць
                            var lot = _dbService.GetParkingLotById(booking.ParkingLotId);
                            if (lot != null && lot.OccupiedSpaces > 0)
                            {
                                lot.OccupiedSpaces--;
                                _dbService.UpdateParkingLot(lot);
                            }
                        }

                        // Позначаємо бронь як неактивну
                        booking.IsActive = false;
                        _dbService.UpdateBooking(booking);
                    }
                }
            }
            catch { }
        }

        private string GenerateReceiptNumber()
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString("D2");
            string day = DateTime.Now.Day.ToString("D2");
            string number = new Random().Next(100, 999).ToString();
            return year + month + day + number;
        }

        private void CreateReceipt(Booking booking, ParkingLot parkingLot)
        {
            var spaceObj = _dbService.GetParkingSpace(booking.ParkingSpaceId);
            var receipt = new Receipt
            {
                ReceiptNumber = booking.ReceiptNumber,
                BookingId = booking.Id,
                CarNumber = booking.CarNumber,
                BookingDate = booking.BookingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Amount = booking.TotalPrice,
                ParkingLotName = parkingLot.Name,
                SpaceNumber = spaceObj?.SpaceNumber ?? 0
            };
            _dbService.AddReceipt(receipt);
        }

        public List<Booking> GetUserBookings(int userId)
        {
            return _dbService.GetUserBookings(userId);
        }
    }
}
