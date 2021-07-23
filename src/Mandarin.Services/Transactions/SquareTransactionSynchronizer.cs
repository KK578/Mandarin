using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Transactions;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Transactions
{
    /// <inheritdoc />
    internal sealed class SquareTransactionSynchronizer : ITransactionSynchronizer
    {
        private readonly ILogger<SquareTransactionSynchronizer> logger;
        private readonly ITransactionService transactionService;
        private readonly ITransactionRepository transactionRepository;
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareTransactionSynchronizer"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="transactionService">The service to fetch transactions from Square.</param>
        /// <param name="transactionRepository">The application repository for interacting with transactions.</param>
        public SquareTransactionSynchronizer(ILogger<SquareTransactionSynchronizer> logger,
                                             ITransactionService transactionService,
                                             ITransactionRepository transactionRepository)
        {
            this.logger = logger;
            this.transactionService = transactionService;
            this.transactionRepository = transactionRepository;
            this.semaphore = new SemaphoreSlim(1);
        }

        /// <inheritdoc />
        public async Task SynchroniseTransactionsAsync(DateTime start, DateTime end)
        {
            var updateCount = 0;
            this.logger.LogInformation("Starting Square transaction synchronisation.");
            await this.semaphore.WaitAsync();
            try
            {
                var existingTransactions = (await this.transactionRepository.GetAllTransactionsAsync()).ToDictionary(x => x.TransactionId);
                var squareTransactions = await this.transactionService.GetAllTransactions(start, end).ToList();

                foreach (var transaction in squareTransactions)
                {
                    if (existingTransactions.TryGetValue(transaction.TransactionId, out var existingTransaction))
                    {
                        if (!this.AreTransactionsEquivalent(transaction, existingTransaction))
                        {
                            this.logger.LogInformation("Updating {TransactionId} to new version: {Transaction}", transaction.TransactionId, transaction);
                            await this.transactionRepository.SaveTransactionAsync(transaction);
                            updateCount++;
                        }
                    }
                    else
                    {
                        this.logger.LogInformation("Inserting new transaction: {Transaction}", transaction);
                        await this.transactionRepository.SaveTransactionAsync(transaction);
                        updateCount++;
                    }
                }
            }
            finally
            {
                this.logger.LogInformation("Finished transaction synchronisation - Update Count: {Count}.", updateCount);
                this.semaphore.Release();
            }
        }

        private bool AreTransactionsEquivalent(Transaction x, Transaction y)
        {
            return x.TransactionId.Equals(y.TransactionId)
                   && x.TotalAmount == y.TotalAmount
                   && x.Timestamp.Equals(y.Timestamp)
                   && x.Subtransactions.OrderBy(s => s.Product.ProductId).SequenceEqual(y.Subtransactions.OrderBy(s => s.Product.ProductId));
        }
    }
}
