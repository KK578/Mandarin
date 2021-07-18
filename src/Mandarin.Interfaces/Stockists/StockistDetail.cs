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
        public StockistId StockistId { get; init; }

        /// <summary>
        /// Gets the Stockist's first name.
        /// </summary>
        public string FirstName { get; init; }

        /// <summary>
        /// Gets the Stockist's last name.
        /// </summary>
        public string LastName { get; init; }

        /// <summary>
        /// Gets the Stockist's artist/display name.
        /// </summary>
        public string DisplayName { get; init; }

        /// <summary>
        /// Gets the stockist's twitter handle.
        /// </summary>
        public string TwitterHandle { get; init; }

        /// <summary>
        /// Gets the stockist's instagram handle.
        /// </summary>
        public string InstagramHandle { get; init; }

        /// <summary>
        /// Gets the stockist's facebook handle.
        /// </summary>
        public string FacebookHandle { get; init; }

        /// <summary>
        /// Gets the stockist's personal website url.
        /// </summary>
        public string WebsiteUrl { get; init; }

        /// <summary>
        /// Gets the stockist's tumblr handle.
        /// </summary>
        public string TumblrHandle { get; init; }

        /// <summary>
        /// Gets the stockist's email address.
        /// </summary>
        public string EmailAddress { get; init; }
    }
}
