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
            // RecurringJob.AddOrUpdate<ITransactionSynchronizer>(s => s.SynchroniseTransactionsAsync(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-2)), Cron.Daily(01));
            RecurringJob.AddOrUpdate<IProductSynchronizer>(s => s.SynchroniseProductsAsync(), Cron.Hourly);
        }

        private static void SynchroniseForExampleDate()
        {
            var startDate = new DateTime(2021, 05, 01, 00, 00, 00, DateTimeKind.Utc);
            for (var i = 0; i < 1; i++)
            {
                BackgroundJob.Enqueue<ITransactionSynchronizer>(s => s.SynchroniseTransactionsAsync(startDate, startDate.AddDays(1)));
                startDate = startDate.AddDays(1);
            }
        }
    }
}
