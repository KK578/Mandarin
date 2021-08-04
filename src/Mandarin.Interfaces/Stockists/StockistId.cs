using Bashi.Core.TinyTypes;

namespace Mandarin.Stockists
{
    /// <summary>
    /// Represents the unique ID for a <see cref="Stockist"/>.
    /// </summary>
    public class StockistId : TinyType<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StockistId"/> class.
        /// </summary>
        /// <param name="value">The unique Stockist ID.</param>
        private StockistId(int value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistId"/> class, or null if the given <paramref name="value"/> is null/empty.
        /// </summary>
        /// <param name="value">The unique Stockist Id.</param>
        /// <returns>A newly created <see cref="StockistId"/> or null/empty.</returns>
        public static StockistId Of(int? value) => value.HasValue ? new StockistId(value.Value) : null;
    }
}
