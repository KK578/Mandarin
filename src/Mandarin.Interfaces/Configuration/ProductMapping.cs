using System;
using System.Collections.Generic;

namespace Mandarin.Configuration
{
    /// <summary>
    /// Contains the set of mappings to be applied to transactions that occur after a specific date.
    /// </summary>
    public class ProductMapping
    {
        /// <summary>
        /// Gets or sets the date from which these mappings should be applied.
        /// </summary>
        public DateTime TransactionsAfterDate { get; set; }

        /// <summary>
        /// Gets or sets the map of transaction product code to their corrected product codes.
        /// </summary>
        public Dictionary<string, string> Mappings { get; set; }
    }
}
