using Bashi.Core.TinyTypes;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents the unique Code for a <see cref="Product"/>.
    /// </summary>
    public class ProductCode : TinyString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCode"/> class.
        /// </summary>
        /// <param name="value">The unique Product Code.</param>
        private ProductCode(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCode"/> class, or null if the given <paramref name="value"/> is null/empty.
        /// </summary>
        /// <param name="value">The unique Product Code.</param>
        /// <returns>A newly created <see cref="ProductCode"/> or null/empty.</returns>
        public static ProductCode Of(string value) => string.IsNullOrEmpty(value) ? null : new ProductCode(value);
    }
}
