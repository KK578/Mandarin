using System;
using Mandarin.App.Commands.Navigation;
using Mandarin.App.Pages.Stockists;
using Mandarin.App.ViewModels.Stockists;
using Mandarin.Models.Artists;
using Mandarin.MVVM.ViewModels;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Stockists
{
    /// <summary>
    /// Represents a command to direct the user to edit an existing <see cref="Stockist"/>.
    /// </summary>
    internal sealed class RedirectToStockistsEditCommand : RedirectToCommandBase
    {
        private readonly IStockistIndexPageViewModel stockistIndexPageViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectToStockistsEditCommand"/> class.
        /// </summary>
        /// <param name="stockistIndexPageViewModel">The <see cref="IViewModel"/> for the <see cref="StockistsIndex"/> page.</param>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public RedirectToStockistsEditCommand(IStockistIndexPageViewModel stockistIndexPageViewModel, NavigationManager navigationManager)
            : base(navigationManager)
        {
            this.stockistIndexPageViewModel = stockistIndexPageViewModel;

            this.Disposables.Add(stockistIndexPageViewModel.StateObservable.Subscribe(this.UpdateCanExecute));
        }

        /// <inheritdoc/>
        public override bool CanExecute => this.stockistIndexPageViewModel.SelectedStockist != null;

        /// <inheritdoc/>
        protected override string GetTargetUri()
        {
            var selectedStockist = this.stockistIndexPageViewModel.SelectedStockist;
            return $"/stockists/edit/{selectedStockist.StockistCode}";
        }
    }
}
