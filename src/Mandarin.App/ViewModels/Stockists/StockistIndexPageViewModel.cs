using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.App.Commands.Stockists;
using Mandarin.Models.Artists;
using Mandarin.MVVM.Commands;
using Mandarin.MVVM.ViewModels;
using Mandarin.Services;

namespace Mandarin.App.ViewModels.Stockists
{
    /// <inheritdoc cref="IStockistIndexPageViewModel" />
    internal sealed class StockistIndexPageViewModel : ViewModelBase, IStockistIndexPageViewModel
    {
        private readonly IArtistService artistService;
        private readonly Lazy<RedirectToStockistsEditCommand> lazyRedirectToStockistsEditCommand;

        private IReadOnlyList<Stockist> stockists;
        private Stockist selectedStockist;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistIndexPageViewModel"/> class.
        /// </summary>
        /// <param name="artistService">The service that can receive artist details.</param>
        /// <param name="redirectToStockistsNewCommand">The command to redirect the user to the create new stockist page.</param>
        /// <param name="lazyRedirectToStockistsEditCommand">The command to redirect the user to the edit stockist page.</param>
        public StockistIndexPageViewModel(IArtistService artistService,
                                          RedirectToStockistsNewCommand redirectToStockistsNewCommand,
                                          Lazy<RedirectToStockistsEditCommand> lazyRedirectToStockistsEditCommand)
        {
            this.artistService = artistService;
            this.lazyRedirectToStockistsEditCommand = lazyRedirectToStockistsEditCommand;
            this.CreateNewStockistCommand = redirectToStockistsNewCommand;
            this.Stockists = new List<Stockist>().AsReadOnly();
        }

        /// <inheritdoc/>
        public ICommand CreateNewStockistCommand { get; }

        /// <inheritdoc/>
        public ICommand EditStockistCommand => this.lazyRedirectToStockistsEditCommand.Value;

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
        protected override async Task DoInitializeAsync()
        {
            this.Stockists = await this.artistService.GetArtistsForCommissionAsync();
        }
    }
}
