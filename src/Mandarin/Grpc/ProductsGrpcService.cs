using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Authorization;
using static Mandarin.Api.Inventory.Products;
using Product = Mandarin.Api.Inventory.Product;

namespace Mandarin.Grpc
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class ProductsGrpcService : ProductsBase
    {
        private readonly IProductRepository productRepository;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsGrpcService"/> class.
        /// </summary>
        /// <param name="productRepository">The application repository for interacting with products.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public ProductsGrpcService(IProductRepository productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetAllProductsResponse> GetAllProducts(GetAllProductsRequest request, ServerCallContext context)
        {
            var products = await this.productRepository.GetAllAsync();
            return new GetAllProductsResponse
            {
                Products = { this.mapper.Map<IEnumerable<Product>>(products) },
            };
        }

        /// <inheritdoc/>
        public override async Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            Inventory.Product product;

            if (request.ProductId != null)
            {
                product = await this.productRepository.GetProductByIdAsync(this.mapper.Map<ProductId>(request.ProductId));
            }
            else if (request.ProductCode != null)
            {
                product = await this.productRepository.GetProductByCodeAsync(this.mapper.Map<ProductCode>(request.ProductCode));
            }
            else if (request.ProductName != null)
            {
                product = await this.productRepository.GetProductByNameAsync(this.mapper.Map<ProductName>(request.ProductName));
            }
            else
            {
                throw new ArgumentException("Cannot search for the given request.");
            }

            return new GetProductResponse
            {
                Product = this.mapper.Map<Product>(product),
            };
        }
    }
}
