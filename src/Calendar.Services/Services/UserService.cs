using Calendar.Services.Interfaces;
using Calendar.Shared.Entities;
using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Calendar.Services.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService>   _logger;
        private readonly ITokenService          _tokenService;
        private readonly SignInManager<User>    _signInManager;
        private readonly UserManager<User>      _userManager;

        public UserService(ILogger<UserService> logger,
            ITokenService tokenService,
            SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            _logger = logger;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ServiceResult<LoginResponse>> Login(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginRequest.Username);

                if (user == null)
                    return ServiceResult<LoginResponse>.BadRequest("Username or password is incorrect");

                var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

                if (!result)
                    return ServiceResult<LoginResponse>.BadRequest("Username or password is incorrect");

                var getTokenResult = _tokenService.GetJwtTokenForUser(user);

                if (getTokenResult.IsOk)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return ServiceResult<LoginResponse>.Ok(BuildLoginResponse(user, getTokenResult.Data!));
                }
                else if (getTokenResult.IsUnauthorized)
                    return ServiceResult<LoginResponse>.Unauthorized(getTokenResult.ErrorMessage);

                return ServiceResult<LoginResponse>.Error("An error happened during login process. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to login user.");
                return ServiceResult<LoginResponse>.Error("An error happened during login process. Please try again later.");
            }
        }

        public async Task<ServiceResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();

                return ServiceResult.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to logout user.");
                return ServiceResult.Error("An error happened during logout process.");
            }
        }

        public async Task<ServiceResult<LoginResponse>> Register(RegisterUserRequest registerRequest)
        {
            try
            {
                var newUser = new User { UserName = registerRequest.Username };

                var createResult = await _userManager.CreateAsync(newUser, registerRequest.Password);

                if (!createResult.Succeeded)
                {
                    return ServiceResult<LoginResponse>.BadRequest(string.Join(", ", createResult.Errors.Select(x => x.Description)));
                }

                var getTokenResult = _tokenService.GetJwtTokenForUser(newUser);

                if (getTokenResult.IsOk)
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: true);
                    return ServiceResult<LoginResponse>.Ok(BuildLoginResponse(newUser, getTokenResult.Data!));
                }
                else if (getTokenResult.IsUnauthorized)
                    return ServiceResult<LoginResponse>.Unauthorized(getTokenResult.ErrorMessage);

                return ServiceResult<LoginResponse>.Error("An error happened during login process. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to register new user.");
                return ServiceResult<LoginResponse>.Error("An error happened during registration process. Please try again later.");
            }
        }

        private LoginResponse BuildLoginResponse(User user, string token)
            => new LoginResponse(user.UserName!, token);
    }
}
