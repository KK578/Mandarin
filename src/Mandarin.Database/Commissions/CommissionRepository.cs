using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Commissions;
using Mandarin.Database.Common;
using Microsoft.Extensions.Logging;

namespace Mandarin.Database.Commissions
{
    /// <inheritdoc cref="Mandarin.Commissions.ICommissionRepository" />
    internal sealed class CommissionRepository : DatabaseRepositoryBase<Commission, CommissionRecord>, ICommissionRepository
    {
        private const string GetCommissionByStockistIdSql = @"
            SELECT * 
            FROM billing.commission c 
            WHERE stockist_id = @stockist_id
            ORDER BY commission_id DESC";

        private const string InsertCommissionSql = @"
            INSERT INTO billing.commission (stockist_id, start_date, end_date, rate)
            VALUES (@stockist_id, @start_date, @end_date, @rate)
            RETURNING commission_id";

        private const string UpdateCommissionSql = @"
            UPDATE billing.commission
            SET start_date = @start_date,
                end_date = @end_date,
                rate = @rate
            WHERE stockist_id = @stockist_id";

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public CommissionRepository(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<CommissionRepository> logger)
            : base(mandarinDbContext, mapper, logger)
        {
        }

        /// <inheritdoc/>
        public Task<Commission> GetCommissionByStockist(int stockistId)
        {
            return this.Get(stockistId,
                       db =>
                       {
                           var parameters = new { stockist_id = stockistId, };
                           return db.QueryFirstAsync<CommissionRecord>(CommissionRepository.GetCommissionByStockistIdSql, parameters);
                       });
        }

        /// <inheritdoc/>
        public Task<Commission> SaveCommissionAsync(int stockistId, Commission commission)
        {
            commission.StockistId = stockistId;
            return this.Upsert(commission);
        }

        /// <inheritdoc/>
        protected override string ExtractDisplayKey(Commission value) => value.CommissionId.ToString();

        /// <inheritdoc/>
        protected override Task<IEnumerable<CommissionRecord>> GetAllRecords(IDbConnection db)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override async Task<CommissionRecord> UpsertRecordAsync(IDbConnection db, CommissionRecord value)
        {
            if (value.commission_id == 0)
            {
                var commissionId = await db.ExecuteScalarAsync<int>(CommissionRepository.InsertCommissionSql, value);
                return value with { commission_id = commissionId };
            }

            await db.ExecuteAsync(CommissionRepository.UpdateCommissionSql, value);
            return value;
        }
    }
}
