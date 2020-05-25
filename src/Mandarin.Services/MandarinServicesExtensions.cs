using System;
using System.Net.Http;
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
    public static class MandarinServicesExtensions
    {
        public static IServiceCollection AddMandarinServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddSendGridServices(configuration);
            services.AddFruityServices(configuration);
            services.AddSquareServices(configuration);

            return services;
        }

        private static void AddSendGridServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SendGridClientOptions>(configuration.GetSection("SendGrid"));
            services.Configure<SendGridConfiguration>(configuration.GetSection("SendGrid"));
            services.AddSingleton(ConfigureSendGridClient);
            services.AddSingleton<IEmailService, SendGridEmailService>();
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
            services.AddTransient<IInventoryService, SquareInventoryService>();

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
