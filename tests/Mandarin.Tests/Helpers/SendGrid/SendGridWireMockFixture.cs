using System;
using WireMock.Server;
using WireMock.Settings;

namespace Mandarin.Tests.Helpers.SendGrid
{
    public sealed class SendGridWireMockFixture : IDisposable
    {
        public const string Host = "http://localhost:20001";

        private readonly WireMockServer server;

        public SendGridWireMockFixture()
        {
            this.server = WireMockServer.Start(SendGridWireMockFixture.CreateSettings());
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
                Urls = new[] { SendGridWireMockFixture.Host },
                Logger = new WireMockSerilogLogger<SendGridWireMockFixture>(),
                ReadStaticMappings = true,
            };

            if (string.Equals(Environment.GetEnvironmentVariable("WIREMOCK_PROXY_ENABLED"), "true", StringComparison.OrdinalIgnoreCase))
            {
                settings.ProxyAndRecordSettings = new ProxyAndRecordSettings
                {
                    Url = "https://api.sendgrid.com/",
                    SaveMapping = true,
                    SaveMappingToFile = true,
                };
            }

            return settings;
        }
    }
}
