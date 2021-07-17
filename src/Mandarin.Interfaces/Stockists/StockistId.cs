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
        public StockistId(int value)
            : base(value)
        {
        }
    }
}
