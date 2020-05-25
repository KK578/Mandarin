using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mandarin.Pages
{
    public class LogoutModel : PageModel
    {
        [ExcludeFromCodeCoverage]
        public async Task OnGet()
        {
            await this.HttpContext.SignOutAsync("Auth0", new AuthenticationProperties { RedirectUri = "/" });
            await this.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
