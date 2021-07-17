using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Inventory;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Inventory
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class MandarinGrpcProductServiceTests : MandarinGrpcIntegrationTestsBase
    {
        private readonly Product expectedProduct = new()
        {
            SquareId = new ProductId("BTWEJWZCPE4XAKZRBJW53DYE"),
            ProductCode = new ProductCode("KT20-001F"),
            ProductName = new ProductName("Clementine (Framed) (Regular)"),
            Description = "vel augue vestibulum ante ipsum primis in",
            UnitPrice = 95.00M,
        };

        public MandarinGrpcProductServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IQueryableProductService Subject => this.Resolve<IQueryableProductService>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveAllProducts()
        {
            var products = await this.Subject.GetAllProductsAsync();
            products.Should().HaveCount(4);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductBySquareId()
        {
            var product = await this.Subject.GetProductBySquareIdAsync(new ProductId("BTWEJWZCPE4XAKZRBJW53DYE"));
            product.Should().BeEquivalentTo(this.expectedProduct);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductByCode()
        {
            var product = await this.Subject.GetProductByProductCodeAsync(new ProductCode("KT20-001F"));
            product.Should().BeEquivalentTo(this.expectedProduct);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveProductName()
        {
            var product = await this.Subject.GetProductByNameAsync(new ProductName("Clementine (Framed)"));
            product.Should().BeEquivalentTo(this.expectedProduct);
        }
    }
}
