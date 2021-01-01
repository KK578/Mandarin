using AutoMapper;
using Mandarin.Client.Services;
using Mandarin.Converters;
using Mandarin.Tests.Helpers;
using Mandarin.Tests.Helpers.Auth;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Mandarin.Tests.Grpc
{
    public abstract class MandarinGrpcIntegrationTestsBase : MandarinIntegrationTestsBase
    {
        private readonly ServiceProvider clientServiceProvider;

        protected MandarinGrpcIntegrationTestsBase(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            var server = this.Fixture.Server;
            var handler = server.CreateHandler();

            var services = new ServiceCollection();
            services.AddMandarinClientServices(server.BaseAddress, () => handler);
            services.AddSingleton<IAccessTokenProvider, TestAuthAccessTokenProvider>();
            services.AddAutoMapper(options => { options.AddProfile<MandarinMapperProfile>(); });
            this.clientServiceProvider = services.BuildServiceProvider();
        }

        protected T Resolve<T>() => this.clientServiceProvider.GetRequiredService<T>();
    }
}
