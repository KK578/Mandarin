using System;
using Bashi.Core.TinyTypes;

namespace Mandarin.Inventory
{
    /// <summary>
    /// Represents the unique Name for a <see cref="Product"/>.
    /// </summary>
    public class ProductName : TinyString
    {
        /// <summary>
        /// Represents the Product named 'eGift Card'.
        /// </summary>
        public static readonly ProductName TlmGiftCard = new("eGift Card");

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductName"/> class.
        /// </summary>
        /// <param name="value">The unique Product Name.</param>
        public ProductName(string value)
            : base(value, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
