using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.App.Extensions
{
    /// <summary>
    /// Extensions for including Mandarin Authentication services to a <see cref="WebAssemblyHostBuilder"/>.
    /// </summary>
    internal static class MandarinAuthenticationWasmExtensions
    {
        /// <summary>
        /// Adds the configuration for Authentication with the Mandarin API.
        /// </summary>
        /// <param name="builder">The <see cref="WebAssemblyHostBuilder"/> instance to register the services into.</param>
        /// <returns>The <see cref="WebAssemblyHostBuilder"/> returned as is, for chaining calls.</returns>
        public static IRemoteAuthenticationBuilder<RemoteAuthenticationState, RemoteUserAccount> AddMandarinAuthentication(this WebAssemblyHostBuilder builder)
        {
            return builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Auth0", options.ProviderOptions);
                options.ProviderOptions.ResponseType = "code";
            });
        }
    }
}
