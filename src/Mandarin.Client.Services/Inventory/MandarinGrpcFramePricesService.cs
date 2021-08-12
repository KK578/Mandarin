using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;
using NodaTime;
using static Mandarin.Api.Inventory.FramePrices;
using FramePrice = Mandarin.Inventory.FramePrice;

namespace Mandarin.Client.Services.Inventory
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcFramePricesService : IFramePricesService
    {
        private readonly FramePricesClient framePricesClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcFramePricesService"/> class.
        /// </summary>
        /// <param name="framePricesClient">The gRPC client to Mandarin API for FramePrices.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcFramePricesService(FramePricesClient framePricesClient, IMapper mapper)
        {
            this.framePricesClient = framePricesClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<FramePrice>> GetAllFramePricesAsync()
        {
            var request = new GetAllFramePricesRequest();
            var response = await this.framePricesClient.GetAllFramePricesAsync(request);
            return this.mapper.Map<List<FramePrice>>(response.FramePrices).AsReadOnly();
        }

        /// <inheritdoc/>
        public async Task<FramePrice> GetFramePriceAsync(ProductCode productCode, Instant transactionTime)
        {
            var request = new GetFramePriceRequest
            {
                ProductCode = this.mapper.Map<string>(productCode),
                TransactionTime = this.mapper.Map<Timestamp>(transactionTime),
            };
            var response = await this.framePricesClient.GetFramePriceAsync(request);
            return this.mapper.Map<FramePrice>(response.FramePrice);
        }

        /// <inheritdoc/>
        public async Task SaveFramePriceAsync(FramePrice commission)
        {
            var request = new SaveFramePriceRequest { FramePrice = this.mapper.Map<Api.Inventory.FramePrice>(commission) };
            await this.framePricesClient.SaveFramePriceAsync(request);
        }

        /// <inheritdoc/>
        public async Task DeleteFramePriceAsync(ProductCode productCode)
        {
            var request = new DeleteFramePriceRequest { ProductCode = this.mapper.Map<string>(productCode) };
            await this.framePricesClient.DeleteFramePriceAsync(request);
        }
    }
}
