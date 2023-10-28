using Calendar.Services.Interfaces;
using Calendar.Shared.Entities;
using Calendar.Shared.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Calendar.Services.Services
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<TokenService> logger)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public ServiceResult<string> GetJwtTokenForUser(User user)
        {
            try
            {
                var signingCredentials = GetTokenSigningCredentials();
                var claims = GetUserClaims(user);
                var tokenOptions = GenerateJwtTokenOptions(signingCredentials, claims);

                return ServiceResult<string>.Ok(new JwtSecurityTokenHandler().WriteToken(tokenOptions));
            }
            catch (UnauthorizedAccessException ex)
            {
                return ServiceResult<string>.Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error trying to get a token for user: [{user.Id}]");
                return ServiceResult<string>.Error($"An error trying to get a token for user: [{user.Id}]");
            }
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
    }
}
