using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Inventory;
using Mandarin.Services.Inventory;
using Mandarin.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Static
{
    [Collection(nameof(MandarinTestsCollectionFixture))]
    public class ProductSynchronizerTests : MandarinIntegrationTestsBase
    {
        private readonly IProductSynchronizer productSynchronizer;
        private readonly IProductRepository productRepository;

        public ProductSynchronizerTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            this.productSynchronizer = fixture.Services.GetRequiredService<IProductSynchronizer>();
            this.productRepository = fixture.Services.GetRequiredService<IProductRepository>();
        }

        [Fact]
        public async Task ShouldSaveSquareDataIntoDatabase()
        {
            await this.productSynchronizer.SynchroniseRepositoryAsync();
            var products = await this.productRepository.GetAllAsync();
            products.Should().HaveCount(4);
        }

        [Fact]
        public async Task ShouldNotUpdateMultipleTimesIfItemsAlreadyExist()
        {
            await this.productSynchronizer.SynchroniseRepositoryAsync();
            await this.productSynchronizer.SynchroniseRepositoryAsync();
            var products = await this.productRepository.GetAllAsync();
            products.Should().HaveCount(4);
        }
    }
}
