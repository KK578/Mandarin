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
using Mandarin.Transactions.External;
using Microsoft.Extensions.Logging;
using NodaTime;
using Npgsql;

namespace Mandarin.Database.Transactions
{
    /// <inheritdoc cref="Mandarin.Transactions.ITransactionRepository" />
    internal sealed class TransactionRepository : DatabaseRepositoryBase<Transaction, TransactionRecord>, ITransactionRepository
    {
        private const string GetTransactionByExternalIdSql = @"
            SELECT *
            FROM billing.transaction
            WHERE external_transaction_id = @external_transaction_id";

        private const string GetSubtransactionByIdSql = @"
            SELECT s.subtransaction_id, s.transaction_id, s.quantity, s.unit_price, 
                   p.product_id, p.stockist_id, p.product_code, p.product_name, p.description, p.unit_price, p.last_updated
            FROM billing.subtransaction s
            INNER JOIN inventory.product p ON s.product_id = p.product_id
            WHERE transaction_id = @transaction_id";

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
            CALL billing.sp_transaction_upsert(@external_transaction_id, @total_amount, @timestamp, @subtransactions)";

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
        public async Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync(Interval interval)
        {
            // TODO: The base repository doesn't support get many with query semantics...
            var all = await this.GetAll();
            return all.Where(x => interval.Contains(x.Timestamp)).AsReadOnlyList();
        }

        /// <inheritdoc/>
        public Task<Transaction> GetTransactionAsync(ExternalTransactionId externalTransactionId)
        {
            return this.Get(externalTransactionId,
                            async db =>
                            {
                                var parameters = new { external_transaction_id = externalTransactionId.Value };
                                var transaction = await db.QueryFirstOrDefaultAsync<TransactionRecord>(TransactionRepository.GetTransactionByExternalIdSql, parameters);
                                if (transaction == null)
                                {
                                    return null;
                                }

                                var subtransactions = await db.QueryAsync<SubtransactionRecord, ProductRecord, SubtransactionRecord>(
                                 TransactionRepository.GetSubtransactionByIdSql,
                                 (s, p) => s with { product_id = p.product_id, Product = p },
                                 new { transaction_id = transaction.transaction_id },
                                 splitOn: "subtransaction_id,product_id");

                                return transaction with { Subtransactions = subtransactions.ToList() };
                            });
        }

        /// <inheritdoc />
        public Task SaveTransactionAsync(Transaction transaction) => this.Upsert(transaction);

        /// <inheritdoc />
        protected override string ExtractDisplayKey(Transaction value) => value.TransactionId?.ToString() ?? value.ExternalTransactionId.ToString();

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
            db.Open();
            (db as NpgsqlConnection)?.TypeMapper.MapComposite<SubtransactionRecord>("billing.tvp_subtransaction");
            await db.ExecuteAsync(TransactionRepository.UpsertTransactionSql, value);
            return value;
        }
    }
}
