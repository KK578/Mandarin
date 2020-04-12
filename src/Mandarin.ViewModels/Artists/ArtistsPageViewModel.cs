using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.Services.Fruity;

namespace Mandarin.ViewModels.Artists
{
    internal sealed class ArtistsPageViewModel : IArtistsPageViewModel
    {
        private readonly IArtistService artistService;

        public ArtistsPageViewModel(IArtistService artistService)
        {
            this.artistService = artistService;
            var unused = this.UpdateViewModels();
        }

        private async Task UpdateViewModels()
        {
            try
            {
                this.IsLoading = true;
                var details = await this.artistService.GetArtistDetailsAsync();
                this.ViewModels = details.Select(x => new ArtistViewModel(x)).ToList().AsReadOnly();
            }
            finally
            {
                this.IsLoading = false;
            }
        }

        public bool IsLoading { get; private set; }
        public IReadOnlyList<IArtistViewModel> ViewModels { get; private set; } = new List<IArtistViewModel>();
    }
}
