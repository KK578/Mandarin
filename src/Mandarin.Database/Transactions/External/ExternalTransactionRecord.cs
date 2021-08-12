using System.Diagnostics.CodeAnalysis;
using NodaTime;

namespace Mandarin.Database.Transactions.External
{
    /// <summary>
    /// Represents the database record for the billing.external_transaction table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record ExternalTransactionRecord
    {
        public string external_transaction_id { get; init; }
        public Instant created_at { get; init; }
        public Instant updated_at { get; init; }
        public string raw_data { get; init; }
    }
}
