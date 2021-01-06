using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandarin.Commissions;
using Mandarin.Common;

namespace Mandarin.Stockists
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
        /// Gets or sets the Stockist's first name.
        /// </summary>
        [Column("first_name")]
        [StringLength(100)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's last name.
        /// </summary>
        [Column("last_name")]
        [StringLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's user-friendly code.
        /// </summary>
        [Required]
        [Column("stockist_code")]
        [StringLength(6)]
        public string StockistCode { get; set; }

        /// <summary>
        /// Gets or sets the reference to the stockist's current active status.
        /// </summary>
        [Column("stockist_status", TypeName = "character varying(25)")]
        public StatusMode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the stockist's personal details.
        /// </summary>
        [InverseProperty(nameof(StockistDetail.Stockist))]
        public virtual StockistDetail Details { get; set; }

        /// <summary>
        /// Gets or sets the history of all commissions related to this stockist.
        /// </summary>
        public virtual ICollection<Commission> Commissions { get; set; }
    }
}
