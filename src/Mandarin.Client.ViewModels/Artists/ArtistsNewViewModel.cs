using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Bashi.Core.Utils;
using FluentValidation;
using FluentValidation.Results;
using Mandarin.Commissions;
using Mandarin.Common;
using Mandarin.Stockists;
using Microsoft.AspNetCore.Components;
using NodaTime;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Artists
{
    /// <inheritdoc cref="IArtistsNewViewModel" />
    internal sealed class ArtistsNewViewModel : ReactiveObject, IArtistsNewViewModel
    {
        private readonly IStockistService stockistService;
        private readonly NavigationManager navigationManager;
        private readonly IValidator<IArtistViewModel> validator;
        private readonly IClock clock;

        private ValidationResult validationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistsNewViewModel"/> class.
        /// </summary>
        /// <param name="stockistService">The application service for interacting with stockists.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        /// <param name="validator">The validator for the Stockist to ensure it can be saved.</param>
        /// <param name="clock">The application clock instance.</param>
        public ArtistsNewViewModel(IStockistService stockistService, NavigationManager navigationManager, IValidator<IArtistViewModel> validator, IClock clock)
        {
            this.stockistService = stockistService;
            this.navigationManager = navigationManager;
            this.validator = validator;
            this.clock = clock;

            this.Save = ReactiveCommand.CreateFromTask(this.OnSave);
            this.Cancel = ReactiveCommand.Create(this.OnCancel);

            var stockist = new Stockist
            {
                StatusCode = StatusMode.Active,
                Details = new StockistDetail(),
                Commission = new Commission
                {
                    Rate = 100,
                    StartDate = clock.GetCurrentInstant().InUtc().Date,
                    EndDate = clock.GetCurrentInstant().InUtc().Date.PlusDays(90),
                },
            };
            this.Stockist = new ArtistViewModel(stockist, clock);
            this.Statuses = EnumUtil.GetValues<StatusMode>().Except(new[] { StatusMode.Unknown }).AsReadOnlyList();
        }

        /// <inheritdoc />
        public IArtistViewModel Stockist { get; }

        /// <inheritdoc />
        public ValidationResult ValidationResult
        {
            get => this.validationResult;
            private set => this.RaiseAndSetIfChanged(ref this.validationResult, value);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<StatusMode> Statuses { get; }

        /// <inheritdoc />
        public ReactiveCommand<Unit, Unit> Save { get; }

        /// <inheritdoc />
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        private async Task OnSave()
        {
            this.ValidationResult = await this.validator.ValidateAsync(this.Stockist);
            if (!this.ValidationResult.IsValid)
            {
                return;
            }

            await this.stockistService.SaveStockistAsync(this.Stockist.ToStockist());
            this.navigationManager.NavigateTo($"/artists/edit/{this.Stockist.StockistCode}");
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/artists");
    }
}
