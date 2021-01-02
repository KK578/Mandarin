using System;
using DbUp;
using DbUp.Engine;
using DbUp.Engine.Output;
using Microsoft.Extensions.Configuration;

namespace Mandarin.Database.Migrations
{
    /// <inheritdoc />
    internal sealed class Migrator : IMigrator
    {
        private readonly UpgradeEngine upgradeEngine;

        /// <summary>
        /// Initializes a new instance of the <see cref="Migrator"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration for configuring services.</param>
        /// <param name="upgradeLog">The logger instance for DbUp.</param>
        public Migrator(IConfiguration configuration, IUpgradeLog upgradeLog)
        {
            var assembly = typeof(Migrator).Assembly;
            this.upgradeEngine = DeployChanges.To.PostgresqlDatabase(configuration.GetConnectionString("MandarinConnection"))
                                              .WithScriptsEmbeddedInAssembly(assembly)
                                              .LogTo(upgradeLog)
                                              .Build();
        }

        /// <inheritdoc/>
        public void RunMigrations()
        {
            var result = this.upgradeEngine.PerformUpgrade();
            if (!result.Successful)
            {
                throw new InvalidOperationException($"Failed to run database migrations ({result.ErrorScript}).", result.Error);
            }
        }
    }
}
