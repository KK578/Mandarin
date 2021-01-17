using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mandarin.Stockists
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
        public int StockistId { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's name.
        /// </summary>
        [Required]
        [Column("short_display_name")]
        [StringLength(250)]
        public string ShortDisplayName { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's name.
        /// </summary>
        [Required]
        [Column("full_display_name")]
        [StringLength(250)]
        public string FullDisplayName { get; set; }

        /// <summary>
        /// Gets or sets a description about the stockist.
        /// </summary>
        [Column("description")]
        [MaxLength(500)]
        public string Description { get; set; }

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
        /// Gets or sets the stockist's thumbnail image url.
        /// </summary>
        [Column("thumbnail_image_url")]
        [MaxLength(150)]
        public string ThumbnailImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the stockist's banner image url.
        /// </summary>
        [Column("image_url")]
        [MaxLength(150)]
        public string BannerImageUrl { get; set; }

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
        [InverseProperty(nameof(Stockists.Stockist.Details))]
        public virtual Stockist Stockist { get; set; }
    }
}
