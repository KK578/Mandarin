using Autofac;
using Mandarin.Emails;
using Mandarin.Inventory;
using Mandarin.Services.Emails;
using Mandarin.Services.Inventory;
using Mandarin.Services.Stockists;
using Mandarin.Services.Transactions.External;
using Mandarin.Stockists;
using Mandarin.Transactions.External;
using Microsoft.Extensions.Configuration;
using NodaTime;
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

            builder.RegisterInstance(SystemClock.Instance).As<IClock>();

            builder.Register(MandarinServicesModule.CreateSendGridClientOptions);
            builder.Register(MandarinServicesModule.ConfigureSendGridConfiguration);
            builder.RegisterType<SendGridEmailService>().As<IEmailService>().InstancePerDependency();

            builder.RegisterType<SquareProductSynchronizer>().As<IProductSynchronizer>().SingleInstance();
            builder.RegisterType<SquareProductService>().As<ISquareProductService>().InstancePerDependency();
            builder.RegisterType<FramePricesService>().As<IFramePricesService>().InstancePerDependency();

            builder.RegisterType<StockistService>().As<IStockistService>().InstancePerDependency();

            builder.RegisterType<SquareTransactionMapper>().As<ISquareTransactionMapper>().InstancePerDependency();
            builder.RegisterType<SquareTransactionService>().As<ISquareTransactionService>().InstancePerDependency();
            builder.RegisterType<SquareTransactionSynchronizer>().As<ITransactionSynchronizer>().SingleInstance();

            builder.Register(MandarinServicesModule.ConfigureSquareClient).As<ISquareClient>().InstancePerDependency();
        }

        private static SendGridClientOptions CreateSendGridClientOptions(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            return new SendGridClientOptions
            {
                ApiKey = configuration.GetValue<string>("SendGrid:ApiKey"),
            };
        }

        private static SendGridConfiguration ConfigureSendGridConfiguration(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();

            return new SendGridConfiguration
            {
                ServiceEmail = configuration.GetValue<string>("SendGrid:ServiceEmail"),
                RealContactEmail = configuration.GetValue<string>("SendGrid:RealContactEmail"),
                RecordOfSalesTemplateId = configuration.GetValue<string>("SendGrid:RecordOfSalesTemplateId"),
            };
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
