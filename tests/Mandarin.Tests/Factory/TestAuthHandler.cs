using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mandarin.Tests.Factory
{
    internal class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string AuthenticationScheme = "Bearer";

        public static readonly AuthenticationHeaderValue AuthorizedToken = new(TestAuthHandler.AuthenticationScheme, "AuthorizedToken");

        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                               ILoggerFactory logger,
                               UrlEncoder encoder,
                               ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var header = this.Context.Request.Headers["Authorization"];
            if (header.Any(x => TestAuthHandler.AuthorizedToken.Equals(AuthenticationHeaderValue.Parse(x))))
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "MandarinTest") };
                var identity = new ClaimsIdentity(claims, "MandarinTestIdentity");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Test");

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
            else
            {
                var result = AuthenticateResult.Fail("Your authorization token is not valid.");
                return Task.FromResult(result);
            }
        }
    }
}
