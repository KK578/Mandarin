using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Database.Common;
using Mandarin.Transactions.External;
using NodaTime;

namespace Mandarin.Database.Transactions.External
{
    /// <inheritdoc cref="IExternalTransactionRepository" />
    internal sealed class ExternalTransactionRepository : DatabaseRepositoryBase<ExternalTransaction, ExternalTransactionRecord>, IExternalTransactionRepository
    {
        private const string GetExternalTransactionDataSql = @"
            SELECT *
            FROM billing.external_transaction
            WHERE external_transaction_id = @external_transaction_id";

        private const string GetExternalTransactionDataByDateSql = @"
            SELECT *
            FROM billing.external_transaction
            WHERE external_transaction_id = @external_transaction_id
              AND updated_at = @updated_at";

        private const string UpsertExternalTransactionDataSql = @"
            INSERT INTO billing.external_transaction (external_transaction_id, created_at, updated_at, raw_data)
            VALUES (@external_transaction_id, @created_at, @updated_at, CAST(@raw_data as JSON))";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalTransactionRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public ExternalTransactionRepository(MandarinDbContext mandarinDbContext, IMapper mapper)
            : base(mandarinDbContext, mapper)
        {
        }

        /// <inheritdoc />
        public Task<ExternalTransaction> GetExternalTransactionAsync(ExternalTransactionId externalTransactionId)
        {
            return this.Get(externalTransactionId,
                            db =>
                            {
                                var parameters = new { external_transaction_id = externalTransactionId.Value };
                                return db.QueryFirstOrDefaultAsync<ExternalTransactionRecord>(ExternalTransactionRepository.GetExternalTransactionDataSql, parameters);
                            });
        }

        /// <inheritdoc />
        public Task<ExternalTransaction> GetExternalTransactionAsync(ExternalTransactionId externalTransactionId, Instant updatedAt)
        {
            return this.Get(externalTransactionId,
                            db =>
                            {
                                var parameters = new { external_transaction_id = externalTransactionId.Value, updated_at = updatedAt };
                                return db.QueryFirstOrDefaultAsync<ExternalTransactionRecord>(ExternalTransactionRepository.GetExternalTransactionDataByDateSql, parameters);
                            });
        }

        /// <inheritdoc />
        public Task SaveExternalTransactionAsync(ExternalTransaction externalTransaction) => this.Upsert(externalTransaction);

        /// <inheritdoc />
        protected override string ExtractDisplayKey(ExternalTransaction value) => value.ExternalTransactionId.Value;

        /// <inheritdoc />
        protected override async Task<ExternalTransactionRecord> UpsertRecordAsync(IDbConnection db, ExternalTransactionRecord value)
        {
            await db.ExecuteAsync(ExternalTransactionRepository.UpsertExternalTransactionDataSql, value);
            return value;
        }
    }
}
