using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <inheritdoc cref="IFixedCommissionsEditViewModel"/>
    internal sealed class FixedCommissionsEditViewModel : ReactiveObject, IFixedCommissionsEditViewModel
    {
        private readonly IFixedCommissionService fixedCommissionService;
        private readonly IQueryableProductService productService;
        private readonly NavigationManager navigationManager;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private readonly ObservableAsPropertyHelper<decimal?> productAmount;
        private readonly ObservableAsPropertyHelper<decimal?> stockistAmount;
        private Product product;
        private decimal? amount;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionsEditViewModel"/> class.
        /// </summary>
        /// <param name="fixedCommissionService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="productService">The application service for interacting with products.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        public FixedCommissionsEditViewModel(IFixedCommissionService fixedCommissionService, IQueryableProductService productService, NavigationManager navigationManager)
        {
            this.fixedCommissionService = fixedCommissionService;
            this.productService = productService;
            this.navigationManager = navigationManager;

            this.LoadData = ReactiveCommand.CreateFromTask<string>(this.OnLoadData);
            this.Save = ReactiveCommand.CreateFromTask(this.OnSave, this.WhenAnyValue(vm => vm.Product, vm => vm.CommissionAmount)
                                                                        .Select(tuple => tuple.Item1 != null && tuple.Item2.HasValue));
            this.Cancel = ReactiveCommand.Create(this.OnCancel);

            this.productAmount = this.WhenAnyValue(vm => vm.Product).WhereNotNull().Select(p => p.UnitPrice).ToProperty(this, x => x.ProductAmount);
            this.stockistAmount = this.WhenAnyValue(vm => vm.ProductAmount, vm => vm.CommissionAmount, (p, c) => p - c).ToProperty(this, x => x.StockistAmount);
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
        public decimal? CommissionAmount
        {
            get => this.amount;
            set => this.RaiseAndSetIfChanged(ref this.amount, value);
        }

        /// <inheritdoc/>
        public decimal? ProductAmount => this.productAmount.Value;

        /// <inheritdoc/>
        public decimal? StockistAmount => this.stockistAmount.Value;

        private async Task OnLoadData(string productCode)
        {
            var existingCommission = await this.fixedCommissionService.GetFixedCommissionAsync(productCode);
            this.Product = await this.productService.GetProductByProductCodeAsync(productCode);
            this.CommissionAmount = existingCommission.Amount;
        }

        private async Task OnSave()
        {
            var fixedCommissionAmount = new FixedCommissionAmount(this.Product.ProductCode, this.CommissionAmount.Value);
            await this.fixedCommissionService.SaveFixedCommissionAsync(fixedCommissionAmount);
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/inventory/fixed-commissions");
    }
}
