using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mandarin.Pages
{
    /// <summary>
    /// Represents the backing model for the Logout page.
    /// </summary>
    [SuppressMessage("StyleCop", "SA1649", Justification = "Model is code behind for the Razor page.")]
    public class LogoutModel : PageModel
    {
        /// <summary>
        /// Called by the framework on the Logout page being rendered.
        /// This signs an authenticated user out from Auth0.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [ExcludeFromCodeCoverage]
        public async Task OnGet()
        {
            await this.HttpContext.SignOutAsync("Auth0", new AuthenticationProperties { RedirectUri = "/" });
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
