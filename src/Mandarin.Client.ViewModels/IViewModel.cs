using System;

namespace Mandarin.Client.ViewModels
{
    /// <summary>
    /// Represents that the user can be notified of PropertyChange events.
    /// </summary>
    public interface IViewModel
    {
        /// <summary>
        /// Gets an observable sequence that receives a value any time a value is changed on the view model.
        /// The value inside the sequence will reflect the name of a property.
        /// </summary>
        IObservable<string> StateObservable { get; }
    }
}
