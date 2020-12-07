using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Server.Extensions
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
        public static IServiceCollection AddMandarinAuthentication(this IServiceCollection services,
                                                                   IConfiguration configuration)
        {
            services.AddAuthentication(MandarinAuthentication.ConfigureAuthentication)
                    .AddJwtBearer(options => MandarinAuthentication.ConfigureJwt(options, configuration));

            return services;
        }

        private static void ConfigureAuthentication(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }

        private static void ConfigureJwt(JwtBearerOptions options, IConfiguration configuration)
        {
            options.Authority = options.Authority = $"https://{configuration["Auth0:Domain"]}";
            options.Audience = configuration["Auth0:Audience"];
        }
    }
}
