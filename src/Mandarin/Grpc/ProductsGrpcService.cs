using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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
        private readonly IQueryableProductService productService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsGrpcService"/> class.
        /// </summary>
        /// <param name="productService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public ProductsGrpcService(IQueryableProductService productService, IMapper mapper)
        {
            this.productService = productService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetAllProductsResponse> GetAllProducts(GetAllProductsRequest request, ServerCallContext context)
        {
            var products = await this.productService.GetAllProductsAsync();
            return new GetAllProductsResponse
            {
                Products = { this.mapper.Map<IEnumerable<Product>>(products) },
            };
        }

        /// <inheritdoc/>
        public override async Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            Inventory.Product product;

            if (request.SquareId != null)
            {
                product = await this.productService.GetProductBySquareIdAsync(request.SquareId);
            }
            else if (request.ProductCode != null)
            {
                product = await this.productService.GetProductByProductCodeAsync(request.ProductCode);
            }
            else if (request.ProductName != null)
            {
                product = await this.productService.GetProductByNameAsync(request.ProductName);
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
