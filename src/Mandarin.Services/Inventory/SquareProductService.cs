﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Inventory;
using NodaTime.Text;
using Serilog;
using Square;
using Square.Models;

namespace Mandarin.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class SquareProductService : ISquareProductService
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<SquareProductService>();
        private static readonly Regex HyphenSeparatedProductNameRegex = new("^.* - (?<name>.*)$");
        private static readonly Regex SquareBracketProductNameRegex = new("^\\[.*\\] (?<name>.*)$");

        private readonly ISquareClient squareClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareProductService"/> class.
        /// </summary>
        /// <param name="squareClient">The Square API client.</param>
        public SquareProductService(ISquareClient squareClient)
        {
            this.squareClient = squareClient;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            return Observable.Create<CatalogObject>(ListFullCatalog)
                             .ToList()
                             .SelectMany(SquareProductService.MergeCatalogItems)
                             .SelectMany(SquareProductService.MapToProduct)
                             .Where(x => x != null)
                             .ToList()
                             .Select(x => (IReadOnlyList<Product>)x.ToList().AsReadOnly())
                             .ToTask();

            async Task ListFullCatalog(IObserver<CatalogObject> o, CancellationToken ct)
            {
                var requestBuilder = new SearchCatalogObjectsRequest.Builder()
                                     .ObjectTypes(new List<string> { "ITEM", "ITEM_VARIATION" })
                                     .IncludeDeletedObjects(true);
                SearchCatalogObjectsResponse response;
                do
                {
                    response = await this.squareClient.CatalogApi.SearchCatalogObjectsAsync(requestBuilder.Build(), ct);
                    ct.ThrowIfCancellationRequested();
                    requestBuilder = requestBuilder.Cursor(response.Cursor);
                    SquareProductService.Log.Debug("Loading Square Inventory - Got {Count} Items", response.Objects.Count);

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
                if (itemVariations.Count > 0)
                {
                    yield return item.ItemData.ToBuilder().Variations(itemVariations).Build();
                }
            }
        }

        private static IEnumerable<Product> MapToProduct(CatalogItem catalogItem)
        {
            if (catalogItem == null)
            {
                yield break;
            }

            var productName = SquareProductService.GetProductName(catalogItem.Name);
            var description = catalogItem.Description;

            foreach (var variation in catalogItem.Variations)
            {
                if (variation.ItemVariationData.Sku == null)
                {
                    continue;
                }

                var price = variation.ItemVariationData.PriceMoney;
                var unitPrice = price?.Amount != null ? decimal.Divide(price.Amount.Value, 100) : (decimal?)null;

                yield return new Product
                {
                    ProductId = ProductId.Of(variation.Id),
                    StockistId = null, // Populated by stored procedure.
                    ProductCode = ProductCode.Of(variation.ItemVariationData.Sku),
                    ProductName = ProductName.Of($"{productName} ({variation.ItemVariationData.Name})"),
                    Description = description,
                    UnitPrice = unitPrice,
                    LastUpdated = InstantPattern.ExtendedIso.Parse(variation.UpdatedAt).GetValueOrThrow(),
                };
            }
        }

        private static string GetProductName(string catalogItemName)
        {
            var squareBracketMatch = SquareProductService.SquareBracketProductNameRegex.Match(catalogItemName);
            if (squareBracketMatch.Success)
            {
                return squareBracketMatch.Groups["name"].ToString();
            }

            var hyphenMatch = SquareProductService.HyphenSeparatedProductNameRegex.Match(catalogItemName);
            if (hyphenMatch.Success)
            {
                return hyphenMatch.Groups["name"].ToString();
            }

            return catalogItemName;
        }
    }
}
