using Mandarin.Models.Commissions;
using Mandarin.ViewModels.Commissions;

namespace Mandarin.ViewModels
{
    public interface IViewModelFactory
    {
        IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(ArtistSales commission);
    }
}
