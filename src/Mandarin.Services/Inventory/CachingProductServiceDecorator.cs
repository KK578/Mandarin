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
        /// <param name="productService">The application service for interacting with products to be decorated.</param>
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
        public async Task<Product> GetProductBySquareIdAsync(ProductId squareId)
        {
            var products = await this.GetAllProductsAsync();
            return products.FirstOrDefault(x => x.ProductId == squareId);
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductByProductCodeAsync(ProductCode productCode)
        {
            var products = await this.GetAllProductsAsync();
            return products.FirstOrDefault(x => x.ProductCode == productCode);
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductByNameAsync(ProductName productName)
        {
            if (productName == null)
            {
                return null;
            }

            // TODO: Move this to a config-based lookup. And move it out of the decorator!
            if (productName.Equals(ProductName.TlmGiftCard))
            {
                return new Product
                {
                    ProductId = new ProductId("TLM-GC"),
                    ProductCode = new ProductCode("TLM-GC"),
                    ProductName = productName,
                    Description = "eGift Card",
                    UnitPrice = null,
                };
            }
            else
            {
                var products = await this.GetAllProductsAsync();
                return products.FirstOrDefault(x => x.ProductName.Value.Contains(productName.Value, StringComparison.OrdinalIgnoreCase));
            }
        }

        private static string CreateCacheKey([CallerMemberName] string caller = null)
        {
            return nameof(IProductService) + "." + caller;
        }
    }
}
