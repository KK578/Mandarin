using System.Collections.Generic;
using System.Linq;
using Mandarin.ViewModels.Components.Images;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.Macarons
{
    /// <inheritdoc />
    internal sealed class MacaronsPageViewModel : IMacaronsPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MacaronsPageViewModel"/> class.
        /// </summary>
        /// <param name="pageContentModel">The public website content model.</param>
        public MacaronsPageViewModel(PageContentModel pageContentModel)
        {
            this.TextContent = pageContentModel.GetMarkupString("Macarons", "MainText");
            this.BannerImageViewModel = new MandarinImageViewModel(pageContentModel.Get<ImageUrlModel>("Macarons", "BannerImage"));
            this.MacaronImageViewModels = pageContentModel.GetAll<ImageUrlModel>("Macarons", "MacaronImages")
                                                          .Select(x => new MandarinImageViewModel(x))
                                                          .ToList()
                                                          .AsReadOnly();
        }

        /// <inheritdoc/>
        public MarkupString TextContent { get; }

        /// <inheritdoc/>
        public IMandarinImageViewModel BannerImageViewModel { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IMandarinImageViewModel> MacaronImageViewModels { get; }
    }
}
