using System;
using Mandarin.Services.Email;
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
            services.AddSendGridClient(configuration);
            services.AddSingleton<IEmailService, SendGridEmailService>();
            services.Decorate<IEmailService, LoggingEmailServiceDecorator>();

            return services;
        }

        private static void AddSendGridClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SendGridClientOptions>(configuration.GetSection("SendGrid"));
            services.AddSingleton(MandarinServicesServiceCollectionExtensions.CreateClient);
        }

        private static ISendGridClient CreateClient(IServiceProvider provider)
        {
            var options = provider.GetRequiredService<IOptions<SendGridClientOptions>>();
            return new SendGridClient(options.Value);
        }
    }
}
