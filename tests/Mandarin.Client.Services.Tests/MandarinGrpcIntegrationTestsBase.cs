using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mandarin.Configuration;
using Mandarin.Tests.Helpers;
using Mandarin.Tests.Helpers.Auth;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests
{
    public abstract class MandarinGrpcIntegrationTestsBase : MandarinIntegrationTestsBase
    {
        private readonly IServiceProvider clientServiceProvider;

        protected MandarinGrpcIntegrationTestsBase(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            var server = this.Fixture.Server;
            var handler = server.CreateHandler();

            var configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(new Dictionary<string, string>
                                {
                                    { "Mandarin:AuthenticationHeaderScheme", TestAuthHandler.AuthenticationScheme },
                                })
                                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddMandarinClientServices(server.BaseAddress, () => handler);
            services.AddSingleton<IAccessTokenProvider, TestAuthAccessTokenProvider>();
            services.Configure<MandarinConfiguration>(x => x.AuthenticationHeaderScheme = TestAuthHandler.AuthenticationScheme);

            var factory = new AutofacServiceProviderFactory();
            var builder = factory.CreateBuilder(services);
            builder.RegisterModule<MandarinClientModule>();

            this.clientServiceProvider = factory.CreateServiceProvider(builder);
        }

        protected T Resolve<T>() => this.clientServiceProvider.GetRequiredService<T>();
    }
}
