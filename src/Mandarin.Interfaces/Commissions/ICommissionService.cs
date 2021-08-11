using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Stockists;
using NodaTime;

namespace Mandarin.Commissions
{
    /// <summary>
    /// Represents a service that can retrieve commission breakdowns by stockists.
    /// </summary>
    public interface ICommissionService
    {
        /// <summary>
        /// Gets the <see cref="RecordOfSales"/> for each active <see cref="Stockist"/>.
        /// The included sales are limited to those between the specified dates.
        /// </summary>
        /// <param name="interval">The date interval to be considered for the Record of Sales.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of the sales broken down by each <see cref="Stockist"/>.</returns>
        Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesAsync(DateInterval interval);
    }
}
