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
    internal sealed class SquareInventoryService : IInventoryService
    {
        private readonly ILogger<SquareTransactionService> logger;
        private readonly ISquareClient squareClient;

        public SquareInventoryService(ILogger<SquareTransactionService> logger, ISquareClient squareClient)
        {
            this.logger = logger;
            this.squareClient = squareClient;
        }

        public IObservable<Product> GetInventory()
        {
            return Observable.Create<CatalogObject>(SubscribeToListCatalog)
                             .SelectMany(SquareInventoryService.MapToProduct);

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
                } while (response.Cursor != null);

                o.OnCompleted();
            }
        }

        private static IEnumerable<Product> MapToProduct(CatalogObject catalogObject)
        {
            var productName = catalogObject.ItemData.Name.Split(" - ").Last();
            var description = catalogObject.ItemData.Description;

            foreach (var variation in catalogObject.ItemData.Variations)
            {
                var variationName = $"{productName} ({variation.ItemVariationData.Name})";
                var productCode = variation.ItemVariationData.Sku;
                var price = variation.ItemVariationData.PriceMoney;
                var unitPrice = price?.Amount != null ? decimal.Divide(price.Amount.Value, 100) : (decimal?)null;
                yield return new Product(productCode, variationName, description, unitPrice);
            }
        }
    }
}
