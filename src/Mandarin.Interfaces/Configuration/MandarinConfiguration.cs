using System.Collections.Generic;

namespace Mandarin.Configuration
{
    /// <summary>
    /// Application configuration for the public website of The Little Mandarin.
    /// </summary>
    public sealed class MandarinConfiguration
    {
        /// <summary>
        /// Gets or sets the directory path to the page content JSON file.
        /// </summary>
        public string PageContentFilePath { get; set; }

        /// <summary>
        /// Gets or sets the directory path to the fixed commission amount JSON file.
        /// </summary>
        public string FixedCommissionAmountFilePath { get; set; }

        /// <summary>
        /// Gets or sets the list of product mappings to be applied.
        /// </summary>
        public List<ProductMapping> ProductMappings { get; set; } = new();
    }
}
