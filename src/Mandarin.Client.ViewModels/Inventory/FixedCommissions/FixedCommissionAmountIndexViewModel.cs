using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using DynamicData.Binding;
using Mandarin.Client.ViewModels.Extensions;
using Mandarin.Inventory;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Inventory.FixedCommissions
{
    /// <inheritdoc cref="IFixedCommissionAmountIndexViewModel" />
    internal sealed class FixedCommissionAmountIndexViewModel : ReactiveObject, IFixedCommissionAmountIndexViewModel
    {
        private readonly IFixedCommissionService fixedCommissionService;
        private readonly ObservableCollection<IFixedCommissionAmountGridRowViewModel> rows;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionAmountIndexViewModel"/> class.
        /// </summary>
        /// <param name="fixedCommissionService">The application service for interacting with commissions and records of sales.</param>
        public FixedCommissionAmountIndexViewModel(IFixedCommissionService fixedCommissionService)
        {
            this.fixedCommissionService = fixedCommissionService;

            this.rows = new ObservableCollection<IFixedCommissionAmountGridRowViewModel>();
            this.Rows = new ReadOnlyObservableCollection<IFixedCommissionAmountGridRowViewModel>(this.rows);

            this.LoadData = ReactiveCommand.CreateFromObservable(this.LoadDataAsync);
            this.LoadData.Subscribe(x => this.rows.Reset(x));
        }

        /// <inheritdoc/>
        public ReactiveCommand<Unit, IReadOnlyCollection<IFixedCommissionAmountGridRowViewModel>> LoadData { get; }

        /// <inheritdoc/>
        public ReadOnlyObservableCollection<IFixedCommissionAmountGridRowViewModel> Rows { get; }

        private IObservable<IReadOnlyCollection<IFixedCommissionAmountGridRowViewModel>> LoadDataAsync()
        {
            return this.fixedCommissionService.GetFixedCommissionAsync()
                       .ToObservable()
                       .Select(l => l.Select(x => new FixedCommissionAmountGridRowViewModel(x)).ToList().AsReadOnly());
        }
    }
}
