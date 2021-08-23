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
            SELECT s.subtransaction_id, s.transaction_id, s.quantity, s.unit_price, s.commission_rate, 
                   p.product_id, p.stockist_id, p.product_code, p.product_name, p.description, p.unit_price, p.last_updated
            FROM billing.subtransaction s
            INNER JOIN inventory.product p ON s.product_id = p.product_id
            WHERE transaction_id = @transaction_id";

        private const string GetAllTransactionsSql = @"
            SELECT *
            FROM billing.transaction
            ORDER BY timestamp";

        private const string GetAllTransactionsInIntervalSql = @"
            SELECT *
            FROM billing.transaction
            WHERE timestamp >= @start_date AND timestamp < @end_date
            ORDER BY timestamp";

        private const string GetAllSubtransactionsSql = @"
            SELECT s.subtransaction_id, s.transaction_id, s.quantity, s.unit_price, s.commission_rate, 
                   p.product_id, p.stockist_id, p.product_code, p.product_name, p.description, p.unit_price, p.last_updated
            FROM billing.subtransaction s
            INNER JOIN inventory.product p ON s.product_id = p.product_id";

        private const string GetAllSubtransactionsInIntervalSql = @"
            SELECT s.subtransaction_id, s.transaction_id, s.quantity, s.unit_price, s.commission_rate, 
                   p.product_id, p.stockist_id, p.product_code, p.product_name, p.description, p.unit_price, p.last_updated
            FROM billing.subtransaction s
            INNER JOIN billing.transaction t on s.transaction_id = t.transaction_id
            INNER JOIN inventory.product p ON s.product_id = p.product_id
            WHERE t.timestamp >= @start_date AND t.timestamp < @end_date";

        private const string UpsertTransactionSql = @"
            CALL billing.sp_transaction_upsert(@external_transaction_id, @total_amount, @timestamp, @subtransactions)";

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public TransactionRepository(MandarinDbContext mandarinDbContext, IMapper mapper)
            : base(mandarinDbContext, mapper)
        {
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync()
        {
            return this.GetAll(async db =>
            {
                var transactions = await db.QueryAsync<TransactionRecord>(TransactionRepository.GetAllTransactionsSql);
                var subtransactions = await db.QueryAsync<SubtransactionRecord, ProductRecord, SubtransactionRecord>(
                 TransactionRepository.GetAllSubtransactionsSql,
                 (s, p) => s with { product_id = p.product_id, Product = p },
                 splitOn: "subtransaction_id,product_id");

                return TransactionRepository.Combine(transactions, subtransactions);
            });
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Transaction>> GetAllTransactionsAsync(Interval interval)
        {
            return this.GetAll(async db =>
            {
                var parameters = new { start_date = interval.Start, end_Date = interval.End };
                var transactions = await db.QueryAsync<TransactionRecord>(TransactionRepository.GetAllTransactionsInIntervalSql, parameters);
                var subtransactions = await db.QueryAsync<SubtransactionRecord, ProductRecord, SubtransactionRecord>(
                 TransactionRepository.GetAllSubtransactionsInIntervalSql,
                 (s, p) => s with { product_id = p.product_id, Product = p },
                 parameters,
                 splitOn: "subtransaction_id,product_id");

                return TransactionRepository.Combine(transactions, subtransactions);
            });
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
        protected override async Task<TransactionRecord> UpsertRecordAsync(IDbConnection db, TransactionRecord value)
        {
            db.Open();
            (db as NpgsqlConnection)?.TypeMapper.MapComposite<SubtransactionRecord>("billing.tvp_subtransaction");
            await db.ExecuteAsync(TransactionRepository.UpsertTransactionSql, value);
            return value;
        }

        private static IEnumerable<TransactionRecord> Combine(IEnumerable<TransactionRecord> transactions, IEnumerable<SubtransactionRecord> subtransactions)
        {
            var subtransactionLookup = subtransactions.GroupBy(x => x.transaction_id).ToDictionary(x => x.Key, x => x.AsReadOnlyList());
            return transactions.Select(transaction => subtransactionLookup.TryGetValue(transaction.transaction_id, out var s)
                                           ? transaction with { Subtransactions = s }
                                           : transaction);
        }
    }
}
