using Bashi.Core.TinyTypes;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents the unique Id for a <see cref="Product"/>.
    /// </summary>
    public class ProductId : TinyString
    {
        /// <summary>
        /// The well known <see cref="ProductId"/> for TLM online delivery fees.
        /// </summary>
        public static readonly ProductId TlmDelivery = new("TLM-DELIVERY");

        /// <summary>
        /// The well known <see cref="ProductId"/> for TLM framing.
        /// </summary>
        public static readonly ProductId TlmFraming = new("TLM-FRAMING");

        /// <summary>
        /// The well known <see cref="ProductId"/> for TLM online tips.
        /// </summary>
        public static readonly ProductId TlmTip = new("TLM-TIP");

        /// <summary>
        /// The well known <see cref="ProductId"/> for unknown items.
        /// </summary>
        public static readonly ProductId TlmUnknown = new("TLM-UNKNOWN");

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductId"/> class.
        /// </summary>
        /// <param name="value">The unique Product Id.</param>
        private ProductId(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductId"/> class, or null if the given <paramref name="value"/> is null/empty.
        /// </summary>
        /// <param name="value">The unique Product Id.</param>
        /// <returns>A newly created <see cref="ProductId"/> or null/empty.</returns>
        public static ProductId Of(string value) => string.IsNullOrEmpty(value) ? null : new ProductId(value);
    }
}
