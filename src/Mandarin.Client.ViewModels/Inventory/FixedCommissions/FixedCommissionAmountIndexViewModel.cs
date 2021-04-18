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
    /// <inheritdoc cref="IFixedCommissionAmountIndexViewModel" />
    internal sealed class FixedCommissionAmountIndexViewModel : ReactiveObject, IFixedCommissionAmountIndexViewModel
    {
        private readonly IFixedCommissionService fixedCommissionService;
        private readonly IQueryableProductService productService;
        private readonly NavigationManager navigationManager;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private IFixedCommissionAmountGridRowViewModel selectedRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionAmountIndexViewModel"/> class.
        /// </summary>
        /// <param name="fixedCommissionService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="productService">The application service for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        public FixedCommissionAmountIndexViewModel(IFixedCommissionService fixedCommissionService, IQueryableProductService productService, NavigationManager navigationManager)
        {
            this.fixedCommissionService = fixedCommissionService;
            this.productService = productService;
            this.navigationManager = navigationManager;

            var rows = new ObservableCollection<IFixedCommissionAmountGridRowViewModel>();
            this.Rows = new ReadOnlyObservableCollection<IFixedCommissionAmountGridRowViewModel>(rows);

            this.LoadData = ReactiveCommand.CreateFromObservable(this.OnLoadData);
            this.CreateNew = ReactiveCommand.Create(this.OnCreateNew);
            this.EditSelected = ReactiveCommand.Create(this.OnEditSelected, this.WhenAnyValue(x => x.SelectedRow).Select(x => x != null));

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe(x => rows.Reset(x));
        }

        /// <inheritdoc/>
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc/>
        public ReactiveCommand<Unit, IReadOnlyCollection<IFixedCommissionAmountGridRowViewModel>> LoadData { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> CreateNew { get; }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, Unit> EditSelected { get; }

        /// <inheritdoc/>
        public ReadOnlyObservableCollection<IFixedCommissionAmountGridRowViewModel> Rows { get; }

        /// <inheritdoc/>
        public IFixedCommissionAmountGridRowViewModel SelectedRow
        {
            get => this.selectedRow;
            set => this.RaiseAndSetIfChanged(ref this.selectedRow, value);
        }

        private IObservable<IReadOnlyCollection<IFixedCommissionAmountGridRowViewModel>> OnLoadData()
        {
            return this.fixedCommissionService.GetFixedCommissionAsync()
                       .ToObservable()
                       .SelectMany(x => x)
                       .SelectMany(CreateViewModel)
                       .ToList()
                       .Select(x => new ReadOnlyCollection<IFixedCommissionAmountGridRowViewModel>(x));

            async Task<IFixedCommissionAmountGridRowViewModel> CreateViewModel(FixedCommissionAmount x)
            {
                var product = await this.productService.GetProductByProductCodeAsync(x.ProductCode);
                return new FixedCommissionAmountGridRowViewModel(x, product);
            }
        }

        private void OnCreateNew() => this.navigationManager.NavigateTo("/inventory/fixed-commissions/new");

        private void OnEditSelected() => this.navigationManager.NavigateTo($"/inventory/fixed-commissions/edit/{this.SelectedRow.ProductCode}");
    }
}
