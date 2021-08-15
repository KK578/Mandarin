using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Commissions;
using Mandarin.Database.Common;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Mandarin.Database.Commissions
{
    /// <inheritdoc cref="IRecordOfSalesRepository" />
    internal sealed class RecordOfSalesRepository : DatabaseRepositoryBase<RecordOfSales, RecordOfSalesRecord>, IRecordOfSalesRepository
    {
        private const string GetSalesSql = @"
            SELECT s.stockist_id,
                   p.product_code,
                   p.product_name,
                   sum.quantity,
                   sum.unit_price,
                   sum.subtotal,
                   -(sum.subtotal * sum.rate) AS commission,
                   sum.subtotal - (sum.subtotal * sum.rate) AS total
            FROM (SELECT s.product_id,
                         SUM(s.quantity * s.unit_price) AS subtotal,
                         SUM(s.quantity) AS quantity,
                         s.unit_price,
                         (AVG(s.commission_rate) / 100)::NUMERIC(6, 2) as rate
                  FROM billing.subtransaction s
                           INNER JOIN billing.transaction t ON s.transaction_id = t.transaction_id
                  WHERE t.timestamp >= @start_date AND t.timestamp < @end_date
                  GROUP BY s.product_id, s.unit_price) sum
                     INNER JOIN inventory.product p ON p.product_id = sum.product_id
                     INNER JOIN inventory.stockist s ON s.stockist_id = p.stockist_id
            ORDER BY p.product_code";

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordOfSalesRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public RecordOfSalesRepository(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<RecordOfSalesRepository> logger)
            : base(mandarinDbContext, mapper, logger)
        {
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesAsync(Interval interval)
        {
            return this.GetAll(async db =>
            {
                var parameters = new { start_date = interval.Start, end_date = interval.End };
                var sales = await db.QueryAsync<SaleRecord>(RecordOfSalesRepository.GetSalesSql, parameters);

                // TODO: Map the sales back to stockists.
                return new List<RecordOfSalesRecord>();
            });
        }

        /// <inheritdoc />
        protected override string ExtractDisplayKey(RecordOfSales value) => value.ToString();

        /// <inheritdoc />
        protected override Task<RecordOfSalesRecord> UpsertRecordAsync(IDbConnection db, RecordOfSalesRecord value)
        {
            throw new System.NotImplementedException();
        }
    }
}
