using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// <param name="assemblies">The assemblies where there are migration scripts to run.</param>
        /// <param name="upgradeLog">The logger instance for DbUp.</param>
        public Migrator(IConfiguration configuration, IEnumerable<Assembly> assemblies, IUpgradeLog upgradeLog)
        {
            this.upgradeEngine = DeployChanges.To.PostgresqlDatabase(configuration.GetConnectionString("MandarinConnection"))
                                              .WithScriptsEmbeddedInAssemblies(assemblies.ToArray())
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
