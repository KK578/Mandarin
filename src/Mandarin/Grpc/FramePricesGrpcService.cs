using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Authorization;
using static Mandarin.Api.Inventory.FramePrices;
using FramePrice = Mandarin.Api.Inventory.FramePrice;

namespace Mandarin.Grpc
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class FramePricesGrpcService : FramePricesBase
    {
        private readonly IFramePricesService framePricesService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePricesGrpcService"/> class.
        /// </summary>
        /// <param name="framePricesService">The application service for interacting with frame prices.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public FramePricesGrpcService(IFramePricesService framePricesService, IMapper mapper)
        {
            this.framePricesService = framePricesService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetAllFramePricesResponse> GetAllFramePrices(GetAllFramePricesRequest request, ServerCallContext context)
        {
            var framePrices = await this.framePricesService.GetAllFramePricesAsync();
            return new GetAllFramePricesResponse
            {
                FramePrices = { this.mapper.Map<IEnumerable<FramePrice>>(framePrices) },
            };
        }

        /// <inheritdoc/>
        public override async Task<GetFramePriceResponse> GetFramePrice(GetFramePriceRequest request, ServerCallContext context)
        {
            var framePrice = await this.framePricesService.GetFramePriceAsync(request.ProductCode);
            return new GetFramePriceResponse
            {
                FramePrice = this.mapper.Map<FramePrice>(framePrice),
            };
        }

        /// <inheritdoc/>
        public override async Task<SaveFramePriceResponse> SaveFramePrice(SaveFramePriceRequest request, ServerCallContext context)
        {
            var framePrice = this.mapper.Map<Inventory.FramePrice>(request.FramePrice);
            await this.framePricesService.SaveFramePriceAsync(framePrice);

            return new SaveFramePriceResponse();
        }

        /// <inheritdoc/>
        public override async Task<DeleteFramePriceResponse> DeleteFramePrice(DeleteFramePriceRequest request, ServerCallContext context)
        {
            await this.framePricesService.DeleteFramePriceAsync(request.ProductCode);
            return new DeleteFramePriceResponse();
        }
    }
}
