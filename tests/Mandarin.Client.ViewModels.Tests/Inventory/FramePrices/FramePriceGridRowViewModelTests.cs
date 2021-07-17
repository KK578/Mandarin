using FluentAssertions;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Mandarin.Inventory;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FramePrices
{
    public class FramePriceGridRowViewModelTests
    {
        private static readonly FramePrice FramePrice = new()
        {
            ProductCode = new ProductCode("TLM-001"),
            Amount = 15.00M,
        };

        private static readonly Product Product = new()
        {
            SquareId = new ProductId("SquareId"),
            ProductCode = new ProductCode("TLM-001"),
            ProductName = new ProductName("Mandarin"),
            Description = "It's a Mandarin!",
            UnitPrice = 45.00M,
        };

        [Fact]
        public void PropertiesShouldMatchExpected()
        {
            var subject = new FramePriceGridRowViewModel(FramePriceGridRowViewModelTests.FramePrice, FramePriceGridRowViewModelTests.Product);
            subject.ProductCode.Should().Be("TLM-001");
            subject.ProductName.Should().Be("Mandarin");
            subject.RetailPrice.Should().Be(45.00M);
            subject.FramePrice.Should().Be(15.00M);
            subject.ArtistPrice.Should().Be(30.00M);
        }
    }
}
