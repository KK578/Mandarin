using System.Collections.Generic;
using Autofac;
using Mandarin.Tests.Data;
using Mandarin.Tests.Helpers.Auth;
using Mandarin.Tests.Helpers.SendGrid;
using Mandarin.Tests.Helpers.Square;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Xunit.Abstractions;
using Assembly = System.Reflection.Assembly;

namespace Mandarin.Tests.Helpers
{
    public class MandarinApplicationFactory : WebApplicationFactory<MandarinStartup>
    {
        private readonly DelegateTestOutputHelper delegateTestOutputHelper = new();

        public ITestOutputHelper TestOutputHelper
        {
            get => this.delegateTestOutputHelper.TestOutputHelper;
            set => this.delegateTestOutputHelper.TestOutputHelper = value;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(MandarinApplicationFactory.AddTestConfiguration);
            builder.ConfigureTestServices(MandarinApplicationFactory.ConfigureTestAuthentication);
            builder.ConfigureLogging(l => l.ClearProviders());
            builder.UseSerilog(this.ConfigureSerilog);
        }

        /// <inheritdoc />
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureContainer<ContainerBuilder>(this.ConfigureTestContainer);
            return base.CreateHost(builder);
        }

        protected virtual void ConfigureTestContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(typeof(MandarinApplicationFactory).Assembly).As<Assembly>();
        }

        private static void AddTestConfiguration(WebHostBuilderContext host, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                { "Auth0:Domain", "localhost" },
                { "Auth0:ClientId", "SuperSecretId" },
                { "Auth0:ClientSecret", "SuperSecretValue" },
                { "ConnectionStrings:MandarinConnection", "Host=localhost;Port=5555;Database=postgres;Username=postgres;Password=password;Maximum Pool Size=5;Include Error Detail=true" },
                { "Hangfire:ConfigureRecurringJobs", "false" },
                { "Mandarin:FixedCommissionAmountFilePath", WellKnownTestData.Commissions.FixedCommissions },
                { "SendGrid:ApiKey", "TestSendGridApiKey" },
                { "SendGrid:Host", SendGridWireMockFixture.Host },
                { "SendGrid:ServiceEmail", "ServiceEmail@thelittlemandarin.co.uk" },
                { "SendGrid:RealContactEmail", "RealContactEmail@thelittlemandarin.co.uk" },
                { "SendGrid:RecordOfSalesTemplateId", "TestRecordOfSalesTemplateId" },
                { "Square:ApiKey", "TestSquareApiKey" },
                { "Square:Host", SquareWireMockFixture.Host },
                { "Square:Environment", "Custom" },
            });
            configurationBuilder.AddUserSecrets(typeof(MandarinApplicationFactory).Assembly);
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
             .WriteTo.TestOutput(this.delegateTestOutputHelper, outputTemplate: "{Timestamp:HH:mm:ss.ffff} {Level:u3} {SourceContext}: {Message:lj}{NewLine}{Exception}");
        }
    }
}
