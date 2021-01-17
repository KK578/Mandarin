using System.Diagnostics.CodeAnalysis;

namespace Mandarin.Database.Stockists
{
    /// <summary>
    /// Represents the database record for the inventory.stockist_detail table.
    /// </summary>
    [SuppressMessage("ReSharper", "SA1300", Justification = "Database record should match exactly to schema.")]
    [SuppressMessage("ReSharper", "SA1516", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "SA1600", Justification = "Database record doesn't need documentation.")]
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Database record should match exactly to schema.")]
    internal sealed record StockistDetailRecord
    {
        public int stockist_id { get; init; }
        public string first_name { get; init; }
        public string last_name { get; init; }
        public string display_name { get; init; }
        public string twitter_handle { get; init; }
        public string instagram_handle { get; init; }
        public string facebook_handle { get; init; }
        public string website_url { get; init; }
        public string tumblr_handle { get; init; }
        public string email_address { get; init; }
    }
}
