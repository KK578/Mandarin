using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Artists;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents a service that can retrieve details about artists.
    /// </summary>
    public interface IArtistService
    {
        /// <summary>
        /// Gets the full list of details for all currently active artists.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing all artists.</returns>
        public Task<IReadOnlyList<ArtistDetailsModel>> GetArtistDetailsAsync();
    }
}
