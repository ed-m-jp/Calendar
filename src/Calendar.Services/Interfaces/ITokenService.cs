using Calendar.ServiceLayer;
using Calendar.Shared.Entities;

namespace Calendar.Services.Interfaces
{
    public interface ITokenService
    {
        ServiceResult<string> GetJwtTokenForUser(User user);
    }
}