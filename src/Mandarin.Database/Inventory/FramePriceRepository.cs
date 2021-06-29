using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Database.Common;
using Mandarin.Inventory;
using Microsoft.Extensions.Logging;

namespace Mandarin.Database.Inventory
{
    /// <inheritdoc cref="IFramePriceRepository" />
    internal sealed class FramePriceRepository : DatabaseRepositoryBase<FramePrice, FramePriceRecord>, IFramePriceRepository
    {
        private const string GetFramePriceByProductCodeSql = @"
            SELECT *
            FROM inventory.frame_price
            WHERE product_code = @product_code
            LIMIT 1";

        private const string GetAllFramePricesSql = @"
            SELECT *
            FROM inventory.frame_price
            ORDER BY product_code";

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
        public Task<FramePrice> GetByProductCodeAsync(string productCode)
        {
            return this.Get(productCode,
                            db =>
                            {
                                var parameters = new { product_code = productCode };
                                return db.QueryFirstOrDefaultAsync<FramePriceRecord>(FramePriceRepository.GetFramePriceByProductCodeSql, parameters);
                            });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<FramePrice>> GetAllAsync()
        {
            return this.GetAll();
        }

        /// <inheritdoc/>
        public Task<FramePrice> SaveAsync(FramePrice framePrice)
        {
            return this.Upsert(framePrice);
        }

        /// <inheritdoc/>
        public Task DeleteByProductCodeAsync(string productCode)
        {
            var parameters = new { product_code = productCode };
            return this.Delete(FramePriceRepository.DeleteFramePriceSql, parameters);
        }

        /// <inheritdoc/>
        protected override string ExtractDisplayKey(FramePrice value) => value.ProductCode;

        /// <inheritdoc/>
        protected override Task<IEnumerable<FramePriceRecord>> GetAllRecords(IDbConnection db)
        {
            return db.QueryAsync<FramePriceRecord>(FramePriceRepository.GetAllFramePricesSql);
        }

        /// <inheritdoc/>
        protected override async Task<FramePriceRecord> UpsertRecordAsync(IDbConnection db, FramePriceRecord record)
        {
            var p = new DynamicParameters();
            p.Add("product_code", record.product_code, DbType.String);
            p.Add("amount", record.amount, DbType.VarNumeric);
            p.Add("created_at", record.created_at, DbType.DateTime);

            await db.ExecuteAsync("sp_frame_price_upsert", p, commandType: CommandType.StoredProcedure);
            return record;
        }
    }
}
