using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Stockists;
using NodaTime;

namespace Mandarin.Commissions
{
    /// <summary>
    /// Represents a repository that can retrieve details about <see cref="RecordOfSales"/>.
    /// </summary>
    public interface IRecordOfSalesRepository
    {
        /// <summary>
        /// Gets the <see cref="IReadOnlyList{T}"/> for the <see cref="RecordOfSales"/> for each <see cref="Stockist"/> active at the current time.
        /// </summary>
        /// <param name="interval">The interval for the record of sales.</param>
        /// <returns>A <see cref="Task"/> containing the list of <see cref="RecordOfSales"/> for each active stockist.</returns>
        Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesAsync(Interval interval);
    }
}
