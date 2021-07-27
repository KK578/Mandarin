using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Mandarin.Transactions;

namespace Mandarin.Services.Transactions
{
    /// <inheritdoc />
    internal class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionService"/> class.
        /// </summary>
        /// <param name="transactionRepository">The application repository for interacting with transactions.</param>
        public TransactionService(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        /// <inheritdoc/>
        public IObservable<Transaction> GetAllTransactions(DateTime start, DateTime end)
        {
            return this.transactionRepository.GetAllTransactionsAsync().ToObservable().SelectMany(x => x);
        }
    }
}
