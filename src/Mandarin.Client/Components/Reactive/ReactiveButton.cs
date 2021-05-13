using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace Mandarin.Client.Components.Reactive
{
    /// <summary>
    /// Represents a Button that wraps a <see cref="ReactiveCommandBase{TParam,TResult}"/>.
    /// </summary>
    /// <typeparam name="TParam">The type of parameter values passed in during command execution.</typeparam>
    /// <typeparam name="TResult">The type of the values that are the result of command execution.</typeparam>
    public sealed class ReactiveButton<TParam, TResult> : Button
    {
        private readonly CompositeDisposable disposables = new();

        /// <summary>
        /// Gets or sets the ReactiveCommand to associate to the Button.
        /// </summary>
        [Parameter]
        public ReactiveCommand<TParam, TResult> ReactiveCommand { get; set; }

        /// <inheritdoc/>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.Clicked = EventCallback.Factory.Create(this, () => this.ReactiveCommand.Execute().ToTask());
            this.ReactiveCommand.CanExecute
                .Zip(this.ReactiveCommand.IsExecuting)
                .Subscribe(tuple =>
                {
                    var (canExecute, isExecuting) = tuple;
                    this.Disabled = isExecuting || !canExecute;
                    this.InvokeAsync(this.StateHasChanged);
                })
                .DisposeWith(this.disposables);
            this.ReactiveCommand.IsExecuting.Subscribe(x =>
            {
                this.Loading = x;
                this.InvokeAsync(this.StateHasChanged);
            }).DisposeWith(this.disposables);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.disposables.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
