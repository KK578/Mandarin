using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Models.Commissions;

namespace Mandarin.Database.Commissions
{
    /// <inheritdoc />
    internal sealed class CommissionRepository : ICommissionRepository
    {
        private readonly MandarinDbContext mandarinDbContext;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public CommissionRepository(MandarinDbContext mandarinDbContext, IMapper mapper)
        {
            this.mandarinDbContext = mandarinDbContext;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroups()
        {
            using var connection = this.mandarinDbContext.GetConnection();

            const string sql = "SELECT * FROM billing.commission_rate_group ORDER BY rate";
            var groupRecords = await connection.QueryAsync<CommissionRateGroupRecord>(sql);

            var groups = this.mapper.Map<List<CommissionRateGroup>>(groupRecords).AsReadOnly();
            return groups;
        }

        /// <inheritdoc/>
        public async Task<Commission> GetCommissionByStockist(int stockistId)
        {
            using var connection = this.mandarinDbContext.GetConnection();

            const string sql = @"SELECT c.*, crg.*
                                 FROM billing.commission c
                                 INNER JOIN billing.commission_rate_group crg on crg.group_id = c.rate_group
                                 WHERE stockist_id = @StockistId
                                 ORDER BY commission_id DESC";
            var commissionRecord = await connection.QueryAsync<CommissionRecord, CommissionRateGroupRecord, CommissionRecord>(sql,
                (c, crg) => c with { CommissionRateGroup = crg },
                new { StockistId = stockistId },
                splitOn: "stockist_id,group_id");

            var commission = this.mapper.Map<Commission>(commissionRecord.FirstOrDefault());
            return commission;
        }
    }
}
