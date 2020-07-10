using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandarin.Models.Artists;

namespace Mandarin.Models.Commissions
{
    /// <summary>
    /// Represents an agreed commission period with a stockist.
    /// </summary>
    [Table("commission", Schema = "billing")]
    public class Commission
    {
        /// <summary>
        /// Gets or sets the commission's Database ID.
        /// </summary>
        [Key]
        [Column("commission_id")]
        public int CommissionId { get; set; }

        /// <summary>
        /// Gets or sets the related stockist ID related to this commission.
        /// </summary>
        [Column("stockist_id")]
        public int? StockistId { get; set; }

        /// <summary>
        /// Gets or sets the start date for this commission.
        /// </summary>
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for this commission.
        /// </summary>
        [Column("end_date")]
        public DateTime EndDate { get; set; }

        /// <summary>
        ///  Gets or sets the agreed commission rate group for this commission.
        /// </summary>
        [Column("rate_group")]
        public int? RateGroupId { get; set; }

        /// <summary>
        /// Gets or sets the time that this commission was created at.
        /// </summary>
        [Column("inserted_at")]
        public DateTime? InsertedAt { get; set; }

        /// <summary>
        /// Gets or sets the related commission rate group for this commission.
        /// </summary>
        [ForeignKey(nameof(Commission.RateGroupId))]
        public virtual CommissionRateGroup RateGroup { get; set; }

        /// <summary>
        /// Gets or sets the related stockist for this commission.
        /// </summary>
        [ForeignKey(nameof(Commission.StockistId))]
        public virtual Stockist Stockist { get; set; }
    }
}
