using System.ComponentModel;

namespace Mandarin.ViewModels.Commissions
{
    /// <summary>
    /// The names of templates available for use in the Record of Sales.
    /// </summary>
    public enum RecordOfSalesTemplateKey
    {
        /// <summary>
        /// For an existing artist's sales.
        /// </summary>
        [Description("Sales")]
        Sales,

        /// <summary>
        /// For a new artist's sales.
        /// </summary>
        [Description("New artist sales")]
        SalesNewArtist,

        /// <summary>
        /// For any artist who made no sales in this month.
        /// </summary>
        [Description("No Sales")]
        NoSales,

        /// <summary>
        /// For any artist who made sales just before COVID-19 closures.
        /// </summary>
        [Description("Sales (COVID-19)")]
        SalesCovid19,

        /// <summary>
        /// For any artist who has not made sales since COVID-19 closures.
        /// </summary>
        [Description("No sales (COVID-19)")]
        NoSalesCovid19,

        /// <summary>
        /// For any artist who's stock arrived outside of this month's dates.
        /// </summary>
        [Description("Stock outside of dates")]
        StockOutsideOfDates,

        /// <summary>
        /// For a new artist's first payment.
        /// </summary>
        [Description("New account check (£1)")]
        SalesNewAccountCheck,
    }
}
