using System.Reactive;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Mandarin.Transactions.External;
using NodaTime;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.DevTools
{
    /// <inheritdoc cref="IDevToolsIndexPageViewModel" />
    internal sealed class DevToolsIndexPageViewModel : ReactiveObject, IDevToolsIndexPageViewModel
    {
        private readonly IProductSynchronizer productSynchronizer;
        private readonly ITransactionSynchronizer transactionSynchronizer;
        private LocalDate startDate;
        private LocalDate endDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevToolsIndexPageViewModel"/> class.
        /// </summary>
        /// <param name="productSynchronizer">The service to force synchronization of products.</param>
        /// <param name="transactionSynchronizer">The service to force synchronization of transactions.</param>
        /// <param name="clock">The application clock instance.</param>
        public DevToolsIndexPageViewModel(IProductSynchronizer productSynchronizer,
                                          ITransactionSynchronizer transactionSynchronizer,
                                          IClock clock)
        {
            this.StartDate = clock.GetCurrentInstant().InUtc().Date.Minus(Period.FromMonths(1));
            this.EndDate = clock.GetCurrentInstant().InUtc().Date;

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
        public LocalDate StartDate
        {
            get => this.startDate;
            set => this.RaiseAndSetIfChanged(ref this.startDate, value);
        }

        /// <inheritdoc />
        public LocalDate EndDate
        {
            get => this.endDate;
            set => this.RaiseAndSetIfChanged(ref this.endDate, value);
        }

        private Task DoSynchronizeProducts() => this.productSynchronizer.SynchronizeProductsAsync();

        private Task DoSynchronizeTransactions()
        {
            return this.transactionSynchronizer.LoadExternalTransactions(this.StartDate, this.EndDate);
        }
    }
}
