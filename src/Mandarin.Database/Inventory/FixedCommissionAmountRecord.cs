using System.Diagnostics.CodeAnalysis;

namespace Mandarin.Database.Inventory
{
    /// <summary>
    /// Represents the database record for the billing.fixed_commission_amount table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record FixedCommissionAmountRecord
    {
        public string product_code { get; init; }
        public double amount { get; init; }
    }
}
