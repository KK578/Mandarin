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
        public async Task ShouldNotShowHistoricFramePrices()
        {
            var today = new DateTime(2021, 06, 30);
            var newFramePrice = new FramePrice
            {
                ProductCode = new ProductCode("KT20-001F"),
                Amount = 25.00M,
                CreatedAt = today,
            };
            await this.Subject.SaveFramePriceAsync(newFramePrice);

            var framePrices = await this.Subject.GetAllFramePricesAsync();
            framePrices.Should().HaveCount(2);
        }

        [Fact]
        public async Task ShouldBeAbleToAddAndRoundTripANewFramePrice()
        {
            var framePrice = new FramePrice
            {
                ProductCode = new ProductCode("OM19-001"),
                Amount = 15.00M,
                CreatedAt = new DateTime(2021, 06, 30),
            };
            await this.Subject.SaveFramePriceAsync(framePrice);

            var newFramePrice = await this.Subject.GetFramePriceAsync(new ProductCode("OM19-001"), DateTime.Now);
            newFramePrice.Should().BeEquivalentTo(framePrice);
        }

        [Fact]
        public async Task ShouldBeAbleToUpdateAndRoundTripAFramePrice()
        {
            var expected = new FramePrice
            {
                ProductCode = new ProductCode("KT20-001F"),
                Amount = 50.00M,
                CreatedAt = new DateTime(2019, 06, 01),
                ActiveUntil = null,
            };

            (await this.Subject.GetFramePriceAsync(new ProductCode("KT20-001F"), DateTime.Now)).Should().Be(expected);

            var today = new DateTime(2021, 06, 30);
            var newFramePrice = new FramePrice
            {
                ProductCode = new ProductCode("KT20-001F"),
                Amount = 25.00M,
                CreatedAt = today,
            };
            await this.Subject.SaveFramePriceAsync(newFramePrice);

            (await this.Subject.GetFramePriceAsync(new ProductCode("KT20-001F"), new DateTime(2021, 05, 15))).Should().Be(expected with { ActiveUntil = today });
            (await this.Subject.GetFramePriceAsync(new ProductCode("KT20-001F"), today)).Should().Be(newFramePrice);
        }

        [Fact]
        public async Task ShouldBeAbleToDeleteAFramePrice()
        {
            await this.Subject.DeleteFramePriceAsync(new ProductCode("KT20-001F"));

            var deletedFramePrice = await this.Subject.GetFramePriceAsync(new ProductCode("KT20-001F"), DateTime.Now);
            deletedFramePrice.Should().BeNull();
            var existingFramePrice = await this.Subject.GetFramePriceAsync(new ProductCode("KT20-002F"), DateTime.Now);
            existingFramePrice.Should().NotBeNull();
        }
    }
}
