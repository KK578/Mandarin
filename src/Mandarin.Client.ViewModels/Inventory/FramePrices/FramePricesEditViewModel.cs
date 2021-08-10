using System;
using System.Reactive;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Components;
using NodaTime;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePricesEditViewModel"/>
    internal sealed class FramePricesEditViewModel : ReactiveObject, IFramePricesEditViewModel
    {
        private readonly IFramePricesService framePricesService;
        private readonly IProductRepository productRepository;
        private readonly NavigationManager navigationManager;
        private readonly IValidator<IFramePriceViewModel> validator;
        private readonly IClock clock;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private IFramePriceViewModel framePrice;
        private ValidationResult validationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesEditViewModel"/> class.
        /// </summary>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        /// <param name="validator">The validator for the FramePrice to ensure it can be saved.</param>
        /// <param name="clock">The application clock instance.</param>
        public FramePricesEditViewModel(IFramePricesService framePricesService,
                                        IProductRepository productRepository,
                                        NavigationManager navigationManager,
                                        IValidator<IFramePriceViewModel> validator,
                                        IClock clock)
        {
            this.framePricesService = framePricesService;
            this.productRepository = productRepository;
            this.navigationManager = navigationManager;
            this.validator = validator;
            this.clock = clock;

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

        /// <inheritdoc />
        public ValidationResult ValidationResult
        {
            get => this.validationResult;
            private set => this.RaiseAndSetIfChanged(ref this.validationResult, value);
        }

        /// <inheritdoc/>
        public ReactiveCommand<ProductCode, Unit> LoadData { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Save { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        private async Task OnLoadData(ProductCode productCode)
        {
            var existingFramePrice = await this.framePricesService.GetFramePriceAsync(productCode, this.clock.GetCurrentInstant());
            var product = await this.productRepository.GetProductAsync(productCode);
            this.FramePrice = new FramePriceViewModel(existingFramePrice, product, this.clock);
        }

        private async Task OnSave()
        {
            this.ValidationResult = await this.validator.ValidateAsync(this.FramePrice);
            if (!this.ValidationResult.IsValid)
            {
                return;
            }

            await this.framePricesService.SaveFramePriceAsync(this.FramePrice.ToFramePrice());
            this.navigationManager.NavigateTo("/inventory/frame-prices");
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/inventory/frame-prices");
    }
}
