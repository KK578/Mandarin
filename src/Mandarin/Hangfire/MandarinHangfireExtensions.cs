using System;
using Hangfire;
using Mandarin.Inventory;
using Mandarin.Transactions;
using Microsoft.AspNetCore.Builder;

namespace Mandarin.Hangfire
{
    /// <summary>
    /// Represents extensions on <see cref="IApplicationBuilder"/> to configure background jobs to Hangfire.
    /// </summary>
    internal static class MandarinHangfireExtensions
    {
        /// <summary>
        /// Adds all Mandarin background jobs.
        /// </summary>
        /// <param name="builder">The application builder instance.</param>
        public static void AddMandarinBackgroundJobs(this IApplicationBuilder builder)
        {
            RecurringJob.AddOrUpdate<ITransactionSynchronizer>(s => s.LoadSquareOrders(DateTime.Today.AddDays(-1), DateTime.Today), Cron.Daily(01));
            RecurringJob.AddOrUpdate<ITransactionSynchronizer>(s => s.LoadSquareOrders(DateTime.Today.AddDays(-45), DateTime.Today), Cron.Monthly(01));
            RecurringJob.AddOrUpdate<IProductSynchronizer>(s => s.SynchroniseProductsAsync(), Cron.Hourly);
        }
    }
}
