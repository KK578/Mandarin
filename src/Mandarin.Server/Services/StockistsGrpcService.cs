using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api;
using Mandarin.Services;
using Microsoft.AspNetCore.Authorization;
using static Mandarin.Api.Stockists;

namespace Mandarin.Server.Services
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class StockistsGrpcService : StockistsBase
    {
        private readonly IArtistService artistService;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistsGrpcService"/> class.
        /// </summary>
        /// <param name="artistService">The service that can receive artist details.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public StockistsGrpcService(IArtistService artistService, IMapper mapper)
        {
            this.artistService = artistService;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public override async Task<GetStockistsResponse> GetStockists(GetStockistsRequest request, ServerCallContext context)
        {
            var artists = await this.artistService.GetArtistsForCommissionAsync();
            return new GetStockistsResponse
            {
                Stockists = { this.mapper.Map<IEnumerable<Stockist>>(artists) },
            };
        }
    }
}
