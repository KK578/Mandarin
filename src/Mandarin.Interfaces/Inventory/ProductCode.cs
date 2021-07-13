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
        public ProductCode(string value)
            : base(value)
        {
        }
    }
}
