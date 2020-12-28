using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api.Stockists;
using Mandarin.Services;
using static Mandarin.Api.Stockists.Stockists;
using Stockist = Mandarin.Models.Artists.Stockist;

namespace Mandarin.App.Client
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcArtistService : IArtistService
    {
        private readonly StockistsClient stockistsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcArtistService"/> class.
        /// </summary>
        /// <param name="stockistsClient">The Mandarin gRPC StockistsClient.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcArtistService(StockistsClient stockistsClient, IMapper mapper)
        {
            this.stockistsClient = stockistsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetArtistByCodeAsync(string stockistCode)
        {
            var request = new GetStockistRequest { StockistCode = stockistCode };
            var response = await this.stockistsClient.GetStockistAsync(request);
            return this.mapper.Map<Stockist>(response.Stockist);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetArtistsForCommissionAsync()
        {
            var request = new GetStockistsRequest();
            var response = await this.stockistsClient.GetStockistsAsync(request);
            var result = this.mapper.Map<List<Stockist>>(response.Stockists);
            return result.AsReadOnly();
        }

        /// <inheritdoc/>
        public async Task SaveArtistAsync(Stockist stockist)
        {
            var request = new SaveStockistRequest { Stockist = this.mapper.Map<Api.Stockists.Stockist>(stockist) };
            await this.stockistsClient.SaveStockistAsync(request);
        }
    }
}
