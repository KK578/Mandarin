using System;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Commissions;
using Microsoft.Extensions.Logging;

namespace Mandarin.Database.Commissions
{
    /// <inheritdoc />
    internal sealed class CommissionRepository : ICommissionRepository
    {
        private const string GetCommissionByStockistSql = @"
            SELECT * 
            FROM billing.commission c 
            WHERE stockist_id = @stockist_id
            ORDER BY commission_id DESC";

        private const string InsertCommissionSql = @"
            INSERT INTO billing.commission (stockist_id, start_date, end_date, rate)
            VALUES (@stockist_id, @start_date, @end_date, @rate)";

        private const string UpdateCommissionSql = @"
            UPDATE billing.commission
            SET start_date = @start_date,
                end_date = @end_date,
                rate = @rate
            WHERE stockist_id = @stockist_id";

        private readonly MandarinDbContext mandarinDbContext;
        private readonly IMapper mapper;
        private readonly ILogger<CommissionRepository> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public CommissionRepository(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<CommissionRepository> logger)
        {
            this.mandarinDbContext = mandarinDbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Commission> GetCommissionByStockist(int stockistId)
        {
            var parameters = new
            {
                stockist_id = stockistId,
            };

            this.logger.LogDebug("Fetching Commission entry for StockistId={StockistId}", stockistId);
            try
            {
                using var connection = this.mandarinDbContext.GetConnection();
                var commissionRecord = await connection.QueryFirstAsync<CommissionRecord>(CommissionRepository.GetCommissionByStockistSql, parameters);
                var commission = this.mapper.Map<Commission>(commissionRecord);
                this.logger.LogInformation("Fetched Commission entry for StockistId={stockistId}: (@{Commission})", stockistId, commission);
                return commission;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to fetch Commission entry for StockistId={StockistId}", stockistId);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task SaveCommissionAsync(int stockistId, Commission commission)
        {
            this.logger.LogDebug("Saving Commission entry for StockistId={StockistId}: {@Commission}", stockistId, commission);

            try
            {
                var commissionRecord = this.mapper.Map<CommissionRecord>(commission) with { stockist_id = stockistId };

                using var db = this.mandarinDbContext.GetConnection();
                db.Open();
                using var transaction = db.BeginTransaction();

                // TODO: CommissionId should probably be nullable.
                if (commissionRecord.commission_id == 0)
                {
                    this.logger.LogDebug("Inserting as new Commission entry for StockistId={StockistId}", stockistId);
                    await CommissionRepository.InsertCommissionAsync(db, commissionRecord);
                    transaction.Commit();
                }
                else
                {
                    this.logger.LogDebug("Updating existing Commission entry for StockistId={StockistId}.", stockistId);
                    await CommissionRepository.UpdateCommissionAsync(db, commissionRecord);
                    transaction.Commit();
                }

                this.logger.LogInformation("Successfully saved Commission entry for StockistId={StockistId}.", stockistId);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to save Commission entry for StockistId={StockistId}.", stockistId);
                throw;
            }
        }

        private static async Task InsertCommissionAsync(IDbConnection db, CommissionRecord commissionRecord)
        {
            await db.ExecuteAsync(CommissionRepository.InsertCommissionSql, commissionRecord);
        }

        private static async Task UpdateCommissionAsync(IDbConnection db, CommissionRecord commissionRecord)
        {
            await db.ExecuteAsync(CommissionRepository.UpdateCommissionSql, commissionRecord);
        }
    }
}
