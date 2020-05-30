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
    internal sealed class CachingInventoryServiceDecorator : IQueryableInventoryService
    {
        private const string CacheKey = nameof(IInventoryService) + "." + nameof(CachingInventoryServiceDecorator.GetInventory);
        private readonly IInventoryService inventoryService;
        private readonly IAppCache appCache;

        public CachingInventoryServiceDecorator(IInventoryService inventoryService, IAppCache appCache)
        {
            this.inventoryService = inventoryService;
            this.appCache = appCache;
        }

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

        public Task<Product> GetProductByIdAsync(string squareId)
        {
            return this.GetInventory().FirstOrDefaultAsync(x => x.SquareId == squareId).ToTask();
        }

        public Task<Product> GetProductByNameAsync(string productName)
        {
            // This should be a look up from configuration. And definitely not in a caching decorator!
            if (string.Equals(productName, "eGift Card", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(new Product("TheGiftCardIdDoesNotExist", "TLM-GC", productName, "eGift Card", null));
            }
            else
            {
                return this.GetInventory().FirstOrDefaultAsync(x => x.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase)).ToTask();
            }
        }
    }
}
