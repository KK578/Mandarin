using Mandarin.Client.ViewModels.Commissions;
using Mandarin.Commissions;
using Mandarin.Emails;
using Mandarin.ViewModels;
using Microsoft.AspNetCore.Components.Authorization;

namespace Mandarin.Client.ViewModels
{
    /// <inheritdoc />
    internal sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly IEmailService emailService;
        private readonly AuthenticationStateProvider authenticationStateProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="emailService">The application service for sending emails.</param>
        /// <param name="authenticationStateProvider">The service which provides access to the current authentication state..</param>
        public ViewModelFactory(IEmailService emailService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.emailService = emailService;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        /// <inheritdoc/>
        public IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(RecordOfSales recordOfSales)
        {
            return new ArtistRecordOfSalesViewModel(this.emailService, this.authenticationStateProvider, recordOfSales);
        }
    }
}
