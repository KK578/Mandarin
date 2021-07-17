using Bashi.Core.TinyTypes;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents the unique Id for a <see cref="Product"/>.
    /// </summary>
    public class ProductId : TinyString
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductId"/> class.
        /// </summary>
        /// <param name="value">The unique Product Id.</param>
        public ProductId(string value)
            : base(value)
        {
        }
    }
}
