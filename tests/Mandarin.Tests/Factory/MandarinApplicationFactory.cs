using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Mandarin.Tests.Factory
{
    internal static class MandarinApplicationFactory
    {
        public static WebApplicationFactory<MandarinStartup> Create()
        {
            var factory = new WebApplicationFactory<MandarinStartup>();
            return factory.WithWebHostBuilder(b => b.ConfigureAppConfiguration(MandarinApplicationFactory.AddTestConfiguration)
                                                    .ConfigureTestAuthentication()
                                                    .UseSerilog(MandarinApplicationFactory.ConfigureSerilog));
        }

        public static IWebHostBuilder ConfigureTestAuthentication(this IWebHostBuilder builder)
        {
            return builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                            options.DefaultSignInScheme = TestAuthHandler.AuthenticationScheme;
                            options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });
            });
        }

        private static void AddTestConfiguration(WebHostBuilderContext host, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Auth0:Domain", "localhost" },
                { "Auth0:ClientId", "SuperSecretId" },
                { "Auth0:ClientSecret", "SuperSecretValue" },
            });
        }

        private static void ConfigureSerilog(WebHostBuilderContext b, LoggerConfiguration c)
        {
            c.MinimumLevel.Verbose()
             .MinimumLevel.Override("Elastic.Apm", LogEventLevel.Error)
             .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss.ffff} {Level:u3} {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
    }
}
