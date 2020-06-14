using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mandarin.Pages
{
    /// <summary>
    /// Represents the backing model for the Login page.
    /// </summary>
    [SuppressMessage("StyleCop", "SA1649", Justification = "Model is code behind for the Razor page.")]
    public class LoginModel : PageModel
    {
        /// <summary>
        /// Called by the framework on the Logout page being rendered.
        /// This challenges the user to sign into Auth0.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [ExcludeFromCodeCoverage]
        public async Task OnGet()
        {
            await this.HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
