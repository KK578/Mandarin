using NodaTime;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents a partial amount on a sale of a framed product, where the frame price is considered as separate to the Artist's commission purposes.
    /// </summary>
    public record FramePrice
    {
        /// <summary>
        /// Gets the product's unique item code.
        /// </summary>
        public ProductCode ProductCode { get; init; }

        /// <summary>
        /// Gets the product's frame price.
        /// </summary>
        public decimal Amount { get; init; }

        /// <summary>
        /// Gets the time the entry was created.
        /// </summary>
        public Instant CreatedAt { get; init; }

        /// <summary>
        /// Gets the last time that the frame price is active til, or null.
        /// </summary>
        public Instant? ActiveUntil { get; init; }
    }
}
