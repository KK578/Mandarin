using System.Diagnostics.CodeAnalysis;
using NodaTime;

namespace Mandarin.Database.Transactions
{
    /// <summary>
    /// Represents the database record for the billing.transaction table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record TransactionSummaryRecord
    {
        public LocalDate date { get; init; }
        public decimal revenue { get; init; }
        public decimal refund { get; init; }
    }
}
