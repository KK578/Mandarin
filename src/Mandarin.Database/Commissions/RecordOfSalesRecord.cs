using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mandarin.Commissions;
using NodaTime;

namespace Mandarin.Database.Commissions
{
    /// <summary>
    /// Represents the database record for the <see cref="RecordOfSales"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record RecordOfSalesRecord
    {
        public string stockist_code { get; init; }
        public string first_name { get; init; }
        public string name { get; init; }
        public string email_address { get; init; }
        public LocalDate start_date { get; init; }
        public LocalDate end_date { get; init; }
        public decimal rate { get; init; }
        public decimal subtotal { get; init; }
        public decimal commission_total { get; init; }
        public decimal total { get; init; }

        public IReadOnlyList<SaleRecord> Sales { get; init; }
    }
}
