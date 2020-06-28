using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Mandarin.Extensions
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> to register authentication services.
    /// </summary>
    public static class MandarinAuthentication
    {
        /// <summary>
        /// Adds cookie based authentication and OpenId via Auth0.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <param name="configuration">Application configuration for configuring services.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(MandarinAuthentication.ConfigureAuthentication)
                    .AddCookie()
                    .AddOpenIdConnect("Auth0", o => MandarinAuthentication.ConfigureOpenId(o, configuration));

            return services;
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
            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
            };

            options.Events = new OpenIdConnectEvents
            {
                OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOut,
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
