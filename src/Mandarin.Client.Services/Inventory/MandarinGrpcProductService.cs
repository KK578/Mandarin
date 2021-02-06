using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;
using static Mandarin.Api.Inventory.Products;
using Product = Mandarin.Inventory.Product;

namespace Mandarin.Client.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcProductService : IQueryableProductService
    {
        private readonly ProductsClient productsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcProductService"/> class.
        /// </summary>
        /// <param name="productsClient">The gRPC client to Mandarin API for Products.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcProductService(ProductsClient productsClient, IMapper mapper)
        {
            this.productsClient = productsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Product>> GetAllProductsAsync()
        {
            var request = new GetAllProductsRequest();
            var response = await this.productsClient.GetAllProductsAsync(request);
            return this.mapper.Map<List<Product>>(response.Products).AsReadOnly();
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductBySquareIdAsync(string squareId)
        {
            var request = new GetProductRequest { SquareId = squareId };
            var response = await this.productsClient.GetProductAsync(request);
            return this.mapper.Map<Product>(response.Product);
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductByProductCodeAsync(string productCode)
        {
            var request = new GetProductRequest { ProductCode = productCode };
            var response = await this.productsClient.GetProductAsync(request);
            return this.mapper.Map<Product>(response.Product);
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductByNameAsync(string productName)
        {
            var request = new GetProductRequest { ProductName = productName };
            var response = await this.productsClient.GetProductAsync(request);
            return this.mapper.Map<Product>(response.Product);
        }
    }
}
