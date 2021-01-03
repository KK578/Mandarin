﻿using System;
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
            this.CreateMap<Models.Common.StatusMode, Api.Common.StatusMode>().ReverseMap();
            this.CreateMap<Models.Commissions.CommissionRateGroup, Api.Commissions.CommissionRateGroup>().ReverseMap();
            this.CreateMap<Models.Commissions.Commission, Api.Commissions.Commission>().ReverseMap();
            this.CreateMap<Models.Commissions.RecordOfSales, Api.Commissions.RecordOfSales>().ReverseMap();
            this.CreateMap<Models.Commissions.Sale, Api.Commissions.Sale>().ReverseMap();
            this.CreateMap<Models.Stockists.Stockist, Api.Stockists.Stockist>().ReverseMap();
            this.CreateMap<Models.Stockists.StockistDetail, Api.Stockists.StockistDetail>().ReverseMap();

            this.CreateMap<DateTime, Timestamp>().ConstructUsing(d => Timestamp.FromDateTime(DateTime.SpecifyKind(d, DateTimeKind.Utc)))
                .ReverseMap().ConstructUsing(t => t.ToDateTime());
        }
    }
}
