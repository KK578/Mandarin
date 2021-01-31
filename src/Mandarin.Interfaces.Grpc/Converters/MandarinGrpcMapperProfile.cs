using System;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Mandarin.Grpc.Converters
{
    /// <summary>
    /// Configuration Profile for <see cref="Mapper"/> to convert between Mandarin gRPC DTOs and Mandarin Domain.
    /// </summary>
    public class MandarinGrpcMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinGrpcMapperProfile"/> class.
        /// </summary>
        public MandarinGrpcMapperProfile()
        {
            this.CreateMap<Common.StatusMode, Api.Common.StatusMode>().ReverseMap();
            this.CreateMap<Commissions.Commission, Api.Commissions.Commission>().ReverseMap();
            this.CreateMap<Commissions.RecordOfSales, Api.Commissions.RecordOfSales>().ReverseMap();
            this.CreateMap<Commissions.Sale, Api.Commissions.Sale>().ReverseMap();
            this.CreateMap<Inventory.Product, Api.Inventory.Product>().ReverseMap();
            this.CreateMap<Inventory.FixedCommissionAmount, Api.Inventory.FixedCommissionAmount>().ReverseMap();
            this.CreateMap<Stockists.Stockist, Api.Stockists.Stockist>().ReverseMap();
            this.CreateMap<Stockists.StockistDetail, Api.Stockists.StockistDetail>().ReverseMap();

            this.CreateMap<DateTime, Timestamp>().ConstructUsing(d => Timestamp.FromDateTime(DateTime.SpecifyKind(d, DateTimeKind.Utc)))
                .ReverseMap().ConstructUsing(t => t.ToDateTime());
        }
    }
}
