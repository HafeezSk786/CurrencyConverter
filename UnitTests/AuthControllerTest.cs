using CurrencyConverter.Authentication;
using CurrencyConverter.Models;
using Microsoft.AspNetCore.Mvc;

namespace UnitTests
{
    public class AuthControllerTest
    {
        [Fact]
        public void TestUsernameAndPassword()
        {
            //Arrange
            var userlogin = new UserLogin
            {
                Username = "test",
                Password = "test"
            };

            //Act
            AuthController _authController = new AuthController();
            var response = _authController.Login(userlogin);

            //Assert
            Assert.IsType<Task<IActionResult>>(response);
            Assert.Equal("test", userlogin.Username.ToLower());
            Assert.Equal("test", userlogin.Password.ToLower());
        }
    }
}