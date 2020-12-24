using System.Threading.Tasks;
using Bashi.Core.Utils;
using Grpc.Core;
using Mandarin.Services;
using static Mandarin.Stockists;

namespace Mandarin.Server.Services
{
    /// <inheritdoc />
    internal sealed class StockistsGrpcService : StockistsBase
    {
        private readonly IArtistService artistService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StockistsGrpcService"/> class.
        /// </summary>
        /// <param name="artistService">The service that can receive artist details.</param>
        public StockistsGrpcService(IArtistService artistService)
        {
            this.artistService = artistService;
        }

        /// <inheritdoc />
        public override async Task GetStockists(GetStockistsRequest request,
                                                IServerStreamWriter<GetStockistsResponse> responseStream,
                                                ServerCallContext context)
        {
            var artists = await this.artistService.GetArtistsForCommissionAsync();
            foreach (var artist in artists)
            {
                // TODO: AutoMapper?
                var stockist = new Stockist
                {
                    StockistId = artist.StockistId,
                    Details = new StockistDetail
                    {
                        BannerImageUrl = artist.Details.BannerImageUrl,
                    },
                    FirstName = artist.FirstName,
                    LastName = artist.LastName,
                    Status = EnumUtil.ParseWithDescription<StatusMode>(artist.StatusCode.ToString()),
                    Commissions = { },
                    StockistCode = artist.StockistCode,
                };

                await responseStream.WriteAsync(new GetStockistsResponse { Stockist = stockist });
            }
        }
    }
}
