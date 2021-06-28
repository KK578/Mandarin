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
    internal sealed class FixedCommissionsGrpcService : FramePricesBase
    {
        private readonly IFixedCommissionService fixedCommissionService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionsGrpcService"/> class.
        /// </summary>
        /// <param name="fixedCommissionService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public FixedCommissionsGrpcService(IFixedCommissionService fixedCommissionService, IMapper mapper)
        {
            this.fixedCommissionService = fixedCommissionService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetAllFramePricesResponse> GetAllFramePrices(GetAllFramePricesRequest request, ServerCallContext context)
        {
            var fixedCommissions = await this.fixedCommissionService.GetFixedCommissionAsync();
            return new GetAllFramePricesResponse
            {
                FramePrices = { this.mapper.Map<IEnumerable<FramePrice>>(fixedCommissions) },
            };
        }

        /// <inheritdoc/>
        public override async Task<GetFramePriceResponse> GetFramePrice(GetFramePriceRequest request, ServerCallContext context)
        {
            var fixedCommission = await this.fixedCommissionService.GetFixedCommissionAsync(request.ProductCode);
            return new GetFramePriceResponse
            {
                FramePrice = this.mapper.Map<FramePrice>(fixedCommission),
            };
        }

        /// <inheritdoc/>
        public override async Task<SaveFramePriceResponse> SaveFramePrice(SaveFramePriceRequest request, ServerCallContext context)
        {
            var fixedCommission = this.mapper.Map<Inventory.FixedCommissionAmount>(request.FramePrice);
            await this.fixedCommissionService.SaveFixedCommissionAsync(fixedCommission);

            return new SaveFramePriceResponse();
        }

        /// <inheritdoc/>
        public override async Task<DeleteFramePriceResponse> DeleteFramePrice(DeleteFramePriceRequest request, ServerCallContext context)
        {
            await this.fixedCommissionService.DeleteFixedCommissionAsync(request.ProductCode);
            return new DeleteFramePriceResponse();
        }
    }
}
