using System.Net.Http;
using Autofac;
using Mandarin.Emails;
using Mandarin.Extensions;
using Mandarin.Inventory;
using Mandarin.Services.Emails;
using Mandarin.Services.Inventory;
using Mandarin.Services.Stockists;
using Mandarin.Services.Transactions.External;
using Mandarin.Stockists;
using Mandarin.Transactions.External;
using Microsoft.Extensions.Configuration;
using SendGrid;
using Square;
using Environment = Square.Environment;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Mandarin.Services
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all Mandarin.Services services.
    /// </summary>
    public sealed class MandarinServicesModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterConfiguration<SendGridClientOptions>("SendGrid");
            builder.RegisterConfiguration<SendGridConfiguration>("SendGrid");
            builder.RegisterType<SendGridEmailService>().As<IEmailService>().InstancePerDependency();
            builder.Register(MandarinServicesModule.ConfigureSendGridClient).As<ISendGridClient>().SingleInstance();

            builder.RegisterType<SquareProductSynchronizer>().As<IProductSynchronizer>().SingleInstance();
            builder.RegisterType<SquareProductService>().As<ISquareProductService>().InstancePerDependency();
            builder.RegisterType<FramePricesService>().As<IFramePricesService>().InstancePerDependency();

            builder.RegisterType<StockistService>().As<IStockistService>().InstancePerDependency();

            builder.RegisterType<SquareTransactionMapper>().As<ISquareTransactionMapper>().InstancePerDependency();
            builder.RegisterType<SquareTransactionService>().As<ISquareTransactionService>().InstancePerDependency();
            builder.RegisterType<SquareTransactionSynchronizer>().As<ITransactionSynchronizer>().SingleInstance();

            builder.Register(MandarinServicesModule.ConfigureSquareClient).As<ISquareClient>().InstancePerDependency();
        }

        private static ISendGridClient ConfigureSendGridClient(IComponentContext context)
        {
            var httpClient = context.Resolve<HttpClient>();
            var configuration = context.Resolve<SendGridClientOptions>();

            return new SendGridClient(httpClient, configuration);
        }

        private static ISquareClient ConfigureSquareClient(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            return new SquareClient.Builder()
                   .CustomUrl(configuration.GetValue<string>("Square:Host"))
                   .Environment(configuration.GetValue<Environment>("Square:Environment"))
                   .AccessToken(configuration.GetValue<string>("Square:ApiKey"))
                   .Build();
        }
    }
}
