using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;
using static Mandarin.Api.Inventory.FixedCommissions;
using static Mandarin.Api.Inventory.Products;
using FixedCommissionAmount = Mandarin.Inventory.FixedCommissionAmount;
using Product = Mandarin.Inventory.Product;

namespace Mandarin.Client.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcInventoryService : IInventoryService
    {
        private readonly FixedCommissionsClient fixedCommissionsClient;
        private readonly ProductsClient productsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcInventoryService"/> class.
        /// </summary>
        /// <param name="fixedCommissionsClient">The gRPC client to Mandarin API for FixedCommissions.</param>
        /// <param name="productsClient">The gRPC client to Mandarin API for Products.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcInventoryService(FixedCommissionsClient fixedCommissionsClient, ProductsClient productsClient, IMapper mapper)
        {
            this.fixedCommissionsClient = fixedCommissionsClient;
            this.productsClient = productsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task AddFixedCommissionAmount(FixedCommissionAmount commission)
        {
            var request = new SaveFixedCommissionRequest { FixedCommission = this.mapper.Map<Api.Inventory.FixedCommissionAmount>(commission) };
            await this.fixedCommissionsClient.SaveFixedCommissionAsync(request);
        }

        /// <inheritdoc/>
        public async Task UpdateFixedCommissionAmount(FixedCommissionAmount commission)
        {
            var request = new SaveFixedCommissionRequest { FixedCommission = this.mapper.Map<Api.Inventory.FixedCommissionAmount>(commission) };
            await this.fixedCommissionsClient.SaveFixedCommissionAsync(request);
        }

        /// <inheritdoc/>
        public async Task DeleteFixedCommissionAmount(string productCode)
        {
            var request = new DeleteFixedCommissionRequest { ProductCode = productCode };
            await this.fixedCommissionsClient.DeleteFixedCommissionAsync(request);
        }

        /// <inheritdoc/>
        public IObservable<FixedCommissionAmount> GetFixedCommissionAmounts()
        {
            var request = new GetAllFixedCommissionsRequest();
            return Observable.FromAsync(() => this.fixedCommissionsClient.GetAllFixedCommissionsAsync(request).ResponseAsync)
                             .SelectMany(x => x.FixedCommissions)
                             .Select(x => this.mapper.Map<FixedCommissionAmount>(x));
        }

        /// <inheritdoc/>
        public IObservable<Product> GetInventory()
        {
            var request = new GetAllProductsRequest();
            return Observable.FromAsync(() => this.productsClient.GetAllProductsAsync(request).ResponseAsync)
                             .SelectMany(x => x.Products)
                             .Select(x => this.mapper.Map<Product>(x));
        }
    }
}
