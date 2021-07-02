using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.Client.ViewModels.Extensions;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Shared
{
    /// <inheritdoc cref="IIndexPageViewModel{TRowViewModel}" />
    internal abstract class IndexPageViewModelBase<TRowViewModel> : ReactiveObject, IIndexPageViewModel<TRowViewModel>
        where TRowViewModel : IReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> isLoading;

        private TRowViewModel selectedRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexPageViewModelBase{TRowViewModel}"/> class.
        /// </summary>
        protected IndexPageViewModelBase()
        {
            var rows = new ObservableCollection<TRowViewModel>();
            this.Rows = new ReadOnlyObservableCollection<TRowViewModel>(rows);

            this.LoadData = ReactiveCommand.CreateFromTask(this.OnLoadData);
            this.CreateNew = ReactiveCommand.Create(this.OnCreateNew);
            this.EditSelected = ReactiveCommand.Create(this.OnEditSelected, this.WhenAnyValue(x => x.SelectedRow).Select(x => x != null));

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.LoadData.Subscribe(x => rows.Reset(x));
        }

        /// <inheritdoc />
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc />
        public ReactiveCommand<Unit, IReadOnlyCollection<TRowViewModel>> LoadData { get; }

        /// <inheritdoc />
        public ReactiveCommand<Unit, Unit> CreateNew { get; }

        /// <inheritdoc />
        public ReactiveCommand<Unit, Unit> EditSelected { get; }

        /// <inheritdoc />
        public ReadOnlyObservableCollection<TRowViewModel> Rows { get; }

        /// <inheritdoc />
        public TRowViewModel SelectedRow
        {
            get => this.selectedRow;
            set => this.RaiseAndSetIfChanged(ref this.selectedRow, value);
        }

        /// <summary>
        /// Loads all rows to be populated into <see cref="Rows"/>.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        protected abstract Task<IReadOnlyCollection<TRowViewModel>> OnLoadData();

        /// <summary>
        /// Handles the request to create a new <typeparamref name="TRowViewModel"/>.
        /// </summary>
        protected abstract void OnCreateNew();

        /// <summary>
        /// Handles the request to update the <typeparamref name="TRowViewModel"/> referenced by <see cref="SelectedRow"/>.
        /// </summary>
        protected abstract void OnEditSelected();
    }
}
