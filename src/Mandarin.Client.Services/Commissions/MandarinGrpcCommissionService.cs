using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Mandarin.Api.Commissions;
using Mandarin.Services;
using static Mandarin.Api.Commissions.Commissions;
using CommissionRateGroup = Mandarin.Models.Commissions.CommissionRateGroup;
using RecordOfSales = Mandarin.Models.Commissions.RecordOfSales;

namespace Mandarin.Client.Services.Commissions
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcCommissionService : ICommissionService
    {
        private readonly CommissionsClient commissionsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcCommissionService"/> class.
        /// </summary>
        /// <param name="commissionsClient">The gRPC client to Mandarin API for Commissions.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcCommissionService(CommissionsClient commissionsClient, IMapper mapper)
        {
            this.commissionsClient = commissionsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroupsAsync()
        {
            var request = new GetCommissionRateGroupsRequest();
            var response = await this.commissionsClient.GetCommissionRateGroupsAsync(request);
            return this.mapper.Map<List<CommissionRateGroup>>(response.RateGroups).AsReadOnly();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesForPeriodAsync(DateTime start, DateTime end)
        {
            var request = new GetRecordOfSalesForPeriodRequest
            {
                StartDate = this.mapper.Map<Timestamp>(start),
                EndDate = this.mapper.Map<Timestamp>(end),
            };
            var response = await this.commissionsClient.GetRecordOfSalesForPeriodAsync(request);
            return this.mapper.Map<List<RecordOfSales>>(response.Sales);
        }
    }
}
