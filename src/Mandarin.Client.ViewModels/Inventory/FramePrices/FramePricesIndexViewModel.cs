using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.Client.ViewModels.Shared;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Components;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePricesIndexViewModel" />
    internal sealed class FramePricesIndexViewModel : IndexPageViewModelBase<IFramePriceGridRowViewModel>, IFramePricesIndexViewModel
    {
        private readonly IFramePricesService framePricesService;
        private readonly IQueryableProductService productService;
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesIndexViewModel"/> class.
        /// </summary>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="productService">The application service for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        public FramePricesIndexViewModel(IFramePricesService framePricesService, IQueryableProductService productService, NavigationManager navigationManager)
        {
            this.framePricesService = framePricesService;
            this.productService = productService;
            this.navigationManager = navigationManager;
        }

        /// <inheritdoc/>
        protected override async Task<IReadOnlyCollection<IFramePriceGridRowViewModel>> OnLoadData()
        {
            var framePrices = await this.framePricesService.GetAllFramePricesAsync();
            var viewModels = await Task.WhenAll(framePrices.Select(CreateViewModel).ToList());
            return viewModels;

            async Task<IFramePriceGridRowViewModel> CreateViewModel(FramePrice x)
            {
                var product = await this.productService.GetProductByProductCodeAsync(x.ProductCode);
                return new FramePriceGridRowViewModel(x, product);
            }
        }

        /// <inheritdoc/>
        protected override void OnCreateNew() => this.navigationManager.NavigateTo("/inventory/frame-prices/new");

        /// <inheritdoc/>
        protected override void OnEditSelected() => this.navigationManager.NavigateTo($"/inventory/frame-prices/edit/{this.SelectedRow.ProductCode}");
    }
}
