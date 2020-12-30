using System;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Commissions;
using Mandarin.Services;
using Microsoft.AspNetCore.Authorization;

namespace Mandarin.Grpc
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class CommissionsGrpcService : Commissions.CommissionsBase
    {
        private readonly ICommissionService commissionService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommissionsGrpcService"/> class.
        /// </summary>
        /// <param name="commissionService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public CommissionsGrpcService(ICommissionService commissionService, IMapper mapper)
        {
            this.commissionService = commissionService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetCommissionRateGroupsResponse> GetCommissionRateGroups(GetCommissionRateGroupsRequest request, ServerCallContext context)
        {
            var commissionRateGroups = await this.commissionService.GetCommissionRateGroupsAsync();

            return new GetCommissionRateGroupsResponse
            {
                RateGroups = { this.mapper.Map<CommissionRateGroup>(commissionRateGroups) },
            };
        }

        /// <inheritdoc/>
        public override async Task<GetRecordsOfSalesResponse> GetRecordsOfSales(GetRecordsOfSalesRequest request, ServerCallContext context)
        {
            var startDate = this.mapper.Map<DateTime>(request.StartDate);
            var endDate = this.mapper.Map<DateTime>(request.EndDate);
            var recordOfSales = await this.commissionService.GetRecordOfSalesAsync(startDate, endDate);

            return new GetRecordsOfSalesResponse
            {
                Sales = { this.mapper.Map<RecordOfSales>(recordOfSales) },
            };
        }
    }
}
