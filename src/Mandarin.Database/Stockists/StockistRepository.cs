﻿using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Database.Commissions;
using Mandarin.Database.Common;
using Mandarin.Stockists;

namespace Mandarin.Database.Stockists
{
    /// <inheritdoc cref="Mandarin.Stockists.IStockistRepository" />
    internal sealed class StockistRepository : DatabaseRepositoryBase<Stockist, StockistRecord>,
                                               IStockistRepository
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

        private const string GetAllActiveStockistsSql = @"
            SELECT s.*, sd.*, c.*
            FROM inventory.stockist s
                INNER JOIN inventory.stockist_detail sd ON s.stockist_id = sd.stockist_id
                INNER JOIN (
                    SELECT DISTINCT ON (stockist_id) *
                    FROM billing.commission
                    ORDER BY stockist_id, inserted_at DESC) c ON s.stockist_id = c.stockist_id
            WHERE s.stockist_status IN ('Active', 'ActiveHidden')
            ORDER BY stockist_code";

        private const string InsertStockistSql = @"
            INSERT INTO inventory.stockist (stockist_code, stockist_status)
            VALUES (@stockist_code, @stockist_Status)
            RETURNING stockist_id";

        private const string InsertStockistDetailSql = @"
            INSERT INTO inventory.stockist_detail (stockist_id, first_name, last_name, display_name, twitter_handle, instagram_handle, facebook_handle, website_url, tumblr_handle, email_address)
            VALUES (@stockist_id, @first_name, @last_name, @display_name, @twitter_handle, @instagram_handle, @facebook_handle, @website_url, @tumblr_handle, @email_address)";

        private const string UpdateStockistSql = @"
            UPDATE inventory.stockist
            SET stockist_code = @stockist_code,
                stockist_status = @stockist_status
            WHERE stockist_id = @stockist_id";

        private const string UpdateStockistDetailSql = @"
            UPDATE inventory.stockist_detail
            SET stockist_id = @stockist_id,
                first_name = @first_name,
                last_name = @last_name,
                display_name = @display_name,
                twitter_handle = @twitter_handle,
                instagram_handle = @instagram_handle,
                facebook_handle = @facebook_handle,
                website_url = @website_url,
                tumblr_handle = @tumblr_handle,
                email_address = @email_address
            WHERE stockist_id = @stockist_id";

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public StockistRepository(MandarinDbContext mandarinDbContext, IMapper mapper)
            : base(mandarinDbContext, mapper)
        {
        }

        /// <inheritdoc/>
        public Task<Stockist> GetStockistAsync(StockistCode stockistCode)
        {
            return this.Get(stockistCode,
                            async db =>
                            {
                                var records = await db.QueryAsync<StockistRecord, StockistDetailRecord, StockistRecord>(
                                 StockistRepository.GetStockistByCodeSql,
                                 (s, sd) => s with { Details = sd },
                                 new { stockist_code = stockistCode.Value },
                                 splitOn: "stockist_id,stockist_id");

                                return records.First();
                            });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<Stockist>> GetAllStockistsAsync()
        {
            return this.GetAll(db => db.QueryAsync<StockistRecord, StockistDetailRecord, StockistRecord>(StockistRepository.GetAllStockistsSql,
                                   (s, sd) => s with { Details = sd },
                                   splitOn: "stockist_id,stockist_id"));
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Stockist>> GetAllActiveStockistsAsync()
        {
            return this.GetAll(db => db.QueryAsync<StockistRecord, StockistDetailRecord, CommissionRecord, StockistRecord>(StockistRepository.GetAllActiveStockistsSql,
                                   (s, sd, c) => s with { Details = sd, Commission = c },
                                   splitOn: "stockist_id,stockist_id,commission_id"));
        }

        /// <inheritdoc/>
        public Task<Stockist> SaveStockistAsync(Stockist stockist)
        {
            return this.Upsert(stockist);
        }

        /// <inheritdoc/>
        protected override string ExtractDisplayKey(Stockist value) => value.StockistCode.Value;

        /// <inheritdoc/>
        protected override async Task<StockistRecord> UpsertRecordAsync(IDbConnection db, StockistRecord value)
        {
            db.Open();
            using var transaction = db.BeginTransaction();

            if (value.stockist_id == 0)
            {
                this.Log.Debug("Inserting as new Stockist entry for StockistCode={StockistCode}.", value.stockist_code);
                var stockistId = await db.ExecuteScalarAsync<int>(StockistRepository.InsertStockistSql, value);
                value = value with
                {
                    stockist_id = stockistId,
                    Details = value.Details with { stockist_id = stockistId },
                };
                await db.ExecuteAsync(StockistRepository.InsertStockistDetailSql, value.Details);
                transaction.Commit();
                return value with { stockist_id = stockistId };
            }

            this.Log.Debug("Updating existing Stockist entry for StockistCode={StockistCode}.", value.stockist_code);
            await db.ExecuteAsync(StockistRepository.UpdateStockistSql, value);
            await db.ExecuteAsync(StockistRepository.UpdateStockistDetailSql, value.Details);
            transaction.Commit();
            return value;
        }
    }
}
