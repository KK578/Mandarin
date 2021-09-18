using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mandarin.Client;
using Mandarin.Tests.Helpers.Auth;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Tests.Helpers
{
    public sealed class MandarinGrpcClientFixture : IServiceProvider
    {
        private readonly IServiceProvider serviceProvider;

        public MandarinGrpcClientFixture()
        {
            this.MandarinServerFixture = new MandarinServerFixture();
            var server = this.MandarinServerFixture.Server;

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

            this.serviceProvider = factory.CreateServiceProvider(builder);
        }

        public MandarinServerFixture MandarinServerFixture { get; }

        /// <inheritdoc />
        public object GetService(Type serviceType)
        {
            return this.serviceProvider.GetService(serviceType);
        }
    }
}
