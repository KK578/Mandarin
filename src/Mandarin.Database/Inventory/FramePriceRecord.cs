using System;
using System.Diagnostics.CodeAnalysis;

namespace Mandarin.Database.Inventory
{
    /// <summary>
    /// Represents the database record for the billing.frame_price table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record FramePriceRecord
    {
        public string product_code { get; init; }
        public decimal amount { get; init; }
        public DateTime created_at { get; init; }
        public DateTime? active_until { get; init; }
    }
}
