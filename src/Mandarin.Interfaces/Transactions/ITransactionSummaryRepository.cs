using System.Collections.Generic;
using System.Threading.Tasks;
using NodaTime;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="Mandarin.Transactions.TransactionSummary"/>.
    /// </summary>
    public interface ITransactionSummaryRepository
    {
        /// <summary>
        /// Gets a list of all <see cref="Mandarin.Transactions.TransactionSummary"/> items between the given <see cref="DateInterval"/>.
        /// </summary>
        /// <param name="interval">The interval in which transaction summaries should be retrieved.</param>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of transaction summaries between the given date interval.</returns>
        Task<IReadOnlyList<TransactionSummary>> GetTransactionSummariesAsync(DateInterval interval);
    }
}
