using AutoMapper;
using Mandarin.Commissions;
using Mandarin.Inventory;
using Mandarin.Stockists;

namespace Mandarin.Converters
{
    /// <summary>
    /// Configuration Profile for <see cref="Mapper"/> to convert primitives and Mandarin Tiny Types.
    /// </summary>
    public class MandarinTinyTypeMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinTinyTypeMapperProfile"/> class.
        /// </summary>
        public MandarinTinyTypeMapperProfile()
        {
            this.CreateMap<int, CommissionId>().ConstructUsing(x => CommissionId.Of(x))
                .ReverseMap().ConstructUsing(x => x.Value);

            this.CreateMap<string, ProductId>().ConstructUsing(x => ProductId.Of(x))
                .ReverseMap().ConstructUsing(x => x.Value);
            this.CreateMap<string, ProductCode>().ConstructUsing(x => ProductCode.Of(x))
                .ReverseMap().ConstructUsing(x => x.Value);
            this.CreateMap<string, ProductName>().ConstructUsing(x => ProductName.Of(x))
                .ReverseMap().ConstructUsing(x => x.Value);

            this.CreateMap<int, StockistId>().ConstructUsing(x => StockistId.Of(x))
                .ReverseMap().ConstructUsing(x => x.Value);
            this.CreateMap<string, StockistCode>().ConstructUsing(x => StockistCode.Of(x))
                .ReverseMap().ConstructUsing(x => x.Value);
        }
    }
}
