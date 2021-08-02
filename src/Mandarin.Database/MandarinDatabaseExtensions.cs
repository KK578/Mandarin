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
using Mandarin.Inventory;
using Mandarin.Stockists;
using Mandarin.Transactions;
using Mandarin.Transactions.External;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Database
{
    /// <summary>
    /// Extensions to <see cref="IServiceCollection"/> to register all services in this assembly.
    /// </summary>
    public static class MandarinDatabaseExtensions
    {
        /// <summary>
        /// Registers the Mandarin.Database assembly implementations into the provided service container.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinDatabase(this IServiceCollection services)
        {
            services.AddSingleton(typeof(Migrator).Assembly);
            services.AddTransient<IUpgradeLog, DbUpLogger>();
            services.AddSingleton<IMigrator, Migrator>();
            services.AddTransient<MandarinDbContext>();

            SqlMapper.AddTypeHandler(new DateTimeUtcHandler());

            services.AddTransient<ICommissionRepository, CommissionRepository>();
            services.AddTransient<IExternalTransactionRepository, ExternalTransactionRepository>();
            services.AddTransient<IFramePriceRepository, FramePriceRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IStockistRepository, StockistRepository>();
            services.AddTransient<ITransactionRepository, TransactionRepository>();

            return services;
        }
    }
}
