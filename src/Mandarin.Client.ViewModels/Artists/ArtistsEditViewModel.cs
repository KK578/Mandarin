using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Bashi.Core.Utils;
using FluentValidation;
using FluentValidation.Results;
using Mandarin.Common;
using Mandarin.Stockists;
using Microsoft.AspNetCore.Components;
using ReactiveUI;

namespace Mandarin.Client.ViewModels.Artists
{
    /// <inheritdoc cref="IArtistsEditViewModel" />
    internal sealed class ArtistsEditViewModel : ReactiveObject, IArtistsEditViewModel
    {
        private readonly IStockistService stockistService;
        private readonly NavigationManager navigationManager;
        private readonly IValidator<IArtistViewModel> validator;
        private readonly ObservableAsPropertyHelper<bool> isLoading;

        private IArtistViewModel stockist;
        private ValidationResult validationResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistsEditViewModel"/> class.
        /// </summary>
        /// <param name="stockistService">The application service for interacting with stockists.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        /// <param name="validator">The validator for the Stockist to ensure it can be saved.</param>
        public ArtistsEditViewModel(IStockistService stockistService, NavigationManager navigationManager, IValidator<IArtistViewModel> validator)
        {
            this.stockistService = stockistService;
            this.navigationManager = navigationManager;
            this.validator = validator;

            this.LoadData = ReactiveCommand.CreateFromTask<string>(this.OnLoadData);
            this.Save = ReactiveCommand.CreateFromTask(this.OnSave);
            this.Cancel = ReactiveCommand.Create(this.OnCancel);

            this.isLoading = this.LoadData.IsExecuting.ToProperty(this, x => x.IsLoading);
            this.Statuses = EnumUtil.GetValues<StatusMode>().Except(new[] { StatusMode.Unknown }).AsReadOnlyList();
        }

        /// <inheritdoc />
        public bool IsLoading => this.isLoading.Value;

        /// <inheritdoc />
        public IArtistViewModel Stockist
        {
            get => this.stockist;
            private set => this.RaiseAndSetIfChanged(ref this.stockist, value);
        }

        /// <inheritdoc />
        public ValidationResult ValidationResult
        {
            get => this.validationResult;
            private set => this.RaiseAndSetIfChanged(ref this.validationResult, value);
        }

        /// <inheritdoc />
        public IReadOnlyCollection<StatusMode> Statuses { get; }

        /// <inheritdoc />
        public ReactiveCommand<string, Unit> LoadData { get; }

        /// <inheritdoc />
        public ReactiveCommand<Unit, Unit> Save { get; }

        /// <inheritdoc />
        public ReactiveCommand<Unit, Unit> Cancel { get; }

        private async Task OnLoadData(string stockistCode)
        {
            var existingStockist = await this.stockistService.GetStockistByCodeAsync(stockistCode);
            this.Stockist = new ArtistViewModel(existingStockist);
        }

        private async Task OnSave()
        {
            this.ValidationResult = await this.validator.ValidateAsync(this.Stockist);
            if (!this.ValidationResult.IsValid)
            {
                return;
            }

            await this.stockistService.SaveStockistAsync(this.Stockist.ToStockist());
            this.navigationManager.NavigateTo("/artists");
        }

        private void OnCancel() => this.navigationManager.NavigateTo("/artists");
    }
}
