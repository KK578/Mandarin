using System;
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
        /// Gets the specified artist by their unique stockist code.
        /// </summary>
        /// <param name="stockistCode">The stockist code of the artist.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous fetch operation of the requested artist.</returns>
        Task<Stockist> GetArtistByCodeAsync(string stockistCode);

        /// <summary>
        /// Gets a list of all artists that should be considered for commission.
        /// TODO: This list should return a time sensitive list of artists.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> containing all artists for commissioning purposes.</returns>
        public IObservable<Stockist> GetArtistsForCommissionAsync();

        /// <summary>
        /// Saves all changes made to the artist. Will automatically detect if they are a new artist and make changes as required.
        /// </summary>
        /// <param name="stockist">The stockist to be added or updated.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous save operation.</returns>
        Task SaveArtistAsync(Stockist stockist);
    }
}
