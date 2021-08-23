using DbUp.Engine;
using DbUp.Engine.Output;
using Serilog;

namespace Mandarin.Database.Migrations
{
    /// <summary>
    /// Redirects the <see cref="IUpgradeLog"/> log entries to Serilog.
    /// </summary>
    internal sealed class DbUpLogger : IUpgradeLog
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<UpgradeEngine>();

        /// <inheritdoc/>
        public void WriteInformation(string format, params object[] args)
        {
            DbUpLogger.Log.Information(format, args);
        }

        /// <inheritdoc/>
        public void WriteError(string format, params object[] args)
        {
            DbUpLogger.Log.Error(format, args);
        }

        /// <inheritdoc/>
        public void WriteWarning(string format, params object[] args)
        {
            DbUpLogger.Log.Warning(format, args);
        }
    }
}
