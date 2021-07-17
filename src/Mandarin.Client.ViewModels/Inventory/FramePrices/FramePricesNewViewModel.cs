using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Client.ViewModels.Extensions;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePricesNewViewModel"/>
    internal sealed class FramePricesNewViewModel : ReactiveObject, IFramePricesNewViewModel
    {
        private readonly IFramePricesService framePricesService;
        private readonly IQueryableProductService productService;
        private readonly NavigationManager navigationManager;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private IFramePriceViewModel framePrice;
        private Product selectedProduct;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesNewViewModel"/> class.
        /// </summary>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="productService">The application service for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        public FramePricesNewViewModel(IFramePricesService framePricesService, IQueryableProductService productService, NavigationManager navigationManager)
        {
            this.framePricesService = framePricesService;
            this.productService = productService;
            this.navigationManager = navigationManager;

            var products = new ObservableCollection<Product>();
            this.Products = new ReadOnlyObservableCollection<Product>(products);
            this.FramePrice = new FramePriceViewModel();

            this.LoadData = ReactiveCommand.CreateFromObservable(this.OnLoadData);
            this.Save = ReactiveCommand.CreateFromTask(this.OnSave);
            this.Cancel = ReactiveCommand.Create(this.OnCancel);

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe(x => products.Reset(x));
            this.WhenAnyValue(x => x.SelectedProduct).WhereNotNull().Subscribe(this.OnProductUpdated);
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
        public ReactiveCommand<Unit, IReadOnlyList<Product>> LoadData { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Save { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        /// <inheritdoc/>
        public ReadOnlyObservableCollection<Product> Products { get; }

        /// <inheritdoc/>
        public Product SelectedProduct
        {
            get => this.selectedProduct;
            set => this.RaiseAndSetIfChanged(ref this.selectedProduct, value);
        }

        private IObservable<IReadOnlyList<Product>> OnLoadData()
        {
            return this.productService.GetAllProductsAsync().ToObservable();
        }

        private void OnProductUpdated(Product product)
        {
            this.FramePrice.ProductCode = product.ProductCode.Value;
            this.FramePrice.ProductName = product.FriendlyString();
            this.FramePrice.RetailPrice = product.UnitPrice;
        }

        private async Task OnSave()
        {
            await this.framePricesService.SaveFramePriceAsync(this.FramePrice.ToFramePrice());
            this.navigationManager.NavigateTo($"/inventory/frame-prices/edit/{this.FramePrice.ProductCode}");
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/inventory/frame-prices");
    }
}
