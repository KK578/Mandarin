﻿using Mandarin.Database.Commissions;
using Mandarin.Database.Stockists;
using Microsoft.Extensions.Configuration;
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
        /// <param name="configuration">Application configuration for configuring services.</param>
        /// <returns>The service container returned as is, for chaining calls.</returns>
        public static IServiceCollection AddMandarinDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<MandarinDbContext>();

            services.AddTransient<IStockistRepository, StockistRepository>();
            services.AddTransient<ICommissionRepository, CommissionRepository>();

            return services;
        }
    }
}
