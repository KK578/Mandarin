using System.Collections.Generic;

namespace Mandarin.ViewModels.Artists
{
    public interface IArtistsPageViewModel
    {
        IReadOnlyList<IArtistViewModel> ViewModels { get; }
    }
}
