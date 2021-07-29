using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Mandarin.Database.Common;
using Mandarin.Transactions;
using Microsoft.Extensions.Logging;

namespace Mandarin.Database.Transactions
{
    /// <inheritdoc cref="Mandarin.Transactions.ITransactionAuditRepository" />
    internal sealed class TransactionAuditRepository : DatabaseRepositoryBase<TransactionAudit, TransactionAuditRecord>, ITransactionAuditRepository
    {
        private const string GetTransactionAuditSql = @"
            SELECT *
            FROM billing.transaction_audit
            WHERE transaction_id = @transaction_id";

        private const string GetTransactionAuditByDateSql = @"
            SELECT *
            FROM billing.transaction_audit
            WHERE transaction_id = @transaction_id
              AND updated_at = @updated_at";

        private const string UpsertTransactionAuditSql = @"
            INSERT INTO billing.transaction_audit (transaction_id, created_at, updated_at, raw_data)
            VALUES (@transaction_id, @created_at, @updated_at, CAST(@raw_data as JSON))";

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionAuditRepository"/> class.
        /// </summary>
        /// <param name="mandarinDbContext">The application database context.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public TransactionAuditRepository(MandarinDbContext mandarinDbContext, IMapper mapper, ILogger<TransactionAuditRepository> logger)
            : base(mandarinDbContext, mapper, logger)
        {
        }

        /// <inheritdoc />
        public Task<TransactionAudit> GetTransactionAuditAsync(TransactionId transactionId)
        {
            return this.Get(transactionId,
                            db =>
                            {
                                var parameters = new { transaction_id = transactionId.Value };
                                return db.QueryFirstOrDefaultAsync<TransactionAuditRecord>(TransactionAuditRepository.GetTransactionAuditSql, parameters);
                            });
        }

        /// <inheritdoc />
        public Task<TransactionAudit> GetTransactionAuditAsync(TransactionId transactionId, DateTime updatedAt)
        {
            return this.Get(transactionId,
                            db =>
                            {
                                var parameters = new { transaction_id = transactionId.Value, updated_at = updatedAt };
                                return db.QueryFirstOrDefaultAsync<TransactionAuditRecord>(TransactionAuditRepository.GetTransactionAuditByDateSql, parameters);
                            });
        }

        /// <inheritdoc />
        public Task SaveTransactionAuditAsync(TransactionAudit transactionAudit) => this.Upsert(transactionAudit);

        /// <inheritdoc />
        protected override string ExtractDisplayKey(TransactionAudit value) => value.TransactionId.Value;

        /// <inheritdoc />
        protected override Task<IEnumerable<TransactionAuditRecord>> GetAllRecords(IDbConnection db)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc />
        protected override async Task<TransactionAuditRecord> UpsertRecordAsync(IDbConnection db, TransactionAuditRecord value)
        {
            await db.ExecuteAsync(TransactionAuditRepository.UpsertTransactionAuditSql, value);
            return value;
        }
    }
}
