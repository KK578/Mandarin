using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Inventory;
using Mandarin.Tests.Data;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Inventory
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class MandarinGrpcProductServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public MandarinGrpcProductServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IProductRepository Subject => this.Resolve<IProductRepository>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveAllProducts()
        {
            var products = await this.Subject.GetAllProductsAsync();
            products.Should().HaveCount(8);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductBySquareId()
        {
            var product = await this.Subject.GetProductAsync(ProductId.Of("BTWEJWZCPE4XAKZRBJW53DYE"));
            product.Should().BeEquivalentTo(WellKnownTestData.Products.ClementineFramed);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductByCode()
        {
            var product = await this.Subject.GetProductAsync(ProductCode.Of("KT20-001F"));
            product.Should().BeEquivalentTo(WellKnownTestData.Products.ClementineFramed);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveGiftCard()
        {
            var product = await this.Subject.GetProductAsync(ProductCode.Of("TLM-GC"));
            product.Should().BeEquivalentTo(WellKnownTestData.Products.GiftCard, o => o.Excluding(x => x.LastUpdated));
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductName()
        {
            var product = await this.Subject.GetProductAsync(ProductName.Of("Clementine (Framed)"));
            product.Should().BeEquivalentTo(WellKnownTestData.Products.ClementineFramed);
        }
    }
}
