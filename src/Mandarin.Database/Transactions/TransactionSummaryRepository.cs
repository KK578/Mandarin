using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Database.Common;
using Mandarin.Transactions;
using NodaTime;

namespace Mandarin.Database.Transactions
{
    /// <inheritdoc cref="Mandarin.Transactions.ITransactionSummaryRepository" />
    internal sealed class TransactionSummaryRepository : DatabaseRepositoryBase<TransactionSummary, TransactionSummaryRecord>, ITransactionSummaryRepository
    {
        private const string GetTransactionSummariesSql = @"
            SELECT date::DATE, SUM(revenue) as revenue, SUM(refund) as refund
            FROM (SELECT to_char(t.timestamp, 'YYYY-MM-DD')                           AS date,
                         CASE WHEN quantity > 0 THEN quantity * unit_price ELSE 0 END AS revenue,
                         CASE WHEN quantity < 0 THEN quantity * unit_price ELSE 0 END AS refund
                  FROM billing.subtransaction
                  INNER JOIN billing.transaction t on subtransaction.transaction_id = t.transaction_id
                  WHERE t.timestamp >= @start_date AND t.timestamp < @end_date
                  ) as TRANSACTION_SUMMARY
            GROUP BY Date
            ORDER BY Date;";

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionSummaryRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public TransactionSummaryRepository(MandarinDbContext mandarinDbContext, IMapper mapper)
            : base(mandarinDbContext, mapper)
        {
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TransactionSummary>> GetTransactionSummariesAsync(DateInterval interval)
        {
            return this.GetAll(async db =>
            {
                var parameters = new { start_date = interval.Start, end_Date = interval.End };
                var transactionSummaries = await db.QueryAsync<TransactionSummaryRecord>(TransactionSummaryRepository.GetTransactionSummariesSql, parameters);

                return transactionSummaries;
            });
        }

        /// <inheritdoc/>
        protected override string ExtractDisplayKey(TransactionSummary value)
        {
            return value.Date.ToString();
        }

        /// <inheritdoc/>
        protected override Task<TransactionSummaryRecord> UpsertRecordAsync(IDbConnection db, TransactionSummaryRecord value)
        {
            throw new System.NotImplementedException();
        }
    }
}
