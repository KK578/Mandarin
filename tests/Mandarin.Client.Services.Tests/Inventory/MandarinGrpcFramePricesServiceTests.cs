using System.Threading.Tasks;
using FluentAssertions;
using Mandarin.Inventory;
using Mandarin.Tests.Helpers;
using NodaTime;
using Xunit;
using Xunit.Abstractions;

namespace Mandarin.Client.Services.Tests.Inventory
{
    public sealed class MandarinGrpcFramePricesServiceTests : MandarinGrpcIntegrationTestsBase
    {
        private static readonly Instant Original = Instant.FromUtc(2019, 06, 01, 00, 00, 00);
        private static readonly Instant LastMonth = Instant.FromUtc(2021, 05, 29, 12, 00, 00);
        private static readonly Instant Today = Instant.FromUtc(2021, 06, 30, 12, 20, 00);

        public MandarinGrpcFramePricesServiceTests(MandarinServerFixture fixture, ITestOutputHelper testOutputHelper)
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
            var newFramePrice = new FramePrice
            {
                ProductCode = ProductCode.Of("KT20-001F"),
                Amount = 25.00M,
                CreatedAt = MandarinGrpcFramePricesServiceTests.Today,
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
                ProductCode = ProductCode.Of("OM19-001"),
                Amount = 15.00M,
                CreatedAt = MandarinGrpcFramePricesServiceTests.Today,
            };
            await this.Subject.SaveFramePriceAsync(framePrice);

            var newFramePrice = await this.Subject.GetFramePriceAsync(ProductCode.Of("OM19-001"), MandarinGrpcFramePricesServiceTests.Today);
            newFramePrice.Should().BeEquivalentTo(framePrice);
        }

        [Fact]
        public async Task ShouldBeAbleToUpdateAndRoundTripAFramePrice()
        {
            var original = new FramePrice
            {
                ProductCode = ProductCode.Of("KT20-001F"),
                Amount = 50.00M,
                CreatedAt = MandarinGrpcFramePricesServiceTests.Original,
                ActiveUntil = null,
            };

            (await this.Subject.GetFramePriceAsync(ProductCode.Of("KT20-001F"), MandarinGrpcFramePricesServiceTests.Today)).Should().Be(original);

            var newFramePrice = new FramePrice
            {
                ProductCode = ProductCode.Of("KT20-001F"),
                Amount = 25.00M,
                CreatedAt = MandarinGrpcFramePricesServiceTests.Today,
            };
            await this.Subject.SaveFramePriceAsync(newFramePrice);

            (await this.Subject.GetFramePriceAsync(ProductCode.Of("KT20-001F"), MandarinGrpcFramePricesServiceTests.LastMonth)).Should().Be(original with { ActiveUntil = MandarinGrpcFramePricesServiceTests.Today });
            (await this.Subject.GetFramePriceAsync(ProductCode.Of("KT20-001F"), MandarinGrpcFramePricesServiceTests.Today)).Should().Be(newFramePrice);
        }

        [Fact]
        public async Task ShouldBeAbleToDeleteAFramePrice()
        {
            await this.Subject.DeleteFramePriceAsync(ProductCode.Of("KT20-001F"));

            var deletedFramePrice = await this.Subject.GetFramePriceAsync(ProductCode.Of("KT20-001F"), MandarinGrpcFramePricesServiceTests.Today);
            deletedFramePrice.Should().BeNull();
            var existingFramePrice = await this.Subject.GetFramePriceAsync(ProductCode.Of("KT20-002F"), MandarinGrpcFramePricesServiceTests.Today);
            existingFramePrice.Should().NotBeNull();
        }
    }
}
