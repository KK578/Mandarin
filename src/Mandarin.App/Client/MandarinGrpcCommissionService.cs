using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api.Commissions;
using Mandarin.Models.Commissions;
using Mandarin.Services;
using static Mandarin.Api.Commissions.Commissions;
using CommissionRateGroup = Mandarin.Models.Commissions.CommissionRateGroup;

namespace Mandarin.App.Client
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcCommissionService : ICommissionService
    {
        private readonly CommissionsClient commissionsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcCommissionService"/> class.
        /// </summary>
        /// <param name="commissionsClient">The Mandarin gRPC CommissionClient.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcCommissionService(CommissionsClient commissionsClient, IMapper mapper)
        {
            this.commissionsClient = commissionsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<CommissionRateGroup>> GetCommissionRateGroups()
        {
            var request = new GetCommissionRateGroupsRequest();
            var response = await this.commissionsClient.GetCommissionRateGroupsAsync(request);
            var result = this.mapper.Map<List<CommissionRateGroup>>(response.RateGroups);
            return result.AsReadOnly();
        }

        /// <inheritdoc/>
        public IObservable<ArtistSales> GetSalesByArtistForPeriod(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }
    }
}
