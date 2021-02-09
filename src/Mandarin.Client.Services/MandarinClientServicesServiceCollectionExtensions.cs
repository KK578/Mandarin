using System;
using System.Net.Http;
using Grpc.Core;
using Grpc.Net.Client.Web;
using Mandarin.Client.Services.Authentication;
using Mandarin.Client.Services.Commissions;
using Mandarin.Client.Services.Emails;
using Mandarin.Client.Services.Inventory;
using Mandarin.Client.Services.Stockists;
using Mandarin.Commissions;
using Mandarin.Emails;
using Mandarin.Inventory;
using Mandarin.Stockists;
using Microsoft.Extensions.DependencyInjection;
using static Mandarin.Api.Commissions.Commissions;
using static Mandarin.Api.Emails.Emails;
using static Mandarin.Api.Inventory.FixedCommissions;
using static Mandarin.Api.Inventory.Products;
using static Mandarin.Api.Stockists.Stockists;

namespace Mandarin.Client.Services
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> to register all services in this assembly.
    /// </summary>
    public static class MandarinClientServicesServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the Mandarin.Client.Services assembly implementations into the provided service container.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <param name="baseAddress">The uri base address of the Mandarin API.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinClientServices(this IServiceCollection services, Uri baseAddress)
        {
            return services.AddMandarinClientServices(baseAddress, () => new HttpClientHandler());
        }

        /// <summary>
        /// Registers the Mandarin.Client.Services assembly implementations into the provided service container.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <param name="baseAddress">The uri base address of the Mandarin API.</param>
        /// <param name="handlerFunc">The <see cref="HttpClient"/> handler to use for gRPC calls.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinClientServices(this IServiceCollection services,
                                                                   Uri baseAddress,
                                                                   Func<HttpMessageHandler> handlerFunc)
        {
            services.AddScoped<JwtHttpMessageHandler>();

            AddMandarinGrpcClient<CommissionsClient>();
            AddMandarinGrpcClient<EmailsClient>();
            AddMandarinGrpcClient<FixedCommissionsClient>();
            AddMandarinGrpcClient<ProductsClient>();
            AddMandarinGrpcClient<StockistsClient>();

            services.AddTransient<ICommissionService, MandarinGrpcCommissionService>();
            services.AddTransient<IEmailService, MandarinGrpcEmailService>();
            services.AddTransient<IFixedCommissionService, MandarinGrpcFixedCommissionService>();
            services.AddTransient<IQueryableProductService, MandarinGrpcProductService>();
            services.AddTransient<IProductService>(s => s.GetService<IQueryableProductService>());
            services.AddTransient<IStockistService, MandarinStockistGrpcService>();

            return services;

            void AddMandarinGrpcClient<T>()
                where T : ClientBase
            {
                services.AddGrpcClient<T>(options => { options.Address = baseAddress; })
                        .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(GrpcWebMode.GrpcWebText, handlerFunc()))
                        .AddHttpMessageHandler<JwtHttpMessageHandler>();
            }
        }
    }
}
