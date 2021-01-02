using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Mandarin.Database
{
    /// <summary>
    /// Represents a Database Context for accessing The Little Mandarin data.
    /// </summary>
    public class MandarinDbContext
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MandarinDbContext"/> class.
        /// </summary>
        /// <param name="configuration">Application configuration for configuring services.</param>
        public MandarinDbContext(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// Creates a new database connection.
        /// </summary>
        /// <returns>The connection to the database.</returns>
        public IDbConnection GetConnection()
        {
            return new NpgsqlConnection(this.configuration.GetConnectionString("MandarinConnection"));
        }

        /// <summary>
        /// Runs database migration scripts to ensure the current database schema is up to date for the application.
        /// </summary>
        public void RunMigrations()
        {
            // TODO: Implement migrations
        }
    }
}
