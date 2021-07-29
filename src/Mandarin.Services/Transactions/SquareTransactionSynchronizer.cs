using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using Mandarin.Transactions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Square.Models;
using Transaction = Mandarin.Transactions.Transaction;

namespace Mandarin.Services.Transactions
{
    /// <inheritdoc />
    internal sealed class SquareTransactionSynchronizer : ITransactionSynchronizer
    {
        private readonly ILogger<SquareTransactionSynchronizer> logger;
        private readonly ISquareTransactionService squareTransactionService;
        private readonly ITransactionAuditRepository transactionAuditRepository;
        private readonly ITransactionRepository transactionRepository;
        private readonly ITransactionMapper transactionMapper;
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareTransactionSynchronizer"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="squareTransactionService">The service to fetch transactions from Square.</param>
        /// <param name="transactionAuditRepository">The application repository for interacting with transaction audits.</param>
        /// <param name="transactionRepository">The application repository for interacting with transactions.</param>
        /// <param name="transactionMapper">The service to map Square orders to a Transation.</param>
        public SquareTransactionSynchronizer(ILogger<SquareTransactionSynchronizer> logger,
                                             ISquareTransactionService squareTransactionService,
                                             ITransactionAuditRepository transactionAuditRepository,
                                             ITransactionRepository transactionRepository,
                                             ITransactionMapper transactionMapper)
        {
            this.logger = logger;
            this.squareTransactionService = squareTransactionService;
            this.transactionAuditRepository = transactionAuditRepository;
            this.transactionRepository = transactionRepository;
            this.transactionMapper = transactionMapper;
            this.semaphore = new SemaphoreSlim(1);
        }

        /// <inheritdoc/>
        public async Task LoadSquareOrders(DateTime start, DateTime end)
        {
            await this.semaphore.WaitAsync();
            var processCount = 0;
            try
            {
                var orders = await this.squareTransactionService.GetAllOrders(start, end).ToList();
                foreach (var order in orders)
                {
                    var transactionId = new TransactionId(order.Id);
                    var lastUpdated = DateTime.Parse(order.UpdatedAt);
                    var existingAudit = await this.transactionAuditRepository.GetTransactionAuditAsync(transactionId, lastUpdated);

                    if (existingAudit is null)
                    {
                        await this.transactionAuditRepository.SaveTransactionAuditAsync(new TransactionAudit
                        {
                            TransactionId = transactionId,
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
        public async Task SynchronizeTransactionAsync(TransactionId transactionId)
        {
            var transactionAudit = await this.transactionAuditRepository.GetTransactionAuditAsync(transactionId);
            var order = JsonConvert.DeserializeObject<Order>(transactionAudit.RawData);

            var transaction = await this.transactionMapper.MapToTransaction(order);
            var existingTransaction = await this.transactionRepository.GetTransactionAsync(transaction.TransactionId);

            if (existingTransaction is null)
            {
                this.logger.LogInformation("Inserting new transaction: {Transaction}", transaction);
                await this.transactionRepository.SaveTransactionAsync(transaction);
            }
            else if (!this.AreTransactionsEquivalent(transaction, existingTransaction))
            {
                this.logger.LogInformation("Updating {TransactionId} to new version: {Transaction}", transaction.TransactionId, transaction);
                await this.transactionRepository.SaveTransactionAsync(transaction);
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
