using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SendGrid;

namespace Mandarin.Services.Email
{
    internal static class SendGridClientServiceCollectionExtensions
    {
        public static IServiceCollection AddSendGridClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SendGridClientOptions>(configuration.GetSection("SendGrid"));
            services.AddSingleton(CreateClient);

            return services;
        }

        private static ISendGridClient CreateClient(IServiceProvider provider)
        {
            var options = provider.GetRequiredService<IOptions<SendGridClientOptions>>();
            return new SendGridClient(options.Value);
        }
    }
}
