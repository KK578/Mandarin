using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Commissions;
using Mandarin.Services;
using Microsoft.AspNetCore.Authorization;
using static Mandarin.Api.Commissions.Commissions;

namespace Mandarin.Server.Services
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class CommissionsGrpcService : CommissionsBase
    {
        private readonly ICommissionService commissionService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionsGrpcService"/> class.
        /// </summary>
        /// <param name="commissionService">The service for interacting with commission details.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public CommissionsGrpcService(ICommissionService commissionService, IMapper mapper)
        {
            this.commissionService = commissionService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetCommissionRateGroupsResponse> GetCommissionRateGroups(GetCommissionRateGroupsRequest request, ServerCallContext context)
        {
            var rateGroups = await this.commissionService.GetCommissionRateGroups();
            return new GetCommissionRateGroupsResponse
            {
                RateGroups = { this.mapper.Map<List<CommissionRateGroup>>(rateGroups) },
            };
        }
    }
}
