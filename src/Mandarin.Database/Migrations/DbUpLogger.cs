using DbUp.Engine;
using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;

namespace Mandarin.Database.Migrations
{
    /// <summary>
    /// Redirects the <see cref="IUpgradeLog"/> log entries to a <see cref="ILogger{TCategoryName}"/> instance.
    /// </summary>
    internal sealed class DbUpLogger : IUpgradeLog
    {
        private readonly ILogger<UpgradeEngine> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbUpLogger"/> class.
        /// </summary>
        /// <param name="logger">The application logger.</param>
        public DbUpLogger(ILogger<UpgradeEngine> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public void WriteInformation(string format, params object[] args)
        {
            this.logger.LogInformation(format, args);
        }

        /// <inheritdoc/>
        public void WriteError(string format, params object[] args)
        {
            this.logger.LogError(format, args);
        }

        /// <inheritdoc/>
        public void WriteWarning(string format, params object[] args)
        {
            this.logger.LogWarning(format, args);
        }
    }
}
