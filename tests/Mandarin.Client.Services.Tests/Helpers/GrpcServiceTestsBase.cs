using AutoMapper;
using Mandarin.Converters;
using Mandarin.Tests.Helpers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Helpers
{
    public abstract class GrpcServiceTestsBase : MandarinIntegrationTestsBase
    {
        private readonly ServiceProvider clientServiceProvider;

        protected GrpcServiceTestsBase(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
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
