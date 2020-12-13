using System;

namespace Mandarin.MVVM
{
    /// <summary>
    /// Represents that the implementer can notify of PropertyChange events.
    /// </summary>
    public interface IPropertyChangedObservable
    {
        /// <summary>
        /// Gets an observable sequence that receives a value any time a value is changed on the view model.
        /// The value inside the sequence will reflect the name of a property.
        /// </summary>
        IObservable<string> StateObservable { get; }
    }
}
