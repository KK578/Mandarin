﻿using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LazyCache;
using Mandarin.Models.Commissions;
using Mandarin.Models.Inventory;
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
            return this.GetOrAddAsync(this.CreateCacheKey(), () => this.inventoryService.GetFixedCommissionAmounts().ToList().ToTask<IEnumerable<FixedCommissionAmount>>())
                       .ToObservable()
                       .SelectMany(x => x);
        }

        /// <inheritdoc/>
        public Task<FixedCommissionAmount> GetFixedCommissionAmount(Product product)
        {
            return this.GetFixedCommissionAmounts().FirstOrDefaultAsync(x => x.ProductCode == product.ProductCode).ToTask();
        }

        /// <inheritdoc/>
        public IObservable<Product> GetInventory()
        {
            return this.GetOrAddAsync(this.CreateCacheKey(), () => this.inventoryService.GetInventory().ToList().ToTask<IEnumerable<Product>>())
                       .ToObservable()
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

        private string CreateCacheKey([CallerMemberName] string caller = null)
        {
            return nameof(IInventoryService) + "." + caller;
        }

        private void ClearFixedCommissionCache()
        {
            this.appCache.Remove(this.CreateCacheKey(nameof(this.GetFixedCommissionAmounts)));
        }
    }
}
