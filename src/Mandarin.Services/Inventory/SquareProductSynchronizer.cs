﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Microsoft.Extensions.Logging;

namespace Mandarin.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class SquareProductSynchronizer : IProductSynchronizer
    {
        private readonly ILogger<SquareProductService> logger;
        private readonly ISquareProductService squareProductService;
        private readonly IProductRepository productRepository;
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareProductSynchronizer"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        /// <param name="squareProductService">The service to fetch products from Square.</param>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        public SquareProductSynchronizer(ILogger<SquareProductService> logger,
                                         ISquareProductService squareProductService,
                                         IProductRepository productRepository)
        {
            this.logger = logger;
            this.squareProductService = squareProductService;
            this.productRepository = productRepository;
            this.semaphore = new SemaphoreSlim(1);
        }

        /// <inheritdoc />
        public async Task SynchronizeProductsAsync()
        {
            var updateCount = 0;
            this.logger.LogInformation("Starting Square product synchronisation.");
            await this.semaphore.WaitAsync();
            try
            {
                var existingProducts = (await this.productRepository.GetAllProductsAsync()).ToDictionary(x => x.ProductId);
                var squareProducts = await this.squareProductService.GetAllProductsAsync();

                foreach (var product in squareProducts)
                {
                    if (existingProducts.TryGetValue(product.ProductId, out var existingProduct))
                    {
                        if (!product.Equals(existingProduct))
                        {
                            this.logger.LogInformation("Updating {ProductId} to new version: {Product}", product.ProductId, product);
                            await this.productRepository.SaveProductAsync(product);
                            updateCount++;
                        }
                    }
                    else
                    {
                        this.logger.LogInformation("Inserting new product: {Product}", product);
                        await this.productRepository.SaveProductAsync(product);
                        updateCount++;
                    }
                }
            }
            finally
            {
                this.logger.LogInformation("Finished product synchronisation - Update Count: {Count}.", updateCount);
                this.semaphore.Release();
            }
        }
    }
}
