using System.Collections.Generic;
using Autofac;
using AutoMapper;
using Mandarin.Configuration;
using Mandarin.Converters;
using Mandarin.Extensions;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Mandarin
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all Mandarin.Interfaces services.
    /// </summary>
    public sealed class MandarinInterfacesModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterConfiguration<MandarinConfiguration>("Mandarin");
            builder.RegisterInstance(SystemClock.Instance).As<IClock>();

            builder.RegisterProfile<MandarinTinyTypeMapperProfile>();
            builder.Register(MandarinInterfacesModule.ConfigureAutoMapper).SingleInstance();
        }

        private static IMapper ConfigureAutoMapper(IComponentContext context)
        {
            var profiles = context.Resolve<IEnumerable<Profile>>();
            var loggerFactory = context.Resolve<ILoggerFactory>();
            return new Mapper(new MapperConfiguration(config => config.AddProfiles(profiles), loggerFactory));
        }
    }
}
