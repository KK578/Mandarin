using Mandarin.Commissions;
using Mandarin.Emails;
using Mandarin.ViewModels.Commissions;
using Microsoft.AspNetCore.Components.Authorization;

namespace Mandarin.ViewModels
{
    /// <inheritdoc />
    internal sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly IEmailService emailService;
        private readonly PageContentModel pageContentModel;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="emailService">The application service for sending emails.</param>
        /// <param name="pageContentModel">The website content model.</param>
        /// <param name="authenticationStateProvider">The service which provides access to the current authentication state..</param>
        public ViewModelFactory(IEmailService emailService, PageContentModel pageContentModel, AuthenticationStateProvider authenticationStateProvider)
        {
            this.emailService = emailService;
            this.pageContentModel = pageContentModel;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        /// <inheritdoc/>
        public IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(RecordOfSales recordOfSales)
        {
            return new ArtistRecordOfSalesViewModel(this.emailService, this.pageContentModel, this.authenticationStateProvider, recordOfSales);
        }
    }
}
