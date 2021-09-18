using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mandarin.Tests.Helpers;
using Mandarin.Tests.Helpers.Auth;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public abstract class MandarinGrpcIntegrationTestsBase : MandarinIntegrationTestsBase
    {
        private readonly IServiceProvider clientServiceProvider;

        protected MandarinGrpcIntegrationTestsBase(MandarinServerFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            var server = this.Fixture.Server;

            var configuration = new ConfigurationBuilder()
                                .AddInMemoryCollection(new Dictionary<string, string>
                                {
                                    { "Mandarin:AuthenticationHeaderScheme", TestAuthHandler.AuthenticationScheme },
                                })
                                .Build();

            var services = new ServiceCollection();
            services.AddHttpClient();

            var factory = new AutofacServiceProviderFactory();
            var builder = factory.CreateBuilder(services);
            builder.RegisterModule(new MandarinClientModule(server.BaseAddress));
            builder.RegisterInstance(configuration).As<IConfiguration>();
            builder.RegisterInstance(server.CreateHandler()).As<HttpMessageHandler>();
            builder.RegisterType<TestAuthAccessTokenProvider>().As<IAccessTokenProvider>().SingleInstance();

            this.clientServiceProvider = factory.CreateServiceProvider(builder);
        }

        protected T Resolve<T>() => this.clientServiceProvider.GetRequiredService<T>();
    }
}
