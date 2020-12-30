using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents a service that can retrieve commission breakdowns by artists.
    /// </summary>
    public interface ICommissionService
    {
        /// <summary>
        /// Gets an observable sequence of all commission rate groups.
        /// </summary>
        /// <returns>Observable sequence of all commission rate groups.</returns>
        Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroupsAsync();

        /// <summary>
        /// Gets an observable sequence of all artist sales between the specified dates.
        /// </summary>
        /// <param name="start">The start datetime to query transactions for.</param>
        /// <param name="end">The end datetime to query transactions for.</param>
        /// <returns>Observable sequence of all artist sales between the specified dates.</returns>
        IObservable<ArtistSales> GetSalesByArtistForPeriod(DateTime start, DateTime end);
    }
}
