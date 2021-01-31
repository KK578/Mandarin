using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LazyCache;
using Mandarin.Inventory;
using Mandarin.Services.Common;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Inventory
{
    /// <summary>
    /// Decorated implementation of <see cref="IProductService"/> that caches results to speed up subsequent requests.
    /// </summary>
    internal sealed class CachingProductServiceDecorator : CachingDecoratorBase, IQueryableProductService
    {
        private readonly IProductService productService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingProductServiceDecorator"/> class.
        /// </summary>
        /// <param name="productService">The inventory service to be decorated.</param>
        /// <param name="appCache">The application memory cache.</param>
        /// <param name="logger">The application logger.</param>
        public CachingProductServiceDecorator(IProductService productService, IAppCache appCache, ILogger<CachingProductServiceDecorator> logger)
            : base(appCache, logger)
        {
            this.productService = productService;
        }

        /// <inheritdoc/>
        public IObservable<Product> GetAllProducts()
        {
            var key = CachingProductServiceDecorator.CreateCacheKey();
            return this.GetOrAddAsync(key, () => this.productService.GetAllProducts().ToList().ToTask<IEnumerable<Product>>())
                       .ToObservable()
                       .Catch((Exception ex) => Observable.Empty<IEnumerable<Product>>())
                       .SelectMany(x => x);
        }

        /// <inheritdoc/>
        public Task<Product> GetProductBySquareIdAsync(string squareId)
        {
            return this.GetAllProducts().FirstOrDefaultAsync(x => x.SquareId == squareId).ToTask();
        }

        /// <inheritdoc/>
        public Task<Product> GetProductByProductCodeAsync(string productCode)
        {
            return this.GetAllProducts().FirstOrDefaultAsync(x => x.ProductCode == productCode).ToTask();
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
                return this.GetAllProducts().FirstOrDefaultAsync(x => x.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase)).ToTask();
            }
        }

        private static string CreateCacheKey([CallerMemberName] string caller = null)
        {
            return nameof(IProductService) + "." + caller;
        }
    }
}
