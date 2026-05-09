using System;
using System.Data;
using Mandarin.Database.Migrations;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Mandarin.Database
{
    /// <summary>
    /// Represents a Database Context for accessing The Little Mandarin data.
    /// </summary>
    public class MandarinDbContext
    {
        private readonly NpgsqlDataSource dataSource;
        private readonly IMigrator migrator;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinDbContext"/> class.
        /// </summary>
        /// <param name="dataSource">The pre-configured Postgres data source to connect to.</param>
        /// <param name="migrator">The service for upgrading the database schema.</param>
        public MandarinDbContext(NpgsqlDataSource dataSource, IMigrator migrator)
        {
            this.dataSource = dataSource;
            this.migrator = migrator;
        }

        /// <summary>
        /// Creates a new database connection.
        /// </summary>
        /// <returns>The connection to the database.</returns>
        public IDbConnection GetConnection()
        {
            return this.dataSource.CreateConnection();
        }

        /// <summary>
        /// Runs database migration scripts to ensure the current database schema is up to date for the application.
        /// </summary>
        public void RunMigrations()
        {
            var ranNewMigrations = this.migrator.RunMigrations();
            if (ranNewMigrations)
            {
                this.RefreshConnectionAfterMigration();
                NpgsqlConnection.ClearAllPools();
            }
        }

        private void RefreshConnectionAfterMigration()
        {
            using var connection = this.GetConnection();
            if (connection is not NpgsqlConnection npgsqlConnection)
            {
                throw new InvalidOperationException("Cannot perform post-migration steps as database connection was not PostgreSQL.");
            }

            npgsqlConnection.Open();
            npgsqlConnection.ReloadTypes();
        }
    }
}
