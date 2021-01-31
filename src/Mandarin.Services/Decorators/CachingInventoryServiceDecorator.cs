using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LazyCache;
using Mandarin.Commissions;
using Mandarin.Inventory;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Decorators
{
    /// <summary>
    /// Decorated implementation of <see cref="IInventoryService"/> that caches results to speed up subsequent requests.
    /// </summary>
    internal sealed class CachingInventoryServiceDecorator : CachingDecoratorBase, IQueryableInventoryService
    {
        private readonly IInventoryService inventoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingInventoryServiceDecorator"/> class.
        /// </summary>
        /// <param name="inventoryService">The inventory service to be decorated.</param>
        /// <param name="appCache">The application memory cache.</param>
        /// <param name="logger">The application logger.</param>
        public CachingInventoryServiceDecorator(IInventoryService inventoryService, IAppCache appCache, ILogger<CachingInventoryServiceDecorator> logger)
            : base(appCache, logger)
        {
            this.inventoryService = inventoryService;
        }

        /// <inheritdoc/>
        public async Task AddFixedCommissionAmount(FixedCommissionAmount commission)
        {
            await this.inventoryService.AddFixedCommissionAmount(commission);
            this.ClearFixedCommissionCache();
        }

        /// <inheritdoc/>
        public async Task UpdateFixedCommissionAmount(FixedCommissionAmount commission)
        {
            await this.inventoryService.UpdateFixedCommissionAmount(commission);
            this.ClearFixedCommissionCache();
        }

        /// <inheritdoc/>
        public async Task DeleteFixedCommissionAmount(string productCode)
        {
            await this.inventoryService.DeleteFixedCommissionAmount(productCode);
            this.ClearFixedCommissionCache();
        }

        /// <inheritdoc/>
        public IObservable<FixedCommissionAmount> GetFixedCommissionAmounts()
        {
            var key = CachingInventoryServiceDecorator.CreateCacheKey();
            return this.GetOrAddAsync(key, () => this.inventoryService.GetFixedCommissionAmounts().ToList().ToTask<IEnumerable<FixedCommissionAmount>>())
                       .ToObservable()
                       .Catch((Exception ex) => Observable.Empty<IEnumerable<FixedCommissionAmount>>())
                       .SelectMany(x => x);
        }

        /// <inheritdoc/>
        public Task<FixedCommissionAmount> GetFixedCommissionAmount(string productCode)
        {
            return this.GetFixedCommissionAmounts().FirstOrDefaultAsync(x => x.ProductCode == productCode).ToTask();
        }

        /// <inheritdoc/>
        public IObservable<Product> GetInventory()
        {
            var key = CachingInventoryServiceDecorator.CreateCacheKey();
            return this.GetOrAddAsync(key, () => this.inventoryService.GetInventory().ToList().ToTask<IEnumerable<Product>>())
                       .ToObservable()
                       .Catch((Exception ex) => Observable.Empty<IEnumerable<Product>>())
                       .SelectMany(x => x);
        }

        /// <inheritdoc/>
        public Task<Product> GetProductBySquareIdAsync(string squareId)
        {
            return this.GetInventory().FirstOrDefaultAsync(x => x.SquareId == squareId).ToTask();
        }

        /// <inheritdoc/>
        public Task<Product> GetProductByProductCodeAsync(string productCode)
        {
            return this.GetInventory().FirstOrDefaultAsync(x => x.ProductCode == productCode).ToTask();
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

        private static string CreateCacheKey([CallerMemberName] string caller = null)
        {
            return nameof(IInventoryService) + "." + caller;
        }

        private void ClearFixedCommissionCache()
        {
            this.Remove(CachingInventoryServiceDecorator.CreateCacheKey(nameof(this.GetFixedCommissionAmounts)));
        }
    }
}
