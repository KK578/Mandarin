using System.ComponentModel.DataAnnotations;

namespace Mandarin.Stockists
{
    /// <summary>
    /// Represents a stockist's personal information.
    /// </summary>
    public record StockistDetail
    {
        /// <summary>
        /// Gets the stockist's Database ID.
        /// </summary>
        [Key]
        public StockistId StockistId { get; init; }

        /// <summary>
        /// Gets the Stockist's first name.
        /// </summary>
        [StringLength(100)]
        public string FirstName { get; init; }

        /// <summary>
        /// Gets the Stockist's last name.
        /// </summary>
        [StringLength(100)]
        public string LastName { get; init; }

        /// <summary>
        /// Gets the Stockist's artist/display name.
        /// </summary>
        [Required]
        [StringLength(250)]
        public string DisplayName { get; init; }

        /// <summary>
        /// Gets the stockist's twitter handle.
        /// </summary>
        [MaxLength(30)]
        public string TwitterHandle { get; init; }

        /// <summary>
        /// Gets the stockist's instagram handle.
        /// </summary>
        [MaxLength(30)]
        public string InstagramHandle { get; init; }

        /// <summary>
        /// Gets the stockist's facebook handle.
        /// </summary>
        [MaxLength(30)]
        public string FacebookHandle { get; init; }

        /// <summary>
        /// Gets the stockist's personal website url.
        /// </summary>
        [Url]
        [MaxLength(150)]
        public string WebsiteUrl { get; init; }

        /// <summary>
        /// Gets the stockist's tumblr handle.
        /// </summary>
        [MaxLength(30)]
        public string TumblrHandle { get; init; }

        /// <summary>
        /// Gets the stockist's email address.
        /// </summary>
        [EmailAddress]
        [MaxLength(100)]
        public string EmailAddress { get; init; }
    }
}
