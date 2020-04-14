using System;
using System.Net.Http;
using Mandarin.Services.Email;
using Mandarin.Services.Fruity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SendGrid;

namespace Mandarin.Services
{
    public static class MandarinServicesServiceCollectionExtensions
    {
        public static IServiceCollection AddMandarinServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEmailServices(configuration);
            services.AddFruityServices(configuration);
            services.AddMemoryCache();

            return services;
        }

        private static void AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SendGridClientOptions>(configuration.GetSection("SendGrid"));
            services.Configure<SendGridConfiguration>(configuration.GetSection("SendGrid"));
            services.AddSingleton(CreateClient);
            services.AddSingleton<IEmailService, SendGridEmailService>();
            services.Decorate<IEmailService, LoggingEmailServiceDecorator>();

            static ISendGridClient CreateClient(IServiceProvider provider)
            {
                var options = provider.GetRequiredService<IOptions<SendGridClientOptions>>();
                return new SendGridClient(options.Value);
            }
        }

        private static void AddFruityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FruityClientOptions>(configuration.GetSection("Fruity"));
            services.AddHttpClient<IArtistService, FruityArtistService>(ConfigureClient);
            services.Decorate<IArtistService, LoggingArtistServiceDecorator>();
            services.Decorate<IArtistService, CachingArtistServiceDecorator>();

            void ConfigureClient(IServiceProvider s, HttpClient client)
            {
                var options = s.GetRequiredService<IOptions<FruityClientOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
            }
        }
    }
}
