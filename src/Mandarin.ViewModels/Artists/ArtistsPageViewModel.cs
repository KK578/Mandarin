using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.Services;

namespace Mandarin.ViewModels.Artists
{
    /// <inheritdoc cref="Mandarin.ViewModels.Artists.IArtistsPageViewModel" />
    internal sealed class ArtistsPageViewModel : ViewModelBase, IArtistsPageViewModel
    {
        private readonly IArtistService artistService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistsPageViewModel"/> class.
        /// </summary>
        /// <param name="artistService">The artist service.</param>
        public ArtistsPageViewModel(IArtistService artistService)
        {
            this.artistService = artistService;
            #pragma warning disable 4014
            this.UpdateViewModels();
            #pragma warning restore 4014
        }

        /// <inheritdoc/>
        public bool IsLoading { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<IArtistViewModel> ViewModels { get; private set; } = new List<IArtistViewModel>();

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
                this.OnPropertyChanged(nameof(this.IsLoading));
                this.OnPropertyChanged(nameof(this.ViewModels));
            }
        }
    }
}
