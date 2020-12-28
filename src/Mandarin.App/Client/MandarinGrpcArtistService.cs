using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api;
using Mandarin.Services;
using Stockist = Mandarin.Models.Artists.Stockist;

namespace Mandarin.App.Client
{
    /// <inheritdoc />
    internal sealed class MandarinGrpcArtistService : IArtistService
    {
        private readonly Stockists.StockistsClient stockistsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcArtistService"/> class.
        /// </summary>
        /// <param name="stockistsClient">The Mandarin gRPC StockistsClient.</param>
        /// <param name="mapper">The mapping service between CLR types.</param>
        public MandarinGrpcArtistService(Stockists.StockistsClient stockistsClient, IMapper mapper)
        {
            this.stockistsClient = stockistsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public Task<Stockist> GetArtistByCodeAsync(string stockistCode)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetArtistsForCommissionAsync()
        {
            var response = await this.stockistsClient.GetStockistsAsync(new GetStockistsRequest());
            return this.mapper.Map<IReadOnlyList<Stockist>>(response.Stockists);
        }

        /// <inheritdoc/>
        public Task SaveArtistAsync(Stockist stockist)
        {
            throw new NotImplementedException();
        }
    }
}
