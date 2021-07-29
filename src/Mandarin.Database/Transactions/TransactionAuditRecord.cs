using System;
using System.Diagnostics.CodeAnalysis;

namespace Mandarin.Database.Transactions
{
    /// <summary>
    /// Represents the database record for the billing.transaction_audit table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record TransactionAuditRecord
    {
        public string transaction_id { get; init; }
        public DateTime created_at { get; init; }
        public DateTime updated_at { get; init; }
        public string raw_data { get; init; }
    }
}
