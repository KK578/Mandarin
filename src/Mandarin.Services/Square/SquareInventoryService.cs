using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        public IObservable<CatalogObject> GetInventory()
        {
            return Observable.Create<CatalogObject>(SubscribeToListCatalog);

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
    }
}
