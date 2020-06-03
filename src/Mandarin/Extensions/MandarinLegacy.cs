using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Mandarin.Extensions
{
    public static class MandarinLegacy
    {
        public static void AddLegacyRedirect(this IApplicationBuilder app, string legacyPath, string redirectPath)
        {
            app.Map(legacyPath, x => x.Run(HandleRedirect));

            Task HandleRedirect(HttpContext c)
            {
                c.Response.Redirect(redirectPath);
                return Task.CompletedTask;
            }
        }
    }
}
