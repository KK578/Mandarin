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
        private const string GetCommissionByProductCodeSql = @"
            SELECT *
            FROM inventory.fixed_commission_amount
            WHERE product_code = @product_code
            LIMIT 1";

        private const string GetAllCommissionsSql = @"
            SELECT *
            FROM inventory.fixed_commission_amount
            ORDER BY product_code";

        private const string UpsertCommissionSql = @"
            INSERT INTO inventory.fixed_commission_amount (product_code, amount)
            VALUES (@product_code, @amount)
            ON CONFLICT (product_code) DO
                UPDATE SET amount = @amount";

        private const string DeleteCommissionSql = @"
            DELETE FROM inventory.fixed_commission_amount
            WHERE product_code = @product_code";

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
        public Task<FixedCommissionAmount> GetByProductCodeAsync(string productCode)
        {
            return this.Get(productCode,
                            db =>
                            {
                                var parameters = new { product_code = productCode };
                                return db.QueryFirstOrDefaultAsync<FixedCommissionAmountRecord>(FixedCommissionAmountRepository.GetCommissionByProductCodeSql, parameters);
                            });
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<FixedCommissionAmount>> GetAllAsync()
        {
            return this.GetAll();
        }

        /// <inheritdoc/>
        public Task<FixedCommissionAmount> SaveAsync(FixedCommissionAmount fixedCommissionAmount)
        {
            return this.Upsert(fixedCommissionAmount);
        }

        /// <inheritdoc/>
        public Task DeleteByProductCodeAsync(string productCode)
        {
            var parameters = new { product_code = productCode };
            return this.Delete(FixedCommissionAmountRepository.DeleteCommissionSql, parameters);
        }

        /// <inheritdoc/>
        protected override string ExtractDisplayKey(FixedCommissionAmount value) => value.ProductCode;

        /// <inheritdoc/>
        protected override Task<IEnumerable<FixedCommissionAmountRecord>> GetAllRecords(IDbConnection db)
        {
            return db.QueryAsync<FixedCommissionAmountRecord>(FixedCommissionAmountRepository.GetAllCommissionsSql);
        }

        /// <inheritdoc/>
        protected override async Task<FixedCommissionAmountRecord> UpsertRecordAsync(IDbConnection db, FixedCommissionAmountRecord record)
        {
            await db.ExecuteAsync(FixedCommissionAmountRepository.UpsertCommissionSql, record);
            return record;
        }
    }
}
