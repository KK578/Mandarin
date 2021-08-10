using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.Client.ViewModels.Shared;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Components;
using NodaTime;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePricesIndexViewModel" />
    internal sealed class FramePricesIndexViewModel : IndexPageViewModelBase<IFramePriceViewModel>, IFramePricesIndexViewModel
    {
        private readonly IFramePricesService framePricesService;
        private readonly IProductRepository productRepository;
        private readonly NavigationManager navigationManager;
        private readonly IClock clock;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesIndexViewModel"/> class.
        /// </summary>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        /// <param name="clock">The application clock instance.</param>
        public FramePricesIndexViewModel(IFramePricesService framePricesService,
                                         IProductRepository productRepository,
                                         NavigationManager navigationManager,
                                         IClock clock)
        {
            this.framePricesService = framePricesService;
            this.productRepository = productRepository;
            this.navigationManager = navigationManager;
            this.clock = clock;
        }

        /// <inheritdoc/>
        protected override async Task<IReadOnlyCollection<IFramePriceViewModel>> OnLoadData()
        {
            var framePrices = await this.framePricesService.GetAllFramePricesAsync();
            var viewModels = await Task.WhenAll(framePrices.Select(CreateViewModel).ToList());
            return viewModels;

            async Task<IFramePriceViewModel> CreateViewModel(FramePrice x)
            {
                var product = await this.productRepository.GetProductAsync(x.ProductCode);
                return new FramePriceViewModel(x, product, this.clock);
            }
        }

        /// <inheritdoc/>
        protected override void OnCreateNew() => this.navigationManager.NavigateTo("/inventory/frame-prices/new");

        /// <inheritdoc/>
        protected override void OnEditSelected() => this.navigationManager.NavigateTo($"/inventory/frame-prices/edit/{this.SelectedRow.ProductCode}");
    }
}
