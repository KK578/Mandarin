using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Artists;

namespace Mandarin.Services.Fruity
{
    public interface IArtistService
    {
        public Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync();
    }
}
