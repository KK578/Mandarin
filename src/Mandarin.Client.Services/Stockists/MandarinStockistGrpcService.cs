using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Mandarin.Api.Stockists;
using Mandarin.Stockists;
using static Mandarin.Api.Stockists.Stockists;
using Stockist = Mandarin.Stockists.Stockist;

namespace Mandarin.Client.Services.Stockists
{
    /// <inheritdoc />
    internal sealed class MandarinStockistGrpcService : IStockistService
    {
        private readonly StockistsClient stockistsClient;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinStockistGrpcService"/> class.
        /// </summary>
        /// <param name="stockistsClient">The gRPC client to Mandarin API for Stockists.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        public MandarinStockistGrpcService(StockistsClient stockistsClient, IMapper mapper)
        {
            this.stockistsClient = stockistsClient;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<Stockist> GetStockistByCodeAsync(StockistCode stockistCode)
        {
            var request = new GetStockistRequest { StockistCode = stockistCode.Value };
            var response = await this.stockistsClient.GetStockistAsync(request);
            return this.mapper.Map<Stockist>(response.Stockist);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<Stockist>> GetStockistsAsync()
        {
            var request = new GetStockistsRequest();
            var response = await this.stockistsClient.GetStockistsAsync(request);
            return this.mapper.Map<List<Stockist>>(response.Stockists).AsReadOnly();
        }

        /// <inheritdoc/>
        public async Task SaveStockistAsync(Stockist stockist)
        {
            var request = new SaveStockistRequest { Stockist = this.mapper.Map<Api.Stockists.Stockist>(stockist) };
            var response = await this.stockistsClient.SaveStockistAsync(request);

            if (!response.Successful)
            {
                throw new Exception(response.Message);
            }
        }
    }
}
