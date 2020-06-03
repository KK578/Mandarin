using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Extensions
{
    public static class MandarinAuthentication
    {
        public static void AddMandarinAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(MandarinAuthentication.ConfigureAuthentication)
                    .AddCookie()
                    .AddOpenIdConnect("Auth0", o => MandarinAuthentication.ConfigureOpenId(o, configuration));
        }

        private static void ConfigureAuthentication(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }

        [ExcludeFromCodeCoverage]
        private static void ConfigureOpenId(OpenIdConnectOptions options, IConfiguration configuration)
        {
            options.ClaimsIssuer = "Auth0";
            options.Authority = $"https://{configuration["Auth0:Domain"]}";
            options.ClientId = configuration["Auth0:ClientId"];
            options.ClientSecret = configuration["Auth0:ClientSecret"];

            options.ResponseType = "code";
            options.CallbackPath = new PathString("/callback");

            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut
            };

            static Task OnRedirectToIdentityProviderForSignOut(RedirectContext context)
            {
                var request = context.Request;
                var returnTo = $"https://{request.Host}{request.PathBase}{context.Properties.RedirectUri}";

                var logoutUri = new UriBuilder(context.Options.Authority);
                logoutUri.Path = "/v2/logout";
                logoutUri.Query = $"client_id={context.Options.ClientId}";
                logoutUri.Query += $"&returnTo={Uri.EscapeDataString(returnTo)}";

                context.Response.Redirect(logoutUri.ToString());
                context.HandleResponse();

                return Task.CompletedTask;
            }
        }
    }
}
