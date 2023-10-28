using Calendar.Services.Interfaces;
using Calendar.Shared.Models.WebApi.Requests;
using Calendar.Shared.Models.WebApi.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Calendar.Api.Controllers
{
    [Route("api/[controller]")]
    public class AccountController: ApiControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Login.
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest loginRequest)
        {
           var loginResult = await _userService.Login(loginRequest);

            if (loginResult.IsOk)
                return Ok(loginResult.Data);
            else if (loginResult.IsUnauthorized)
                return Unauthorized();
            else if (loginResult.IsBadRequest)
                return BadRequest(loginResult.ErrorMessage);

            throw new ApplicationException(loginResult.ErrorMessage);
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var logoutResult = await _userService.Logout();

            if (logoutResult.IsOk)
                return NoContent();

            throw new ApplicationException(logoutResult.ErrorMessage);
        }

        /// <summary>
        /// Register new user.
        /// </summary>
        /// <param name="registerRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register(RegisterUserRequest registerRequest)
        {
            var registerResult = await _userService.Register(registerRequest);

            if (registerResult.IsOk)
                return Ok(registerResult.Data);
            else if (registerResult.IsBadRequest)
                return BadRequest(registerResult.ErrorMessage);

            throw new ApplicationException(registerResult.ErrorMessage);
        }
    }
}
