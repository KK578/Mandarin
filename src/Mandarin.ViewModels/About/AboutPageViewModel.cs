using Mandarin.ViewModels.Components.Images;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.About
{
    /// <inheritdoc />
    internal sealed class AboutPageViewModel : IAboutPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutPageViewModel"/> class.
        /// </summary>
        /// <param name="pageContentModel">The public website content model.</param>
        public AboutPageViewModel(PageContentModel pageContentModel)
        {
            this.ImageViewModel = new MandarinImageViewModel(pageContentModel.Get<ImageUrlModel>("About", "Image"));
            this.MainContent = pageContentModel.GetMarkupString("About", "MainText");
            this.GiftCardContent = pageContentModel.GetMarkupString("About", "GiftCards", "Text");
            this.GiftCardImageViewModel = new MandarinImageViewModel(pageContentModel.Get<ImageUrlModel>("About", "GiftCards", "AnimationImage"));
        }

        /// <inheritdoc/>
        public IMandarinImageViewModel ImageViewModel { get; }

        /// <inheritdoc/>
        public MarkupString MainContent { get; }

        /// <inheritdoc/>
        public MarkupString GiftCardContent { get; }

        /// <inheritdoc/>
        public IMandarinImageViewModel GiftCardImageViewModel { get; }
    }
}
