using System;
using System.Reactive.Disposables;
using Mandarin.MVVM.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Components.Pages
{
    /// <summary>
    /// Represents a <see cref="ComponentBase"/> for pages that is backed by a <see cref="IViewModel"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IViewModel"/> that is to be injected into the page.</typeparam>
    public abstract class MandarinPageBase<T> : ComponentBase, IDisposable
        where T : IViewModel
    {
        private readonly CompositeDisposable disposables = new();

        /// <summary>
        /// Gets the ViewModel for the page.
        /// </summary>
        [Inject]
        protected T ViewModel { get; init; }

        /// <inheritdoc />
        public void Dispose()
        {
            this.disposables.Dispose();
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            this.disposables.Add(this.ViewModel.StateObservable.Subscribe(this.OnViewModelChanged));
            this.ViewModel.InitializeAsync(); // Async call purposefully run in background.
        }

        private void OnViewModelChanged(string propertyName)
        {
            this.InvokeAsync(this.StateHasChanged);
        }
    }
}
