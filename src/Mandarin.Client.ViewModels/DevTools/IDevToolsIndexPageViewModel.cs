using System;
using System.Reactive;
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
        DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date for synchronizing transactions.
        /// </summary>
        DateTime? EndDate { get; set; }
    }
}
