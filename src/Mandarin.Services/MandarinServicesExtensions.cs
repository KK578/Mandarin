using System;
using Mandarin.Services.Artists;
using Mandarin.Services.Commission;
using Mandarin.Services.Decorators;
using Mandarin.Services.SendGrid;
using Mandarin.Services.Square;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            services.AddLazyCache();
            services.AddMandarinDomainServices();
            services.AddSendGridServices(configuration);
            services.AddArtistServices();
            services.AddSquareServices(configuration);

            return services;
        }

        private static void AddMandarinDomainServices(this IServiceCollection services)
        {
            services.AddTransient<ITransactionMapper, TransactionMapper>();
            services.AddTransient<ICommissionService, CommissionService>();
        }

        private static void AddSendGridServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSendGrid(ConfigureSendGrid);
            services.Configure<SendGridConfiguration>(configuration.GetSection("SendGrid"));
            services.AddTransient<IEmailService, SendGridEmailService>();
            services.Decorate<IEmailService, LoggingEmailServiceDecorator>();

            void ConfigureSendGrid(IServiceProvider s, SendGridClientOptions options)
            {
                options.ApiKey = configuration.GetValue<string>("SendGrid:ApiKey");
            }
        }

        private static void AddArtistServices(this IServiceCollection services)
        {
            services.AddScoped<IArtistService, DatabaseArtistService>();
        }

        private static void AddSquareServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(ConfigureSquareClient);
            services.AddTransient<ITransactionService, SquareTransactionService>();
            services.Decorate<ITransactionService, CachingTransactionServiceDecorator>();
            services.AddTransient<IInventoryService, SquareInventoryService>();
            services.Decorate<IInventoryService, CachingInventoryServiceDecorator>();
            services.AddTransient(s => (IQueryableInventoryService)s.GetRequiredService<IInventoryService>());

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
