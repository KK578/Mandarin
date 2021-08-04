using Bashi.Core.TinyTypes;

namespace Mandarin.Stockists
{
    /// <summary>
    /// Represents the unique Code for a <see cref="Stockist"/>.
    /// </summary>
    public class StockistCode : TinyString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockistCode"/> class.
        /// </summary>
        /// <param name="value">The unique Stockist Code.</param>
        private StockistCode(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistCode"/> class, or null if the given <paramref name="value"/> is null/empty.
        /// </summary>
        /// <param name="value">The unique Stockist Code.</param>
        /// <returns>A newly created <see cref="StockistCode"/> or null/empty.</returns>
        public static StockistCode Of(string value) => string.IsNullOrEmpty(value) ? null : new StockistCode(value);
    }
}
