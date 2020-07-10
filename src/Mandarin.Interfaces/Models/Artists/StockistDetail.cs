using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandarin.Models.Artists
{
    /// <summary>
    /// Represents a stockist's personal information.
    /// </summary>
    [Table("stockist_detail", Schema = "inventory")]
    public class StockistDetail
    {
        /// <summary>
        /// Gets or sets the stockist's Database ID.
        /// </summary>
        [Key]
        [Column("stockist_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StockistId { get; set; }

        /// <summary>
        /// Gets or sets the stockist's twitter handle.
        /// </summary>
        [Column("twitter_handle")]
        [MaxLength(30)]
        public string TwitterHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's instagram handle.
        /// </summary>
        [Column("instagram_handle")]
        [MaxLength(30)]
        public string InstagramHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's facebook handle.
        /// </summary>
        [Column("facebook_handle")]
        [MaxLength(30)]
        public string FacebookHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's personal website url.
        /// </summary>
        [Column("website_url")]
        [Url]
        [MaxLength(150)]
        public string WebsiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the stockist's banner image url.
        /// </summary>
        [Column("image_url")]
        [MaxLength(150)]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the stockist's tumblr handle.
        /// </summary>
        [Column("tumblr_handle")]
        [MaxLength(30)]
        public string TumblrHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's email address.
        /// </summary>
        [Column("email_address")]
        [EmailAddress]
        [MaxLength(100)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the stockist related to these details.
        /// </summary>
        [ForeignKey(nameof(StockistDetail.StockistId))]
        public virtual Stockist Stockist { get; set; }
    }
}
