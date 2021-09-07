using System;
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
        /// <summary>
        /// Gets or sets the base address for the gRPC channel.
        /// </summary>
        private readonly Uri baseAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinClientModule"/> class.
        /// </summary>
        /// <param name="baseAddress">The base address for the gRPC channel.</param>
        public MandarinClientModule(Uri baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule(new MandarinClientServicesModule(this.baseAddress));
            builder.RegisterModule<MandarinClientViewModelsModule>();
            builder.RegisterModule<MandarinInterfacesModule>();
            builder.RegisterModule<MandarinInterfacesGrpcModule>();
        }
    }
}
