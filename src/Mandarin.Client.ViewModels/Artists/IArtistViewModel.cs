using System;
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
        /// Gets the Stockist's unique ID.
        /// </summary>
        public int? StockistId { get; }

        /// <summary>
        /// Gets or sets the Stockist's user-friendly code.
        /// </summary>
        public string StockistCode { get; set; }

        /// <summary>
        /// Gets or sets the reference to the stockist's current active status.
        /// </summary>
        public StatusMode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the Stockist's artist/display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the stockist's twitter handle.
        /// </summary>
        public string TwitterHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's instagram handle.
        /// </summary>
        public string InstagramHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's facebook handle.
        /// </summary>
        public string FacebookHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's personal website url.
        /// </summary>
        public string WebsiteUrl { get; set; }

        /// <summary>
        /// Gets or sets the stockist's tumblr handle.
        /// </summary>
        public string TumblrHandle { get; set; }

        /// <summary>
        /// Gets or sets the stockist's email address.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets the stockist's latest commission id.
        /// </summary>
        public int? CommissionId { get; }

        /// <summary>
        /// Gets or sets the start date for this commission.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for this commission.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Gets or sets the agreed commission rate group for this commission.
        /// </summary>
        public int Rate { get; set; }

        /// <summary>
        /// Builds the complete <see cref="Stockist"/> from the current values in the ViewModel.
        /// </summary>
        /// <returns>The fully populated <see cref="Stockist"/>.</returns>
        public Stockist ToStockist();
    }
}
