using System.ComponentModel.DataAnnotations;

namespace Mandarin.Stockists
{
    /// <summary>
    /// Represents a stockist's personal information.
    /// </summary>
    public class StockistDetail
    {
        /// <summary>
        /// Gets or sets the stockist's Database ID.
        /// </summary>
        [Key]
        public int StockistId { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's first name.
        /// </summary>
        [StringLength(100)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's last name.
        /// </summary>
        [StringLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's artist/display name.
        /// </summary>
        [Required]
        [StringLength(250)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the stockist's twitter handle.
        /// </summary>
        [MaxLength(30)]
        public string TwitterHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's instagram handle.
        /// </summary>
        [MaxLength(30)]
        public string InstagramHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's facebook handle.
        /// </summary>
        [MaxLength(30)]
        public string FacebookHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's personal website url.
        /// </summary>
        [Url]
        [MaxLength(150)]
        public string WebsiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the stockist's tumblr handle.
        /// </summary>
        [MaxLength(30)]
        public string TumblrHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's email address.
        /// </summary>
        [EmailAddress]
        [MaxLength(100)]
        public string EmailAddress { get; set; }
    }
}
