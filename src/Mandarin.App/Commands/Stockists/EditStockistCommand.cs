using System;
using System.Threading.Tasks;
using Mandarin.App.ViewModels.Stockists;
using Mandarin.Models.Artists;
using Mandarin.MVVM.Commands;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Stockists
{
    /// <summary>
    /// Represents a command to direct the user to create a new <see cref="Stockist"/>.
    /// </summary>
    internal sealed class EditStockistCommand : CommandBase
    {
        private readonly IStockistIndexPageViewModel stockistIndexPageViewModel;
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditStockistCommand"/> class.
        /// </summary>
        /// <param name="stockistIndexPageViewModel">The ViewModel for the Index page.</param>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public EditStockistCommand(IStockistIndexPageViewModel stockistIndexPageViewModel, NavigationManager navigationManager)
        {
            this.stockistIndexPageViewModel = stockistIndexPageViewModel;
            this.navigationManager = navigationManager;

            this.Disposables.Add(stockistIndexPageViewModel.StateObservable.Subscribe(this.UpdateCanExecute));
        }

        /// <inheritdoc/>
        public override bool CanExecute => this.stockistIndexPageViewModel.SelectedStockist != null;

        /// <inheritdoc/>
        public override Task ExecuteAsync()
        {
            var selectedStockist = this.stockistIndexPageViewModel.SelectedStockist;
            this.navigationManager.NavigateTo($"/stockists/edit/{selectedStockist.StockistCode}");
            return Task.CompletedTask;
        }
    }
}
