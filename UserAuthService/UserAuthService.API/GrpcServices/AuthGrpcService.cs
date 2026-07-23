using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using UserAuthService.Infrastructure.Persitence.Data;
using UserAuthService.Domain.Entities;

namespace UserAuthService.API.GrpcServices
{
    public class AuthGrpcService : AuthGrpc.AuthGrpcBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserAuthServiceDbContext _dbContext;

        public AuthGrpcService(
            IConfiguration configuration,
            UserAuthServiceDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public override async Task<TokenResponse> ValidateToken(TokenRequest request, ServerCallContext context)
        {
            try
            {
                var user = await GetValidatedUserAsync(request.Token, context.CancellationToken);
                if (user is null)
                {
                    return new TokenResponse { IsValid = false };
                }

                return new TokenResponse
                {
                    IsValid = true,
                    UserId = user.UserId.ToString(),
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch
            {
                return new TokenResponse { IsValid = false };
            }
        }

        private async Task<User?> GetValidatedUserAsync(string token, CancellationToken cancellationToken)
        {
            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secret))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            var email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                return null;
            }

            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    item => item.UserId == parsedUserId && item.Email == email,
                    cancellationToken);

            return user is null || user.Isactive == false ? null : user;
        }
    }
}
