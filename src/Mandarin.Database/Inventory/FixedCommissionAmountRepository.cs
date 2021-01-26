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
    /// <inheritdoc cref="IFixedCommissionAmountRepository" />
    internal sealed class FixedCommissionAmountRepository : DatabaseRepositoryBase<FixedCommissionAmount, FixedCommissionAmountRecord>,
                                                            IFixedCommissionAmountRepository
    {
        private const string GetByProductCodeSql = @"
            SELECT *
            FROM inventory.fixed_commission_amount
            WHERE product_code = @product_code
            LIMIT 1";

        private const string UpsertSql = @"
            INSERT INTO inventory.fixed_commission_amount (product_code, amount)
            VALUES (@product_code, @amount)
            ON CONFLICT (product_code) DO
                UPDATE SET amount = @amount";

        private const string GetAllSql = @"
            SELECT *
            FROM inventory.fixed_commission_amount
            ORDER BY product_code";

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionAmountRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public FixedCommissionAmountRepository(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<FixedCommissionAmountRepository> logger)
            : base(mandarinDbContext, mapper, logger)
        {
        }

        /// <inheritdoc/>
        public Task<FixedCommissionAmount> GetFixedCommissionAmountByProductCode(string productCode)
        {
            return this.Get(productCode,
                            db =>
                            {
                                var parameters = new { product_code = productCode };
                                return db.QueryFirstAsync<FixedCommissionAmountRecord>(FixedCommissionAmountRepository.GetByProductCodeSql, parameters);
                            });
        }

        /// <inheritdoc/>
        public Task<FixedCommissionAmount> SaveFixedCommissionAmountAsync(FixedCommissionAmount fixedCommissionAmount)
        {
            return this.UpsertAsync(fixedCommissionAmount);
        }

        /// <inheritdoc/>
        protected override string ExtractDisplayKey(FixedCommissionAmount value) => value.ProductCode;

        /// <inheritdoc/>
        protected override Task<IEnumerable<FixedCommissionAmountRecord>> GetAllRecords(IDbConnection db)
        {
            return db.QueryAsync<FixedCommissionAmountRecord>(FixedCommissionAmountRepository.GetAllSql);
        }

        /// <inheritdoc/>
        protected override async Task<FixedCommissionAmountRecord> UpsertRecordAsync(IDbConnection db, FixedCommissionAmountRecord record)
        {
            await db.ExecuteAsync(FixedCommissionAmountRepository.UpsertSql, record);
            return record;
        }
    }
}
