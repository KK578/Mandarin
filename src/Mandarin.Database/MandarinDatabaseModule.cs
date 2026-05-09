using System.Data;
using Autofac;
using Dapper;
using DbUp.Engine.Output;
using Mandarin.Commissions;
using Mandarin.Database.Commissions;
using Mandarin.Database.Converters;
using Mandarin.Database.Inventory;
using Mandarin.Database.Migrations;
using Mandarin.Database.Stockists;
using Mandarin.Database.Transactions;
using Mandarin.Database.Transactions.External;
using Mandarin.Extensions;
using Mandarin.Inventory;
using Mandarin.Stockists;
using Mandarin.Transactions;
using Mandarin.Transactions.External;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Npgsql;
using Assembly = System.Reflection.Assembly;

namespace Mandarin.Database
{
    /// <summary>
    /// Represents an Autofac <see cref="Module"/> which registers all Mandarin.Database services.
    /// </summary>
    public sealed class MandarinDatabaseModule : Module
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            SqlMapper.AddTypeMap(typeof(Instant), DbType.DateTime2);
            SqlMapper.AddTypeMap(typeof(LocalDate), DbType.Date);
            SqlMapper.AddTypeMap(typeof(LocalDateTime), DbType.DateTime);
            SqlMapper.AddTypeMap(typeof(LocalTime), DbType.Time);
            SqlMapper.AddTypeMap(typeof(OffsetDateTime), DbType.DateTimeOffset);

            builder.Register(MandarinDatabaseModule.BuildNpgsqlDataSource).SingleInstance();

            builder.RegisterInstance(this.ThisAssembly).As<Assembly>().AsSelf();
            builder.RegisterType<DbUpLogger>().As<IUpgradeLog>().InstancePerDependency();
            builder.RegisterType<Migrator>().As<IMigrator>().SingleInstance();
            builder.RegisterType<MandarinDbContext>().AsSelf().InstancePerDependency();

            builder.RegisterProfile<MandarinDatabaseMapperProfile>();
            builder.RegisterType<CommissionRepository>().As<ICommissionRepository>().InstancePerDependency();
            builder.RegisterType<ExternalTransactionRepository>().As<IExternalTransactionRepository>().InstancePerDependency();
            builder.RegisterType<FramePriceRepository>().As<IFramePriceRepository>().InstancePerDependency();
            builder.RegisterType<ProductRepository>().As<IProductRepository>().InstancePerDependency();
            builder.RegisterType<RecordOfSalesRepository>().As<IRecordOfSalesRepository>().InstancePerDependency();
            builder.RegisterType<StockistRepository>().As<IStockistRepository>().InstancePerDependency();
            builder.RegisterType<TransactionRepository>().As<ITransactionRepository>().InstancePerDependency();
        }

        private static NpgsqlDataSource BuildNpgsqlDataSource(IComponentContext context)
        {
            var configuration = context.Resolve<IConfiguration>();
            var connectionString = configuration.GetConnectionString("MandarinConnection");
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
            dataSourceBuilder.UseNodaTime();
            dataSourceBuilder.MapComposite<SubtransactionRecord>("billing.tvp_subtransaction");

            return dataSourceBuilder.Build();
        }
    }
}
