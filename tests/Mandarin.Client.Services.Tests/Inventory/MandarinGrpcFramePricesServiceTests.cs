using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Inventory;
using Mandarin.Tests.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Inventory
{
    [Collection(nameof(MandarinClientServicesTestsCollectionFixture))]
    public class MandarinGrpcFramePricesServiceTests : MandarinGrpcIntegrationTestsBase
    {
        public MandarinGrpcFramePricesServiceTests(MandarinTestFixture fixture, ITestOutputHelper testOutputHelper)
            : base(fixture, testOutputHelper)
        {
        }

        private IFramePricesService Subject => this.Resolve<IFramePricesService>();

        [Fact]
        public async Task ShouldBeAbleToRetrieveAllFramePrices()
        {
            var framePrices = await this.Subject.GetAllFramePricesAsync();
            framePrices.Should().HaveCount(2);
        }

        [Fact]
        public async Task ShouldBeAbleToAddAndRoundTripANewFramePrice()
        {
            var framePrice = new FramePrice("OM19-001", 15.00M);
            await this.Subject.SaveFramePriceAsync(framePrice);

            var newFramePrice = await this.Subject.GetFramePriceAsync("OM19-001");
            newFramePrice.Should().BeEquivalentTo(framePrice);
        }

        [Fact]
        public async Task ShouldBeAbleToDeleteAFramePrice()
        {
            await this.Subject.DeleteFramePriceAsync("KT20-001F");

            var deletedFramePrice = await this.Subject.GetFramePriceAsync("KT20-001F");
            deletedFramePrice.Should().BeNull();
            var existingFramePrice = await this.Subject.GetFramePriceAsync("KT20-002F");
            existingFramePrice.Should().NotBeNull();
        }
    }
}
