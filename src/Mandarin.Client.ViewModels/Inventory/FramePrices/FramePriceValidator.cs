using FluentValidation;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <summary>
    /// Represents a <see cref="IValidator{T}"/> for <see cref="IFramePriceViewModel"/>.
    /// </summary>
    internal sealed class FramePriceValidator : AbstractValidator<IFramePriceViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FramePriceValidator"/> class.
        /// </summary>
        public FramePriceValidator()
        {
            this.RuleFor(x => x.ProductCode).NotEmpty().MaximumLength(12);
            this.RuleFor(x => x.RetailPrice).NotNull().GreaterThan(0);
            this.RuleFor(x => x.FramePrice).NotNull().GreaterThanOrEqualTo(0).LessThan(x => x.RetailPrice);
        }
    }
}
