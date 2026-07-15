using UserAuthService.Domain.Entities;

namespace UserAuthService.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}
