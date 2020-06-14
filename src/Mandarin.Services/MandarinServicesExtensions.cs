using System;
using System.Net.Http;
using Mandarin.Services.Commission;
using Mandarin.Services.Decorators;
using Mandarin.Services.Fruity;
using Mandarin.Services.SendGrid;
using Mandarin.Services.Square;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
            services.AddLazyCache();
            services.AddMandarinDomainServices();
            services.AddSendGridServices(configuration);
            services.AddFruityServices(configuration);
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
            services.Configure<SendGridClientOptions>(configuration.GetSection("SendGrid"));
            services.Configure<SendGridConfiguration>(configuration.GetSection("SendGrid"));
            services.AddTransient(ConfigureSendGridClient);
            services.AddTransient<IEmailService, SendGridEmailService>();
            services.Decorate<IEmailService, LoggingEmailServiceDecorator>();

            static ISendGridClient ConfigureSendGridClient(IServiceProvider provider)
            {
                var options = provider.GetRequiredService<IOptions<SendGridClientOptions>>();
                return new SendGridClient(options.Value);
            }
        }

        private static void AddFruityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FruityClientOptions>(configuration.GetSection("Fruity"));
            services.AddHttpClient<IArtistService, FruityArtistService>(ConfigureFruityClient);
            services.Decorate<IArtistService, LoggingArtistServiceDecorator>();
            services.Decorate<IArtistService, CachingArtistServiceDecorator>();

            void ConfigureFruityClient(IServiceProvider s, HttpClient client)
            {
                var options = s.GetRequiredService<IOptions<FruityClientOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
            }
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
