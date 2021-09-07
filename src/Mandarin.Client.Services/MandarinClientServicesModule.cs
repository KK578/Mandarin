using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Mandarin.Client.Services.Authentication;
using Mandarin.Client.Services.Commissions;
using Mandarin.Client.Services.Emails;
using Mandarin.Client.Services.Inventory;
using Mandarin.Client.Services.Stockists;
using Mandarin.Client.Services.Transactions;
using Mandarin.Commissions;
using Mandarin.Emails;
using Mandarin.Inventory;
using Mandarin.Stockists;
using Mandarin.Transactions.External;
using Microsoft.Extensions.Http;
using static Mandarin.Api.Commissions.Commissions;
using static Mandarin.Api.Emails.Emails;
using static Mandarin.Api.Inventory.FramePrices;
using static Mandarin.Api.Inventory.Products;
using static Mandarin.Api.Stockists.Stockists;
using static Mandarin.Api.Transactions.Transactions;

namespace Mandarin.Client.Services
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all Mandarin.Client.Services services.
    /// </summary>
    public sealed class MandarinClientServicesModule : Module
    {
        /// <summary>
        /// Gets or sets the base address for the gRPC channel.
        /// </summary>
        private readonly Uri baseAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinClientServicesModule"/> class.
        /// </summary>
        /// <param name="baseAddress">The base address for the gRPC channel.</param>
        public MandarinClientServicesModule(Uri baseAddress)
        {
            this.baseAddress = baseAddress;
        }

        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(this.ConfigureChannel).As<ChannelBase>().SingleInstance();
            builder.RegisterType<HttpClientHandler>().As<HttpMessageHandler>().InstancePerDependency();
            builder.Register(_ => new GrpcWebHandler(GrpcWebMode.GrpcWebText)).As<DelegatingHandler>().InstancePerLifetimeScope();
            builder.RegisterType<JwtHttpMessageHandler>().InstancePerLifetimeScope().As<DelegatingHandler>().InstancePerLifetimeScope();

            builder.RegisterType<CommissionsClient>().As<CommissionsClient>().InstancePerDependency();
            builder.RegisterType<EmailsClient>().As<EmailsClient>().InstancePerDependency();
            builder.RegisterType<FramePricesClient>().As<FramePricesClient>().InstancePerDependency();
            builder.RegisterType<ProductsClient>().As<ProductsClient>().InstancePerDependency();
            builder.RegisterType<StockistsClient>().As<StockistsClient>().InstancePerDependency();
            builder.RegisterType<TransactionsClient>().As<TransactionsClient>().InstancePerDependency();

            builder.RegisterType<MandarinGrpcEmailService>().As<IEmailService>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcFramePricesService>().As<IFramePricesService>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcProductRepository>().As<IProductRepository>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcProductSynchronizer>().As<IProductSynchronizer>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcRecordOfSalesRepository>().As<IRecordOfSalesRepository>().InstancePerDependency();
            builder.RegisterType<MandarinStockistGrpcService>().As<IStockistService>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcTransactionSynchronizer>().As<ITransactionSynchronizer>().InstancePerDependency();
        }

        private GrpcChannel ConfigureChannel(IComponentContext context)
        {
            var builder = context.Resolve<HttpMessageHandlerBuilder>();
            builder.PrimaryHandler = context.Resolve<HttpMessageHandler>();
            foreach (var handler in context.Resolve<IEnumerable<DelegatingHandler>>())
            {
                builder.AdditionalHandlers.Add(handler);
            }

            var options = new GrpcChannelOptions { HttpHandler = builder.Build() };
            return GrpcChannel.ForAddress(this.baseAddress, options);
        }
    }
}
