using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Stockists;
using Microsoft.Extensions.Logging;

namespace Mandarin.Database.Stockists
{
    /// <inheritdoc />
    internal sealed class StockistRepository : IStockistRepository
    {
        private const string GetStockistByCodeSql = @"
            SELECT s.*, sd.*
            FROM inventory.stockist s
                INNER JOIN inventory.stockist_detail sd ON s.stockist_id = sd.stockist_id
            WHERE s.stockist_code = @stockist_code
            ORDER BY stockist_code
            LIMIT 1";

        private const string GetAllStockistsSql = @"
            SELECT s.*, sd.*
            FROM inventory.stockist s
                INNER JOIN inventory.stockist_detail sd ON s.stockist_id = sd.stockist_id
            ORDER BY stockist_code";

        private const string InsertStockistSql = @"
            INSERT INTO inventory.stockist (stockist_code, stockist_status, first_name, last_name)
            VALUES (@stockist_code, @stockist_Status, @first_name, @last_name)
            RETURNING stockist_id";

        private const string InsertStockistDetailSql = @"
            INSERT INTO inventory.stockist_detail (stockist_id, twitter_handle, instagram_handle, facebook_handle, website_url, image_url, tumblr_handle, email_address, description, full_display_name, short_display_name, thumbnail_image_url)
            VALUES (@stockist_id, @twitter_handle, @instagram_handle, @facebook_handle, @website_url, @image_url, @tumblr_handle, @email_address, @description, @full_display_name, @short_display_name, @thumbnail_image_url)";

        private const string UpdateStockistSql = @"
            UPDATE inventory.stockist
            SET stockist_code = @stockist_code,
                stockist_status = @stockist_status,
                first_name = @first_name,
                last_name = @last_name
            WHERE stockist_id = @stockist_id";

        private const string UpdateStockistDetailSql = @"
            UPDATE inventory.stockist_detail
            SET stockist_id = @stockist_id,
                twitter_handle = @twitter_handle,
                instagram_handle = @instagram_handle,
                facebook_handle = @facebook_handle,
                website_url = @website_url,
                image_url = @image_url,
                tumblr_handle = @tumblr_handle,
                email_address = @email_address,
                description = @description,
                full_display_name = @full_display_name,
                short_display_name = @short_display_name,
                thumbnail_image_url = @thumbnail_image_url
            WHERE stockist_id = @stockist_id";

        private readonly MandarinDbContext mandarinDbContext;
        private readonly IMapper mapper;
        private readonly ILogger<StockistRepository> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public StockistRepository(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<StockistRepository> logger)
        {
            this.mandarinDbContext = mandarinDbContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCode(string stockistCode)
        {
            this.logger.LogDebug("Fetching Stockist entry for StockistCode={StockistCode}.", stockistCode);

            try
            {
                using var connection = this.mandarinDbContext.GetConnection();

                var stockistRecords = await connection.QueryAsync<StockistRecord, StockistDetailRecord, StockistRecord>(
                 StockistRepository.GetStockistByCodeSql,
                 (s, sd) => s with { Details = sd },
                 new { stockist_code = stockistCode },
                 splitOn: "stockist_id,stockist_id");

                var stockist = this.mapper.Map<Stockist>(stockistRecords.SingleOrDefault());
                this.logger.LogInformation("Successfully fetched Stockist entry for StockistCode={StockistCode}: {@Stockist}",
                                           stockistCode,
                                           stockist);
                return stockist;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to fetch Stockist entry for StockistCode={StockistCode}.", stockistCode);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetAllStockists()
        {
            this.logger.LogDebug("Fetching all Stockist entries.");
            try
            {
                using var db = this.mandarinDbContext.GetConnection();

                var stockistRecords = await db.QueryAsync<StockistRecord, StockistDetailRecord, StockistRecord>(
                 StockistRepository.GetAllStockistsSql,
                 (s, sd) => s with { Details = sd },
                 splitOn: "stockist_id,stockist_id");

                var stockists = this.mapper.Map<List<Stockist>>(stockistRecords).AsReadOnly();
                this.logger.LogInformation("Successfully fetched {Count} Stockist entries.", stockists.Count);
                return stockists;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to fetch all Stockist entries.");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> SaveStockistAsync(Stockist stockist)
        {
            this.logger.LogDebug("Saving Stockist entry for StockistCode={StockistCode}: @{Stockist}", stockist.StockistCode, stockist);
            try
            {
                var stockistRecord = this.mapper.Map<StockistRecord>(stockist);

                using var db = this.mandarinDbContext.GetConnection();
                db.Open();
                using var transaction = db.BeginTransaction();

                // TODO: StockistId should probably be nullable.
                if (stockist.StockistId == 0)
                {
                    this.logger.LogDebug("Inserting as new Stockist entry for StockistCode={StockistCode}.", stockist.StockistCode);
                    var stockistId = await StockistRepository.InsertStockistAsync(db, stockistRecord);
                    transaction.Commit();
                    return stockistId;
                }
                else
                {
                    this.logger.LogDebug("Updating existing Stockist entry for StockistCode={StockistCode}.", stockist.StockistCode);
                    await StockistRepository.UpdateStockistAsync(db, stockistRecord);
                    transaction.Commit();
                    return stockist.StockistId;
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to save Stockist for StockistCode={StockistCode}.", stockist.StockistCode);
                throw;
            }
        }

        private static async Task<int> InsertStockistAsync(IDbConnection db, StockistRecord stockistRecord)
        {
            var stockistId = await db.ExecuteScalarAsync<int>(StockistRepository.InsertStockistSql, stockistRecord);
            await db.ExecuteAsync(StockistRepository.InsertStockistDetailSql, stockistRecord.Details with { stockist_id = stockistId });
            return stockistId;
        }

        private static async Task UpdateStockistAsync(IDbConnection db, StockistRecord stockistRecord)
        {
            await db.ExecuteAsync(StockistRepository.UpdateStockistSql, stockistRecord);
            await db.ExecuteAsync(StockistRepository.UpdateStockistDetailSql, stockistRecord.Details);
        }
    }
}
