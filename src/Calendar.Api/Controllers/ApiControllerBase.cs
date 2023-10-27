using Asp.Versioning;
using Calendar.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Calendar.Api.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ApiControllerBase : ControllerBase
    {
        // Get userId from JWT token used to query our system.
        protected string? UserId
            => Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value.ToInternalId();
    }
}
