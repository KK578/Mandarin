using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using Mandarin.Database;
using Mandarin.Inventory;
using Mandarin.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Tests.Inventory
{
    public sealed class ProductSynchronizerTests : MandarinIntegrationTestsBase
    {
        private readonly IProductSynchronizer productSynchronizer;
        private readonly IProductRepository productRepository;

        public ProductSynchronizerTests(MandarinServerFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
            this.productSynchronizer = fixture.Services.GetRequiredService<IProductSynchronizer>();
            this.productRepository = fixture.Services.GetRequiredService<IProductRepository>();
        }

        [Fact]
        public async Task ShouldSaveSquareDataIntoDatabase()
        {
            await this.GivenProductTableIsEmptyAsync();
            await this.productSynchronizer.SynchronizeProductsAsync();
            var products = await this.productRepository.GetAllProductsAsync();
            products.Should().HaveCount(4); // Only contains data from Square - excludes well known items added by migrations.
        }

        [Fact]
        public async Task ShouldNotUpdateMultipleTimesIfItemsAlreadyExist()
        {
            await this.productSynchronizer.SynchronizeProductsAsync();
            var products = await this.productRepository.GetAllProductsAsync();
            products.Should().HaveCount(12);
        }

        private async Task GivenProductTableIsEmptyAsync()
        {
            var db = this.Fixture.Services.GetRequiredService<MandarinDbContext>();
            await db.GetConnection().ExecuteAsync(@"
                DELETE FROM billing.subtransaction;
                DELETE FROM billing.transaction;
                DELETE FROM billing.external_transaction;
                DELETE FROM inventory.product;");
        }
    }
}
