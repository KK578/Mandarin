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
            this.CreateMap<int, CommissionId>().ConstructUsing(x => new CommissionId(x))
                .ReverseMap().ConstructUsing(x => x.Value);

            this.CreateMap<string, ProductId>().ConstructUsing(x => new ProductId(x))
                .ReverseMap().ConstructUsing(x => x.Value);
            this.CreateMap<string, ProductCode>().ConstructUsing(x => new ProductCode(x))
                .ReverseMap().ConstructUsing(x => x.Value);
            this.CreateMap<string, ProductName>().ConstructUsing(x => new ProductName(x))
                .ReverseMap().ConstructUsing(x => x.Value);

            this.CreateMap<int, StockistId>().ConstructUsing(x => new StockistId(x))
                .ReverseMap().ConstructUsing(x => x.Value);
            this.CreateMap<string, StockistCode>().ConstructUsing(x => new StockistCode(x))
                .ReverseMap().ConstructUsing(x => x.Value);
        }
    }
}
