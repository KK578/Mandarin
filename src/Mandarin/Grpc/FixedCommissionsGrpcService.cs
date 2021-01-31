using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Inventory;
using Mandarin.Inventory;
using Microsoft.AspNetCore.Authorization;
using static Mandarin.Api.Inventory.FixedCommissions;
using FixedCommissionAmount = Mandarin.Api.Inventory.FixedCommissionAmount;

namespace Mandarin.Grpc
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class FixedCommissionsGrpcService : FixedCommissionsBase
    {
        private readonly IQueryableInventoryService inventoryService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedCommissionsGrpcService"/> class.
        /// </summary>
        /// <param name="inventoryService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public FixedCommissionsGrpcService(IQueryableInventoryService inventoryService, IMapper mapper)
        {
            this.inventoryService = inventoryService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetAllFixedCommissionsResponse> GetAllFixedCommissions(GetAllFixedCommissionsRequest request, ServerCallContext context)
        {
            var fixedCommissions = await this.inventoryService.GetFixedCommissionAmounts().ToList();
            return new GetAllFixedCommissionsResponse
            {
                FixedCommissions = { this.mapper.Map<IEnumerable<FixedCommissionAmount>>(fixedCommissions) },
            };
        }

        /// <inheritdoc/>
        public override async Task<GetFixedCommissionResponse> GetFixedCommission(GetFixedCommissionRequest request, ServerCallContext context)
        {
            var fixedCommission = await this.inventoryService.GetFixedCommissionAmount(request.ProductCode);
            return new GetFixedCommissionResponse
            {
                FixedCommission = this.mapper.Map<FixedCommissionAmount>(fixedCommission),
            };
        }

        /// <inheritdoc/>
        public override async Task<SaveFixedCommissionResponse> SaveFixedCommission(SaveFixedCommissionRequest request, ServerCallContext context)
        {
            var fixedCommission = this.mapper.Map<Inventory.FixedCommissionAmount>(request.FixedCommission);
            var existing = await this.inventoryService.GetFixedCommissionAmount(fixedCommission.ProductCode);
            if (existing == null)
            {
                await this.inventoryService.AddFixedCommissionAmount(fixedCommission);
            }
            else
            {
                await this.inventoryService.UpdateFixedCommissionAmount(fixedCommission);
            }

            return new SaveFixedCommissionResponse();
        }

        /// <inheritdoc/>
        public override async Task<DeleteFixedCommissionResponse> DeleteFixedCommission(DeleteFixedCommissionRequest request, ServerCallContext context)
        {
            await this.inventoryService.DeleteFixedCommissionAmount(request.ProductCode);
            return new DeleteFixedCommissionResponse();
        }
    }
}
