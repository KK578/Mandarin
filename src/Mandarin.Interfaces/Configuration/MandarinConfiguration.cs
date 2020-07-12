namespace Mandarin.Configuration
{
    /// <summary>
    /// Application configuration for the public website of The Little Mandarin.
    /// </summary>
    public sealed class MandarinConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Contact page will allow user to add attachments.
        /// </summary>
        public bool EnableAttachments { get; set; }

        /// <summary>
        /// Gets or sets the directory path to the page content JSON file.
        /// </summary>
        public string PageContentFilePath { get; set; }

        /// <summary>
        /// Gets or sets the directory path to the fixed commission amount JSON file.
        /// </summary>
        public string FixedCommissionAmountFilePath { get; set; }
    }
}
