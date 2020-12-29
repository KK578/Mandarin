using System;
using System.Net.Http;
using AutoMapper;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Grpc.Net.Client.Web;
using Mandarin.App.Client;
using Mandarin.App.Commands.Navigation;
using Mandarin.App.Commands.Stockists;
using Mandarin.App.ViewModels;
using Mandarin.App.ViewModels.Stockists;
using Mandarin.Converters;
using Mandarin.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Mandarin.Api.Commissions.Commissions;
using static Mandarin.Api.Stockists.Stockists;

namespace Mandarin.App
{
    /// <summary>
    /// Extensions for including Mandarin REST Client services to a <see cref="WebAssemblyHostBuilder" />.
    /// </summary>
    internal static class MandarinWasmExtensions
    {
        /// <summary>
        /// Adds the Mandarin WASM Application dependencies.
        /// </summary>
        /// <param name="builder">The <see cref="WebAssemblyHostBuilder"/> instance to register the services into.</param>
        /// <returns>The <see cref="WebAssemblyHostBuilder"/> returned as is, for chaining calls.</returns>
        public static WebAssemblyHostBuilder AddMandarinWasm(this WebAssemblyHostBuilder builder)
        {
            MandarinWasmExtensions.AddBlazorise(builder.Services);
            MandarinWasmExtensions.AddAuthentication(builder);
            MandarinWasmExtensions.AddMandarinServices(builder.Services, new Uri(builder.HostEnvironment.BaseAddress));
            MandarinWasmExtensions.AddMapper(builder.Services);
            MandarinWasmExtensions.AddCommands(builder.Services);
            MandarinWasmExtensions.AddViewModels(builder.Services);

            return builder;
        }

        private static void AddBlazorise(IServiceCollection services)
        {
            services.AddBlazorise(o => o.DelayTextOnKeyPress = true).AddBootstrapProviders().AddFontAwesomeIcons();
        }

        private static void AddAuthentication(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Auth0", options.ProviderOptions);
                options.ProviderOptions.ResponseType = "code";
            });
        }

        private static void AddMandarinServices(IServiceCollection services, Uri baseAddress)
        {
            services.AddScoped<JwtHttpMessageHandler>();
            AddMandarinGrpcClient<StockistsClient>();
            AddMandarinGrpcClient<CommissionsClient>();

            services.AddTransient<IArtistService, MandarinGrpcArtistService>();
            services.AddTransient<ICommissionService, MandarinGrpcCommissionService>();

            void AddMandarinGrpcClient<T>()
                where T : class
            {
                services.AddGrpcClient<T>(options => options.Address = baseAddress)
                        .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler()))
                        .AddHttpMessageHandler<JwtHttpMessageHandler>();
            }
        }

        private static void AddMapper(IServiceCollection services)
        {
            services.AddAutoMapper(options =>
            {
                options.AddProfile<MandarinMapperProfile>();
            });
        }

        private static void AddCommands(IServiceCollection services)
        {
            services.AddTransient<RedirectToLoginCommand>();
            services.AddLazyTransient<RedirectToStockistsEditCommand>();
            services.AddTransient<RedirectToStockistsIndexCommand>();
            services.AddTransient<RedirectToStockistsNewCommand>();
            services.AddLazyTransient<SaveNewStockistCommand>();
        }

        private static void AddViewModels(IServiceCollection services)
        {
            services.AddScoped<IIndexPageViewModel, IndexPageViewModel>();
            services.AddScoped<IStockistIndexPageViewModel, StockistIndexPageViewModel>();
            services.AddScoped<IStockistsNewPageViewModel, StockistsNewPageViewModel>();
            services.AddScoped<IStockistsEditPageViewModel, StockistsEditPageViewModel>();
        }
    }
}
