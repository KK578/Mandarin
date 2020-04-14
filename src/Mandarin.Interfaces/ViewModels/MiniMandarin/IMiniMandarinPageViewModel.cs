using System.Collections.Generic;
using Mandarin.ViewModels.Components.Images;

namespace Mandarin.ViewModels.MiniMandarin
{
    public interface IMiniMandarinPageViewModel
    {
        IReadOnlyList<string> Paragraphs { get; }
        IMandarinImageViewModel BannerImageViewModel { get; }
        IReadOnlyList<IMandarinImageViewModel> MacaronImageViewModels { get; }
    }
}
