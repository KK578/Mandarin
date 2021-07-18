﻿using System;
using Mandarin.Stockists;

namespace Mandarin.Commissions
{
    /// <summary>
    /// Represents an agreed commission period with a stockist.
    /// </summary>
    public record Commission
    {
        /// <summary>
        /// Gets the commission's Database ID.
        /// </summary>
        public CommissionId CommissionId { get; init; }

        /// <summary>
        /// Gets the related stockist ID related to this commission.
        /// </summary>
        public StockistId StockistId { get; init; }

        /// <summary>
        /// Gets the start date for this commission.
        /// </summary>
        public DateTime StartDate { get; init; }

        /// <summary>
        /// Gets the end date for this commission.
        /// </summary>
        public DateTime EndDate { get; init; }

        /// <summary>
        /// Gets the agreed commission rate group for this commission.
        /// </summary>
        public int Rate { get; init; }

        /// <summary>
        /// Gets the time that this commission was created at.
        /// </summary>
        public DateTime? InsertedAt { get; init; }
    }
}
