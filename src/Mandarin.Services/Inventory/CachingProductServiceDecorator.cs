using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            try
            {
                var key = CachingProductServiceDecorator.CreateCacheKey();
                return await this.GetOrAddAsync(key, GetAllProducts);
            }
            catch
            {
                return new List<Product>().AsReadOnly();
            }

            async Task<IEnumerable<Product>> GetAllProducts() => await this.productService.GetAllProductsAsync();
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductBySquareIdAsync(string squareId)
        {
            var products = await this.GetAllProductsAsync();
            return products.FirstOrDefault(x => x.SquareId == squareId);
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductByProductCodeAsync(string productCode)
        {
            var products = await this.GetAllProductsAsync();
            return products.FirstOrDefault(x => x.ProductCode == productCode);
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductByNameAsync(string productName)
        {
            // TODO: Move this to a config-based lookup. And move it out of the decorator!
            if (string.Equals(productName, "eGift Card", StringComparison.OrdinalIgnoreCase))
            {
                return new Product("TLM-GC", "TLM-GC", productName, "eGift Card", null);
            }
            else
            {
                var products = await this.GetAllProductsAsync();
                return products.FirstOrDefault(x => x.ProductName.Contains(productName, StringComparison.OrdinalIgnoreCase));
            }
        }

        private static string CreateCacheKey([CallerMemberName] string caller = null)
        {
            return nameof(IProductService) + "." + caller;
        }
    }
}
