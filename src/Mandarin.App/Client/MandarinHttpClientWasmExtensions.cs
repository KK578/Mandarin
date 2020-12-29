using System;
using Mandarin.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.App.Client
{
    /// <summary>
    /// Extensions for including Mandarin REST Client services to a <see cref="WebAssemblyHostBuilder" />.
    /// </summary>
    internal static class MandarinHttpClientWasmExtensions
    {
        /// <summary>
        /// Adds the shared Mandarin Services implemented to communicate via REST.
        /// </summary>
        /// <param name="builder">The <see cref="WebAssemblyHostBuilder"/> instance to register the services into.</param>
        /// <returns>The <see cref="WebAssemblyHostBuilder"/> returned as is, for chaining calls.</returns>
        public static WebAssemblyHostBuilder AddMandarinClient(this WebAssemblyHostBuilder builder)
        {
            builder.Services.AddScoped<JwtHttpMessageHandler>();
            builder.Services.AddHttpClient("Mandarin", options => options.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                   .AddHttpMessageHandler<JwtHttpMessageHandler>();

            builder.Services.AddTransient<MandarinHttpClient>();
            builder.Services.AddTransient<IArtistService, MandarinRestfulArtistService>();

            return builder;
        }
    }
}
