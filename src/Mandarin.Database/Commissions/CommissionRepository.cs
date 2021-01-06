using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Commissions;

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
        public async Task<Commission> GetCommissionByStockist(int stockistId)
        {
            using var connection = this.mandarinDbContext.GetConnection();

            const string sql = @"SELECT *
                                 FROM billing.commission c
                                 WHERE stockist_id = @stockist_id
                                 ORDER BY commission_id DESC";
            var commissionRecord = await connection.QueryAsync<CommissionRecord>(sql, new { stockist_id = stockistId });

            var commission = this.mapper.Map<Commission>(commissionRecord.FirstOrDefault());
            return commission;
        }

        /// <inheritdoc/>
        public async Task SaveCommissionAsync(int stockistId, Commission commission)
        {
            var commissionRecord = this.mapper.Map<CommissionRecord>(commission) with { stockist_id = stockistId };

            using var db = this.mandarinDbContext.GetConnection();
            db.Open();
            using var transaction = db.BeginTransaction();

            // TODO: CommissionId should probably be nullable.
            if (commissionRecord.commission_id == 0)
            {
                await CommissionRepository.InsertCommissionAsync(db, commissionRecord);
                transaction.Commit();
            }
            else
            {
                await CommissionRepository.UpdateCommissionAsync(db, commissionRecord);
                transaction.Commit();
            }
        }

        private static async Task InsertCommissionAsync(IDbConnection db, CommissionRecord commissionRecord)
        {
            const string sql = @"INSERT INTO billing.commission (stockist_id, start_date, end_date, rate)
                                 VALUES (@stockist_id, @start_date, @end_date, @rate)";
            await db.ExecuteAsync(sql, commissionRecord);
        }

        private static async Task UpdateCommissionAsync(IDbConnection db, CommissionRecord commissionRecord)
        {
            const string sql = @"UPDATE billing.commission
                                 SET start_date = @start_date,
                                     end_date = @end_date,
                                     rate = @rate
                                 WHERE stockist_id = @stockist_id";
            await db.ExecuteAsync(sql, commissionRecord);
        }
    }
}
