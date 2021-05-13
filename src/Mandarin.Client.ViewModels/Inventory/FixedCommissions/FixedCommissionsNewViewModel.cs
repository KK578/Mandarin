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
    /// <inheritdoc cref="IFixedCommissionsNewViewModel"/>
    internal sealed class FixedCommissionsNewViewModel : ReactiveObject, IFixedCommissionsNewViewModel
    {
        private readonly IFixedCommissionService fixedCommissionService;
        private readonly IQueryableProductService productService;
        private readonly NavigationManager navigationManager;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private readonly ObservableAsPropertyHelper<decimal?> productAmount;
        private readonly ObservableAsPropertyHelper<decimal?> stockistAmount;
        private Product selectedProduct;
        private decimal amount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionsNewViewModel"/> class.
        /// </summary>
        /// <param name="fixedCommissionService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="productService">The application service for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        public FixedCommissionsNewViewModel(IFixedCommissionService fixedCommissionService, IQueryableProductService productService, NavigationManager navigationManager)
        {
            this.fixedCommissionService = fixedCommissionService;
            this.productService = productService;
            this.navigationManager = navigationManager;

            var products = new ObservableCollection<Product>();
            this.Products = new ReadOnlyObservableCollection<Product>(products);

            this.LoadData = ReactiveCommand.CreateFromObservable(this.OnLoadData);
            this.Save = ReactiveCommand.CreateFromTask(this.OnSave);
            this.Cancel = ReactiveCommand.Create(this.OnCancel);

            this.productAmount = this.WhenAnyValue(vm => vm.SelectedProduct).WhereNotNull().Select(p => p.UnitPrice).ToProperty(this, x => x.ProductAmount);
            this.stockistAmount = this.WhenAnyValue(vm => vm.ProductAmount, vm => vm.CommissionAmount, (p, c) => p - c).ToProperty(this, x => x.StockistAmount);
            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe(x => products.Reset(x));
        }

        /// <inheritdoc/>
        public bool IsLoading => this.isLoading.Value;

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

        /// <inheritdoc/>
        public decimal CommissionAmount
        {
            get => this.amount;
            set => this.RaiseAndSetIfChanged(ref this.amount, value);
        }

        /// <inheritdoc/>
        public decimal? ProductAmount => this.productAmount.Value;

        /// <inheritdoc/>
        public decimal? StockistAmount => this.stockistAmount.Value;

        private IObservable<IReadOnlyList<Product>> OnLoadData()
        {
            return this.productService.GetAllProductsAsync().ToObservable();
        }

        private async Task OnSave()
        {
            var fixedCommissionAmount = new FixedCommissionAmount(this.SelectedProduct.ProductCode, this.CommissionAmount);
            await this.fixedCommissionService.SaveFixedCommissionAsync(fixedCommissionAmount);
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/inventory/fixed-commissions");
    }
}
