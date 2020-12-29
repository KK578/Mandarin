using System;
using System.Reactive.Disposables;
using Mandarin.MVVM.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mandarin.App.Components.Pages
{
    /// <summary>
    /// Represents a <see cref="ComponentBase"/> for pages that is backed by a <see cref="IViewModel"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IViewModel"/> that is to be injected into the page.</typeparam>
    public abstract class MandarinPageBase<T> : OwningComponentBase<T>, IDisposable
        where T : IViewModel
    {
        private readonly CompositeDisposable disposables = new();

        /// <summary>
        /// Gets the page's logger instance.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Gets the ViewModel for the page.
        /// </summary>
        protected T ViewModel => this.Service;

        /// <inheritdoc />
        public void Dispose()
        {
            this.disposables.Dispose();
        }

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            var logFactory = this.ScopedServices.GetRequiredService<ILoggerFactory>();
            this.Logger = logFactory.CreateLogger(this.GetType().Name);

            try
            {
                this.disposables.Add(this.ViewModel.StateObservable.Subscribe(this.OnViewModelChanged));
                this.ViewModel.InitializeAsync(); // Async call purposefully run in background.
                this.Logger.LogInformation("Loaded Page {Page}.", this.GetType().Name);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "An error occurred whilst loading page {Page}.", this.GetType().Name);
            }
        }

        private void OnViewModelChanged(string propertyName)
        {
            this.InvokeAsync(this.StateHasChanged);
        }
    }
}
