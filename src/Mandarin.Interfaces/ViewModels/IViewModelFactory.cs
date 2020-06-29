using Mandarin.Models.Commissions;
using Mandarin.ViewModels.Commissions;

namespace Mandarin.ViewModels
{
    /// <summary>
    /// Provides the ability to create various ViewModels.
    /// </summary>
    // TODO: Add all view models that come from the container to the factory.
    public interface IViewModelFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IArtistRecordOfSalesViewModel"/>.
        /// </summary>
        /// <param name="commission">The artist commission breakdown to be included.</param>
        /// <returns>Instantiated instance of <see cref="IArtistRecordOfSalesViewModel"/>.</returns>
        IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(ArtistSales commission);

        /// <summary>
        /// Creates a new instance of the <see cref="IFixedCommissionAmountViewModel"/>.
        /// </summary>
        /// <param name="fixedCommissionAmount">The commission amount reference.</param>
        /// <returns>A populated view model from the given fixed commission amount reference.</returns>
        IFixedCommissionAmountViewModel CreateFixedCommissionAmountViewModel(FixedCommissionAmount fixedCommissionAmount);
    }
}
