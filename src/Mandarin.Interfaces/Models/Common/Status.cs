using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mandarin.Models.Artists;

namespace Mandarin.Models.Common
{
    /// <summary>
    /// Represents the status of an item.
    /// </summary>
    [Table("status", Schema = "static")]
    public class Status
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Status"/> class.
        /// </summary>
        public Status()
        {
            this.Stockists = new HashSet<Stockist>();
        }

        /// <summary>
        /// Gets or sets the Database ID of this status.
        /// </summary>
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        /// <summary>
        /// Gets or sets the code for this status.
        /// </summary>
        [Column("status_code")]
        [MaxLength(25)]
        public string StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the description of this status.
        /// </summary>
        [Column("description")]
        [MaxLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the set of all stockist's that share this status.
        /// </summary>
        public virtual ICollection<Stockist> Stockists { get; set; }
    }
}
