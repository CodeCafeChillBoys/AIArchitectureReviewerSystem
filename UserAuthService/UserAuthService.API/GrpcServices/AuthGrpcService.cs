using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace UserAuthService.API.GrpcServices
{
    public class AuthGrpcService : AuthGrpc.AuthGrpcBase
    {
        private readonly IConfiguration _configuration;

        public AuthGrpcService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override Task<TokenResponse> ValidateToken(TokenRequest request, ServerCallContext context)
        {
            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secret))
            {
                return Task.FromResult(new TokenResponse { IsValid = false });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secret);

            try
            {
                tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
                var email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;

                return Task.FromResult(new TokenResponse
                {
                    IsValid = true,
                    UserId = userId,
                    Email = email
                });
            }
            catch
            {
                return Task.FromResult(new TokenResponse { IsValid = false });
            }
        }
    }
}
