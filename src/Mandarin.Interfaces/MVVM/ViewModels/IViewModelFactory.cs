using Mandarin.Models.Commissions;
using Mandarin.MVVM.ViewModels.Commissions;

namespace Mandarin.MVVM.ViewModels
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
    }
}
