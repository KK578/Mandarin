using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Stockists;

namespace Mandarin.Database.Stockists
{
    /// <inheritdoc />
    internal sealed class StockistRepository : IStockistRepository
    {
        private readonly MandarinDbContext mandarinDbContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public StockistRepository(MandarinDbContext mandarinDbContext, IMapper mapper)
        {
            this.mandarinDbContext = mandarinDbContext;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCode(string stockistCode)
        {
            using var connection = this.mandarinDbContext.GetConnection();

            const string sql = @"SELECT s.*, sd.*
                                 FROM inventory.stockist s
                                 INNER JOIN inventory.stockist_detail sd ON s.stockist_id = sd.stockist_id
                                 WHERE s.stockist_code = @stockist_code
                                 ORDER BY stockist_code
                                 LIMIT 1";
            var stockistRecords = await connection.QueryAsync<StockistRecord, StockistDetailRecord, StockistRecord>(sql,
                (s, sd) => s with { Details = sd },
                new { stockist_code = stockistCode },
                splitOn: "stockist_id,stockist_id");

            var stockist = this.mapper.Map<Stockist>(stockistRecords.FirstOrDefault());
            return stockist;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetAllStockists()
        {
            using var db = this.mandarinDbContext.GetConnection();

            const string sql = @"SELECT s.*, sd.*
                                 FROM inventory.stockist s
                                 INNER JOIN inventory.stockist_detail sd ON s.stockist_id = sd.stockist_id
                                 ORDER BY stockist_code";
            var stockistRecords = await db.QueryAsync<StockistRecord, StockistDetailRecord, StockistRecord>(sql,
                (s, sd) => s with { Details = sd },
                splitOn: "stockist_id,stockist_id");

            var stockists = this.mapper.Map<List<Stockist>>(stockistRecords).AsReadOnly();
            return stockists;
        }

        /// <inheritdoc/>
        public async Task<int> SaveStockistAsync(Stockist stockist)
        {
            var stockistRecord = this.mapper.Map<StockistRecord>(stockist);

            using var db = this.mandarinDbContext.GetConnection();
            db.Open();
            using var transaction = db.BeginTransaction();

            // TODO: StockistId should probably be nullable.
            if (stockist.StockistId == 0)
            {
                var stockistId = await StockistRepository.InsertStockistAsync(db, stockistRecord);
                transaction.Commit();
                return stockistId;
            }
            else
            {
                await StockistRepository.UpdateStockistAsync(db, stockistRecord);
                transaction.Commit();
                return stockist.StockistId;
            }
        }

        private static async Task<int> InsertStockistAsync(IDbConnection db, StockistRecord stockistRecord)
        {
            var sql = @"INSERT INTO inventory.stockist (stockist_code, stockist_status, first_name, last_name)
                        VALUES (@stockist_code, @stockist_Status, @first_name, @last_name)
                        RETURNING stockist_id";
            var stockistId = await db.ExecuteScalarAsync<int>(sql, stockistRecord);

            sql = @"INSERT INTO inventory.stockist_detail (stockist_id, twitter_handle, instagram_handle, facebook_handle, website_url, image_url, tumblr_handle, email_address, description, full_display_name, short_display_name, thumbnail_image_url)
                    VALUES (@stockist_id, @twitter_handle, @instagram_handle, @facebook_handle, @website_url, @image_url, @tumblr_handle, @email_address, @description, @full_display_name, @short_display_name, @thumbnail_image_url)";
            await db.ExecuteAsync(sql, stockistRecord.Details with { stockist_id = stockistId });

            return stockistId;
        }

        private static async Task UpdateStockistAsync(IDbConnection db, StockistRecord stockistRecord)
        {
            var sql = @"UPDATE inventory.stockist
                        SET stockist_code = @stockist_code,
                            stockist_status = @stockist_status,
                            first_name = @first_name,
                            last_name = @last_name
                        WHERE stockist_id = @stockist_id";
            await db.ExecuteAsync(sql, stockistRecord);

            sql = @"UPDATE inventory.stockist_detail
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
            await db.ExecuteAsync(sql, stockistRecord.Details);
        }
    }
}
