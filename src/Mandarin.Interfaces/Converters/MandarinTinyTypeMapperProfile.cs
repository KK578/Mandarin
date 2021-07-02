﻿using AutoMapper;
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
            this.CreateMap<int, StockistId>().ConstructUsing(x => new StockistId(x))
                .ReverseMap().ConstructUsing(x => x.Value);
        }
    }
}
