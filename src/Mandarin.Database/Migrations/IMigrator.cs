namespace Mandarin.Database.Migrations
{
    /// <summary>
    /// Represents a service that can upgrade the application's database schema.
    /// </summary>
    public interface IMigrator
    {
        /// <summary>
        /// Runs database migration scripts to ensure the current database schema is up to date for the application.
        /// </summary>
        /// <returns>A boolean value representing whether or not any migrations were run.</returns>
        bool RunMigrations();
    }
}
