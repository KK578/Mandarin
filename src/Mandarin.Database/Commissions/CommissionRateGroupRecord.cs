using System.Diagnostics.CodeAnalysis;

namespace Mandarin.Database.Commissions
{
    /// <summary>
    /// Represents the database record for the billing.commission_rate_group table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record CommissionRateGroupRecord
    {
        public int group_id { get; init; }
        public int rate { get; init; }
    }
}