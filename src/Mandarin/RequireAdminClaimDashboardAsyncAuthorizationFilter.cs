using System.Threading.Tasks;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;

namespace Mandarin
{
    /// <summary>
    /// Represents a <see cref="IDashboardAsyncAuthorizationFilter"/> that requires an admin token.
    /// </summary>
    public class RequireAdminClaimDashboardAsyncAuthorizationFilter : IDashboardAsyncAuthorizationFilter
    {
        /// <inheritdoc />
        public async Task<bool> AuthorizeAsync(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            await httpContext.ChallengeAsync("Bearer", new AuthenticationProperties { RedirectUri = httpContext.Request.Path });

            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}
