using Autofac;
using Mandarin.Converters;
using Mandarin.Extensions;

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

            builder.RegisterProfile<MandarinTinyTypeMapperProfile>();
        }
    }
}
