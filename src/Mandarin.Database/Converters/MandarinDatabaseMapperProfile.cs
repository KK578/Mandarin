using AutoMapper;
using Mandarin.Commissions;
using Mandarin.Database.Commissions;
using Mandarin.Database.Stockists;
using Mandarin.Stockists;

namespace Mandarin.Database.Converters
{
    /// <summary>
    /// Configuration Profile for <see cref="Mapper"/> to convert between Mandarin Database Records and Mandarin Domain.
    /// </summary>
    public sealed class MandarinDatabaseMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinDatabaseMapperProfile"/> class.
        /// </summary>
        public MandarinDatabaseMapperProfile()
        {
            this.SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
            this.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
            this.CreateMap<StockistRecord, Stockist>()
                .ForMember(x => x.StatusCode, o => o.MapFrom(src => src.stockist_status))
                .ReverseMap()
                .ForMember(x => x.stockist_status, o => o.MapFrom(src => src.StatusCode));
            this.CreateMap<StockistDetailRecord, StockistDetail>().ReverseMap();

            this.CreateMap<CommissionRecord, Commission>().ReverseMap();
        }
    }
}
