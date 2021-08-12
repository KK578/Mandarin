using System;
using FluentAssertions;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Mandarin.Inventory;
using Mandarin.Tests.Data;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FramePrices
{
    public class FramePriceViewModelTests
    {
        private static readonly Instant Today = Instant.FromUtc(2021, 08, 08, 12, 20, 00);
        private static readonly FramePrice FramePrice = new()
        {
            ProductCode = ProductCode.Of("TLM-001"),
            Amount = 15.00M,
            CreatedAt = Instant.FromUtc(2021, 07, 01, 00, 00, 00),
        };

        private readonly IClock clock = new FakeClock(FramePriceViewModelTests.Today);
        private readonly IFramePriceViewModel subject;

        public FramePriceViewModelTests()
        {
            this.subject = new FramePriceViewModel(FramePriceViewModelTests.FramePrice, WellKnownTestData.Products.Mandarin, this.clock);
        }

        [Fact]
        public void PropertiesShouldMatchExpected()
        {
            this.subject.ProductCode.Should().Be("TLM-001");
            this.subject.ProductName.Should().Be("[TLM-001] Mandarin");
            this.subject.RetailPrice.Should().Be(45.00M);
            this.subject.FramePrice.Should().Be(15.00M);
            this.subject.ArtistPrice.Should().Be(30.00M);
        }

        public class ArtistPriceTests : FramePriceViewModelTests
        {
            [Fact]
            public void ShouldRaisePropertyChangedOnRetailPriceChanging()
            {
                var monitor = this.subject.Monitor();
                this.subject.RetailPrice = 99.00M;
                monitor.Should().RaisePropertyChangeFor(x => x.ArtistPrice);
            }

            [Fact]
            public void ShouldRaisePropertyChangedOnFramePriceChanging()
            {
                var monitor = this.subject.Monitor();
                this.subject.FramePrice = 99.00M;
                monitor.Should().RaisePropertyChangeFor(x => x.ArtistPrice);
            }

            [Theory]
            [InlineData(50, null, 35)]
            [InlineData(55, null, 40)]
            [InlineData(null, 10, 35)]
            [InlineData(null, 12, 33)]
            [InlineData(55, 25, 30)]
            public void ShouldUpdateOnRetailPriceUpdating(int? retailPrice, int? framePrice, int expectedArtistPrice)
            {
                if (retailPrice.HasValue)
                {
                    this.subject.RetailPrice = retailPrice;
                }

                if (framePrice.HasValue)
                {
                    this.subject.FramePrice = framePrice.Value;
                }

                this.subject.ArtistPrice.Should().Be(expectedArtistPrice);
            }
        }

        public class CreatedAtTests : FramePriceViewModelTests
        {
            [Fact]
            public void ShouldDefaultToNow()
            {
                new FramePriceViewModel(this.clock).CreatedAt.Should().Be(FramePriceViewModelTests.Today);
            }

            [Fact]
            public void CreatedAtShouldAutomaticallyUpdateOnChangingCommissionAmount()
            {
                this.subject.CreatedAt.Should().Be(FramePriceViewModelTests.FramePrice.CreatedAt);

                this.subject.FramePrice = 20.00M;
                this.subject.CreatedAt.Should().Be(FramePriceViewModelTests.Today);

                this.subject.FramePrice = 15.00M;
                this.subject.CreatedAt.Should().Be(FramePriceViewModelTests.FramePrice.CreatedAt);
            }
        }
    }
}
