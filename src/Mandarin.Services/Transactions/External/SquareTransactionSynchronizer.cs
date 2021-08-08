using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Mandarin.Transactions;
using Mandarin.Transactions.External;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Square.Models;
using Transaction = Mandarin.Transactions.Transaction;

namespace Mandarin.Services.Transactions.External
{
    /// <inheritdoc />
    internal sealed class SquareTransactionSynchronizer : ITransactionSynchronizer
    {
        private readonly ILogger<SquareTransactionSynchronizer> logger;
        private readonly ISquareTransactionService squareTransactionService;
        private readonly IExternalTransactionRepository externalTransactionRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly ISquareTransactionMapper squareTransactionMapper;
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareTransactionSynchronizer"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="squareTransactionService">The service to fetch transactions from Square.</param>
        /// <param name="externalTransactionRepository">The application repository for interacting with external transactions.</param>
        /// <param name="transactionRepository">The application repository for interacting with transactions.</param>
        /// <param name="squareTransactionMapper">The service to map Square orders to a Transaction.</param>
        public SquareTransactionSynchronizer(ILogger<SquareTransactionSynchronizer> logger,
                                             ISquareTransactionService squareTransactionService,
                                             IExternalTransactionRepository externalTransactionRepository,
                                             ITransactionRepository transactionRepository,
                                             ISquareTransactionMapper squareTransactionMapper)
        {
            this.logger = logger;
            this.squareTransactionService = squareTransactionService;
            this.externalTransactionRepository = externalTransactionRepository;
            this.transactionRepository = transactionRepository;
            this.squareTransactionMapper = squareTransactionMapper;
            this.semaphore = new SemaphoreSlim(1);
        }

        /// <inheritdoc/>
        public async Task LoadExternalTransactions(DateTime start, DateTime end)
        {
            await this.semaphore.WaitAsync();
            var processCount = 0;
            try
            {
                var orders = await this.squareTransactionService.GetAllOrders(start, end).ToList();
                foreach (var order in orders)
                {
                    var transactionId = ExternalTransactionId.Of(order.Id);
                    var lastUpdated = DateTime.Parse(order.UpdatedAt);
                    var externalTransaction = await this.externalTransactionRepository.GetExternalTransactionAsync(transactionId, lastUpdated);

                    if (externalTransaction is null)
                    {
                        await this.externalTransactionRepository.SaveExternalTransactionAsync(new ExternalTransaction
                        {
                            ExternalTransactionId = transactionId,
                            CreatedAt = DateTime.Parse(order.CreatedAt),
                            UpdatedAt = lastUpdated,
                            RawData = JsonConvert.SerializeObject(order),
                        });
                        BackgroundJob.Enqueue<ITransactionSynchronizer>(s => s.SynchronizeTransactionAsync(transactionId));
                        processCount++;
                    }
                }
            }
            finally
            {
                this.logger.LogInformation("Queued {Count} transactions for synchronization.", processCount);
                this.semaphore.Release();
            }
        }

        /// <inheritdoc />
        public async Task SynchronizeTransactionAsync(ExternalTransactionId externalTransactionId)
        {
            var externalTransaction = await this.externalTransactionRepository.GetExternalTransactionAsync(externalTransactionId);
            var order = JsonConvert.DeserializeObject<Order>(externalTransaction.RawData);

            var transaction = await this.squareTransactionMapper.MapToTransaction(order);
            var existingTransaction = await this.transactionRepository.GetTransactionAsync(transaction.ExternalTransactionId);

            if (existingTransaction is null)
            {
                this.logger.LogInformation("Inserting new transaction: {Transaction}", transaction);
                await this.transactionRepository.SaveTransactionAsync(transaction);
            }
            else if (!SquareTransactionSynchronizer.AreTransactionsEquivalent(transaction, existingTransaction))
            {
                this.logger.LogInformation("Updating {TransactionId} to new version: {Transaction}", transaction.ExternalTransactionId, transaction);
                await this.transactionRepository.SaveTransactionAsync(transaction);
            }
        }

        private static bool AreTransactionsEquivalent(Transaction x, Transaction y)
        {
            return x.ExternalTransactionId.Equals(y.ExternalTransactionId)
                   && x.TotalAmount == y.TotalAmount
                   && x.Timestamp.Equals(y.Timestamp)
                   && x.Subtransactions.OrderBy(s => s.Product.ProductId).SequenceEqual(y.Subtransactions.OrderBy(s => s.Product.ProductId));
        }
    }
}
