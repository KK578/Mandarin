using System;
using System.Diagnostics.CodeAnalysis;

namespace Mandarin.Database.Commissions
{
    /// <summary>
    /// Represents the database record for the billing.commission table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record CommissionRecord
    {
        public int commission_id { get; init; }
        public int stockist_id { get; init; }
        public DateTime start_date { get; init; }
        public DateTime end_date { get; init; }
        public int rate { get; init; }
        public DateTime inserted_at { get; init; }
    }
}
