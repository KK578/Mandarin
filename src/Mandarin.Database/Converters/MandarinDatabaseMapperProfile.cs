using AutoMapper;
using Mandarin.Database.Commissions;
using Mandarin.Database.Stockists;
using Mandarin.Models.Commissions;
using Mandarin.Models.Stockists;

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
            this.CreateMap<StockistDetailRecord, StockistDetail>()
                .ForMember(x => x.BannerImageUrl, o => o.MapFrom(src => src.image_url))
                .ReverseMap();

            this.CreateMap<CommissionRecord, Commission>()
                .ForMember(x => x.RateGroup, o => o.MapFrom(src => src.CommissionRateGroup))
                .ForMember(x => x.RateGroupId, o => o.MapFrom(src => src.rate_group))
                .ReverseMap()
                .ForMember(x => x.CommissionRateGroup, o => o.MapFrom(src => src.RateGroup))
                .ForMember(x => x.rate_group, o => o.MapFrom(src => src.RateGroupId));
            this.CreateMap<CommissionRateGroupRecord, CommissionRateGroup>().ReverseMap();
        }
    }
}
