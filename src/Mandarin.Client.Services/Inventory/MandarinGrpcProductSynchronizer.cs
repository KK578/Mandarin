using System.Threading.Tasks;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;

namespace Mandarin.Client.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcProductSynchronizer : IProductSynchronizer
    {
        private readonly Products.ProductsClient productsClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcProductSynchronizer"/> class.
        /// </summary>
        /// <param name="productsClient">The gRPC client to Mandarin API for Products.</param>
        public MandarinGrpcProductSynchronizer(Products.ProductsClient productsClient)
        {
            this.productsClient = productsClient;
        }

        /// <inheritdoc />
        public async Task SynchronizeProductsAsync()
        {
            var request = new SynchronizeProductsRequest();
            await this.productsClient.SynchronizeProductsAsync(request);
        }
    }
}
