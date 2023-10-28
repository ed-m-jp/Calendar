using Calendar.Services;
using Calendar.Services.Interfaces;
using Calendar.Services.Services;
using Calendar.Services.Tests.Helpers;
using Calendar.Shared.Entities;
using Calendar.Shared.Models.WebApi.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace Calendar.Service.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly Mock<ITokenService> _mockTokenService;

        private readonly User _user = new User { UserName = "testUsername" };

        const string TOKEN = "mockJwtToken";

        public UserServiceTests()
        {
            _mockUserManager = MockHelper.MockUserManager<User>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockSignInManager = new Mock<SignInManager<User>>(_mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null);

            _mockTokenService = new Mock<ITokenService>();
        }

        [TestMethod]
        public async Task Login_ValidUser_ReturnsLoginResponse()
        {
            // Arrange
            var userService = new UserService(_mockLogger.Object,
                _mockTokenService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);

            var loginRequest = new LoginRequest
            {
                Username = "testUsername",
                Password = "testPassword1",
            };

            _mockUserManager
                .Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockSignInManager
                .Setup(sim => sim.SignInAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            _mockTokenService
                .Setup(ts => ts.GetJwtTokenForUser(It.IsAny<User>()))
                .Returns(ServiceResult<string>.Ok(TOKEN));

            // Act
            var result = await userService.Login(loginRequest);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(result.Data!.Token, TOKEN);
            Assert.AreEqual(result.Data!.Username, _user.UserName);
        }

        [TestMethod]
        public async Task Login_WrongUsername_ReturnsBadRequest()
        {
            // Arrange
            var userService = new UserService(_mockLogger.Object,
                _mockTokenService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);

            var loginRequest = new LoginRequest
            {
                Username = "testUsername",
                Password = "testPassword1",
            };

           _mockUserManager
                .Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync((User)null);

            // Act
            var result = await userService.Login(loginRequest);

            // Assert
            Assert.IsTrue(result.IsBadRequest);
            Assert.AreEqual(result.ErrorMessage, "Username or password is incorrect");
        }

        [TestMethod]
        public async Task Login_WrongPassword_ReturnsBadRequest()
        {
            // Arrange
            var userService = new UserService(_mockLogger.Object,
                _mockTokenService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);

            var loginRequest = new LoginRequest
            {
                Username = "testUsername",
                Password = "testPassword1",
            };

            _mockUserManager
                .Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var result = await userService.Login(loginRequest);

            // Assert
            Assert.IsTrue(result.IsBadRequest);
            Assert.AreEqual(result.ErrorMessage, "Username or password is incorrect");
        }

        [TestMethod]
        public async Task Login_UnauthorizedDomain_ReturnsUnauthorized()
        {
            // Arrange
            var userService = new UserService(_mockLogger.Object,
                _mockTokenService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);

            var loginRequest = new LoginRequest
            {
                Username = "testUsername",
                Password = "testPassword1",
            };

            _mockUserManager
                .Setup(um => um.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(_user);

            _mockUserManager
                .Setup(um => um.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            _mockTokenService
                .Setup(ts => ts.GetJwtTokenForUser(It.IsAny<User>()))
                .Returns(ServiceResult<string>.Unauthorized("Unauthorized origin."));

            // Act
            var result = await userService.Login(loginRequest);

            // Assert
            Assert.IsTrue(result.IsUnauthorized);
            Assert.AreEqual(result.ErrorMessage, "Unauthorized origin.");
        }

        [TestMethod]
        public async Task Register_SuccessfulRegistration_ReturnsLoginResponse()
        {
            // Arrange
            var userService = new UserService(_mockLogger.Object,
                _mockTokenService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);

            var registerRequest = new RegisterUserRequest
            {
                Username = "testUsername",
                Password = "testPassword1",
            };

            _mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockTokenService
                .Setup(ts => ts.GetJwtTokenForUser(It.IsAny<User>()))
                .Returns(ServiceResult<string>.Ok(TOKEN));

            // Act
            var result = await userService.Register(registerRequest);

            // Assert
            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(result.Data!.Token, TOKEN);
            Assert.AreEqual(result.Data!.Username, registerRequest.Username);
        }

        [TestMethod]
        public async Task Register_RegistrationFailed_ReturnsBadRequest()
        {
            // Arrange
            var userService = new UserService(_mockLogger.Object,
                _mockTokenService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);

            var registerRequest = new RegisterUserRequest
            {
                Username = "testUsername",
                Password = "testPassword1",
            };

            var identityError = new IdentityError { Description = "Error" };
            _mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(identityError));

            // Act
            var result = await userService.Register(registerRequest);

            // Assert
            Assert.IsTrue(result.IsBadRequest);
            Assert.AreEqual(result.ErrorMessage, "Error");
        }

        [TestMethod]
        public async Task Register_UnauthorizedDomain_ReturnsUnauthorized()
        {
            // Arrange
            var userService = new UserService(_mockLogger.Object,
                _mockTokenService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);

            var registerRequest = new RegisterUserRequest
            {
                Username = "testUsername",
                Password = "testPassword1",
            };

            _mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _mockTokenService
                .Setup(ts => ts.GetJwtTokenForUser(It.IsAny<User>()))
                .Returns(ServiceResult<string>.Unauthorized("Unauthorized origin."));

            // Act
            var result = await userService.Register(registerRequest);

            // Assert
            Assert.IsTrue(result.IsUnauthorized);
            Assert.AreEqual(result.ErrorMessage, "Unauthorized origin.");
        }

        [TestMethod]
        public async Task Register_GeneralErrorDuringRegistration_ReturnsError()
        {
            // Arrange
            var userService = new UserService(_mockLogger.Object,
                _mockTokenService.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);

            var registerRequest = new RegisterUserRequest
            {
                Username = "testUsername",
                Password = "testPassword1",
            };

            _mockUserManager
                .Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .Throws(new Exception("Error"));

            // Act
            var result = await userService.Register(registerRequest);

            // Assert
            Assert.IsTrue(result.IsError);
            Assert.AreEqual(result.ErrorMessage, "An error happened during registration process. Please try again later.");
        }

    }
}
