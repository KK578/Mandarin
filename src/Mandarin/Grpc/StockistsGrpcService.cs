using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Mandarin.Api.Stockists;
using Mandarin.Stockists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using static Mandarin.Api.Stockists.Stockists;
using Stockist = Mandarin.Api.Stockists.Stockist;

namespace Mandarin.Grpc
{
    /// <inheritdoc />
    [Authorize]
    internal sealed class StockistsGrpcService : StockistsBase
    {
        private readonly IStockistService stockistService;
        private readonly IMapper mapper;
        private readonly ILogger<StockistsGrpcService> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistsGrpcService"/> class.
        /// </summary>
        /// <param name="stockistService">The application service for interacting with stockists.</param>
        /// <param name="mapper">The mapper to translate between different object types.</param>
        /// <param name="logger">The application logger.</param>
        public StockistsGrpcService(IStockistService stockistService, IMapper mapper, ILogger<StockistsGrpcService> logger)
        {
            this.stockistService = stockistService;
            this.mapper = mapper;
            this.logger = logger;
        }

        /// <inheritdoc />
        public override async Task<GetStockistResponse> GetStockist(GetStockistRequest request, ServerCallContext context)
        {
            var stockistCode = this.mapper.Map<StockistCode>(request.StockistCode);
            var stockist = await this.stockistService.GetStockistByCodeAsync(stockistCode);

            return new GetStockistResponse
            {
                Stockist = this.mapper.Map<Stockist>(stockist),
            };
        }

        /// <inheritdoc />
        public override async Task<GetStockistsResponse> GetStockists(GetStockistsRequest request, ServerCallContext context)
        {
            var stockists = await this.stockistService.GetStockistsAsync();

            return new GetStockistsResponse
            {
                Stockists = { this.mapper.Map<List<Stockist>>(stockists) },
            };
        }

        /// <inheritdoc />
        public override async Task<SaveStockistResponse> SaveStockist(SaveStockistRequest request, ServerCallContext context)
        {
            try
            {
                var stockist = this.mapper.Map<Stockists.Stockist>(request.Stockist);
                await this.stockistService.SaveStockistAsync(stockist);

                return new SaveStockistResponse
                {
                    Successful = true,
                };
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Failed to save stockist.");
                return new SaveStockistResponse
                {
                    Successful = false,
                    Message = ex.Message,
                };
            }
        }
    }
}
