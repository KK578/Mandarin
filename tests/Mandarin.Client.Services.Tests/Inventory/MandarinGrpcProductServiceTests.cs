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
            var products = await this.Subject.GetAllAsync();
            products.Should().HaveCount(5);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductBySquareId()
        {
            var product = await this.Subject.GetProductByIdAsync(new ProductId("BTWEJWZCPE4XAKZRBJW53DYE"));
            product.Should().BeEquivalentTo(WellKnownTestData.Products.ClementineFramed);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductByCode()
        {
            var product = await this.Subject.GetProductByCodeAsync(new ProductCode("KT20-001F"));
            product.Should().BeEquivalentTo(WellKnownTestData.Products.ClementineFramed);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveGiftCard()
        {
            var product = await this.Subject.GetProductByCodeAsync(new ProductCode("TLM-GC"));
            product.Should().BeEquivalentTo(WellKnownTestData.Products.GiftCard, o => o.Excluding(x => x.LastUpdated));
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductName()
        {
            var product = await this.Subject.GetProductByNameAsync(new ProductName("Clementine (Framed)"));
            product.Should().BeEquivalentTo(WellKnownTestData.Products.ClementineFramed);
        }
    }
}
