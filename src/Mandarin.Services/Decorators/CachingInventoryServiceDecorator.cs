using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using LazyCache;
using Mandarin.Models.Inventory;
using Microsoft.Extensions.Caching.Memory;

namespace Mandarin.Services.Decorators
{
    /// <summary>
    /// Decorated implementation of <see cref="IInventoryService"/> that caches results to speed up subsequent requests.
    /// </summary>
    internal sealed class CachingInventoryServiceDecorator : IQueryableInventoryService
    {
        private const string CacheKey = nameof(IInventoryService) + "." + nameof(CachingInventoryServiceDecorator.GetInventory);
        private readonly IInventoryService inventoryService;
        private readonly IAppCache appCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingInventoryServiceDecorator"/> class.
        /// </summary>
        /// <param name="inventoryService">The inventory service to be decorated.</param>
        /// <param name="appCache">The application memory cache.</param>
        public CachingInventoryServiceDecorator(IInventoryService inventoryService, IAppCache appCache)
        {
            this.inventoryService = inventoryService;
            this.appCache = appCache;
        }

        /// <inheritdoc/>
        public IObservable<Product> GetInventory()
        {
            return Observable.FromAsync(() => this.appCache.GetOrAddAsync(CachingInventoryServiceDecorator.CacheKey, CreateEntry))
                             .SelectMany(x => x);

            async Task<IReadOnlyList<Product>> CreateEntry(ICacheEntry e)
            {
                try
                {
                    e.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                    var result = await this.inventoryService.GetInventory().ToList().ToTask();
                    return result.ToList().AsReadOnly();
                }
                catch (Exception)
                {
                    e.AbsoluteExpiration = DateTimeOffset.MinValue;
                    return new List<Product>().AsReadOnly();
                }
            }
        }

        /// <inheritdoc/>
        public Task<Product> GetProductByIdAsync(string squareId)
        {
            return this.GetInventory().FirstOrDefaultAsync(x => x.SquareId == squareId).ToTask();
        }

        /// <inheritdoc/>
        public Task<Product> GetProductByNameAsync(string productName)
        {
            // TODO: Move this to a config-based lookup. And move it out of the decorator!
            if (string.Equals(productName, "eGift Card", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new Product("TLM-GC", "TLM-GC", productName, "eGift Card", null));
            }
            else
            {
                return this.GetInventory().FirstOrDefaultAsync(x => x.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase)).ToTask();
            }
        }
    }
}
