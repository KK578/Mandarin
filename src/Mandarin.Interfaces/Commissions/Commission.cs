using System;
using System.ComponentModel.DataAnnotations;
using Mandarin.Stockists;

namespace Mandarin.Commissions
{
    /// <summary>
    /// Represents an agreed commission period with a stockist.
    /// </summary>
    public class Commission
    {
        /// <summary>
        /// Gets or sets the commission's Database ID.
        /// </summary>
        [Key]
        public CommissionId CommissionId { get; set; }

        /// <summary>
        /// Gets or sets the related stockist ID related to this commission.
        /// </summary>
        [Required]
        public StockistId StockistId { get; set; }

        /// <summary>
        /// Gets or sets the start date for this commission.
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for this commission.
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the agreed commission rate group for this commission.
        /// </summary>
        [Required]
        public int Rate { get; set; }

        /// <summary>
        /// Gets or sets the time that this commission was created at.
        /// </summary>
        public DateTime? InsertedAt { get; set; }
    }
}
