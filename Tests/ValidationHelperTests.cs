using Xunit;
using ParkingApp.Utils;

namespace ParkingApp.Tests
{
    public class ValidationHelperTests
    {
        
        public void IsValidEmail_WithValidEmail_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";

            // Act
            bool result = ValidationHelper.IsValidEmail(email);

            // Assert
            Assert.True(result);
        }

        
        public void IsValidEmail_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            string email = "invalid-email";

            // Act
            bool result = ValidationHelper.IsValidEmail(email);

            // Assert
            Assert.False(result);
        }

        
        public void IsValidCardNumber_With16Digits_ReturnsTrue()
        {
            // Arrange
            string cardNumber = "1234567890123456";

            // Act
            bool result = ValidationHelper.IsValidCardNumber(cardNumber);

            // Assert
            Assert.True(result);
        }

        
        public void IsValidCardNumber_WithInvalidLength_ReturnsFalse()
        {
            // Arrange
            string cardNumber = "123456789"; // Менше 16 цифр

            // Act
            bool result = ValidationHelper.IsValidCardNumber(cardNumber);

            // Assert
            Assert.False(result);
        }
    }
}