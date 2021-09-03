using Autofac;
using Mandarin.Client.Services;
using Mandarin.Client.ViewModels;
using Mandarin.Grpc;

namespace Mandarin.Client
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all services to run the Mandarin.Client app.
    /// </summary>
    internal sealed class MandarinClientModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule<MandarinClientServicesModule>();
            builder.RegisterModule<MandarinClientViewModelsModule>();
            builder.RegisterModule<MandarinInterfacesModule>();
            builder.RegisterModule<MandarinInterfacesGrpcModule>();
        }
    }
}
