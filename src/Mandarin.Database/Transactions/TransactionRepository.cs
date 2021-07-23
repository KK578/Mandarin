using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bashi.Core.Extensions;
using Dapper;
using Mandarin.Database.Common;
using Mandarin.Database.Inventory;
using Mandarin.Transactions;
using Microsoft.Extensions.Logging;

namespace Mandarin.Database.Transactions
{
    /// <inheritdoc cref="Mandarin.Transactions.ITransactionRepository" />
    internal sealed class TransactionRepository : DatabaseRepositoryBase<Transaction, TransactionRecord>, ITransactionRepository
    {
        private const string GetAllTransactionsSql = @"
            SELECT *
            FROM billing.transaction
            ORDER BY timestamp";

        private const string GetAllSubtransactionsSql = @"
            SELECT *
            FROM billing.subtransaction";

        private const string GetAllProductsSql = @"
            SELECT *
            FROM inventory.product";

        private const string UpsertTransactionSql = @"
            INSERT INTO billing.transaction (transaction_id, total_amount, timestamp)
            VALUES (@transaction_id, @total_amount, @timestamp)";

        private const string UpsertSubtransactionSql = @"
            INSERT INTO billing.subtransaction (transaction_id, product_id, quantity, subtotal)
            VALUES (@transaction_id, @product_id, @quantity, @subtotal)";

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public TransactionRepository(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<TransactionRepository> logger)
            : base(mandarinDbContext, mapper, logger)
        {
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync() => this.GetAll();

        /// <inheritdoc />
        public Task SaveTransactionAsync(Transaction transaction) => this.Upsert(transaction);

        /// <inheritdoc />
        protected override string ExtractDisplayKey(Transaction value) => value.TransactionId.Value;

        /// <inheritdoc />
        protected override async Task<IEnumerable<TransactionRecord>> GetAllRecords(IDbConnection db)
        {
            var transactions = await db.QueryAsync<TransactionRecord>(TransactionRepository.GetAllTransactionsSql);
            var subtransactions = await db.QueryAsync<SubtransactionRecord>(TransactionRepository.GetAllSubtransactionsSql);
            var products = await db.QueryAsync<ProductRecord>(TransactionRepository.GetAllProductsSql);

            var productLookup = products.ToDictionary(x => x.product_id);
            var subtransactionLookup = subtransactions.Select(x => x with { Product = productLookup[x.product_id] })
                                                      .GroupBy(x => x.transaction_id).ToDictionary(x => x.Key, x => x.AsReadOnlyList());

            return transactions.Select(transaction => subtransactionLookup.TryGetValue(transaction.transaction_id, out var s)
                                           ? transaction with { Subtransactions = s }
                                           : transaction);
        }

        /// <inheritdoc />
        protected override async Task<TransactionRecord> UpsertRecordAsync(IDbConnection db, TransactionRecord value)
        {
            await db.ExecuteAsync(TransactionRepository.UpsertTransactionSql, value);
            foreach (var subtransaction in value.Subtransactions.NullToEmpty())
            {
                await db.ExecuteAsync(TransactionRepository.UpsertSubtransactionSql, subtransaction with { transaction_id = value.transaction_id });
            }

            return value;
        }
    }
}
