using Autofac;
using AutoMapper;
using Mandarin.Converters;

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

            builder.RegisterType<MandarinTinyTypeMapperProfile>().As<Profile>();
        }
    }
}
