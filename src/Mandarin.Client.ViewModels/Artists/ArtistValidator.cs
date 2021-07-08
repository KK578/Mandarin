using FluentValidation;

namespace Mandarin.Client.ViewModels.Artists
{
    /// <summary>
    /// Represents a <see cref="IValidator{T}"/> for <see cref="IArtistViewModel"/>.
    /// </summary>
    internal sealed class ArtistValidator : AbstractValidator<IArtistViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistValidator"/> class.
        /// </summary>
        public ArtistValidator()
        {
            this.RuleFor(x => x.StockistCode).NotEmpty().MaximumLength(6);
            this.RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
            this.RuleFor(x => x.LastName).MaximumLength(100);
            this.RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(250);
            this.RuleFor(x => x.EmailAddress).EmailAddress().MaximumLength(100);
            this.RuleFor(x => x.TwitterHandle).MaximumLength(30);
            this.RuleFor(x => x.FacebookHandle).MaximumLength(30);
            this.RuleFor(x => x.InstagramHandle).MaximumLength(30);
            this.RuleFor(x => x.TumblrHandle).MaximumLength(30);
            this.RuleFor(x => x.WebsiteUrl).MaximumLength(150);
            this.RuleFor(x => x.Rate).InclusiveBetween(0, 100);
            this.RuleFor(x => x.StartDate).LessThan(x => x.EndDate);
            this.RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate);
        }
    }
}
