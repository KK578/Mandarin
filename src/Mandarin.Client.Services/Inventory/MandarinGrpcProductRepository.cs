using System;
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
    internal sealed class MandarinGrpcProductRepository : IProductRepository
    {
        private readonly ProductsClient productsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcProductRepository"/> class.
        /// </summary>
        /// <param name="productsClient">The gRPC client to Mandarin API for Products.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcProductRepository(ProductsClient productsClient, IMapper mapper)
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
        public async Task<Product> GetProductAsync(ProductId squareId)
        {
            var request = new GetProductRequest { ProductId = this.mapper.Map<string>(squareId) };
            var response = await this.productsClient.GetProductAsync(request);
            return this.mapper.Map<Product>(response.Product);
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductAsync(ProductCode productCode)
        {
            var request = new GetProductRequest { ProductCode = this.mapper.Map<string>(productCode) };
            var response = await this.productsClient.GetProductAsync(request);
            return this.mapper.Map<Product>(response.Product);
        }

        /// <inheritdoc/>
        public async Task<Product> GetProductAsync(ProductName productName)
        {
            var request = new GetProductRequest { ProductName = this.mapper.Map<string>(productName) };
            var response = await this.productsClient.GetProductAsync(request);
            return this.mapper.Map<Product>(response.Product);
        }

        /// <inheritdoc />
        public Task<Product> SaveProductAsync(Product product)
        {
            throw new NotSupportedException("Mandarin API currently does not support saving new products. Please use Square instead.");
        }
    }
}
