using System;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Mandarin.Client.Services;
using Mandarin.Grpc.Converters;
using Mandarin.ViewModels;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.App
{
    /// <summary>
    /// Application entrypoint for Mandarin WebAssembly App.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entrypoint for Mandarin WebAssembly App.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A <see cref="Task"/> representing the application running state.</returns>
        public static Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBlazorise(o => o.DelayTextOnKeyPress = true).AddBootstrapProviders().AddFontAwesomeIcons();
            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Auth0", options.ProviderOptions);
                options.ProviderOptions.ResponseType = "code";
            });
            builder.Services.AddMandarinClientServices(new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddMandarinViewModels();
            builder.Services.AddAutoMapper(options =>
            {
                options.AddProfile<MandarinGrpcMapperProfile>();
            });

            var host = builder.Build();
            host.Services.UseBootstrapProviders().UseFontAwesomeIcons();
            return host.RunAsync();
        }
    }
}
