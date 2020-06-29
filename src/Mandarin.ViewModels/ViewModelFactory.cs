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
        private readonly IQueryableInventoryService inventoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelFactory"/> class.
        /// </summary>
        /// <param name="emailService">The email service.</param>
        /// <param name="pageContentModel">The website content model.</param>
        /// <param name="httpContextAccessor">The HttpContext accessor.</param>
        /// <param name="inventoryService">The inventory service.</param>
        public ViewModelFactory(IEmailService emailService, PageContentModel pageContentModel, IHttpContextAccessor httpContextAccessor, IQueryableInventoryService inventoryService)
        {
            this.emailService = emailService;
            this.pageContentModel = pageContentModel;
            this.httpContextAccessor = httpContextAccessor;
            this.inventoryService = inventoryService;
        }

        /// <inheritdoc/>
        public IArtistRecordOfSalesViewModel CreateArtistRecordOfSalesViewModel(ArtistSales commission)
        {
            return new ArtistRecordOfSalesViewModel(this.emailService, this.pageContentModel, this.httpContextAccessor, commission);
        }

        /// <inheritdoc/>
        public IFixedCommissionAmountViewModel CreateFixedCommissionAmountViewModel(FixedCommissionAmount fixedCommissionAmount)
        {
            return new FixedCommissionAmountViewModel
            {
                Amount = fixedCommissionAmount.Amount,
                ProductCode = fixedCommissionAmount.ProductCode,
            };
        }
    }
}
