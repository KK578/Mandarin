using System;
using Newtonsoft.Json;
using Serilog;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace Mandarin.Tests.Helpers.SendGrid
{
    internal sealed class WireMockSerilogLogger<T> : IWireMockLogger
    {
        // TODO: Should be static readonly. We shouldn't need to create the logger everytime.
        // Unfortunately currently due to the ordering of the fixture setup, the static instance we create will always be a NullLogger.
        private static ILogger Log => Serilog.Log.ForContext<T>();

        /// <inheritdoc />
        public void Debug(string formatString, params object[] args)
        {
            WireMockSerilogLogger<T>.Log.Debug(formatString, args);
        }

        /// <inheritdoc />
        public void Info(string formatString, params object[] args)
        {
            WireMockSerilogLogger<T>.Log.Information(formatString, args);
        }

        /// <inheritdoc />
        public void Warn(string formatString, params object[] args)
        {
            WireMockSerilogLogger<T>.Log.Warning(formatString, args);
        }

        /// <inheritdoc />
        public void Error(string formatString, params object[] args)
        {
            WireMockSerilogLogger<T>.Log.Error(formatString, args);
        }

        /// <inheritdoc />
        public void Error(string formatString, Exception exception)
        {
            WireMockSerilogLogger<T>.Log.Error(exception, formatString);
        }

        /// <inheritdoc />
        public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
        {
            var json = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented);
            WireMockSerilogLogger<T>.Log.Debug("RequestResponse: {NewLine}{LogEntry}", Environment.NewLine, json);
        }
    }
}
