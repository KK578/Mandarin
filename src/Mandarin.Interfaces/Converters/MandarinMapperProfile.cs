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
            this.CreateMap<Models.Common.StatusMode, Api.Common.StatusMode>().ReverseMap();
            this.CreateMap<Models.Commissions.CommissionRateGroup, Api.Commissions.CommissionRateGroup>().ReverseMap();
            this.CreateMap<Models.Commissions.Commission, Api.Commissions.Commission>().ReverseMap();
            this.CreateMap<Models.Commissions.RecordOfSales, Api.Commissions.RecordOfSales>().ReverseMap();
            this.CreateMap<Models.Commissions.Sale, Api.Commissions.Sale>().ReverseMap();
            this.CreateMap<Models.Artists.Stockist, Api.Stockists.Stockist>().ReverseMap();
            this.CreateMap<Models.Artists.StockistDetail, Api.Stockists.StockistDetail>().ReverseMap();

            this.CreateMap<DateTime, Timestamp>().ConstructUsing(d => Timestamp.FromDateTime(d.ToUniversalTime()))
                .ReverseMap().ConstructUsing(t => t.ToDateTime());
        }
    }
}
