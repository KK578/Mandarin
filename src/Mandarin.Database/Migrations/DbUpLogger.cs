using System;
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
        public void LogTrace(string format, params object[] args)
        {
            DbUpLogger.Log.Verbose(format, args);
        }

        /// <inheritdoc/>
        public void LogDebug(string format, params object[] args)
        {
            DbUpLogger.Log.Debug(format, args);
        }

        /// <inheritdoc/>
        public void LogInformation(string format, params object[] args)
        {
            DbUpLogger.Log.Information(format, args);
        }

        /// <inheritdoc/>
        public void LogWarning(string format, params object[] args)
        {
            DbUpLogger.Log.Warning(format, args);
        }

        /// <inheritdoc/>
        public void LogError(string format, params object[] args)
        {
            DbUpLogger.Log.Error(format, args);
        }

        /// <inheritdoc/>
        public void LogError(Exception ex, string format, params object[] args)
        {
            DbUpLogger.Log.Error(ex, format, args);
        }
    }
}
