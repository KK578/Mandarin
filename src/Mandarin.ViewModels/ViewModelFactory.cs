using Mandarin.Models.Commissions;
using Mandarin.Services;
using Mandarin.ViewModels.Commissions;

namespace Mandarin.ViewModels
{
    /// <inheritdoc />
    internal sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly IEmailService emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="emailService">The email service.</param>
        public ViewModelFactory(IEmailService emailService)
        {
            this.emailService = emailService;
        }

        /// <inheritdoc/>
        public IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(ArtistSales commission)
        {
            return new ArtistRecordOfSalesViewModel(this.emailService, commission);
        }
    }
}
