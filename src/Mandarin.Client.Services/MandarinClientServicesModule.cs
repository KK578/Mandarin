using Autofac;
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

namespace Mandarin.Client.Services
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all Mandarin.Client.Services services.
    /// </summary>
    public sealed class MandarinClientServicesModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<JwtHttpMessageHandler>().InstancePerLifetimeScope();
            builder.RegisterType<MandarinGrpcEmailService>().As<IEmailService>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcFramePricesService>().As<IFramePricesService>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcProductRepository>().As<IProductRepository>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcProductSynchronizer>().As<IProductSynchronizer>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcRecordOfSalesRepository>().As<IRecordOfSalesRepository>().InstancePerDependency();
            builder.RegisterType<MandarinStockistGrpcService>().As<IStockistService>().InstancePerDependency();
            builder.RegisterType<MandarinGrpcTransactionSynchronizer>().As<ITransactionSynchronizer>().InstancePerDependency();
        }
    }
}
