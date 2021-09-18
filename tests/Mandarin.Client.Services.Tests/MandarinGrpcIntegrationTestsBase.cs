using Mandarin.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public abstract class MandarinGrpcIntegrationTestsBase : MandarinIntegrationTestsBase
    {
        private readonly MandarinGrpcClientFixture mandarinGrpcClientFixture;

        protected MandarinGrpcIntegrationTestsBase(MandarinGrpcClientFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture.MandarinServerFixture, testOutputHelper)
        {
            this.mandarinGrpcClientFixture = fixture;
        }

        protected T Resolve<T>() => this.mandarinGrpcClientFixture.GetRequiredService<T>();
    }
}
