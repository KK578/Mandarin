using System;
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
using Npgsql;

namespace Mandarin.Database.Transactions
{
    /// <inheritdoc cref="Mandarin.Transactions.ITransactionRepository" />
    internal sealed class TransactionRepository : DatabaseRepositoryBase<Transaction, TransactionRecord>, ITransactionRepository
    {
        private const string GetTransactionByIdSql = @"
            SELECT *
            FROM billing.transaction
            WHERE transaction_id = @transaction_id";

        private const string GetSubtransactionByIdSql = @"
            SELECT s.subtransaction_id, s.transaction_id, s.quantity, s.subtotal, 
                   p.product_id, product_code, product_name, description, unit_price, last_updated
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
            CALL billing.sp_transaction_upsert(@transaction_id, @total_amount, @timestamp, @subtransactions)";

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
        public async Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync(DateTime start, DateTime end)
        {
            // TODO: The base repository doesn't support get many with query semantics...
            var all = await this.GetAll();
            return all.Where(x => x.Timestamp >= start && x.Timestamp <= end).AsReadOnlyList();
        }

        /// <inheritdoc/>
        public Task<Transaction> GetTransactionAsync(TransactionId transactionId)
        {
            return this.Get(transactionId,
                            async db =>
                            {
                                var parameters = new { transaction_id = transactionId.Value };
                                var transaction = await db.QueryFirstOrDefaultAsync<TransactionRecord>(TransactionRepository.GetTransactionByIdSql, parameters);
                                if (transaction == null)
                                {
                                    return null;
                                }

                                var subtransactions = await db.QueryAsync<SubtransactionRecord, ProductRecord, SubtransactionRecord>(
                                 TransactionRepository.GetSubtransactionByIdSql,
                                 (s, p) => s with { product_id = p.product_id, Product = p },
                                 parameters,
                                 splitOn: "subtransaction_id,product_id");

                                return transaction with { Subtransactions = subtransactions.ToList() };
                            });
        }

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
            db.Open();
            (db as NpgsqlConnection)?.TypeMapper.MapComposite<SubtransactionRecord>("billing.tvp_subtransaction");
            await db.ExecuteAsync(TransactionRepository.UpsertTransactionSql, value);
            return value;
        }
    }
}
