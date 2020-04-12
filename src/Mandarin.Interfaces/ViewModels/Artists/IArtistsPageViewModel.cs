using System.Collections.Generic;

namespace Mandarin.ViewModels.Artists
{
    public interface IArtistsPageViewModel
    {
        bool IsLoading { get; }
        IReadOnlyList<IArtistViewModel> ViewModels { get; }
    }
}
