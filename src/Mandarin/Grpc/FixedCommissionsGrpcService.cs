using System.Collections.Generic;
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
        public override async Task<GetAllFixedCommissionsResponse> GetAllFixedCommissions(GetAllFixedCommissionsRequest request, ServerCallContext context)
        {
            var fixedCommissions = await this.fixedCommissionService.GetFixedCommissionAsync();
            return new GetAllFixedCommissionsResponse
            {
                FixedCommissions = { this.mapper.Map<IEnumerable<FixedCommissionAmount>>(fixedCommissions) },
            };
        }

        /// <inheritdoc/>
        public override async Task<GetFixedCommissionResponse> GetFixedCommission(GetFixedCommissionRequest request, ServerCallContext context)
        {
            var fixedCommission = await this.fixedCommissionService.GetFixedCommissionAsync(request.ProductCode);
            return new GetFixedCommissionResponse
            {
                FixedCommission = this.mapper.Map<FixedCommissionAmount>(fixedCommission),
            };
        }

        /// <inheritdoc/>
        public override async Task<SaveFixedCommissionResponse> SaveFixedCommission(SaveFixedCommissionRequest request, ServerCallContext context)
        {
            var fixedCommission = this.mapper.Map<Inventory.FixedCommissionAmount>(request.FixedCommission);
            await this.fixedCommissionService.SaveFixedCommissionAsync(fixedCommission);

            return new SaveFixedCommissionResponse();
        }

        /// <inheritdoc/>
        public override async Task<DeleteFixedCommissionResponse> DeleteFixedCommission(DeleteFixedCommissionRequest request, ServerCallContext context)
        {
            await this.fixedCommissionService.DeleteFixedCommissionAsync(request.ProductCode);
            return new DeleteFixedCommissionResponse();
        }
    }
}
