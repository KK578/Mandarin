using Hangfire;
using Mandarin.Inventory;
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
            RecurringJob.AddOrUpdate<IProductSynchronizer>(s => s.SynchroniseProductsAsync(), () => "0 * * * *");
        }
    }
}
