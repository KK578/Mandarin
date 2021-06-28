using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;
using static Mandarin.Api.Inventory.FramePrices;
using FixedCommissionAmount = Mandarin.Inventory.FixedCommissionAmount;

namespace Mandarin.Client.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcFixedCommissionService : IFixedCommissionService
    {
        private readonly FramePricesClient framePricesClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcFixedCommissionService"/> class.
        /// </summary>
        /// <param name="framePricesClient">The gRPC client to Mandarin API for FixedCommissions.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcFixedCommissionService(FramePricesClient framePricesClient, IMapper mapper)
        {
            this.framePricesClient = framePricesClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<FixedCommissionAmount>> GetFixedCommissionAsync()
        {
            var request = new GetAllFramePricesRequest();
            var response = await this.framePricesClient.GetAllFramePricesAsync(request);
            return this.mapper.Map<List<FixedCommissionAmount>>(response.FramePrices).AsReadOnly();
        }

        /// <inheritdoc/>
        public async Task<FixedCommissionAmount> GetFixedCommissionAsync(string productCode)
        {
            var request = new GetFramePriceRequest { ProductCode = productCode };
            var response = await this.framePricesClient.GetFramePriceAsync(request);
            return this.mapper.Map<FixedCommissionAmount>(response.FramePrice);
        }

        /// <inheritdoc/>
        public async Task SaveFixedCommissionAsync(FixedCommissionAmount commission)
        {
            var request = new SaveFramePriceRequest { FramePrice = this.mapper.Map<FramePrice>(commission) };
            await this.framePricesClient.SaveFramePriceAsync(request);
        }

        /// <inheritdoc/>
        public async Task DeleteFixedCommissionAsync(string productCode)
        {
            var request = new DeleteFramePriceRequest { ProductCode = productCode };
            await this.framePricesClient.DeleteFramePriceAsync(request);
        }
    }
}
