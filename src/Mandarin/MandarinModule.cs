using System.Collections.Generic;
using Autofac;
using AutoMapper;
using Mandarin.Configuration;
using Mandarin.Extensions;

namespace Mandarin
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all Mandarin services.
    /// </summary>
    internal sealed class MandarinModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterConfiguration<MandarinConfiguration>("Mandarin");
            builder.Register(MandarinModule.ConfigureAutoMapper).SingleInstance();
        }

        private static IMapper ConfigureAutoMapper(IComponentContext context)
        {
            var profiles = context.Resolve<IEnumerable<Profile>>();
            return new Mapper(new MapperConfiguration(config => config.AddProfiles(profiles)));
        }
    }
}
