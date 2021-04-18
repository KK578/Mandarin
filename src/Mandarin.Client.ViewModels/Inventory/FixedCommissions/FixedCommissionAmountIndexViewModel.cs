using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Mandarin.Client.ViewModels.Extensions;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <inheritdoc cref="IFixedCommissionAmountIndexViewModel" />
    internal sealed class FixedCommissionAmountIndexViewModel : ReactiveObject, IFixedCommissionAmountIndexViewModel
    {
        private readonly IFixedCommissionService fixedCommissionService;
        private readonly IQueryableProductService productService;

        private readonly ObservableAsPropertyHelper<bool> isLoading;
        private readonly ObservableCollection<IFixedCommissionAmountGridRowViewModel> rows;
        private IFixedCommissionAmountGridRowViewModel selectedRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionAmountIndexViewModel"/> class.
        /// </summary>
        /// <param name="fixedCommissionService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="productService">The application service for interacting with products.</param>
        public FixedCommissionAmountIndexViewModel(IFixedCommissionService fixedCommissionService, IQueryableProductService productService)
        {
            this.fixedCommissionService = fixedCommissionService;
            this.productService = productService;

            this.rows = new ObservableCollection<IFixedCommissionAmountGridRowViewModel>();
            this.Rows = new ReadOnlyObservableCollection<IFixedCommissionAmountGridRowViewModel>(this.rows);

            this.LoadData = ReactiveCommand.CreateFromObservable(this.LoadDataAsync);

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe(x => this.rows.Reset(x));
        }

        /// <inheritdoc/>
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc/>
        public ReactiveCommand<Unit, IReadOnlyCollection<IFixedCommissionAmountGridRowViewModel>> LoadData { get; }

        /// <inheritdoc/>
        public ReadOnlyObservableCollection<IFixedCommissionAmountGridRowViewModel> Rows { get; }

        /// <inheritdoc/>
        public IFixedCommissionAmountGridRowViewModel SelectedRow
        {
            get => this.selectedRow;
            set => this.RaiseAndSetIfChanged(ref this.selectedRow, value);
        }

        private IObservable<IReadOnlyCollection<IFixedCommissionAmountGridRowViewModel>> LoadDataAsync()
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
    }
}
