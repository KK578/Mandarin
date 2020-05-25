using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Square.Models;

namespace Mandarin.Services.Decorators
{
    internal sealed class CachingInventoryServiceDecorator : IInventoryService
    {
        private const string CacheKey = nameof(IInventoryService) + "." + nameof(CachingInventoryServiceDecorator.GetInventory);
        private readonly IInventoryService inventoryService;
        private readonly IMemoryCache memoryCache;

        public CachingInventoryServiceDecorator(IInventoryService inventoryService, IMemoryCache memoryCache)
        {
            this.inventoryService = inventoryService;
            this.memoryCache = memoryCache;
        }

        public IObservable<CatalogObject> GetInventory()
        {
            return Observable.FromAsync(() => this.memoryCache.GetOrCreateAsync(CachingInventoryServiceDecorator.CacheKey, CreateEntry))
                             .SelectMany(x => x);

            async Task<IReadOnlyList<CatalogObject>> CreateEntry(ICacheEntry e)
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
                    return new List<CatalogObject>().AsReadOnly();
                }
            }
        }
    }
}
