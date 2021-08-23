using System.Threading;
using System.Threading.Tasks;
using Mandarin.Inventory;
using Serilog;

namespace Mandarin.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class SquareProductSynchronizer : IProductSynchronizer
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<SquareProductService>();

        private readonly ISquareProductService squareProductService;
        private readonly IProductRepository productRepository;
        private readonly SemaphoreSlim semaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="SquareProductSynchronizer"/> class.
        /// </summary>
        /// <param name="squareProductService">The service to fetch products from Square.</param>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        public SquareProductSynchronizer(ISquareProductService squareProductService, IProductRepository productRepository)
        {
            this.squareProductService = squareProductService;
            this.productRepository = productRepository;
            this.semaphore = new SemaphoreSlim(1);
        }

        /// <inheritdoc />
        public async Task SynchronizeProductsAsync()
        {
            var updateCount = 0;
            Log.Information("Starting Square product synchronisation.");
            await this.semaphore.WaitAsync();
            try
            {
                var squareProducts = await this.squareProductService.GetAllProductsAsync();
                foreach (var product in squareProducts)
                {
                    var existingProduct = await this.productRepository.GetProductAsync(product.ProductId);

                    if (existingProduct is null)
                    {
                        Log.Information("Inserting new product: {Product}", product);
                        await this.productRepository.SaveProductAsync(product);
                        updateCount++;
                    }
                    else if (!SquareProductSynchronizer.AreProductsEquivalent(product, existingProduct))
                    {
                        Log.Information("Updating {ProductId} to new version: {Product}", product.ProductId, product);
                        await this.productRepository.SaveProductAsync(product);
                        updateCount++;
                    }
                }
            }
            finally
            {
                Log.Information("Finished product synchronisation - Update Count: {Count}.", updateCount);
                this.semaphore.Release();
            }
        }

        private static bool AreProductsEquivalent(Product x, Product y)
        {
            return x.ProductId.Equals(y.ProductId)
                   && x.ProductCode.Equals(y.ProductCode)
                   && x.ProductName.Equals(y.ProductName)
                   && x.Description == y.Description
                   && x.UnitPrice.Equals(y.UnitPrice)
                   && x.LastUpdated.Equals(y.LastUpdated);
        }
    }
}
