using System;
using Mandarin.Commissions;
using Mandarin.Emails;
using Mandarin.Inventory;
using Mandarin.Services.Commission;
using Mandarin.Services.Emails;
using Mandarin.Services.Inventory;
using Mandarin.Services.Stockists;
using Mandarin.Services.Transactions.External;
using Mandarin.Stockists;
using Mandarin.Transactions.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;
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
            services.AddMandarinDomainServices();
            services.AddEmailServices(configuration);
            services.AddInventoryServices();
            services.AddStockistServices();
            services.AddTransactionServices();
            services.AddSquareServices(configuration);

            return services;
        }

        private static void AddMandarinDomainServices(this IServiceCollection services)
        {
            services.AddTransient<ICommissionService, CommissionService>();
        }

        private static void AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSendGrid(ConfigureSendGrid);
            services.Configure<SendGridConfiguration>(configuration.GetSection("SendGrid"));
            services.AddTransient<IEmailService, SendGridEmailService>();

            void ConfigureSendGrid(IServiceProvider s, SendGridClientOptions options)
            {
                options.ApiKey = configuration.GetValue<string>("SendGrid:ApiKey");
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
                       .Environment(configuration.GetValue<Environment>("Square:Environment"))
                       .AccessToken(configuration.GetValue<string>("Square:ApiKey"))
                       .Build();
            }
        }
    }
}
