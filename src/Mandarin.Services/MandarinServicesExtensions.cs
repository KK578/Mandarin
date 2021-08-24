using System;
using System.Net.Http;
using Mandarin.Emails;
using Mandarin.Inventory;
using Mandarin.Services.Emails;
using Mandarin.Services.Inventory;
using Mandarin.Services.Stockists;
using Mandarin.Services.Transactions.External;
using Mandarin.Stockists;
using Mandarin.Transactions.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using SendGrid;
using Square;
using Environment = Square.Environment;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Mandarin.Services
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> to register all services in this assembly.
    /// </summary>
    public static class MandarinServicesExtensions
    {
        /// <summary>
        /// Registers the Mandarin.Services assembly implementations into the provided service container.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <param name="configuration">Application configuration for configuring services.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IClock>(SystemClock.Instance);
            services.AddLazyCache();
            services.AddEmailServices(configuration);
            services.AddInventoryServices();
            services.AddStockistServices();
            services.AddTransactionServices();
            services.AddSquareServices(configuration);

            return services;
        }

        private static void AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<ISendGridClient, SendGridClient>(ConfigureSendGridClient);
            services.Configure<SendGridClientOptions>(configuration.GetSection("SendGrid"));
            services.Configure<SendGridConfiguration>(configuration.GetSection("SendGrid"));
            services.AddTransient<IEmailService, SendGridEmailService>();

            SendGridClient ConfigureSendGridClient(HttpClient httpClient, IServiceProvider s)
            {
                var options = s.GetRequiredService<IOptions<SendGridClientOptions>>();
                return new SendGridClient(httpClient, options.Value);
            }
        }

        private static void AddInventoryServices(this IServiceCollection services)
        {
            services.AddSingleton<IProductSynchronizer, SquareProductSynchronizer>();
            services.AddTransient<ISquareProductService, SquareProductService>();
            services.AddTransient<IFramePricesService, FramePricesService>();
        }

        private static void AddStockistServices(this IServiceCollection services)
        {
            services.AddTransient<IStockistService, StockistService>();
        }

        private static void AddTransactionServices(this IServiceCollection services)
        {
            services.AddTransient<ISquareTransactionMapper, SquareTransactionMapper>();
            services.AddTransient<ISquareTransactionService, SquareTransactionService>();
            services.AddSingleton<ITransactionSynchronizer, SquareTransactionSynchronizer>();
        }

        private static void AddSquareServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(ConfigureSquareClient);

            ISquareClient ConfigureSquareClient(IServiceProvider provider)
            {
                return new SquareClient.Builder()
                       .CustomUrl(configuration.GetValue<string>("Square:Host"))
                       .Environment(configuration.GetValue<Environment>("Square:Environment"))
                       .AccessToken(configuration.GetValue<string>("Square:ApiKey"))
                       .Build();
            }
        }
    }
}
