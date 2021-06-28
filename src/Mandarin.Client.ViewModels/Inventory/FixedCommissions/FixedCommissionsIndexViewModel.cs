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

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <inheritdoc cref="IFixedCommissionsIndexViewModel" />
    internal sealed class FixedCommissionsIndexViewModel : ReactiveObject, IFixedCommissionsIndexViewModel
    {
        private readonly IFramePricesService framePricesService;
        private readonly IQueryableProductService productService;
        private readonly NavigationManager navigationManager;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private IFixedCommissionsGridRowViewModel selectedRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionsIndexViewModel"/> class.
        /// </summary>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="productService">The application service for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        public FixedCommissionsIndexViewModel(IFramePricesService framePricesService, IQueryableProductService productService, NavigationManager navigationManager)
        {
            this.framePricesService = framePricesService;
            this.productService = productService;
            this.navigationManager = navigationManager;

            var rows = new ObservableCollection<IFixedCommissionsGridRowViewModel>();
            this.Rows = new ReadOnlyObservableCollection<IFixedCommissionsGridRowViewModel>(rows);

            this.LoadData = ReactiveCommand.CreateFromObservable(this.OnLoadData);
            this.CreateNew = ReactiveCommand.Create(this.OnCreateNew);
            this.EditSelected = ReactiveCommand.Create(this.OnEditSelected, this.WhenAnyValue(x => x.SelectedRow).Select(x => x != null));

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe(x => rows.Reset(x));
        }

        /// <inheritdoc/>
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc/>
        public ReactiveCommand<Unit, IReadOnlyCollection<IFixedCommissionsGridRowViewModel>> LoadData { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> CreateNew { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> EditSelected { get; }

        /// <inheritdoc/>
        public ReadOnlyObservableCollection<IFixedCommissionsGridRowViewModel> Rows { get; }

        /// <inheritdoc/>
        public IFixedCommissionsGridRowViewModel SelectedRow
        {
            get => this.selectedRow;
            set => this.RaiseAndSetIfChanged(ref this.selectedRow, value);
        }

        private IObservable<IReadOnlyCollection<IFixedCommissionsGridRowViewModel>> OnLoadData()
        {
            return this.framePricesService.GetAllFramePricesAsync()
                       .ToObservable()
                       .SelectMany(x => x)
                       .SelectMany(CreateViewModel)
                       .ToList()
                       .Select(x => new ReadOnlyCollection<IFixedCommissionsGridRowViewModel>(x));

            async Task<IFixedCommissionsGridRowViewModel> CreateViewModel(FramePrice x)
            {
                var product = await this.productService.GetProductByProductCodeAsync(x.ProductCode);
                return new FixedCommissionsGridRowViewModel(x, product);
            }
        }

        private void OnCreateNew() => this.navigationManager.NavigateTo("/inventory/fixed-commissions/new");

        private void OnEditSelected() => this.navigationManager.NavigateTo($"/inventory/fixed-commissions/edit/{this.SelectedRow.ProductCode}");
    }
}
