using FluentValidation.Results;
using ReactiveUI;

namespace Mandarin.Client.ViewModels
{
    /// <summary>
    /// Represents a ViewModel that provides validation automatically when attempting actions.
    /// </summary>
    public interface IValidatableViewModel : IReactiveObject
    {
        /// <summary>
        /// Gets the result of the last attempt to validate the current state.
        /// </summary>
        ValidationResult ValidationResult { get; }
    }
}
