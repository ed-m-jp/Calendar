using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Calendar.Services.Tests.Helpers
{
    public static class MockHelper
    {
        public static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());
            return mgr;
        }

        public static SignInManager<TUser> TestSignInManager<TUser>(UserManager<TUser> userManager, IHttpContextAccessor contextAccessor) where TUser : class
        {
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>().Object;
            var loggerSignInManager = new Mock<ILogger<SignInManager<TUser>>>().Object;
            var schemes = new Mock<IAuthenticationSchemeProvider>().Object;
            var options = new Mock<IOptions<IdentityOptions>>().Object;

            return new SignInManager<TUser>(userManager, contextAccessor, claimsFactory, options, loggerSignInManager, schemes);
        }
    }
}
