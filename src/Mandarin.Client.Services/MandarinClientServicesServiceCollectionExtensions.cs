using System;
using System.Net.Http;
using Grpc.Core;
using Grpc.Net.Client.Web;
using Mandarin.Client.Services.Authentication;
using Microsoft.Extensions.DependencyInjection;
using static Mandarin.Api.Commissions.Commissions;
using static Mandarin.Api.Emails.Emails;
using static Mandarin.Api.Inventory.FramePrices;
using static Mandarin.Api.Inventory.Products;
using static Mandarin.Api.Stockists.Stockists;
using static Mandarin.Api.Transactions.Transactions;

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
            AddMandarinGrpcClient<CommissionsClient>();
            AddMandarinGrpcClient<EmailsClient>();
            AddMandarinGrpcClient<FramePricesClient>();
            AddMandarinGrpcClient<ProductsClient>();
            AddMandarinGrpcClient<StockistsClient>();
            AddMandarinGrpcClient<TransactionsClient>();

            return services;

            void AddMandarinGrpcClient<T>()
                where T : ClientBase
            {
                services.AddGrpcClient<T>(options => options.Address = baseAddress)
                        .ConfigurePrimaryHttpMessageHandler(() => new GrpcWebHandler(GrpcWebMode.GrpcWebText, handlerFunc()))
                        .AddHttpMessageHandler<JwtHttpMessageHandler>();
            }
        }
    }
}
