using System.Collections.Generic;
using System.Linq;
using Mandarin.Services.Fruity;

namespace Mandarin.ViewModels.Artists
{
    internal sealed class ArtistsPageViewModel : IArtistsPageViewModel
    {
        public ArtistsPageViewModel(IArtistsService artistsService)
        {
            var details = artistsService.GetArtistDetails().Result;
            this.ViewModels = details.Select(x => new ArtistViewModel(x)).ToList().AsReadOnly();
        }

        public IReadOnlyList<IArtistViewModel> ViewModels { get; }
    }
}
