using System;
using System.ComponentModel.DataAnnotations;
using Mandarin.Common;
using Mandarin.Stockists;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Artists
{
    /// <summary>
    /// Represents a <see cref="Stockist"/> for viewing/editing.
    /// </summary>
    public interface IArtistViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets or sets the Stockist's user-friendly code.
        /// </summary>
        [Required]
        [StringLength(6)]
        public string StockistCode { get; set; }

        /// <summary>
        /// Gets or sets the reference to the stockist's current active status.
        /// </summary>
        public StatusMode StatusCode { get; set; }

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
        ///  Gets or sets the agreed commission rate group for this commission.
        /// </summary>
        [Required]
        public int Rate { get; set; }
    }
}
