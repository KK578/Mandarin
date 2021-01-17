using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Commissions;
using Mandarin.Commissions;
using Microsoft.AspNetCore.Authorization;
using static Mandarin.Api.Commissions.Commissions;
using RecordOfSales = Mandarin.Api.Commissions.RecordOfSales;

namespace Mandarin.Grpc
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
        /// <param name="commissionService">The application service for interacting with commissions and records of sales.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public CommissionsGrpcService(ICommissionService commissionService, IMapper mapper)
        {
            this.commissionService = commissionService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetRecordOfSalesForPeriodResponse> GetRecordOfSalesForPeriod(GetRecordOfSalesForPeriodRequest request, ServerCallContext context)
        {
            var startDate = this.mapper.Map<DateTime>(request.StartDate);
            var endDate = this.mapper.Map<DateTime>(request.EndDate);
            var recordOfSales = await this.commissionService.GetRecordOfSalesForPeriodAsync(startDate, endDate);

            return new GetRecordOfSalesForPeriodResponse
            {
                Sales = { this.mapper.Map<List<RecordOfSales>>(recordOfSales) },
            };
        }
    }
}
