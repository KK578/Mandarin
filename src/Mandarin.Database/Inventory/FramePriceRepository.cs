using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Database.Common;
using Mandarin.Inventory;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Mandarin.Database.Inventory
{
    /// <inheritdoc cref="IFramePriceRepository" />
    internal sealed class FramePriceRepository : DatabaseRepositoryBase<FramePrice, FramePriceRecord>, IFramePriceRepository
    {
        private const string GetFramePriceByProductCodeSql = @"
            SELECT *
            FROM inventory.frame_price
            WHERE product_code = @product_code
              AND created_at <= @created_at
              AND (active_until IS NULL OR active_until > @created_at)
            LIMIT 1";

        private const string GetAllFramePricesSql = @"
            SELECT *
            FROM inventory.frame_price
            WHERE active_until IS NULL
            ORDER BY product_code";

        private const string UpsertFramePriceSql = @"
            CALL inventory.sp_frame_price_upsert(@product_code, @amount, @created_at)";

        private const string DeleteFramePriceSql = @"
            DELETE FROM inventory.frame_price
            WHERE product_code = @product_code";

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePriceRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public FramePriceRepository(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<FramePriceRepository> logger)
            : base(mandarinDbContext, mapper, logger)
        {
        }

        /// <inheritdoc/>
        public Task<FramePrice> GetByProductCodeAsync(ProductCode productCode, Instant activeSince)
        {
            return this.Get(productCode,
                            db =>
                            {
                                var parameters = new { product_code = productCode.Value, created_at = activeSince };
                                return db.QueryFirstOrDefaultAsync<FramePriceRecord>(FramePriceRepository.GetFramePriceByProductCodeSql, parameters);
                            });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<FramePrice>> GetAllAsync()
        {
            return this.GetAll(db => db.QueryAsync<FramePriceRecord>(FramePriceRepository.GetAllFramePricesSql));
        }

        /// <inheritdoc/>
        public Task<FramePrice> SaveAsync(FramePrice framePrice)
        {
            return this.Upsert(framePrice);
        }

        /// <inheritdoc/>
        public Task DeleteByProductCodeAsync(ProductCode productCode)
        {
            var parameters = new { product_code = productCode.Value };
            return this.Delete(FramePriceRepository.DeleteFramePriceSql, parameters);
        }

        /// <inheritdoc/>
        protected override string ExtractDisplayKey(FramePrice value) => value.ProductCode.Value;

        /// <inheritdoc/>
        protected override async Task<FramePriceRecord> UpsertRecordAsync(IDbConnection db, FramePriceRecord record)
        {
            await db.ExecuteAsync(FramePriceRepository.UpsertFramePriceSql, record);
            return record;
        }
    }
}
