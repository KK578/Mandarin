using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Configuration;
using Mandarin.Models.Commissions;
using Mandarin.Models.Inventory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Square;
using Square.Models;

namespace Mandarin.Services.Square
{
    /// <inheritdoc />
    internal sealed class SquareInventoryService : IInventoryService
    {
        private readonly ILogger<SquareTransactionService> logger;
        private readonly ISquareClient squareClient;
        private readonly IOptions<MandarinConfiguration> mandarinConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareInventoryService"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="squareClient">The Square API client.</param>
        /// <param name="mandarinConfiguration">The application configuration wrapped in an <see cref="IOptions{TOptions}"/>.</param>
        public SquareInventoryService(ILogger<SquareTransactionService> logger,
                                      ISquareClient squareClient,
                                      IOptions<MandarinConfiguration> mandarinConfiguration)
        {
            this.logger = logger;
            this.squareClient = squareClient;
            this.mandarinConfiguration = mandarinConfiguration;
        }

        /// <inheritdoc />
        public Task AddFixedCommissionAmount(FixedCommissionAmount commission)
        {
            return this.CommitData(this.GetFixedCommissionAmounts().Append(commission));
        }

        /// <inheritdoc/>
        public Task UpdateFixedCommissionAmount(FixedCommissionAmount commission)
        {
            return this.CommitData(this.GetFixedCommissionAmounts().Select(x => x.ProductCode == commission.ProductCode ? commission : x));
        }

        /// <inheritdoc/>
        public Task DeleteFixedCommissionAmount(string productCode)
        {
            return this.CommitData(this.GetFixedCommissionAmounts().Where(x => x.ProductCode != productCode));
        }

        /// <inheritdoc />
        public IObservable<FixedCommissionAmount> GetFixedCommissionAmounts()
        {
            return Observable.FromAsync(ReadDataFromFile)
                             .SelectMany(JsonConvert.DeserializeObject<List<FixedCommissionAmount>>);

            Task<string> ReadDataFromFile()
            {
                var file = this.mandarinConfiguration.Value.FixedCommissionAmountFilePath;
                if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
                {
                    this.logger.LogWarning("Cannot read Fixed Commission Amounts from file: '{FileName}", file);
                    return Task.FromResult("[]");
                }

                return File.ReadAllTextAsync(file);
            }
        }

        /// <inheritdoc/>
        public IObservable<Product> GetInventory()
        {
            return Observable.Create<CatalogObject>(ListFullCatalog)
                             .ToList()
                             .SelectMany(SquareInventoryService.MergeCatalogItems)
                             .SelectMany(SquareInventoryService.MapToProduct)
                             .Where(x => x != null);

            async Task ListFullCatalog(IObserver<CatalogObject> o, CancellationToken ct)
            {
                var requestBuilder = new SearchCatalogObjectsRequest.Builder()
                                     .ObjectTypes(new List<string> { "ITEM", "ITEM_VARIATION" })
                                     .IncludeDeletedObjects(true);
                SearchCatalogObjectsResponse response = null;
                do
                {
                    response = await this.squareClient.CatalogApi.SearchCatalogObjectsAsync(requestBuilder.Build(), ct);
                    ct.ThrowIfCancellationRequested();
                    requestBuilder = requestBuilder.Cursor(response.Cursor);
                    this.logger.LogDebug("Loading Square Inventory - Got {Count} Items", response.Objects.Count);

                    foreach (var item in response.Objects)
                    {
                        o.OnNext(item);
                    }
                }
                while (response.Cursor != null);

                o.OnCompleted();
            }
        }

        private static IEnumerable<CatalogItem> MergeCatalogItems(IList<CatalogObject> catalog)
        {
            var variations = catalog.Where(x => x.Type == "ITEM_VARIATION").ToList();
            var items = catalog.Where(x => x.Type == "ITEM").ToList();

            foreach (var item in items)
            {
                var itemVariations = variations.Where(x => x.ItemVariationData.ItemId == item.Id).ToList();
                yield return item.ItemData.ToBuilder().Variations(itemVariations).Build();
            }
        }

        private static IEnumerable<Product> MapToProduct(CatalogItem catalogItem)
        {
            if (catalogItem == null)
            {
                yield break;
            }

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

        private async Task CommitData(IObservable<FixedCommissionAmount> dataObservable)
        {
            var data = await dataObservable.ToList().ToTask();
            var fs = File.Create(this.mandarinConfiguration.Value.FixedCommissionAmountFilePath);
            await using var writer = new StreamWriter(fs);
            using var jsonWriter = new JsonTextWriter(writer);

            JsonSerializer.CreateDefault().Serialize(jsonWriter, data);
        }
    }
}
