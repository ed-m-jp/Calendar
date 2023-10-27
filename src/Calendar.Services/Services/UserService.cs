using Calendar.ServiceLayer.Interfaces;
using Calendar.Shared.Entities;
using Calendar.Shared.Extensions;
using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Calendar.ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User>      _userManager;
        private readonly IConfiguration         _configuration;
        private readonly IHttpContextAccessor   _httpContextAccessor;
        private readonly SignInManager<User>    _signInManager;
        private readonly ILogger<UserService>   _logger;

        public UserService(UserManager<User> userManager, IConfiguration configuration,IHttpContextAccessor httpContextAccessor,
            SignInManager<User> signInManager, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
            _logger = logger;
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

                await _signInManager.SignInAsync(user, isPersistent: true);

                var token = GetJwtTokenForUser(user);

                return ServiceResult<LoginResponse>.Ok(BuildLoginResponse(user, token));
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResult<LoginResponse>.Unauthorized(ex.Message);
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
            } catch (Exception ex)
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

                await _signInManager.SignInAsync(newUser, isPersistent: true);

                var token = GetJwtTokenForUser(newUser);

                return ServiceResult<LoginResponse>.Ok(BuildLoginResponse(newUser, token));
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResult<LoginResponse>.Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error trying to register new user.");
                return ServiceResult<LoginResponse>.Error("An error happened during registration process. Please try again later.");
            }
        }

        private string GetJwtTokenForUser(User user)
        {
            var signingCredentials = GetTokenSigningCredentials();
            var claims = GetUserClaims(user);
            var tokenOptions = GenerateJwtTokenOptions(signingCredentials, claims);
            
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);            
        }

        private SigningCredentials GetTokenSigningCredentials()
        {
            var jwtConfig = _configuration.GetSection("jwtConfig");
            var key = Encoding.UTF8.GetBytes(jwtConfig["Secret"]!);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        private List<Claim> GetUserClaims(User user)
        {
            var encodedId = user.Id.ToPublicId();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, encodedId)
            };

            return claims;
        }

        private JwtSecurityToken GenerateJwtTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtConfig");
            var currentAudience = DetermineAudienceBasedOnRequestOrigin(jwtSettings);

            var tokenOptions = new JwtSecurityToken
            (
                issuer: jwtSettings["validIssuer"],
                audience: currentAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expiresIn"])),
                signingCredentials: signingCredentials
            );

            return tokenOptions;
        }

        private string DetermineAudienceBasedOnRequestOrigin(IConfigurationSection jwtSettings)
        {
            var origin = _httpContextAccessor.HttpContext.Request.Headers["Origin"].ToString();
            var validAudiences = jwtSettings.GetSection("validAudiences").Get<List<string>>();

            if (validAudiences!.Contains(origin))
            {
                return origin;
            }

            throw new UnauthorizedAccessException("Unauthorized origin.");
        }

        private LoginResponse BuildLoginResponse(User user, string token)
            => new LoginResponse(user.UserName!, token);
    }
}
