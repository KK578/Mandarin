// using System;
// using System.Collections.Generic;
// using Mandarin.Models.Artists;
//
// namespace Mandarin
// {
//     /// <summary>
//     /// Represents an agreed commission period with a stockist.
//     /// </summary>
//     public partial class Commission
//     {
//         /// <summary>
//         /// Gets or sets the commission's Database ID.
//         /// </summary>
//         public int CommissionId { get; set; }
//
//         /// <summary>
//         /// Gets or sets the related stockist ID related to this commission.
//         /// </summary>
//         public int? StockistId { get; set; }
//
//         /// <summary>
//         /// Gets or sets the start date for this commission.
//         /// </summary>
//         public DateTime StartDate { get; set; }
//
//         /// <summary>
//         /// Gets or sets the end date for this commission.
//         /// </summary>
//         public DateTime EndDate { get; set; }
//
//         /// <summary>
//         ///  Gets or sets the agreed commission rate group for this commission.
//         /// </summary>
//         public int? RateGroup { get; set; }
//
//         /// <summary>
//         /// Gets or sets the time that this commission was created at.
//         /// </summary>
//         public DateTime? InsertedAt { get; set; }
//
//
//         /// <summary>
//         /// Gets or sets the related commission rate group for this commission.
//         /// </summary>
//         public virtual CommissionRateGroup RateGroupNavigation { get; set; }
//
//         /// <summary>
//         /// Gets or sets the related stockist for this commission.
//         /// </summary>
//         public virtual Stockist Stockist { get; set; }
//     }
// }
