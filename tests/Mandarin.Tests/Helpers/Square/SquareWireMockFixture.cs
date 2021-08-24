using System;
using Mandarin.Tests.Helpers.Logging;
using WireMock.Server;
using WireMock.Settings;

namespace Mandarin.Tests.Helpers.Square
{
    public sealed class SquareWireMockFixture : IDisposable
    {
        public const string Host = "http://localhost:20002";

        private readonly WireMockServer server;

        public SquareWireMockFixture()
        {
            this.server = WireMockServer.Start(SquareWireMockFixture.CreateSettings());
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.server?.Dispose();
        }

        private static WireMockServerSettings CreateSettings()
        {
            var settings = new WireMockServerSettings
            {
                Urls = new[] { SquareWireMockFixture.Host },
                Logger = new WireMockSerilogLogger<SquareWireMockFixture>(),
                ReadStaticMappings = true,
            };

            if (string.Equals(Environment.GetEnvironmentVariable("WIREMOCK_PROXY_ENABLED"), "true", StringComparison.OrdinalIgnoreCase))
            {
                settings.ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "https://connect.squareupsandbox.com",
                    SaveMapping = true,
                    SaveMappingToFile = true,
                };
            }

            return settings;
        }
    }
}
