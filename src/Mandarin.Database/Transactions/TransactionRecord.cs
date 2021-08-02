using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mandarin.Database.Transactions
{
    /// <summary>
    /// Represents the database record for the billing.transaction table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record TransactionRecord
    {
        public int transaction_id { get; init; }
        public string external_transaction_id { get; init; }
        public decimal? total_amount { get; init; }
        public DateTime timestamp { get; init; }
        public IReadOnlyList<SubtransactionRecord> Subtransactions { get; set; }
    }
}
