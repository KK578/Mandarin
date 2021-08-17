using System.Diagnostics.CodeAnalysis;
using Mandarin.Commissions;

namespace Mandarin.Database.Commissions
{
    /// <summary>
    /// Represents the database record for the <see cref="Sale"/>s in a <see cref="RecordOfSales"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record SaleRecord
    {
        public int stockist_id { get; init; }
        public string product_code { get; init; }
        public string product_name { get; init; }
        public int quantity { get; init; }
        public decimal unit_price { get; init; }
        public decimal subtotal { get; init; }
        public decimal commission { get; init; }
        public decimal total { get; init; }
    }
}
