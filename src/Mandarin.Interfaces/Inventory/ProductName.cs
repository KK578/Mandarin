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
        /// Initializes a new instance of the <see cref="ProductName"/> class.
        /// </summary>
        /// <param name="value">The unique Product Name.</param>
        private ProductName(string value)
            : base(value, StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductName"/> class, or null if the given <paramref name="value"/> is null/empty.
        /// </summary>
        /// <param name="value">The unique Product Name.</param>
        /// <returns>A newly created <see cref="ProductName"/> or null/empty.</returns>
        public static ProductName Of(string value) => string.IsNullOrEmpty(value) ? null : new ProductName(value);
    }
}
