using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Artists;

namespace Mandarin.Services.Fruity
{
    public interface IArtistsService
    {
        public Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetails();
    }
}
