using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Models.Inventory;
using Microsoft.Extensions.Logging;
using Square;
using Square.Models;

namespace Mandarin.Services.Square
{
    /// <inheritdoc />
    internal sealed class SquareInventoryService : IInventoryService
    {
        private readonly ILogger<SquareTransactionService> logger;
        private readonly ISquareClient squareClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareInventoryService"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="squareClient">The Square API client.</param>
        public SquareInventoryService(ILogger<SquareTransactionService> logger, ISquareClient squareClient)
        {
            this.logger = logger;
            this.squareClient = squareClient;
        }

        /// <inheritdoc/>
        public IObservable<Product> GetInventory()
        {
            return Observable.Create<CatalogObject>(SubscribeToListCatalog)
                             .SelectMany(MapToProducts)
                             .Where(x => x != null);

            async Task SubscribeToListCatalog(IObserver<CatalogObject> o, CancellationToken ct)
            {
                ListCatalogResponse response = null;
                do
                {
                    response = await this.squareClient.CatalogApi.ListCatalogAsync(response?.Cursor, "ITEM", ct);
                    ct.ThrowIfCancellationRequested();
                    this.logger.LogDebug("Loading Square Inventory - Got {Count} Items", response.Objects.Count);
                    foreach (var item in response.Objects)
                    {
                        o.OnNext(item);
                    }
                }
                while (response.Cursor != null);

                o.OnCompleted();
            }

            static IEnumerable<Product> MapToProducts(CatalogObject catalogObject)
            {
                return catalogObject.ItemData != null
                    ? SquareInventoryService.MapToProduct(catalogObject.ItemData)
                    : Enumerable.Empty<Product>();
            }
        }

        private static IEnumerable<Product> MapToProduct(CatalogItem catalogItem)
        {
            var productName = catalogItem.Name.Split(" - ").Last();
            var description = catalogItem.Description;

            foreach (var variation in catalogItem.Variations)
            {
                var squareId = variation.Id;
                var variationName = $"{productName} ({variation.ItemVariationData.Name})";
                var productCode = variation.ItemVariationData.Sku;
                var price = variation.ItemVariationData.PriceMoney;
                var unitPrice = price?.Amount != null ? decimal.Divide(price.Amount.Value, 100) : (decimal?)null;
                yield return new Product(squareId, productCode, variationName, description, unitPrice);
            }
        }
    }
}
