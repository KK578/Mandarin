using Mandarin.Models.Inventory;

namespace Mandarin.ViewModels.Commissions
{
    /// <summary>
    /// Represents the component content and interactive model for a fixed commission amount on an artist's product.
    /// </summary>
    public interface IFixedCommissionAmountViewModel : IViewModel
    {
        /// <summary>
        /// Gets or sets the product code that the commission is being applied.
        /// </summary>
        string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets the amount that is a fixed commission on the product.
        /// </summary>
        decimal Amount { get; set; }
    }
}
