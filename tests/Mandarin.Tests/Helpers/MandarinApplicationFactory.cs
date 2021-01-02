using System.Collections.Generic;
using Mandarin.Tests.Helpers.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Mandarin.Tests.Helpers
{
    public class MandarinApplicationFactory : WebApplicationFactory<MandarinStartup>
    {
        public ITestOutputHelper TestOutputHelper { get; set; } = new TestOutputHelper();

        protected override void Dispose(bool disposing)
        {
            // using var scope = this.Services.CreateScope();
            // var mandarinDbContext = scope.ServiceProvider.GetRequiredService<MandarinDbContext>();
            // var migrator = mandarinDbContext.GetInfrastructure().GetRequiredService<IMigrator>();
            // migrator.Migrate("0");
            base.Dispose(disposing);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(MandarinApplicationFactory.AddTestConfiguration);
            builder.ConfigureTestServices(services =>
            {
                MandarinApplicationFactory.ConfigureTestAuthentication(services);
                this.ConfigureTestServices(services);
            });
            builder.ConfigureLogging(l => l.ClearProviders());
            builder.UseSerilog(this.ConfigureSerilog, true, true);
        }

        protected virtual void ConfigureTestServices(IServiceCollection services)
        {
        }

        private static void AddTestConfiguration(WebHostBuilderContext host, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Auth0:Domain", "localhost" },
                { "Auth0:ClientId", "SuperSecretId" },
                { "Auth0:ClientSecret", "SuperSecretValue" },
                { "ConnectionStrings:MandarinConnection", "Host=localhost;Port=5555;Database=postgres;Username=postgres;Password=password;Include Error Detail=true" },
                { "SendGrid:ServiceEmail", "ServiceEmail@example.com" },
                { "SendGrid:RealContactEmail", "RealContactEmail@example.com" },
            });
        }

        private static void ConfigureTestAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = TestAuthHandler.AuthenticationScheme;
                        options.DefaultSignInScheme = TestAuthHandler.AuthenticationScheme;
                        options.DefaultChallengeScheme = TestAuthHandler.AuthenticationScheme;
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, _ => { });
        }

        private void ConfigureSerilog(WebHostBuilderContext b, LoggerConfiguration c)
        {
            c.MinimumLevel.Verbose()
             .MinimumLevel.Override("Elastic.Apm", LogEventLevel.Error)
             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
             .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
             .WriteTo.TestOutput(this.TestOutputHelper, outputTemplate: "{Timestamp:HH:mm:ss.ffff} {Level:u3} {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
    }
}
