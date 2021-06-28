using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Client.ViewModels.Extensions;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FramePrices
{
    /// <inheritdoc cref="IFramePricesIndexViewModel" />
    internal sealed class FramePricesIndexViewModel : ReactiveObject, IFramePricesIndexViewModel
    {
        private readonly IFramePricesService framePricesService;
        private readonly IQueryableProductService productService;
        private readonly NavigationManager navigationManager;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private IFramePriceGridRowViewModel selectedRow;

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

            var rows = new ObservableCollection<IFramePriceGridRowViewModel>();
            this.Rows = new ReadOnlyObservableCollection<IFramePriceGridRowViewModel>(rows);

            this.LoadData = ReactiveCommand.CreateFromObservable(this.OnLoadData);
            this.CreateNew = ReactiveCommand.Create(this.OnCreateNew);
            this.EditSelected = ReactiveCommand.Create(this.OnEditSelected, this.WhenAnyValue(x => x.SelectedRow).Select(x => x != null));

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe(x => rows.Reset(x));
        }

        /// <inheritdoc/>
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc/>
        public ReactiveCommand<Unit, IReadOnlyCollection<IFramePriceGridRowViewModel>> LoadData { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> CreateNew { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> EditSelected { get; }

        /// <inheritdoc/>
        public ReadOnlyObservableCollection<IFramePriceGridRowViewModel> Rows { get; }

        /// <inheritdoc/>
        public IFramePriceGridRowViewModel SelectedRow
        {
            get => this.selectedRow;
            set => this.RaiseAndSetIfChanged(ref this.selectedRow, value);
        }

        private IObservable<IReadOnlyCollection<IFramePriceGridRowViewModel>> OnLoadData()
        {
            return this.framePricesService.GetAllFramePricesAsync()
                       .ToObservable()
                       .SelectMany(x => x)
                       .SelectMany(CreateViewModel)
                       .ToList()
                       .Select(x => new ReadOnlyCollection<IFramePriceGridRowViewModel>(x));

            async Task<IFramePriceGridRowViewModel> CreateViewModel(FramePrice x)
            {
                var product = await this.productService.GetProductByProductCodeAsync(x.ProductCode);
                return new FramePriceGridRowViewModel(x, product);
            }
        }

        private void OnCreateNew() => this.navigationManager.NavigateTo("/inventory/frame-prices/new");

        private void OnEditSelected() => this.navigationManager.NavigateTo($"/inventory/frame-prices/edit/{this.SelectedRow.ProductCode}");
    }
}
