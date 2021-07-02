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
        public StockistCode(string value)
            : base(value)
        {
        }
    }
}
