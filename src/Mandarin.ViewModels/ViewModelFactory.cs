using Mandarin.Models.Commissions;
using Mandarin.Services;
using Mandarin.ViewModels.Commissions;
using Microsoft.AspNetCore.Http;

namespace Mandarin.ViewModels
{
    /// <inheritdoc />
    internal sealed class ViewModelFactory : IViewModelFactory
    {
        private readonly IEmailService emailService;
        private readonly PageContentModel pageContentModel;
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="emailService">The email service.</param>
        /// <param name="pageContentModel">The website content model.</param>
        /// <param name="httpContextAccessor">The HttpContext accessor.</param>
        public ViewModelFactory(IEmailService emailService, PageContentModel pageContentModel, IHttpContextAccessor httpContextAccessor)
        {
            this.emailService = emailService;
            this.pageContentModel = pageContentModel;
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc/>
        public IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(RecordOfSales recordOfSales)
        {
            return new ArtistRecordOfSalesViewModel(this.emailService, this.pageContentModel, this.httpContextAccessor, recordOfSales);
        }
    }
}
