using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Mandarin.Converters
{
    /// <summary>
    /// Configuration Profile for <see cref="Mapper"/> to convert Mandarin Domain types.
    /// </summary>
    public class MandarinMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinMapperProfile"/> class.
        /// </summary>
        public MandarinMapperProfile()
        {
            this.CreateMap<Models.Common.StatusMode, Api.StatusMode>();
            this.CreateMap<Models.Artists.Stockist, Api.Stockist>().ReverseMap();
            this.CreateMap<Models.Artists.StockistDetail, Api.StockistDetail>().ReverseMap();
            this.CreateMap<Models.Commissions.Commission, Api.Commission>()
                .ForMember(x => x.Rate, o => o.MapFrom(src => src.RateGroup.Rate))
                .ReverseMap()
                .ForMember(x => x.RateGroup,
                           o => o.MapFrom(src => new Models.Commissions.CommissionRateGroup { Rate = src.Rate }));
            this.CreateMap<DateTime, Timestamp>()
                .ConstructUsing(d => Timestamp.FromDateTime(d.ToUniversalTime()))
                .ReverseMap()
                .ConstructUsing(t => t.ToDateTime());
        }
    }
}
