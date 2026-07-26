using System.Security.Claims;
using System.Text.Encodings.Web;
using DiagramManager.API.GrpcClients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace DiagramManager.API.Authentication
{
    public class GrpcAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "GrpcBearer";
        private readonly AuthGrpc.AuthGrpcClient _authClient;

        public GrpcAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            AuthGrpc.AuthGrpcClient authClient)
            : base(options, logger, encoder)
        {
            _authClient = authClient;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorization = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authorization) ||
                !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            var token = authorization["Bearer ".Length..].Trim();
            if (string.IsNullOrWhiteSpace(token))
            {
                return AuthenticateResult.Fail("Bearer token is missing.");
            }

            try
            {
                var response = await _authClient.ValidateTokenAsync(
                    new TokenRequest { Token = token },
                    cancellationToken: Context.RequestAborted);

                if (!response.IsValid)
                {
                    return AuthenticateResult.Fail("Token is invalid or expired.");
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, response.UserId),
                    new(ClaimTypes.Email, response.Email),
                    new(ClaimTypes.Role, response.Role.ToString())
                };
                var identity = new ClaimsIdentity(claims, SchemeName);
                var principal = new ClaimsPrincipal(identity);

                return AuthenticateResult.Success(
                    new AuthenticationTicket(principal, SchemeName));
            }
            catch (Exception exception)
            {
                Logger.LogWarning(exception, "Could not validate the bearer token through UserAuthService.");
                return AuthenticateResult.Fail("Authentication service is unavailable.");
            }
        }
    }
}
