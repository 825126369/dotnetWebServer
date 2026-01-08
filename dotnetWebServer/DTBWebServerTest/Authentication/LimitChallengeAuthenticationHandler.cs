using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;

namespace DTBWebServer.Authentication
{
    public class LimitChallengeAuthenticationHandler : IAuthenticationHandler
    {
        public const string SchemeName = "LimitChallenge";
        AuthenticationScheme _scheme;
        HttpContext _context;

        JwtConfig jwtconfig;
        public LimitChallengeAuthenticationHandler(IOptions<JwtConfig> option)
        {
            jwtconfig = option.Value;
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            var req = _context.Request.Headers;
            var LCPwd = req["PWD"].FirstOrDefault();
            if (LCPwd != jwtconfig.pwd)
            {
                return Task.FromResult(AuthenticateResult.Fail("Pwd InCorrect!"));
            }

            AuthenticationTicket ticket = GetAuthTicket("test", "test");
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        public Task ChallengeAsync(AuthenticationProperties? properties)
        {
            _context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties? properties)
        {
            _context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            _scheme = scheme;
            _context = context;
            return Task.CompletedTask;
        }

        private AuthenticationTicket GetAuthTicket(string name, string role)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity("name");
            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationTicket(principal, _scheme.Name);
        }
    }
}