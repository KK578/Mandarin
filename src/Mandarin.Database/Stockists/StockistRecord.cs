using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mandarin.Database.Commissions;

namespace Mandarin.Database.Stockists
{
    /// <summary>
    /// Represents the database record for the inventory.stockist table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record StockistRecord
    {
        public int stockist_id { get; init; }
        public string stockist_code { get; init; }
        public string stockist_status { get; init; }

        public StockistDetailRecord Details { get; init; }
        public IReadOnlyList<CommissionRecord> Commissions { get; init; }
    }
}
