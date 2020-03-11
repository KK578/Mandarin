using System.Collections.Generic;
using System.Linq;

namespace Mandarin.ViewModels.Artists
{
    internal sealed class ArtistsPageViewModel : IArtistsPageViewModel
    {
        public ArtistsPageViewModel(IEnumerable<IArtistViewModel> viewModels)
        {
            this.ViewModels = viewModels.ToList().AsReadOnly();
        }

        public IReadOnlyList<IArtistViewModel> ViewModels { get; }
    }
}
