using Mandarin.Commissions;

namespace Mandarin.ViewModels
{
    /// <summary>
    /// Provides the ability to create various ViewModels.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IArtistRecordOfSalesViewModel"/>.
        /// </summary>
        /// <param name="recordOfSales">The artist commission breakdown.</param>
        /// <returns>Instantiated instance of <see cref="IArtistRecordOfSalesViewModel"/>.</returns>
        IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(RecordOfSales recordOfSales);
    }
}
