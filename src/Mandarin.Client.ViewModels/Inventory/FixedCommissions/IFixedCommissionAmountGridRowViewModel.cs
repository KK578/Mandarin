using Mandarin.Inventory;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <summary>
    /// Represents a <see cref="FixedCommissionAmount"/> for display in a grid.
    /// </summary>
    public interface IFixedCommissionAmountGridRowViewModel
    {
        /// <summary>
        /// Gets the product code associated to the Fixed Commission Amount.
        /// </summary>
        string ProductCode { get; }

        /// <summary>
        /// Gets the commission amount associated to the Fixed Commission Amount.
        /// </summary>
        decimal Amount { get; }
    }
}
