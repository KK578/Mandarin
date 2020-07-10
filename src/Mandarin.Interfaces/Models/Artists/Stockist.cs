using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandarin.Models.Commissions;
using Mandarin.Models.Common;

namespace Mandarin.Models.Artists
{
    /// <summary>
    /// Represents a stockist who is a person provides stock/products with The Little Mandarin.
    /// </summary>
    [Table("stockist", Schema = "inventory")]
    public class Stockist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Stockist"/> class.
        /// </summary>
        public Stockist()
        {
            this.Commissions = new HashSet<Commission>();
        }

        /// <summary>
        /// Gets or sets the Database Stockist ID.
        /// </summary>
        [Key]
        [Column("stockist_id")]
        public int StockistId { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's name.
        /// </summary>
        [Column("stockist_name")]
        [Required]
        public string StockistName { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's user-friendly code.
        /// </summary>
        [Column("stockist_code")]
        [Required]
        public string StockistCode { get; set; }

        /// <summary>
        /// Gets or sets the reference to the stockist's current active status.
        /// </summary>
        [Column("stockist_status")]
        public string StatusCode { get; set; }

        /// <summary>
        /// Gets or sets a description about the stockist.
        /// </summary>
        [Column("description")]
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the current status of stockist.
        /// </summary>
        [ForeignKey(nameof(Stockist.StatusCode))]
        [MaxLength(25)]
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets the stockist's personal details.
        /// </summary>
        [InverseProperty(nameof(StockistDetail.Stockist))]
        public virtual StockistDetail Details { get; set; }

        /// <summary>
        /// Gets or sets the history of all commissions related to this stockist.
        /// </summary>
        [InverseProperty(nameof(Commission.Stockist))]
        public virtual ICollection<Commission> Commissions { get; set; }
    }
}
