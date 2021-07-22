using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Extensions
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> to register authentication services.
    /// </summary>
    public static class MandarinAuthentication
    {
        /// <summary>
        /// Adds JWT Authentication via Auth0.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <param name="configuration">Application configuration for configuring services.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(ConfigureAuthentication)
                    .AddCookie()
                    .AddJwtBearer(ConfigureJwt)
                    .AddOpenIdConnect("Auth0", ConfigureOpenId);

            return services;

            static void ConfigureAuthentication(AuthenticationOptions options)
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }

            void ConfigureJwt(JwtBearerOptions o)
            {
                o.Authority = $"https://{configuration["Auth0:Domain"]}";
                o.Audience = configuration["Auth0:Audience"];
            }

            void ConfigureOpenId(OpenIdConnectOptions options)
            {
                options.ClaimsIssuer = "Auth0";
                options.Authority = $"https://{configuration["Auth0:Domain"]}";
                options.ClientId = configuration["Auth0:ClientId"];
                options.ClientSecret = configuration["Auth0:ClientSecret"];

                options.ResponseType = "code";
                options.CallbackPath = new PathString("/callback");
            }
        }

        /// <summary>
        /// Adds Authorization policies for accessing privileged services.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinAuthorization(this IServiceCollection services)
        {
            return services.AddAuthorization(ConfigureAuthorization);

            static void ConfigureAuthorization(AuthorizationOptions options)
            {
                options.AddPolicy("Hangfire", AddHangfirePolicy);
            }

            static void AddHangfirePolicy(AuthorizationPolicyBuilder builder)
            {
                builder.AddAuthenticationSchemes("Auth0").RequireAuthenticatedUser();
            }
        }
    }
}
