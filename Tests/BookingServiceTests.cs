using System;
using Xunit;
using ParkingApp.Models;
using ParkingApp.Services;

namespace ParkingApp.Tests
{
    public class BookingServiceTests
    {
        private readonly DatabaseService _dbService;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _dbService = new DatabaseService();
            _bookingService = new BookingService(_dbService, new PaymentService());
        }

        
        public void CreateBooking_WithValidData_ShouldSucceed()
        {
            // Arrange
            int userId = 1;
            int parkingLotId = 1;
            int spaceNumber = 1;
            string carNumber = "АА 1234 НА";
            var startTime = DateTime.Now.AddHours(1);
            var endTime = DateTime.Now.AddHours(3);

            // Act
            var (success, booking, message) = _bookingService.CreateBooking(
                userId, parkingLotId, spaceNumber, carNumber, startTime, endTime);

            // Assert
            Assert.True(success);
            Assert.NotNull(booking);
            Assert.Equal(carNumber, booking.CarNumber);
        }

        
        public void CreateBooking_WithOccupiedSpace_ShouldFail()
        {
            // Arrange
            var lot = _dbService.GetParkingLotById(1);
            var spaces = _dbService.GetParkingSpacesByLotId(1);
            var space = spaces[0];
            space.IsOccupied = true;
            _dbService.UpdateParkingSpace(space);

            // Act
            var (success, booking, message) = _bookingService.CreateBooking(
                1, 1, space.SpaceNumber, "АА 1234 НА", DateTime.Now, DateTime.Now.AddHours(2));

            // Assert
            Assert.False(success);
            Assert.Contains("зайнято", message.ToLower());
        }

        
        public void CreateBooking_WithInvalidTime_ShouldFail()
        {
            // Arrange
            var startTime = DateTime.Now.AddHours(3);
            var endTime = DateTime.Now.AddHours(1); // Час закінчення раніше за початок

            // Act
            var (success, booking, message) = _bookingService.CreateBooking(
                1, 1, 1, "АА 1234 НА", startTime, endTime);

            // Assert
            Assert.False(success);
        }
    }
}