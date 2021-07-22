using System;
using System.Diagnostics.CodeAnalysis;

namespace Mandarin.Database.Inventory
{
    /// <summary>
    /// Represents the database record for the inventory.product table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record ProductRecord
    {
        public string product_id { get; init; }
        public string product_code { get; init; }
        public string product_name { get; init; }
        public string description { get; init; }
        public decimal? unit_price { get; init; }
        public DateTime last_updated { get; init; }
    }
}
