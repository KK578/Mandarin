using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Mandarin.Extensions
{
    /// <summary>
    /// Extensions to <see cref="IApplicationBuilder"/> to register legacy redirect routes.
    /// </summary>
    public static class MandarinLegacy
    {
        /// <summary>
        /// Branches the request pipeline based on matches of the given <paramref name="legacyPath"/>. If the request path starts with
        /// the given path, the branch is redirected to the <paramref name="redirectPath"/>.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance.</param>
        /// <param name="legacyPath">The request path to match.</param>
        /// <param name="redirectPath">The path to redirect to.</param>
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
