using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandarin.Models.Commissions
{
    /// <summary>
    /// Represents a specific commission rate shared by a group of commission periods.
    /// </summary>
    [Table("commission_rate_group", Schema = "billing")]
    public class CommissionRateGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionRateGroup"/> class.
        /// </summary>
        public CommissionRateGroup()
        {
            this.Commissions = new HashSet<Commission>();
        }

        /// <summary>
        /// Gets or sets the commission rate group's Database ID.
        /// </summary>
        [Key]
        [Column("group_id")]
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the commission rate as an integer percentage.
        /// </summary>
        [Column("rate")]
        [Range(0, 100)]
        public int? Rate { get; set; }

        /// <summary>
        /// Gets or sets a set of commission periods that use this commission rate.
        /// </summary>
        [InverseProperty(nameof(Commission.RateGroup))]
        public virtual ICollection<Commission> Commissions { get; set; }
    }
}
