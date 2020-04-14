using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.MiniMandarin
{
    internal sealed class MiniMandarinPageViewModel : IMiniMandarinPageViewModel
    {
        public MiniMandarinPageViewModel()
        {
            this.BannerImageViewModel = new MandarinImageViewModel("/images/the-mini-mandarin/TheMiniMandarin-Banner.jpg", "The Mini Mandarin - Bearcarons");
            this.MacaronImageViewModels = new List<IMandarinImageViewModel>
            {
                new MandarinImageViewModel("/images/the-mini-mandarin/TheMiniMandarin-Macaron-Chocolate.jpg", "The Mini Mandarin - Chocolate Macaron"),
                new MandarinImageViewModel("/images/the-mini-mandarin/TheMiniMandarin-Macaron-Strawberry.jpg", "The Mini Mandarin - Strawberry Macaron"),
                new MandarinImageViewModel("/images/the-mini-mandarin/TheMiniMandarin-Macaron-Matcha.jpg", "The Mini Mandarin - Matcha Macaron"),
            }.AsReadOnly();
        }

        public IMandarinImageViewModel BannerImageViewModel { get; }
        public IReadOnlyList<IMandarinImageViewModel> MacaronImageViewModels { get; }
    }
}
