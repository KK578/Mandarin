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
        private readonly ObservableAsPropertyHelper<decimal?> productAmount;
        private readonly ObservableAsPropertyHelper<decimal?> stockistAmount;
        private Product product;
        private decimal? frameAmount;
        private DateTime? createdAt;

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

            this.LoadData = ReactiveCommand.CreateFromTask<string>(this.OnLoadData);
            this.Save = ReactiveCommand.CreateFromTask(this.OnSave, this.WhenAnyValue(vm => vm.Product,
                                                                                      vm => vm.FrameAmount,
                                                                                      vm => vm.CreatedAt,
                                                                                      (p, f, c) => p != null && f.HasValue && c.HasValue));
            this.Cancel = ReactiveCommand.Create(this.OnCancel);

            this.productAmount = this.WhenAnyValue(vm => vm.Product).WhereNotNull().Select(p => p.UnitPrice).ToProperty(this, x => x.ProductAmount);
            this.stockistAmount = this.WhenAnyValue(vm => vm.ProductAmount, vm => vm.FrameAmount, (p, c) => p - c).ToProperty(this, x => x.StockistAmount);
            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe();
        }

        /// <inheritdoc/>
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc/>
        public ReactiveCommand<string, Unit> LoadData { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Save { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        /// <inheritdoc/>
        public Product Product
        {
            get => this.product;
            private set => this.RaiseAndSetIfChanged(ref this.product, value);
        }

        /// <inheritdoc/>
        public decimal? FrameAmount
        {
            get => this.frameAmount;
            set => this.RaiseAndSetIfChanged(ref this.frameAmount, value);
        }

        /// <inheritdoc/>
        public DateTime? CreatedAt
        {
            get => this.createdAt;
            set => this.RaiseAndSetIfChanged(ref this.createdAt, value);
        }

        /// <inheritdoc/>
        public decimal? ProductAmount => this.productAmount.Value;

        /// <inheritdoc/>
        public decimal? StockistAmount => this.stockistAmount.Value;

        private async Task OnLoadData(string productCode)
        {
            var existingFramePrice = await this.framePricesService.GetFramePriceAsync(productCode);
            this.Product = await this.productService.GetProductByProductCodeAsync(productCode);
            this.FrameAmount = existingFramePrice.Amount;
            this.CreatedAt = existingFramePrice.CreatedAt;
        }

        private async Task OnSave()
        {
            if (!this.FrameAmount.HasValue || !this.CreatedAt.HasValue)
            {
                return;
            }

            var framePrice = new FramePrice
            {
                ProductCode = this.Product.ProductCode,
                Amount = this.FrameAmount.Value,
                CreatedAt = this.CreatedAt.Value,
            };
            await this.framePricesService.SaveFramePriceAsync(framePrice);
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/inventory/frame-prices");
    }
}
