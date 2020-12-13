using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mandarin.App.Commands.Stockists;
using Mandarin.Models.Artists;
using Mandarin.MVVM.Commands;
using Mandarin.MVVM.ViewModels;
using Mandarin.Services;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.ViewModels.Stockists
{
    /// <inheritdoc cref="IStockistIndexPageViewModel" />
    internal sealed class StockistIndexPageViewModel : ViewModelBase, IStockistIndexPageViewModel
    {
        private readonly IArtistService artistService;

        private IReadOnlyList<Stockist> stockists;
        private Stockist selectedStockist;
        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistIndexPageViewModel"/> class.
        /// </summary>
        /// <param name="artistService">The service that can receive artist details.</param>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public StockistIndexPageViewModel(IArtistService artistService, NavigationManager navigationManager)
        {
            this.artistService = artistService;

            // TODO: Microsoft.Extensions.DependencyInjection doesn't automatically bind Lazy<T> so can't resolve the
            //       circular reference.
            this.CreateNewStockistCommand = new CreateNewStockistCommand(navigationManager);
            this.EditStockistCommand = new EditStockistCommand(this, navigationManager);
        }

        /// <inheritdoc/>
        public bool IsLoading
        {
            get => this.isLoading;
            private set => this.RaiseAndSetPropertyChanged(ref this.isLoading, value);
        }

        /// <inheritdoc/>
        public ICommand CreateNewStockistCommand { get; }

        /// <inheritdoc/>
        public ICommand EditStockistCommand { get; }

        /// <inheritdoc/>
        public IReadOnlyList<Stockist> Stockists
        {
            get => this.stockists;
            private set => this.RaiseAndSetPropertyChanged(ref this.stockists, value);
        }

        /// <inheritdoc/>
        public Stockist SelectedStockist
        {
            get => this.selectedStockist;
            set => this.RaiseAndSetPropertyChanged(ref this.selectedStockist, value);
        }

        /// <inheritdoc cref="IViewModel" />
        public override async Task InitializeAsync()
        {
            try
            {
                this.IsLoading = true;
                this.Stockists = await this.artistService.GetArtistsForCommissionAsync();
            }
            finally
            {
                this.IsLoading = false;
            }
        }
    }
}
