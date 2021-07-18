using Mandarin.Commissions;
using Mandarin.Common;

namespace Mandarin.Stockists
{
    /// <summary>
    /// Represents a stockist who is a person provides stock/products with The Little Mandarin.
    /// </summary>
    public record Stockist
    {
        /// <summary>
        /// Gets the Stockist's unique ID.
        /// </summary>
        public StockistId StockistId { get; init; }

        /// <summary>
        /// Gets the Stockist's user-friendly code.
        /// </summary>
        public StockistCode StockistCode { get; init; }

        /// <summary>
        /// Gets the reference to the stockist's current active status.
        /// </summary>
        public StatusMode StatusCode { get; init; }

        /// <summary>
        /// Gets the stockist's personal details.
        /// </summary>
        public StockistDetail Details { get; init; }

        /// <summary>
        /// Gets the history of all commissions related to this stockist.
        /// </summary>
        public Commission Commission { get; init; }
    }
}
