using System;
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
            var framePrice = new FramePrice
            {
                ProductCode = "OM19-001",
                Amount = 15.00M,
                CreatedAt = new DateTime(2021, 06, 30),
            };
            await this.Subject.SaveFramePriceAsync(framePrice);

            var newFramePrice = await this.Subject.GetFramePriceAsync("OM19-001");
            newFramePrice.Should().BeEquivalentTo(framePrice);
        }

        [Fact]
        public async Task ShouldBeAbleToUpdateAndRoundTripAFramePrice()
        {
            var existingFramePrice = await this.Subject.GetFramePriceAsync("KT20-001F");
            existingFramePrice.Amount.Should().Be(50.00M);
            existingFramePrice.CreatedAt.Should().Be(new DateTime(2019, 06, 01));
            existingFramePrice.ActiveUntil.Should().BeNull();

            var newFramePrice = new FramePrice
            {
                ProductCode = "KT20-001F",
                Amount = 25.00M,
                CreatedAt = new DateTime(2021, 06, 30),
            };
            await this.Subject.SaveFramePriceAsync(newFramePrice);

            existingFramePrice = await this.Subject.GetFramePriceAsync("KT20-001F");
            existingFramePrice.Amount.Should().Be(25.00M);
            existingFramePrice.CreatedAt.Should().Be(new DateTime(2021, 06, 30));
            existingFramePrice.ActiveUntil.Should().BeNull();
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
