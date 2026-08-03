using UserAuthService.Domain.Entities;

namespace UserAuthService.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
}
