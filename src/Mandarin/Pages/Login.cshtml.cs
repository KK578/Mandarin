using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mandarin.Pages
{
    public class LoginModel : PageModel
    {
        [ExcludeFromCodeCoverage]
        public async Task OnGet()
        {
            await this.HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}
