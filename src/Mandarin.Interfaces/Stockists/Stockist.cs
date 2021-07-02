﻿using System.ComponentModel.DataAnnotations;
using Bashi.Core.TinyTypes;
using Mandarin.Commissions;
using Mandarin.Common;

namespace Mandarin.Stockists
{
    /// <summary>
    /// Represents a stockist who is a person provides stock/products with The Little Mandarin.
    /// </summary>
    public class Stockist
    {
        /// <summary>
        /// Gets or sets the Stockist's unique ID.
        /// </summary>
        [Key]
        public StockistId StockistId { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's user-friendly code.
        /// </summary>
        [Required]
        [StringLength(6)]
        public StockistCode StockistCode { get; set; }

        /// <summary>
        /// Gets or sets the reference to the stockist's current active status.
        /// </summary>
        public StatusMode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the stockist's personal details.
        /// </summary>
        public StockistDetail Details { get; set; }

        /// <summary>
        /// Gets or sets the history of all commissions related to this stockist.
        /// </summary>
        public Commission Commission { get; set; }
    }
}
