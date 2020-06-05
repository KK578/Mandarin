using Mandarin.Models.Commissions;
using Mandarin.Services;
using Mandarin.ViewModels.Commissions;

namespace Mandarin.ViewModels
{
    internal sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly IEmailService emailService;

        public ViewModelFactory(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        public IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(ArtistSales commission)
        {
            return new ArtistRecordOfSalesViewModel(this.emailService, commission);
        }
    }
}
