using FluentAssertions;
using FluentValidation;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Mandarin.Tests.Data;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FramePrices
{
    public class FramePriceValidatorTests
    {
        private readonly IValidator<FramePriceViewModel> subject;
        private readonly FramePriceViewModel value;

        public FramePriceValidatorTests()
        {
            this.subject = new FramePriceValidator();
            this.value = new FramePriceViewModel(WellKnownTestData.FramePrices.Clementine, WellKnownTestData.Products.ClementineFramed);
        }

        [Theory]
        [InlineData(null, "'Product Code' must not be empty.")]
        [InlineData("AnInvalidProductCodeBecauseItIsLong", "The length of 'Product Code' must be 12 characters or fewer. You entered 35 characters.")]
        public void ShouldNotBeValidIfProductCodeIsInvalid(string productCode, string expectedError)
        {
            this.value.ProductCode = productCode;
            var result = this.subject.Validate(this.value, o => o.IncludeProperties(x => x.ProductCode));
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expectedError);
        }

        [Theory]
        [InlineData(-1, "'Frame Price' must be greater than or equal to '0'.")]
        [InlineData(5678, "'Frame Price' must be less than '95.00'.")]
        public void ShouldNotBeValidIfFramePriceIsInvalid(int framePrice, string expectedError)
        {
            this.value.FramePrice = framePrice;
            var result = this.subject.Validate(this.value, o => o.IncludeProperties(x => x.FramePrice));
            result.IsValid.Should().BeFalse();
            result.Errors[0].ErrorMessage.Should().Be(expectedError);
        }

        [Fact]
        public void ShouldBeValidIfValid()
        {
            var result = this.subject.Validate(this.value);
            result.IsValid.Should().BeTrue();
        }
    }
}
