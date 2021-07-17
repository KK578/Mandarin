using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePricesEditViewModel"/>
    internal sealed class FramePricesEditViewModel : ReactiveObject, IFramePricesEditViewModel
    {
        private readonly IFramePricesService framePricesService;
        private readonly IQueryableProductService productService;
        private readonly NavigationManager navigationManager;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private IFramePriceViewModel framePrice;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesEditViewModel"/> class.
        /// </summary>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="productService">The application service for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        public FramePricesEditViewModel(IFramePricesService framePricesService, IQueryableProductService productService, NavigationManager navigationManager)
        {
            this.framePricesService = framePricesService;
            this.productService = productService;
            this.navigationManager = navigationManager;

            this.LoadData = ReactiveCommand.CreateFromTask<ProductCode>(this.OnLoadData);
            this.Save = ReactiveCommand.CreateFromTask(this.OnSave);
            this.Cancel = ReactiveCommand.Create(this.OnCancel);

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe();
        }

        /// <inheritdoc/>
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc/>
        public IFramePriceViewModel FramePrice
        {
            get => this.framePrice;
            private set => this.RaiseAndSetIfChanged(ref this.framePrice, value);
        }

        /// <inheritdoc/>
        public ReactiveCommand<ProductCode, Unit> LoadData { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Save { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        private async Task OnLoadData(ProductCode productCode)
        {
            var existingFramePrice = await this.framePricesService.GetFramePriceAsync(productCode, DateTime.Now);
            var product = await this.productService.GetProductByProductCodeAsync(productCode);
            this.FramePrice = new FramePriceViewModel(existingFramePrice, product);
        }

        private async Task OnSave()
        {
            await this.framePricesService.SaveFramePriceAsync(this.FramePrice.ToFramePrice());
            this.navigationManager.NavigateTo("/inventory/frame-prices");
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/inventory/frame-prices");
    }
}
