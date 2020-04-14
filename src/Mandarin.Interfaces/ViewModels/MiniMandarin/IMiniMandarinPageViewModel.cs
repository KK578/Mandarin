using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.MiniMandarin
{
    public interface IMiniMandarinPageViewModel
    {
        IMandarinImageViewModel BannerImageViewModel { get; }
        IReadOnlyList<IMandarinImageViewModel> MacaronImageViewModels { get; }
    }
}
