using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Mandarin.App.Commands.Navigation;
using Mandarin.App.Commands.Stockists;
using Mandarin.Models.Artists;
using Mandarin.Models.Commissions;
using Mandarin.MVVM.Commands;
using Mandarin.MVVM.ViewModels;
using Mandarin.Services;
using Microsoft.Extensions.Logging;

namespace Mandarin.App.ViewModels.Stockists
{
    /// <inheritdoc cref="IStockistsEditPageViewModel" />
    internal sealed class StockistsEditPageViewModel : ViewModelBase, IStockistsEditPageViewModel
    {
        private readonly ICommissionService commissionService;
        private readonly IArtistService artistService;
        private readonly ILogger<StockistsEditPageViewModel> logger;

        private bool isEditing;
        private string stockistCode;
        private Stockist stockist;
        private IReadOnlyList<CommissionRateGroup> commissionRateGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistsEditPageViewModel"/> class.
        /// </summary>
        /// <param name="commissionService">The service for interacting with commission details.</param>
        /// <param name="artistService">The service that can receive artist details.</param>
        /// <param name="logger">The application logger.</param>
        /// <param name="redirectToStockistsIndexCommand">The command to redirect the user to the stockists index page.</param>
        public StockistsEditPageViewModel(ICommissionService commissionService,
                                          IArtistService artistService,
                                          ILogger<StockistsEditPageViewModel> logger,
                                          RedirectToStockistsIndexCommand redirectToStockistsIndexCommand)
        {
            this.commissionService = commissionService;
            this.artistService = artistService;
            this.logger = logger;

            this.CloseCommand = redirectToStockistsIndexCommand;

            this.Disposables.Add(this.StateObservable
                                     .Where(x => x == nameof(this.StockistCode))
                                     .DistinctUntilChanged()
                                     .Subscribe(_ => this.UpdateStockist()));
        }

        /// <inheritdoc/>
        public bool IsEditing
        {
            get => this.isEditing;
            set => this.RaiseAndSetPropertyChanged(ref this.isEditing, value);
        }

        /// <inheritdoc/>
        public string StockistCode
        {
            get => this.stockistCode;
            set => this.RaiseAndSetPropertyChanged(ref this.stockistCode, value);
        }

        /// <inheritdoc/>
        public Stockist Stockist
        {
            get => this.stockist;
            set => this.RaiseAndSetPropertyChanged(ref this.stockist, value);
        }

        /// <inheritdoc/>
        public IReadOnlyList<CommissionRateGroup> CommissionRateGroups
        {
            get => this.commissionRateGroups;
            private set => this.RaiseAndSetPropertyChanged(ref this.commissionRateGroups, value);
        }

        /// <inheritdoc/>
        public ICommand CloseCommand { get; }

        /// <inheritdoc/>
        protected override async Task DoInitializeAsync()
        {
            this.CommissionRateGroups = await this.commissionService.GetCommissionRateGroups();
        }

        private async void UpdateStockist()
        {
            this.logger.LogInformation("Selected Stockist Code: {Code}", this.StockistCode);

            if (!string.IsNullOrEmpty(this.StockistCode))
            {
                this.Stockist = await this.artistService.GetArtistByCodeAsync(this.StockistCode);
            }
        }
    }
}
