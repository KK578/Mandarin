using System.Diagnostics.CodeAnalysis;
using Mandarin.Database.Inventory;

namespace Mandarin.Database.Transactions
{
    /// <summary>
    /// Represents the database record for the billing.subtransaction table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record SubtransactionRecord
    {
        public int subtransaction_id { get; init; }
        public int transaction_id { get; init; }
        public string product_id { get; init; }
        public int quantity { get; init; }
        public decimal unit_price { get; init; }

        public ProductRecord Product { get; init; }
    }
}
