using System.Threading.Tasks;
using Mandarin.App.Pages.Stockists;
using Mandarin.App.ViewModels.Stockists;
using Mandarin.Models.Artists;
using Mandarin.MVVM.Commands;
using Mandarin.MVVM.ViewModels;
using Mandarin.Services;
using Microsoft.AspNetCore.Components;

namespace Mandarin.App.Commands.Stockists
{
    /// <summary>
    /// Represents a command to save the newly created <see cref="Stockist"/> and redirect the user to the edit page.
    /// </summary>
    public class SaveNewStockistCommand : CommandBase
    {
        private readonly IStockistsNewPageViewModel stockistsNewPageViewModel;
        private readonly IArtistService artistService;
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveNewStockistCommand"/> class.
        /// </summary>
        /// <param name="stockistsNewPageViewModel">The <see cref="IViewModel"/> for the <see cref="StockistsNew"/> page.</param>
        /// <param name="artistService">The service that can receive artist details.</param>
        /// <param name="navigationManager">Service for getting and updating the current navigation URL.</param>
        public SaveNewStockistCommand(IStockistsNewPageViewModel stockistsNewPageViewModel, IArtistService artistService, NavigationManager navigationManager)
        {
            this.stockistsNewPageViewModel = stockistsNewPageViewModel;
            this.artistService = artistService;
            this.navigationManager = navigationManager;
        }

        /// <inheritdoc />
        public override bool CanExecute => true;

        /// <inheritdoc />
        public override async Task ExecuteAsync()
        {
            await this.artistService.SaveArtistAsync(this.stockistsNewPageViewModel.Stockist);
            var targetUri = $"/stockists/edit/{this.stockistsNewPageViewModel.Stockist.StockistCode}";
            this.navigationManager.NavigateTo(targetUri);
        }
    }
}
