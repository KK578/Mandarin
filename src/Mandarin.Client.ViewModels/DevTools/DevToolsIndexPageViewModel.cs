using System;
using System.Reactive;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Mandarin.Transactions.External;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.DevTools
{
    /// <inheritdoc cref="IDevToolsIndexPageViewModel" />
    internal sealed class DevToolsIndexPageViewModel : ReactiveObject, IDevToolsIndexPageViewModel
    {
        private readonly IProductSynchronizer productSynchronizer;
        private readonly ITransactionSynchronizer transactionSynchronizer;
        private DateTime? startDate;
        private DateTime? endDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevToolsIndexPageViewModel"/> class.
        /// </summary>
        /// <param name="productSynchronizer">The service to force synchronization of products.</param>
        /// <param name="transactionSynchronizer">The service to force synchronization of transactions.</param>
        public DevToolsIndexPageViewModel(IProductSynchronizer productSynchronizer,
                                          ITransactionSynchronizer transactionSynchronizer)
        {
            this.StartDate = DateTime.Today.AddMonths(-1);
            this.EndDate = DateTime.Today;

            this.productSynchronizer = productSynchronizer;
            this.transactionSynchronizer = transactionSynchronizer;
            this.SynchronizeProducts = ReactiveCommand.CreateFromTask(this.DoSynchronizeProducts);
            var canSynchronizeTransactions = this.WhenAnyValue(x => x.StartDate, x => x.EndDate, (start, end) => end >= start);
            this.SynchronizeTransactions = ReactiveCommand.CreateFromTask(this.DoSynchronizeTransactions, canSynchronizeTransactions);
        }

        /// <inheritdoc />
        public ReactiveCommand<Unit, Unit> SynchronizeProducts { get; }

        /// <inheritdoc />
        public ReactiveCommand<Unit, Unit> SynchronizeTransactions { get; }

        /// <inheritdoc />
        public DateTime? StartDate
        {
            get => this.startDate;
            set => this.RaiseAndSetIfChanged(ref this.startDate, value);
        }

        /// <inheritdoc />
        public DateTime? EndDate
        {
            get => this.endDate;
            set => this.RaiseAndSetIfChanged(ref this.endDate, value);
        }

        private Task DoSynchronizeProducts() => this.productSynchronizer.SynchronizeProductsAsync();

        private Task DoSynchronizeTransactions()
        {
            var start = this.StartDate ?? throw new Exception("Cannot synchronize with null start date.");
            var end = this.EndDate ?? throw new Exception("Cannot synchronize with null end date.");
            return this.transactionSynchronizer.LoadExternalTransactions(start, end);
        }
    }
}
