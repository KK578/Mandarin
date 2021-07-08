using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bashi.Core.Extensions;
using Mandarin.Client.ViewModels.Shared;
using Mandarin.Stockists;
using Microsoft.AspNetCore.Components;

namespace Mandarin.Client.ViewModels.Artists
{
    /// <inheritdoc cref="IArtistsIndexViewModel" />
    internal sealed class ArtistsIndexViewModel : IndexPageViewModelBase<IArtistViewModel>, IArtistsIndexViewModel
    {
        private readonly IStockistService stockistService;
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistsIndexViewModel"/> class.
        /// </summary>
        /// <param name="stockistService">The application service for interacting with stockists.</param>
        /// <param name="navigationManager">The service for querying and changing the current URL.</param>
        public ArtistsIndexViewModel(IStockistService stockistService, NavigationManager navigationManager)
        {
            this.stockistService = stockistService;
            this.navigationManager = navigationManager;
        }

        /// <inheritdoc/>
        protected override async Task<IReadOnlyCollection<IArtistViewModel>> OnLoadData()
        {
            var stockists = await this.stockistService.GetStockistsAsync();
            return stockists.Select(CreateViewModel).AsReadOnlyList();

            static IArtistViewModel CreateViewModel(Stockist stockist)
            {
                return new ArtistViewModel(stockist);
            }
        }

        /// <inheritdoc/>
        protected override void OnCreateNew() => this.navigationManager.NavigateTo("/artists/new");

        /// <inheritdoc/>
        protected override void OnEditSelected() => this.navigationManager.NavigateTo($"/artists/edit/{this.SelectedRow.StockistCode}");
    }
}
