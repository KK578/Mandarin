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
        private readonly IQueryableInventoryService inventoryService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsGrpcService"/> class.
        /// </summary>
        /// <param name="inventoryService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public ProductsGrpcService(IQueryableInventoryService inventoryService, IMapper mapper)
        {
            this.inventoryService = inventoryService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetAllProductsResponse> GetAllProducts(GetAllProductsRequest request, ServerCallContext context)
        {
            var products = await this.inventoryService.GetInventory().ToList();
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
                product = await this.inventoryService.GetProductBySquareIdAsync(request.SquareId);
            }
            else if (request.ProductCode != null)
            {
                product = await this.inventoryService.GetProductByProductCodeAsync(request.ProductCode);
            }
            else if (request.ProductName != null)
            {
                product = await this.inventoryService.GetProductByNameAsync(request.ProductName);
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
