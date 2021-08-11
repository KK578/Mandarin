using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Google.Type;
using Mandarin.Api.Commissions;
using Mandarin.Commissions;
using NodaTime;
using static Mandarin.Api.Commissions.Commissions;
using RecordOfSales = Mandarin.Commissions.RecordOfSales;

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
        public async Task<IReadOnlyList<RecordOfSales>> GetRecordOfSalesAsync(DateInterval interval)
        {
            var request = new GetRecordOfSalesForPeriodRequest
            {
                StartDate = this.mapper.Map<Date>(interval.Start),
                EndDate = this.mapper.Map<Date>(interval.End),
            };
            var response = await this.commissionsClient.GetRecordOfSalesForPeriodAsync(request);
            return this.mapper.Map<List<RecordOfSales>>(response.Sales);
        }
    }
}
