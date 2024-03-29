﻿using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Google.Type;
using NodaTime;
using NodaTime.Serialization.Protobuf;

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
            this.CreateMap<Inventory.FramePrice, Api.Inventory.FramePrice>().ReverseMap();
            this.CreateMap<Stockists.Stockist, Api.Stockists.Stockist>().ReverseMap();
            this.CreateMap<Stockists.StockistDetail, Api.Stockists.StockistDetail>().ReverseMap();

            this.CreateMap<Instant, Timestamp>().ConstructUsing(instant => instant.ToTimestamp())
                .ReverseMap().ConstructUsing(timestamp => timestamp.ToInstant());
            this.CreateMap<LocalDate, Date>().ConstructUsing(localDate => localDate.ToDate())
                .ReverseMap().ConstructUsing(date => date.ToLocalDate());
        }
    }
}
