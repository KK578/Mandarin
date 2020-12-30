using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Models.Commissions;
using Mandarin.Models.Stockists;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents a service that can retrieve commission breakdowns by stockists.
    /// </summary>
    public interface ICommissionService
    {
        /// <summary>
        /// Gets all currently available commission rate groups.
        /// </summary>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of all commission rate groups.</returns>
        Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroupsAsync();

        /// <summary>
        /// Gets the <see cref="RecordOfSales"/> for each active <see cref="Stockist"/>.
        /// The included sales are limited to those between the specified dates.
        /// </summary>
        /// <param name="start">The start datetime to query transactions for.</param>
        /// <param name="end">The end datetime to query transactions for.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of the sales broken down by each <see cref="Stockist"/>.</returns>
        Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesForPeriodAsync(DateTime start, DateTime end);
    }
}
