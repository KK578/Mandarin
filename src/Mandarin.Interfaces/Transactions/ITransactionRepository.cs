using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mandarin.Transactions
{
    /// <summary>
    /// Represents a repository that can retrieve and update details about <see cref="Transaction"/>.
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Gets a list of all known transactions.
        /// </summary>
        /// <returns>A <see cref="Task"/> containing a <see cref="IReadOnlyList{T}"/> of all transactions.</returns>
        Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync();

        /// <summary>
        /// Saves the provided <see cref="Transaction"/>, and returns the new version of the <see cref="Transaction"/> after it
        /// has been saved successfully.
        /// </summary>
        /// <param name="transaction">The <see cref="Transaction"/> to be saved.</param>
        /// <returns>A <see cref="Task"/> containing the new <see cref="Transaction"/> after the operation is complete.</returns>
        Task SaveTransactionAsync(Transaction transaction);
    }
}
