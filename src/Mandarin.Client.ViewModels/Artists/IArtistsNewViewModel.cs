using System.Collections.Generic;
using System.Reactive;
using Mandarin.Common;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Artists
{
    /// <summary>
    /// Represents the ViewModel for creating a new <see cref="Mandarin.Stockists.Stockist"/>.
    /// </summary>
    public interface IArtistsNewViewModel : IValidatableViewModel
    {
        /// <summary>
        /// Gets the updatable ViewModel representing a <see cref="Mandarin.Stockists.Stockist"/>.
        /// </summary>
        IArtistViewModel Stockist { get; }

        /// <summary>
        /// Gets the set of all available statuses that a <see cref="Mandarin.Stockists.Stockist"/> can be in.
        /// </summary>
        IReadOnlyCollection<StatusMode> Statuses { get; }

        /// <summary>
        /// Gets the command to save the newly created <see cref="Mandarin.Stockists.Stockist"/>.
        /// </summary>
        ReactiveCommand<Unit, Unit> Save { get; }

        /// <summary>
        /// Gets the command to cancel and go back to viewing existing <see cref="Mandarin.Stockists.Stockist"/>s.
        /// </summary>
        ReactiveCommand<Unit, Unit> Cancel { get; }
    }
}
