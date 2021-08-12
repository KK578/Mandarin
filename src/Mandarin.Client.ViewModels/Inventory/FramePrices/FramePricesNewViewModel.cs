using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Mandarin.Client.ViewModels.Extensions;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Components;
using NodaTime;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePricesNewViewModel"/>
    internal sealed class FramePricesNewViewModel : ReactiveObject, IFramePricesNewViewModel
    {
        private readonly IFramePricesService framePricesService;
        private readonly IProductRepository productRepository;
        private readonly NavigationManager navigationManager;
        private readonly IValidator<IFramePriceViewModel> validator;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private Product selectedProduct;
        private ValidationResult validationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesNewViewModel"/> class.
        /// </summary>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        /// <param name="validator">The validator for the FramePrice to ensure it can be saved.</param>
        /// <param name="clock">The application clock instance.</param>
        public FramePricesNewViewModel(IFramePricesService framePricesService,
                                       IProductRepository productRepository,
                                       NavigationManager navigationManager,
                                       IValidator<IFramePriceViewModel> validator,
                                       IClock clock)
        {
            this.framePricesService = framePricesService;
            this.productRepository = productRepository;
            this.navigationManager = navigationManager;
            this.validator = validator;

            var products = new ObservableCollection<Product>();
            this.Products = new ReadOnlyObservableCollection<Product>(products);
            this.FramePrice = new FramePriceViewModel(clock);

            this.LoadData = ReactiveCommand.CreateFromTask(this.OnLoadData);
            this.Save = ReactiveCommand.CreateFromTask(this.OnSave);
            this.Cancel = ReactiveCommand.Create(this.OnCancel);

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe(x => products.Reset(x));
            this.WhenAnyValue(x => x.SelectedProduct).WhereNotNull().Subscribe(this.OnProductUpdated);
        }

        /// <inheritdoc/>
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc />
        public ValidationResult ValidationResult
        {
            get => this.validationResult;
            private set => this.RaiseAndSetIfChanged(ref this.validationResult, value);
        }

        /// <inheritdoc/>
        public IFramePriceViewModel FramePrice { get; }

        /// <inheritdoc/>
        public ReadOnlyObservableCollection<Product> Products { get; }

        /// <inheritdoc/>
        public Product SelectedProduct
        {
            get => this.selectedProduct;
            set => this.RaiseAndSetIfChanged(ref this.selectedProduct, value);
        }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, IReadOnlyList<Product>> LoadData { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Save { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        private Task<IReadOnlyList<Product>> OnLoadData()
        {
            return this.productRepository.GetAllProductsAsync();
        }

        private void OnProductUpdated(Product product)
        {
            this.FramePrice.ProductCode = product.ProductCode.Value;
            this.FramePrice.ProductName = product.FriendlyString();
            this.FramePrice.RetailPrice = product.UnitPrice;
        }

        private async Task OnSave()
        {
            this.ValidationResult = await this.validator.ValidateAsync(this.FramePrice);
            if (!this.ValidationResult.IsValid)
            {
                return;
            }

            await this.framePricesService.SaveFramePriceAsync(this.FramePrice.ToFramePrice());
            this.navigationManager.NavigateTo($"/inventory/frame-prices/edit/{this.FramePrice.ProductCode}");
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/inventory/frame-prices");
    }
}
