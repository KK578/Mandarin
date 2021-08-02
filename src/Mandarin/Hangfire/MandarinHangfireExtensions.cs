using System;
using Hangfire;
using Hangfire.PostgreSql;
using Mandarin.Inventory;
using Mandarin.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mandarin.Hangfire
{
    /// <summary>
    /// Represents extensions on <see cref="IApplicationBuilder"/> to configure background jobs to Hangfire.
    /// </summary>
    internal static class MandarinHangfireExtensions
    {
        /// <summary>
        /// Registers the Hangfire services into the provided service container.
        /// </summary>
        /// <param name="services">Service container to add registrations to.</param>
        /// <param name="configuration">Application configuration for configuring services.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(o => o.UseSimpleAssemblyNameTypeSerializer()
                                       .UseRecommendedSerializerSettings()
                                       .UsePostgreSqlStorage(configuration.GetConnectionString("MandarinConnection")));
            services.AddHangfireServer(o => o.WorkerCount = 1);

            return services;
        }

        /// <summary>
        /// Configures the Hangfire Server with registered background jobs for Mandarin services.
        /// </summary>
        /// <param name="app">The application builder instance.</param>
        public static void UseMandarinHangfire(this IApplicationBuilder app)
        {
            app.UseEndpoints(e => e.MapHangfireDashboardWithAuthorizationPolicy("Hangfire"));
            RecurringJob.AddOrUpdate<ITransactionSynchronizer>("Transaction Refresh - Daily",
                                                               s => s.LoadSquareOrders(DateTime.UtcNow.Date.AddDays(-1), DateTime.UtcNow.Date),
                                                               Cron.Daily(01));
            RecurringJob.AddOrUpdate<ITransactionSynchronizer>("Transaction Refresh - Monthly",
                                                               s => s.LoadSquareOrders(DateTime.UtcNow.Date.AddDays(-45), DateTime.UtcNow.Date),
                                                               Cron.Monthly(01));
            RecurringJob.AddOrUpdate<IProductSynchronizer>(s => s.SynchronizeProductsAsync(), Cron.Hourly);
        }
    }
}
