using FluentAssertions;
using FluentValidation;
using Mandarin.Client.ViewModels.Inventory.FramePrices;
using Mandarin.Tests.Data;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace Mandarin.Client.ViewModels.Tests.Inventory.FramePrices
{
    public class FramePriceValidatorTests
    {
        private static readonly Instant Today = Instant.FromUtc(2021, 08, 02, 12, 20, 00);

        private readonly IClock clock = new FakeClock(FramePriceValidatorTests.Today);
        private readonly IFramePriceViewModel value;
        private readonly IValidator<IFramePriceViewModel> subject;

        public FramePriceValidatorTests()
        {
            this.value = new FramePriceViewModel(WellKnownTestData.FramePrices.Clementine, WellKnownTestData.Products.ClementineFramed, this.clock);
            this.subject = new FramePriceValidator();
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
