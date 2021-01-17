using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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
                                              .WithTransactionPerScript()
                                              .WithScriptNameComparer(new MigrationIdComparer())
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

        private class MigrationIdComparer : IComparer<string>
        {
            private static readonly Regex NumberRegex = new(@"\d{3}");

            public int Compare(string x, string y)
            {
                return (x, y) switch
                {
                    (null, null) => 0,
                    (null, _) => -1,
                    (_, null) => 1,
                    (_, _) => MigrationIdComparer.Parse(x, y),
                };
            }

            private static int Parse(string x, string y)
            {
                var xId = int.Parse(MigrationIdComparer.NumberRegex.Match(x).Value);
                var yId = int.Parse(MigrationIdComparer.NumberRegex.Match(y).Value);

                return xId - yId;
            }
        }
    }
}
