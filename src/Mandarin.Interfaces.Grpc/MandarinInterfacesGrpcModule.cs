using Autofac;
using Mandarin.Extensions;
using Mandarin.Grpc.Converters;

namespace Mandarin.Grpc
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all Mandarin.Interfaces.Grpc services.
    /// </summary>
    public sealed class MandarinInterfacesGrpcModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterProfile<MandarinGrpcMapperProfile>();
        }
    }
}
