using System.Reactive;
using NodaTime;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.DevTools
{
    /// <summary>
    /// Represents the ViewModel to access various Developer tools.
    /// </summary>
    public interface IDevToolsIndexPageViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets the command to synchronize all <see cref="Mandarin.Inventory.Product"/>s now.
        /// </summary>
        ReactiveCommand<Unit, Unit> SynchronizeProducts { get; }

        /// <summary>
        /// Gets the command to synchronize all <see cref="Mandarin.Transactions.Transaction"/>s between the given date and now.
        /// </summary>
        ReactiveCommand<Unit, Unit> SynchronizeTransactions { get; }

        /// <summary>
        /// Gets or sets the start date for synchronizing transactions.
        /// </summary>
        LocalDate StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for synchronizing transactions.
        /// </summary>
        LocalDate EndDate { get; set; }
    }
}
