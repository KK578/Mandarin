using System.Collections.Generic;
using System.Linq;
using Mandarin.ViewModels.Components.Images;
using Microsoft.AspNetCore.Components;

namespace Mandarin.ViewModels.MiniMandarin
{
    /// <inheritdoc />
    internal sealed class MiniMandarinPageViewModel : IMiniMandarinPageViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MiniMandarinPageViewModel"/> class.
        /// </summary>
        /// <param name="pageContentModel">The public website content model.</param>
        public MiniMandarinPageViewModel(PageContentModel pageContentModel)
        {
            this.TextContent = pageContentModel.GetMarkupString("MiniMandarin", "MainText");
            this.BannerImageViewModel = new MandarinImageViewModel(pageContentModel.Get<ImageUrlModel>("MiniMandarin", "BannerImage"));
            this.MacaronImageViewModels = pageContentModel.GetAll<ImageUrlModel>("MiniMandarin", "MacaronImages")
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
