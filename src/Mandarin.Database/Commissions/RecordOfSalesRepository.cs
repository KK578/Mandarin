﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Bashi.Core.Extensions;
using Dapper;
using Mandarin.Commissions;
using Mandarin.Database.Common;
using Mandarin.Stockists;
using NodaTime;

namespace Mandarin.Database.Commissions
{
    /// <inheritdoc cref="IRecordOfSalesRepository" />
    internal sealed class RecordOfSalesRepository : DatabaseRepositoryBase<Sale, SaleRecord>, IRecordOfSalesRepository
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

        private readonly IStockistRepository stockistRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordOfSalesRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="stockistRepository">The application repository for interacting with stockists.</param>
        public RecordOfSalesRepository(MandarinDbContext mandarinDbContext, IMapper mapper, IStockistRepository stockistRepository)
            : base(mandarinDbContext, mapper)
        {
            this.stockistRepository = stockistRepository;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesAsync(DateInterval interval)
        {
            var allSales = await this.GetAll(db =>
            {
                var parameters = new { start_date = interval.Start, end_date = interval.End };
                return db.QueryAsync<SaleRecord>(RecordOfSalesRepository.GetSalesSql, parameters);
            });
            var salesLookup = allSales.GroupBy(x => x.StockistId).ToDictionary(x => x.Key, x => x.ToList());

            var stockists = await this.stockistRepository.GetAllActiveStockistsAsync();
            var recordOfSales = stockists.Select(s => ToRecordOfSales(s, salesLookup.GetValueOrDefault(s.StockistId, new List<Sale>()))).ToList();
            return recordOfSales.AsReadOnly();

            RecordOfSales ToRecordOfSales(Stockist stockist, List<Sale> stockistSales)
            {
                var rate = decimal.Divide(stockist.Commission.Rate, 100);
                var subtotal = stockistSales.Sum(x => x.Subtotal);
                var commission = stockistSales.Sum(x => x.Commission);

                return new RecordOfSales
                {
                    StockistCode = stockist.StockistCode.Value,
                    FirstName = stockist.Details.FirstName,
                    Name = stockist.Details.DisplayName,
                    EmailAddress = stockist.Details.EmailAddress,
                    CustomMessage = string.Empty,
                    StartDate = interval.Start,
                    EndDate = interval.End,
                    Rate = rate,
                    Sales = stockistSales.AsReadOnlyList(),
                    Subtotal = subtotal,
                    CommissionTotal = commission,
                    Total = subtotal + commission,
                };
            }
        }

        /// <inheritdoc />
        protected override string ExtractDisplayKey(Sale value) => value.ToString();

        /// <inheritdoc />
        protected override Task<SaleRecord> UpsertRecordAsync(IDbConnection db, SaleRecord value)
        {
            throw new System.NotImplementedException();
        }
    }
}
