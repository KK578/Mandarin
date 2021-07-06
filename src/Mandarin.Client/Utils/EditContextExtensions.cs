using System;
using FluentValidation.Results;
using Mandarin.Client.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using ReactiveUI;

namespace Mandarin.Client.Utils
{
    /// <summary>
    /// Provides extensions on <see cref="EditContext"/> to apply a <see cref="ValidationResult"/> that
    /// has already been created.
    /// </summary>
    internal static class EditContextExtensions
    {
        /// <summary>
        /// Connects updates to the <see cref="ValidationResult"/> on <see cref="IValidatableViewModel"/> to the
        /// provided <see cref="EditContext"/>. This does not trigger validation to occur.
        /// </summary>
        /// <param name="editContext">The EditContext to attach to.</param>
        /// <param name="viewModel">The ViewModel to be attached to the EditContext.</param>
        /// <param name="callback">A callback to notify after the validation state is updated.</param>
        /// <returns><see cref="IDisposable" /> object used to unsubscribe from the observable sequence.</returns>
        public static IDisposable SubscribeToViewModel(this EditContext editContext, IValidatableViewModel viewModel, Action callback)
        {
            var messages = new ValidationMessageStore(editContext);
            return viewModel.WhenAnyValue(x => x.ValidationResult).Subscribe(HandleValidationResult);

            void HandleValidationResult(ValidationResult validationResult)
            {
                messages.Clear();
                foreach (var error in validationResult.Errors)
                {
                    messages.Add(new FieldIdentifier(editContext.Model, error.PropertyName), error.ErrorMessage);
                }

                editContext.NotifyValidationStateChanged();
                callback?.Invoke();
            }
        }
    }
}
