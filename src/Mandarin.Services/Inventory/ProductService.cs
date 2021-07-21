using System.Collections.Generic;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Inventory
{
    /// <inheritdoc cref="Mandarin.Inventory.IProductService" />
    internal sealed class ProductService : IProductService
    {
        private readonly ILogger<ProductService> logger;
        private readonly IProductRepository productRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        public ProductService(ILogger<ProductService> logger, IProductRepository productRepository)
        {
            this.logger = logger;
            this.productRepository = productRepository;
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            this.logger.LogDebug("Fetching all products.");
            return this.productRepository.GetAllAsync();
        }
    }
}
